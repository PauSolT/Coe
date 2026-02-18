using UnityEngine;

[RequireComponent (typeof(BoxCollider2D))]
public class PlayerController : MonoBehaviour
{
    [field: SerializeField] InputReader inputReader;
    [field: SerializeField] int verticalRaycount = 4;
    [field: SerializeField] int horizontalRaycount = 4;

    const float skinWidth = 0.015f;
    float horizontalRaySpacing;
    float verticalRaySpacing;

    BoxCollider2D capsuleCollider;
    RaycastOrigins raycastOrigins;
    void Start()
    {
        capsuleCollider = GetComponent<BoxCollider2D>();

        inputReader.MoveEvent += Move;
        inputReader.JumpEvent += Jump;
        inputReader.CancelJumpEvent += CancelJump;

    }

    void FixedUpdate()
    {
        UpdateRaycastOrigins();
        CalculateRaySpacing();

        for (int i = 0; i < verticalRaycount; i++)
        {
            Debug.DrawRay(raycastOrigins.bottomLeft + i * verticalRaySpacing * Vector2.right, Vector2.up * -2f, Color.red);
        }
    }

    void Move(Vector2 input)
    {
        Log.Info($"Player moving: {input}", this);
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
        verticalRaycount = Mathf.Clamp(verticalRaycount, 2, int.MaxValue);

        //-1 to get the number of spaces between raycasts
        horizontalRaySpacing = bounds.size.y / (horizontalRaycount - 1);
        verticalRaySpacing = bounds.size.x / (verticalRaycount - 1);
    }


    struct RaycastOrigins
    {
        public Vector2 topLeft, topRight, bottomLeft, bottomRight;
    }

}
