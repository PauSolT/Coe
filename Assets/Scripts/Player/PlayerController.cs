using Unity.Mathematics.Geometry;
using UnityEngine;

[RequireComponent (typeof(BoxCollider2D))]
public class PlayerController : MonoBehaviour
{
    public LayerMask collisionMask;

    [field: SerializeField] int verticalRaycount = 4;
    [field: SerializeField] int horizontalRaycount = 4;

    const float skinWidth = 0.015f;
    float horizontalRaySpacing;
    float verticalRaySpacing;
    float maxClimbAngle = 75;

    BoxCollider2D capsuleCollider;
    RaycastOrigins raycastOrigins;

    public CollisionInfo collisions;

    void Start()
    {
        capsuleCollider = GetComponent<BoxCollider2D>();
        CalculateRaySpacing();
    }

    /// <summary>
    /// Move the player and checking collisions
    /// </summary>
    /// <param name="velocity">Current velocity of the player</param>
    public void Move(Vector3 velocity)
    {
        UpdateRaycastOrigins();

        collisions.Reset();

        if(velocity.x != 0)
        {
            HorizontalCollisions(ref velocity);
        }
        if (velocity.y != 0)
        {
            VerticalCollisions(ref velocity);
        }
        transform.Translate(velocity);
    }

    /// <summary>
    /// Checks for horizontal collisions
    /// </summary>
    /// <param name="velocity">Current velocity of the player</param>
    void HorizontalCollisions(ref Vector3 velocity)
    {
        float directionX = Mathf.Sign(velocity.x);
        float rayLength = Mathf.Abs(velocity.x) + skinWidth;

        for (int i = 0; i < verticalRaycount; i++)
        {
            //Depending of the direction of X, where the origin of rays should start
            Vector2 rayOrigin = directionX == -1 ? raycastOrigins.bottomLeft : raycastOrigins.bottomRight;
            rayOrigin += Vector2.up * (horizontalRaySpacing * i);

            RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.right * directionX, rayLength, collisionMask);
            Debug.DrawRay(rayOrigin, Vector2.right * directionX * rayLength, Color.red);

            if (hit)
            {
                //Get angle of the slope
                float slopeAngle = Vector2.Angle(hit.normal, Vector2.up);

                //If user is climbing a slope
                if ( i== 0 && slopeAngle <= maxClimbAngle)
                {
                    float distanceToSlopeStart = 0;
                    //If user starts to climb new slope
                    //Clamp the user to actually touch the slope    
                    if (slopeAngle != collisions.slopeAngleOld)
                    {
                        distanceToSlopeStart = hit.distance - skinWidth;
                        velocity.x -= distanceToSlopeStart * directionX;
                    }
                    ClimbSlope(ref velocity, slopeAngle);
                    velocity.x += distanceToSlopeStart * directionX;
                }

                //If user is not climbing a slope
                if (!collisions.climbingSlope || slopeAngle > maxClimbAngle)
                {
                    //If hit, make the velocity the remaining distance to collide
                    velocity.x = Mathf.Min(Mathf.Abs(velocity.x), (hit.distance - skinWidth)) * directionX;
                    //Make the raylenght the same as hit distance to avoid collision problems
                    //When multiple rays hit collisions with different distances
                    rayLength = Mathf.Min(Mathf.Abs(velocity.x) + skinWidth, hit.distance);

                    if (collisions.climbingSlope)
                    {
                        //Update the Y velocity when climbing slopes and encountering an obstacle on the slope
                        velocity.y = Mathf.Tan(collisions.slopeAngle * Mathf.Deg2Rad) * Mathf.Abs(velocity.x);
                    }

                    //Check if the collision is hitting something left or right
                    collisions.left = directionX == -1;
                    collisions.right = directionX == 1;
                }
            }
        }
    }

    /// <summary>
    /// Checks for vertical collisions
    /// </summary>
    /// <param name="velocity">Current velocity of the player</param>
    void VerticalCollisions(ref Vector3 velocity)
    {
        float directionY = Mathf.Sign(velocity.y);
        float rayLength = Mathf.Abs(velocity.y) + skinWidth;

        for (int i = 0; i < verticalRaycount; i++)
        {
            //Depending of the direction of X, where the origin of rays should start
            Vector2 rayOrigin = directionY == -1 ? raycastOrigins.bottomLeft : raycastOrigins.topLeft;
            //Adding velocity X because casts will be when player has moved on the X axis
            rayOrigin += Vector2.right * (verticalRaySpacing * i + velocity.x);

            RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.up * directionY, rayLength, collisionMask);
            Debug.DrawRay(rayOrigin, Vector2.up * directionY * rayLength, Color.red);

            if (hit)
            {
                //If hit, make the velocity the remaining distance to collide
                velocity.y = (hit.distance - skinWidth) * directionY;
                //Make the raylenght the same as hit distance to avoid collision problems
                //When multiple rays hit collisions with different distances
                rayLength = hit.distance;

                if (collisions.climbingSlope)
                {
                    //Update the X velocity when climbing slopes and encountering a ceiling on the slope
                    velocity.x = velocity.y / Mathf.Tan(collisions.slopeAngle * Mathf.Deg2Rad) * Mathf.Sign(velocity.x);
                }


                //Check if the collision is hitting something above or below
                collisions.below = directionY == -1;
                collisions.above = directionY == 1;
            }
        }
    }


    /// <summary>
    /// Get the bounds of the collision
    /// </summary>
    void UpdateRaycastOrigins()
    {
        Bounds bounds = capsuleCollider.bounds;
        bounds.Expand(skinWidth * -2f);

        raycastOrigins.bottomLeft = new Vector2(bounds.min.x, bounds.min.y);
        raycastOrigins.bottomRight = new Vector2(bounds.max.x, bounds.min.y);
        raycastOrigins.topLeft = new Vector2(bounds.min.x, bounds.max.y);
        raycastOrigins.topRight = new Vector2(bounds.max.x, bounds.max.y);
    }

    /// <summary>
    /// Calculates the raycasts spacinf depending on the number of rayctasts
    /// </summary>
    void CalculateRaySpacing()
    {
        Bounds bounds = capsuleCollider.bounds;
        bounds.Expand(skinWidth * -2f);

        //Minimum of 2 raycasts, one at the start, one at the end
        horizontalRaycount = Mathf.Clamp(horizontalRaycount, 2, int.MaxValue);
        verticalRaycount = Mathf.Clamp(verticalRaycount, 2, int.MaxValue);

        //-1 to get the number of spaces between raycasts
        horizontalRaySpacing = bounds.size.y / (horizontalRaycount - 1);
        verticalRaySpacing = bounds.size.x / (verticalRaycount - 1);
    }

    /// <summary>
    /// Allows the user to climb slopes
    /// </summary>
    /// <param name="velocity">Velocity of the user</param>
    /// <param name="slopeAngle">Angle of the slope tha is being climbed currently</param>
    void ClimbSlope(ref Vector3 velocity, float slopeAngle)
    {
        float moveDistance = Mathf.Abs(velocity.x);
        float climbVelocityY = Mathf.Sin(slopeAngle * Mathf.Deg2Rad) * moveDistance;


        if(velocity.y <= climbVelocityY)
        {
            //Assume that the user is not jumping
            velocity.y = Mathf.Sin(slopeAngle * Mathf.Deg2Rad) * moveDistance;
            velocity.x = Mathf.Cos(slopeAngle * Mathf.Deg2Rad) * moveDistance * Mathf.Sign(velocity.x);
            collisions.below = true;
            collisions.climbingSlope = true;
            collisions.slopeAngle = slopeAngle;
        }
    }

    struct RaycastOrigins
    {
        public Vector2 topLeft, topRight, bottomLeft, bottomRight;
    }

    public struct CollisionInfo
    {
        public bool above, below, left, right;
        public bool climbingSlope;
        public float slopeAngle, slopeAngleOld;

        /// <summary>
        /// Reset collisions info
        /// </summary>
        public void Reset()
        {
            above = below = left = right = climbingSlope = false;
            slopeAngleOld = slopeAngle;
            slopeAngle = 0f;

        }
    }

}
