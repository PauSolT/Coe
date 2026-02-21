using System.Collections.Generic;
using UnityEngine;

public class PlatformController : RaycastController
{
    public LayerMask passengerMask;

    public Vector3[] localWaypoints;
    Vector3[] globalWaypoints;

    public float speed;
    public bool cyclic;
    public float waitTime;
    [Range(0,2)]
    public float easeAmount;

    int waypointIndex;
    float percentBetweenWaypoints;
    float nextMoveTime;

    List<PassengerMovement> passengerMovement;
    Dictionary<Transform, Controller2D> passengersDictionary = new Dictionary<Transform, Controller2D>();

    protected override void Start()
    {
        base.Start();
        globalWaypoints = new Vector3[localWaypoints.Length];
        for (int i = 0; i < localWaypoints.Length; i++)
        {
            globalWaypoints[i] = localWaypoints[i] + transform.position;
        }
    }

    void Update()
    {
        UpdateRaycastOrigins();

        Vector3 velocity = CalculatePlatformMovement();
        
        CalculatePassangerMovement(velocity);
        MovePassangers(true);
        transform.Translate(velocity);
        MovePassangers(false);

    }

    /// <summary>
    /// Calculate the ease of movement when arriving/exiting from waypoints
    /// </summary>
    /// <param name="x">The percent distance between waypoints</param>
    /// <returns>Returns the current easing</returns>
    float Ease(float x)
    {
        float a = easeAmount + 1;
        return Mathf.Pow(x, a) / (Mathf.Pow(x, a) + Mathf.Pow(1 - x, a));
    }

    /// <summary>
    /// Calculate the the movement of the platform
    /// </summary>
    /// <returns>Returns the position of the next frame</returns>
    Vector3 CalculatePlatformMovement()
    {
        //If there is wait time, don't move while waiting
        if (Time.time < nextMoveTime)
        {
            return Vector3.zero;
        }

        //Cycle waypoints
        waypointIndex %= globalWaypoints.Length;
        int toWayPointIndex = (waypointIndex + 1) % globalWaypoints.Length;

        float distanceBetweenWaypoints = Vector3.Distance(globalWaypoints[waypointIndex], globalWaypoints[toWayPointIndex]);
        percentBetweenWaypoints += Time.deltaTime * speed / distanceBetweenWaypoints;
        percentBetweenWaypoints = Mathf.Clamp01(percentBetweenWaypoints);
        float easedPercentBetweenWaypoints = Ease(percentBetweenWaypoints);

        Vector3 newPos = Vector3.Lerp(globalWaypoints[waypointIndex], globalWaypoints[toWayPointIndex], easedPercentBetweenWaypoints);

        //If reached next waypoint
        if (percentBetweenWaypoints >= 1)
        {
            percentBetweenWaypoints = 0;
            waypointIndex++;

            if (!cyclic)
            {
                if (waypointIndex >= globalWaypoints.Length - 1)
                {
                    waypointIndex = 0;
                    System.Array.Reverse(globalWaypoints);
                }
            }
            nextMoveTime = Time.time + waitTime;
        }

        return newPos - transform.position;
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
                if (hit && hit.distance != 0)
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
                if (hit && hit.distance != 0)
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
                if (hit && hit.distance != 0)
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

    private void OnDrawGizmos()
    {
        if (localWaypoints != null)
        {
            Gizmos.color = Color.red;
            float size = 0.3f;

            for (int i = 0; i < localWaypoints.Length; i++)
            {
                Vector3 globalWaypointPosition = Application.isPlaying ? globalWaypoints[i] : localWaypoints[i]  + transform.position;
                Gizmos.DrawLine(globalWaypointPosition - Vector3.up * size, globalWaypointPosition + Vector3.up * size);
                Gizmos.DrawLine(globalWaypointPosition - Vector3.left * size, globalWaypointPosition + Vector3.left * size);
            }
        }
    }
}
