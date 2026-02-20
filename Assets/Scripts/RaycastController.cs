using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class RaycastController : MonoBehaviour
{
    public LayerMask collisionMask;

    [field: SerializeField] protected int verticalRaycount = 4;
    [field: SerializeField] protected int horizontalRaycount = 4;

    protected const float skinWidth = 0.015f;
    protected float horizontalRaySpacing;
    protected float verticalRaySpacing;

    protected BoxCollider2D capsuleCollider;
    protected RaycastOrigins raycastOrigins;

    protected virtual void Start()
    {
        capsuleCollider = GetComponent<BoxCollider2D>();
        CalculateRaySpacing();
    }


    /// <summary>
    /// Get the bounds of the collision
    /// </summary>
    protected void UpdateRaycastOrigins()
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
    protected void CalculateRaySpacing()
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


    protected struct RaycastOrigins
    {
        public Vector2 topLeft, topRight, bottomLeft, bottomRight;
    }

}
