using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class PlayerController : MonoBehaviour
{
    [field: SerializeField] InputReader inputReader;
    [field: SerializeField] int horizontalRaycount = 4;
    [field: SerializeField] float moveSpeed = 6f;
    float direction = 0f;

    const float skinWidth = 0.015f;
    float horizontalRaySpacing;

    BoxCollider2D capsuleCollider;
    RaycastOrigins raycastOrigins;

    Rigidbody2D rigidbody2d;

    void Start()
    {
        capsuleCollider = GetComponent<BoxCollider2D>();
        rigidbody2d = GetComponent<Rigidbody2D>();

        inputReader.MoveEvent += Move;
        inputReader.JumpEvent += Jump;
        inputReader.CancelJumpEvent += CancelJump;
    }

    void FixedUpdate()
    {
        UpdateRaycastOrigins();
        CalculateRaySpacing();
    }

    private void Update()
    {
        rigidbody2d.linearVelocityX = moveSpeed * direction;
    }

    void Move(Vector2 input)
    {
        Log.Info($"Player moving: {input}", this);
        direction = input.x;
    }

    void Jump()
    {
        Log.Info($"Player jumped", this);
    }

    void CancelJump()
    {
        Log.Info($"Player canceled jumped", this);
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

        //-1 to get the number of spaces between raycasts
        horizontalRaySpacing = bounds.size.y / (horizontalRaycount - 1);
    }


    struct RaycastOrigins
    {
        public Vector2 topLeft, topRight, bottomLeft, bottomRight;
    }

}