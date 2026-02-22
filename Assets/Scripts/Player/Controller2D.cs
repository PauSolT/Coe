using Unity.Mathematics.Geometry;
using UnityEngine;


public class Controller2D : RaycastController
{
    [field: SerializeField] float maxSlopeAngle = 75;

    public CollisionInfo collisions;
    Vector2 playerInput;

    protected override void Start()
    {
        base.Start();
        collisions.faceDir = 1;
    }

    public void Move (Vector2 deltaVelocity, bool standingOnPlatform = false)
    {
        Move(deltaVelocity, Vector2.zero, standingOnPlatform);
    }

    /// <summary>
    /// Move the player and checking collisions
    /// </summary>
    /// <param name="deltaVelocity">Current deltaVelocity of the player</param>
    public void Move(Vector2 deltaVelocity, Vector2 input,bool standingOnPlatform = false)
    {
        UpdateRaycastOrigins();
        collisions.Reset();
        collisions.deltaVelocityOld = deltaVelocity;
        playerInput = input;

        //User is descending slope or falling
        if (deltaVelocity.y < 0)
        {
            DescendSlope(ref deltaVelocity);
        }

        if (deltaVelocity.x != 0)
        {
            collisions.faceDir = (int)Mathf.Sign(deltaVelocity.x);
        }


        HorizontalCollisions(ref deltaVelocity);
        //If user is moving
        if (deltaVelocity.y != 0)
        {
            VerticalCollisions(ref deltaVelocity);
        }
        transform.Translate(deltaVelocity);

        //If standing on platform, assume user is grounded and can jump
        if (standingOnPlatform)
        {
            collisions.below = true;
        }
    }

    /// <summary>
    /// Checks for horizontal collisions
    /// </summary>
    /// <param name="deltaVelocity">Current deltaVelocity of the player</param>
    void HorizontalCollisions(ref Vector2 deltaVelocity)
    {
        float directionX = collisions.faceDir;
        float rayLength = Mathf.Abs(deltaVelocity.x) + skinWidth;

        //If player is not moving, shorten the ray length
        if (Mathf.Abs(deltaVelocity.x) < skinWidth)
        {
            rayLength = skinWidth * 2;
        }

        for (int i = 0; i < horizontalRaycount; i++)
        {
            //Depending of the direction of X, where the origin of rays should start
            Vector2 rayOrigin = directionX == -1 ? raycastOrigins.bottomLeft : raycastOrigins.bottomRight;
            rayOrigin += Vector2.up * (horizontalRaySpacing * i);

            RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.right * directionX, rayLength, collisionMask);
            Debug.DrawRay(rayOrigin, Vector2.right * directionX * rayLength, Color.red);

            if (hit)
            {
                //If raycast hit ditance is 0, skip this ray
                if (hit.distance == 0)
                {
                    continue;
                }
               

                //Get angle of the slope
                float slopeAngle = Vector2.Angle(hit.normal, Vector2.up);

                //If user is climbing a slope
                if ( i== 0 && slopeAngle <= maxSlopeAngle)
                {
                    if (collisions.descendingSlope)
                    {
                        collisions.descendingSlope = false;
                        deltaVelocity = collisions.deltaVelocityOld;
                    }
                    float distanceToSlopeStart = 0;
                    //If user starts to climb new slope
                    //Clamp the user to actually touch the slope    
                    if (slopeAngle != collisions.slopeAngleOld)
                    {
                        distanceToSlopeStart = hit.distance - skinWidth;
                        deltaVelocity.x -= distanceToSlopeStart * directionX;
                    }
                    ClimbSlope(ref deltaVelocity, slopeAngle, hit.normal);
                    deltaVelocity.x += distanceToSlopeStart * directionX;
                }

                //If user is not climbing a slope
                if (!collisions.climbingSlope || slopeAngle > maxSlopeAngle)
                {
                    //If hit, make the deltaVelocity the remaining distance to collide
                    deltaVelocity.x = Mathf.Min(Mathf.Abs(deltaVelocity.x), (hit.distance - skinWidth)) * directionX;
                    //Make the raylenght the same as hit distance to avoid collision problems
                    //When multiple rays hit collisions with different distances
                    rayLength = Mathf.Min(Mathf.Abs(deltaVelocity.x) + skinWidth, hit.distance);

                    if (collisions.climbingSlope)
                    {
                        //Update the Y deltaVelocity when climbing slopes and encountering an obstacle on the slope
                        deltaVelocity.y = Mathf.Tan(collisions.slopeAngle * Mathf.Deg2Rad) * Mathf.Abs(deltaVelocity.x);
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
    /// <param name="deltaVelocity">Current deltaVelocity of the player</param>
    void VerticalCollisions(ref Vector2 deltaVelocity)
    {
        float directionY = Mathf.Sign(deltaVelocity.y);
        float rayLength = Mathf.Abs(deltaVelocity.y) + skinWidth;

        for (int i = 0; i < verticalRaycount; i++)
        {
            //Depending of the direction of X, where the origin of rays should start
            Vector2 rayOrigin = directionY == -1 ? raycastOrigins.bottomLeft : raycastOrigins.topLeft;
            //Adding deltaVelocity X because casts will be when player has moved on the X axis
            rayOrigin += Vector2.right * (verticalRaySpacing * i + deltaVelocity.x);

            RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.up * directionY, rayLength, collisionMask);
            Debug.DrawRay(rayOrigin, Vector2.up * directionY * rayLength, Color.red);

            if (hit)
            {
                //If a platform has tag "Through"
                if (hit.collider.tag == "Through")
                {
                    //Pass through above it
                    if (directionY == 1 || hit.distance == 0)
                    {
                        continue;
                    }

                    //Pass through below it
                    if (collisions.fallingThroughPlatform)
                    {
                        continue;
                    }

                    if (playerInput.y == -1)
                    {
                        collisions.fallingThroughPlatform = true;
                        Invoke(nameof(ResetFallingThroughPlatform), 0.5f);
                        continue;
                    }
                }

                //If hit, make the deltaVelocity the remaining distance to collide
                deltaVelocity.y = (hit.distance - skinWidth) * directionY;
                //Make the raylenght the same as hit distance to avoid collision problems
                //When multiple rays hit collisions with different distances
                rayLength = hit.distance;

                if (collisions.climbingSlope)
                {
                    //Update the X deltaVelocity when climbing slopes and encountering a ceiling on the slope
                    deltaVelocity.x = deltaVelocity.y / Mathf.Tan(collisions.slopeAngle * Mathf.Deg2Rad) * Mathf.Sign(deltaVelocity.x);
                }


                //Check if the collision is hitting something above or below
                collisions.below = directionY == -1;
                collisions.above = directionY == 1;
            }
        }

        //When climbing slope
        if (collisions.climbingSlope)
        {
            float directionX = Mathf.Sign(deltaVelocity.x);
            rayLength = Mathf.Abs(deltaVelocity.x) + skinWidth;
            Vector2 rayOrigin = ((directionX == -1) ? raycastOrigins.bottomLeft : raycastOrigins.bottomRight) + Vector2.up * deltaVelocity.y;
            RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.right * directionX, rayLength, collisionMask);

            if (hit)
            {
                float slopeAngle = Vector2.Angle(hit.normal, Vector2.up);
                //And changing slope angle
                if(slopeAngle != collisions.slopeAngle)
                {
                    //Make it so user snaps smoothly to next slope
                    deltaVelocity.x = (hit.distance - skinWidth) * directionX;
                    collisions.slopeAngle = slopeAngle;
                    collisions.slopeNormal = hit.normal;
                }
            }
        }
    }



    /// <summary>
    /// Allows the user to climb slopes
    /// </summary>
    /// <param name="deltaVelocity">deltaVelocity of the user</param>
    /// <param name="slopeAngle">Angle of the slope tha is being climbed currently</param>
    void ClimbSlope(ref Vector2 deltaVelocity, float slopeAngle, Vector2 slopeNormal)
    {
        float moveDistance = Mathf.Abs(deltaVelocity.x);
        float climbdeltaVelocityY = Mathf.Sin(slopeAngle * Mathf.Deg2Rad) * moveDistance;


        if(deltaVelocity.y <= climbdeltaVelocityY)
        {
            //Assume that the user is not jumping
            deltaVelocity.y = Mathf.Sin(slopeAngle * Mathf.Deg2Rad) * moveDistance;
            deltaVelocity.x = Mathf.Cos(slopeAngle * Mathf.Deg2Rad) * moveDistance * Mathf.Sign(deltaVelocity.x);
            collisions.below = true;
            collisions.climbingSlope = true;
            collisions.slopeAngle = slopeAngle;
            collisions.slopeNormal = slopeNormal;

        }
    }

    /// <summary>
    /// Allows the user to descend slopes
    /// </summary>
    /// <param name="deltaVelocity">deltaVelocity of the usre</param>
    void DescendSlope(ref Vector2 deltaVelocity)
    {
        RaycastHit2D maxSlopeHitLeft = Physics2D.Raycast(raycastOrigins.bottomLeft, Vector2.down, Mathf.Abs(deltaVelocity.y) + skinWidth, collisionMask);
        RaycastHit2D maxSlopeHitRight = Physics2D.Raycast(raycastOrigins.bottomRight, Vector2.down, Mathf.Abs(deltaVelocity.y) + skinWidth, collisionMask);

        if (maxSlopeHitLeft ^ maxSlopeHitRight)
        {
            SlideDownMaxSlope(maxSlopeHitLeft, ref deltaVelocity);
            SlideDownMaxSlope(maxSlopeHitRight, ref deltaVelocity);
        }

        if (!collisions.slidingDownMaxSlope)
        {
            //Depending of the direction of X, where the origin of rays should start
            float directionX = Mathf.Sign(deltaVelocity.x);
            Vector2 rayOrigin = (directionX == -1) ? raycastOrigins.bottomRight : raycastOrigins.bottomLeft;
            RaycastHit2D hit = Physics2D.Raycast(rayOrigin, -Vector2.up, Mathf.Infinity, collisionMask);

            //If user hits ground
            if (hit)
            {
                float slopeAngle = Vector2.Angle(hit.normal, Vector2.up);

                //If ground is slope
                if (slopeAngle != 0 && slopeAngle <= maxSlopeAngle)
                {
                    //If user is moving down the slope
                    if (Mathf.Sign(hit.normal.x) == directionX)
                    {
                        //If user is close to the slope
                        if (hit.distance - skinWidth <= Mathf.Tan(slopeAngle * Mathf.Deg2Rad) * Mathf.Abs(deltaVelocity.x))
                        {
                            //Descend slope
                            float moveDistance = Mathf.Abs(deltaVelocity.x);
                            float descenddeltaVelocityY = Mathf.Sin(slopeAngle * Mathf.Deg2Rad) * moveDistance;
                            deltaVelocity.x = Mathf.Cos(slopeAngle * Mathf.Deg2Rad) * moveDistance * Mathf.Sign(deltaVelocity.x);
                            deltaVelocity.y -= descenddeltaVelocityY;

                            collisions.slopeAngle = slopeAngle;
                            collisions.descendingSlope = true;
                            collisions.below = true;
                            collisions.slopeNormal = hit.normal;
                        }
                    }
                }
            }
        }
    }

    void SlideDownMaxSlope(RaycastHit2D hit, ref Vector2 deltaVelocity)
    {
        if (hit)
        {
            float slopeAngle = Vector2.Angle(hit.normal, Vector2.up);
            if (slopeAngle > maxSlopeAngle)
            {
                deltaVelocity.x = (Mathf.Abs(deltaVelocity.y) - hit.distance) / Mathf.Tan(slopeAngle * Mathf.Deg2Rad) * hit.normal.x;

                collisions.slopeAngle = slopeAngle;
                collisions.slidingDownMaxSlope = true;
                collisions.slopeNormal = hit.normal;
            }
        }
    }

    /// <summary>
    /// Resets fallingThroughPlatform to false
    /// </summary>
    void ResetFallingThroughPlatform()
    {
        collisions.fallingThroughPlatform = false;
    }

    public struct CollisionInfo
    {
        public bool above, below, left, right;
        public bool climbingSlope, descendingSlope;
        public bool slidingDownMaxSlope;
        public float slopeAngle, slopeAngleOld;
        public Vector2 slopeNormal;
        public Vector2 deltaVelocityOld;
        public int faceDir;
        public bool fallingThroughPlatform;

        /// <summary>
        /// Reset collisions info
        /// </summary>
        public void Reset()
        {
            above = below = left = right = climbingSlope = descendingSlope = slidingDownMaxSlope = false;
            slopeAngleOld = slopeAngle;
            slopeAngle = 0f;
            slopeNormal = Vector2.zero;

        }
    }

}
