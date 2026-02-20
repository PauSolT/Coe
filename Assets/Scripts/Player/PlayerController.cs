using Unity.Mathematics.Geometry;
using UnityEngine;


public class PlayerController : RaycastController
{
    float maxClimbAngle = 75;
    float maxDescendAngle = 75;

    public CollisionInfo collisions;

    protected override void Start()
    {
        base.Start();
    }

    /// <summary>
    /// Move the player and checking collisions
    /// </summary>
    /// <param name="velocity">Current velocity of the player</param>
    public void Move(Vector3 velocity)
    {
        UpdateRaycastOrigins();
        collisions.Reset();
        collisions.velocityOld = velocity;

        //User is descending slope or falling
        if (velocity.y < 0)
        {
            DescendSlope(ref velocity);
        }

        //If user is moving
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
                    if (collisions.descendingSlope)
                    {
                        collisions.descendingSlope = false;
                        velocity = collisions.velocityOld;
                    }
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

        //When climbing slope
        if (collisions.climbingSlope)
        {
            float directionX = Mathf.Sign(velocity.x);
            rayLength = Mathf.Abs(velocity.x) + skinWidth;
            Vector2 rayOrigin = ((directionX == -1) ? raycastOrigins.bottomLeft : raycastOrigins.bottomRight) + Vector2.up * velocity.y;
            RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.right * directionX, rayLength, collisionMask);

            if (hit)
            {
                float slopeAngle = Vector2.Angle(hit.normal, Vector2.up);
                //And changing slope angle
                if(slopeAngle != collisions.slopeAngle)
                {
                    //Make it so user snaps smoothly to next slope
                    velocity.x = (hit.distance - skinWidth) * directionX;
                    collisions.slopeAngle = slopeAngle;
                }
            }
        }
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

    /// <summary>
    /// Allows the user to descend slopes
    /// </summary>
    /// <param name="velocity">Velocity of the usre</param>
    void DescendSlope(ref Vector3 velocity)
    {
        //Depending of the direction of X, where the origin of rays should start
        float directionX = Mathf.Sign(velocity.x);
        Vector2 rayOrigin = (directionX == -1) ? raycastOrigins.bottomRight : raycastOrigins.bottomLeft;
        RaycastHit2D hit = Physics2D.Raycast(rayOrigin, -Vector2.up, Mathf.Infinity, collisionMask);

        //If user hits ground
        if (hit)
        {
            float slopeAngle = Vector2.Angle(hit.normal, Vector2.up);

             //If ground is slope
            if(slopeAngle != 0 && slopeAngle <= maxDescendAngle)
            {
                //If user is moving down the slope
                if (Mathf.Sign(hit.normal.x) == directionX)
                {
                    //If user is close to the slope
                    if (hit.distance - skinWidth <= Mathf.Tan(slopeAngle * Mathf.Deg2Rad) * Mathf.Abs(velocity.x))
                    {
                        //Descend slope
                        float moveDistance = Mathf.Abs(velocity.x);
                        float descendVelocityY = Mathf.Sin(slopeAngle * Mathf.Deg2Rad) * moveDistance;
                        velocity.x = Mathf.Cos(slopeAngle * Mathf.Deg2Rad) * moveDistance * Mathf.Sign(velocity.x);
                        velocity.y -= descendVelocityY;

                        collisions.slopeAngle = slopeAngle;
                        collisions.descendingSlope = true;
                        collisions.below = true;
                    }
                }
            }
        }
    }

    public struct CollisionInfo
    {
        public bool above, below, left, right;
        public bool climbingSlope, descendingSlope;
        public float slopeAngle, slopeAngleOld;
        public Vector3 velocityOld;

        /// <summary>
        /// Reset collisions info
        /// </summary>
        public void Reset()
        {
            above = below = left = right = climbingSlope = descendingSlope = false;
            slopeAngleOld = slopeAngle;
            slopeAngle = 0f;

        }
    }

}
