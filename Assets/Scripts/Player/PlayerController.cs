using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;
using static UnityEditor.ShaderGraph.Internal.KeywordDependentCollection;

[RequireComponent(typeof(BoxCollider2D))]
public class PlayerController : MonoBehaviour
{
    [field: SerializeField] InputReader inputReader;
    [field: SerializeField] int horizontalRaycount = 4;
    [field: SerializeField] float moveSpeed = 6f;
    float direction = 0f;

    const float skinWidth = 0.015f;
    const float rayLength = 0.15f;
    float horizontalRaySpacing;

    BoxCollider2D capsuleCollider;
    RaycastOrigins raycastOrigins;

    Rigidbody2D rigidbody2d;
    [field: SerializeField] LayerMask groundLayer;

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
     
    }

    private void Update()
    {
        rigidbody2d.linearVelocityX = moveSpeed * direction;
        for (int i = 0; i < horizontalRaycount; i++)
        {
            Debug.DrawRay(raycastOrigins.bottomLeft + Vector2.right * (horizontalRaySpacing * i), Vector2.down * rayLength, Color.red);
        }
    }

    void Move(Vector2 input)
    {
        Log.Info($"Player moving: {input}", this);
        direction = input.x;
    }

    void Jump()
    {
        if (CheckGround())
        {
            rigidbody2d.linearVelocityY = moveSpeed;
        }
        Log.Info($"Player jumped", this);
    }

    void CancelJump()
    {
        Log.Info($"Player canceled jumped", this);
    }

    bool CheckGround()
    {
        UpdateRaycastOrigins();
        CalculateRaySpacing();
        RaycastHit2D hit;
        for (int i = 0; i < horizontalRaycount; i++)
        {
            Log.Info("HOOOLLAAAAAAAAAAAAAAAAAA");
            hit = Physics2D.Raycast(raycastOrigins.bottomLeft + Vector2.right *(horizontalRaySpacing * i), Vector2.down, rayLength, groundLayer);
            if (hit.collider != null)
            {
                Log.Info(">>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>");
                return true;
            }
        }
        Log.Info("<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<");

        return false;
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
        horizontalRaySpacing = bounds.size.x / (horizontalRaycount - 1);
    }


    struct RaycastOrigins
    {
        public Vector2 topLeft, topRight, bottomLeft, bottomRight;
    }

}