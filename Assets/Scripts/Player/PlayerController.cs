using Unity.VisualScripting;
using UnityEditor.Tilemaps;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.Windows;
using static UnityEditor.ShaderGraph.Internal.KeywordDependentCollection;

[RequireComponent(typeof(BoxCollider2D))]
public class PlayerController : MonoBehaviour
{
    [field: SerializeField] InputReader inputReader;
    [field: SerializeField] int horizontalRaycount = 4;
    [field: SerializeField] float moveSpeed = 6f;
    float direction = 0f;

    bool facingRight = true;

    const float skinWidth = 0.015f;
    const float rayLength = 0.15f;
    float horizontalRaySpacing;

    BoxCollider2D capsuleCollider;
    RaycastOrigins raycastOrigins;
    Player player;

    Rigidbody2D rigidbody2d;
    [field: SerializeField] LayerMask groundLayer;

    void Awake()
    {
        capsuleCollider = GetComponent<BoxCollider2D>();
        rigidbody2d = GetComponent<Rigidbody2D>();
        player = GetComponent<Player>();
    }

    private void OnEnable()
    {
        inputReader.MoveEvent += Move;
        inputReader.JumpEvent += Jump;
        inputReader.CancelJumpEvent += CancelJump;
        inputReader.PreviousElementEvent += player.PreviousElement;
        inputReader.NextElementEvent += player.NextElement;
        inputReader.Ability1Event += player.UseAbility1;
        inputReader.Ability2Event += player.UseAbility2;
        inputReader.Ability3Event += player.UseAbility3;
    }
    private void OnDisable()
    {
        inputReader.MoveEvent -= Move;
        inputReader.JumpEvent -= Jump;
        inputReader.CancelJumpEvent -= CancelJump;
        inputReader.PreviousElementEvent -= player.PreviousElement;
        inputReader.NextElementEvent -= player.NextElement;
        inputReader.Ability1Event -= player.UseAbility1;
        inputReader.Ability2Event -= player.UseAbility2;
        inputReader.Ability3Event -= player.UseAbility3;
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
        Log.Info($"Player moving: {input}", LogCategory.Input, this);
        direction = input.x;

        if (direction < 0 && facingRight)
            Flip();
        if (direction > 0 && !facingRight)
            Flip();
            
    }

    void Flip()
    {
        facingRight = !facingRight;
        Vector3 scale = transform.localScale;
        scale.x *= -1;
        transform.localScale = scale;
        Log.Info($"Player flipped. Facing right: {facingRight}", LogCategory.Input, this);
    }

    void Jump()
    {
        if (CheckGround())
        {
            rigidbody2d.linearVelocityY = moveSpeed;
        }
        Log.Info($"Player jumped", LogCategory.Input, this);
    }

    void CancelJump()
    {
        Log.Info($"Player canceled jumped", LogCategory.Input, this);
    }

    bool CheckGround()
    {
        UpdateRaycastOrigins();
        CalculateRaySpacing();
        RaycastHit2D hit;
        for (int i = 0; i < horizontalRaycount; i++)
        {
            hit = Physics2D.Raycast(raycastOrigins.bottomLeft + Vector2.right *(horizontalRaySpacing * i), Vector2.down, rayLength, groundLayer);
            if (hit.collider != null)
            {
                return true;
            }
        }

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