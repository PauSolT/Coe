using System.Collections.Generic;
using UnityEngine;

public class PlatformController : RaycastController
{
    public LayerMask passengerMask;

    public Vector3 move;
    protected override void Start()
    {
        base.Start();
    }

    void Update()
    {
        UpdateRaycastOrigins();

        Vector3 velocity = move * Time.deltaTime;
        
        MovePassangers(velocity);
        transform.Translate(velocity);
    }

    /// <summary>
    /// Move objects that are above the platform
    /// </summary>
    /// <param name="velocity">Velocity of the platform</param>
    void MovePassangers(Vector3 velocity)
    {
        HashSet<Transform> passangers = new HashSet<Transform>();

        float directionX = Mathf.Sign(velocity.x);
        float directionY = Mathf.Sign(velocity.y);

        //Moving the platform vertically
        if (velocity.y != 0)
        {
            float rayLength = Mathf.Abs(velocity.y) + skinWidth;

            for (int i = 0; i < verticalRaycount; i++)
            {
                //Depending of the direction of Y, where the origin of rays should start
                Vector2 rayOrigin = directionY == -1 ? raycastOrigins.bottomLeft : raycastOrigins.topLeft;
                rayOrigin += Vector2.right * (verticalRaySpacing * i);

                RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.up * directionY, rayLength, passengerMask);

                //If something is above or below
                if (hit)
                {
                    if(!passangers.Contains(hit.transform))
                    {
                        //Move the passanger with the platform
                        passangers.Add(hit.transform);
                        float pushX = directionY == 1 ? velocity.x : 0;
                        float pushY = velocity.y - (hit.distance - skinWidth) * directionY;

                        hit.transform.Translate(new Vector3(pushX, pushY));
                    }
                }
            }
        }

        //Moving the platform horizontally
        if (velocity.x != 0)
        {
            float rayLength = Mathf.Abs(velocity.x) + skinWidth;

            for (int i = 0; i < horizontalRaycount; i++)
            {
                //Depending of the direction of X, where the origin of rays should start
                Vector2 rayOrigin = directionX == -1 ? raycastOrigins.bottomLeft : raycastOrigins.bottomRight;
                rayOrigin += Vector2.up * (verticalRaySpacing * i);

                RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.right * directionX, rayLength, passengerMask);

                //If something is left or right
                if (hit)
                {
                    if (!passangers.Contains(hit.transform))
                    {
                        //Move the passanger with the platform
                        passangers.Add(hit.transform);
                        float pushX = velocity.x - (hit.distance - skinWidth) * directionX;
                        float pushY = 0;

                        hit.transform.Translate(new Vector3(pushX, pushY));
                    }
                }
            }
        }

        //If passanger is oin top of a horizontal or downward moving platform
        if (directionY == -1 || velocity.y == 0 && velocity.x != 0)
        {
            float rayLength = skinWidth * 2;

            for (int i = 0; i < verticalRaycount; i++)
            {
                Vector2 rayOrigin = raycastOrigins.topLeft + Vector2.right * (verticalRaySpacing * i);
                RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.up, rayLength, passengerMask);

                //If there is a passanger on the platform
                if (hit)
                {
                    if (!passangers.Contains(hit.transform))
                    {
                        //Move the passanger with the platform
                        passangers.Add(hit.transform);
                        float pushX = velocity.x;
                        float pushY = velocity.y;

                        hit.transform.Translate(new Vector3(pushX, pushY));
                    }
                }
            }
        }
    }
}
