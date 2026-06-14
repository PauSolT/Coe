using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class TargetSystem : MonoBehaviour
{
    public static TargetSystem Instance { get; private set; }
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        Instance = this;

    }

    public event Action<ITargetable> OnTargetChanged;

    public ITargetable CurrentTarget { get; private set; }

    [Header("Settings")]
    [SerializeField] private LayerMask targetLayer;
    //[SerializeField] private float rayDistance = 10f;

    private Camera mainCamera;

    private void Start()
    {
        mainCamera = Camera.main;
    }

    private void Update()
    {
        Vector2 mouseScreenPos = Mouse.current.position.ReadValue();
        Vector2 mouseWorldPos = mainCamera.ScreenToWorldPoint(mouseScreenPos);
        RaycastHit2D hit = Physics2D.Raycast(mouseWorldPos, Vector2.zero, 0f, targetLayer);

        if (hit.collider != null)
        {
            ITargetable hitTarget = hit.collider.GetComponentInParent<ITargetable>();

            if (hitTarget != null && hitTarget != CurrentTarget)
            {
                SetTarget(hitTarget);
            }
        }
        else
        {
            if (CurrentTarget != null)
            {
                ClearTarget();
            }
        }
    }

    /// <summary>
    /// Sets a new target for an abilty
    /// </summary>
    /// <param name="newTarget">New target</param>
    private void SetTarget(ITargetable newTarget)
    {
        Log.Info($"New target: {newTarget.GetGameObject().name}", LogCategory.System);
        CurrentTarget = newTarget;
        OnTargetChanged?.Invoke(CurrentTarget);
    }

    /// <summary>
    /// Clears the target for an ability
    /// </summary>
    private void ClearTarget()
    {
        Log.Info($"Target cleared", LogCategory.System);
        CurrentTarget = null;
        OnTargetChanged?.Invoke(null);
    }
}
