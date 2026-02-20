using System.Collections.Generic;
using UnityEngine;

public class PlatformController : RaycastController
{
    public LayerMask passengerMask;
    public Vector3 move;

    List<PassengerMovement> passengerMovement;
    Dictionary<Transform, Controller2D> passengersDictionary = new Dictionary<Transform, Controller2D>();

    protected override void Start()
    {
        base.Start();
    }

    void FixedUpdate()
    {
        UpdateRaycastOrigins();

        Vector3 velocity = move * Time.deltaTime;
        
        CalculatePassangerMovement(velocity);
        MovePassangers(true);
        transform.Translate(velocity);
        MovePassangers(false);

    }

    /// <summary>
    /// Move the passengers
    /// </summary>
    /// <param name="beforeMovePlatform">If the passanger moves before the platform moves</param>
    void MovePassangers(bool beforeMovePlatform)
    {
        foreach (PassengerMovement passenger in passengerMovement)
        {
            if (!passengersDictionary.ContainsKey(passenger.transform))
            {
                passengersDictionary.Add(passenger.transform, passenger.transform.GetComponent<Controller2D>());
            }

            if (passenger.moveBeforePlatform == beforeMovePlatform)
            {
                passengersDictionary[passenger.transform].Move(passenger.velocity, passenger.standingOnPlatform);
            }
        }
    }

    /// <summary>
    /// Calculate the movement of the passengers
    /// </summary>
    /// <param name="velocity">Velocity of the platform</param>
    void CalculatePassangerMovement(Vector3 velocity)
    {
        HashSet<Transform> passangers = new HashSet<Transform>();
        passengerMovement = new List<PassengerMovement>();

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

                        passengerMovement.Add(new PassengerMovement(hit.transform, new Vector3(pushX, pushY), directionY == 1, true));
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
                        float pushY = -skinWidth;

                        passengerMovement.Add(new PassengerMovement(hit.transform, new Vector3(pushX, pushY), false, true));
                    }
                }
            }
        }

        //If passanger is on top of a horizontal or downward moving platform
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

                        passengerMovement.Add(new PassengerMovement(hit.transform, new Vector3(pushX, pushY), true, false));
                    }
                }
            }
        }
    }

    struct PassengerMovement
    {
        public Transform transform;
        public Vector3 velocity;
        public bool standingOnPlatform;
        public bool moveBeforePlatform;

        public PassengerMovement(Transform _transform, Vector3 _velocity, bool _standingOnPlatform, bool _moveBeforePlatform)
        {
            transform = _transform;
            velocity = _velocity;
            standingOnPlatform = _standingOnPlatform;
            moveBeforePlatform = _moveBeforePlatform;
        }
    }
}
