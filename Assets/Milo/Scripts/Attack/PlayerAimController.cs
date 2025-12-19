using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerAimController : MonoBehaviour
{
    [SerializeField] private LayerMask groundMask;
    [SerializeField] private float maxRaycastDistance = 200f;

    [Header("Input References")] 
    [SerializeField] private InputActionReference mousePositionAction;
    [SerializeField] private InputActionReference aimAction; 

    private Camera mainCamera;

    // The "State" that other scripts (like WeaponHandler) will check
    public bool IsAiming { get; private set; }

    private void Awake()
    {
        if (mousePositionAction?.action != null) mousePositionAction.action.Enable();
        
        if (aimAction?.action != null)
        {
            aimAction.action.Enable();
            // Using Events instead of checking IsPressed in Update
            aimAction.action.performed += _ => IsAiming = true;
            aimAction.action.canceled += _ => IsAiming = false;
        }
    }

    private void Start()
    {
        mainCamera = Camera.main;
        if (mainCamera == null)
        {
            Debug.LogError("FATAL: Main Camera tag not found. Aiming will fail.");
        }
    }

    private void OnDestroy()
    {
        if (mousePositionAction?.action != null) mousePositionAction.action.Disable();
        
        if (aimAction?.action != null)
        {
            aimAction.action.performed -= _ => IsAiming = true;
            aimAction.action.canceled -= _ => IsAiming = false;
            aimAction.action.Disable();
        }
    }

    private void Update()
    {
        // Even with events for state, we rotate in Update so the player 
        // tracks the mouse even if the mouse/player stops moving.
        if (IsAiming)
        {
            HandleRotation();
        }
    }

    private void HandleRotation()
    {
        if (GetMouseWorldPositionOnGround(out var targetPosition))
        {
            Vector3 lookDir = targetPosition - transform.position;
            lookDir.y = 0; // Keep player upright

            if (lookDir.sqrMagnitude > 0.01f)
            {
                transform.rotation = Quaternion.LookRotation(lookDir);
            }
        }
    }

    public bool TryGetAimDirection(Vector3 origin, out Vector3 direction)
    {
        if (GetMouseWorldPositionOnGround(out var targetPosition))
        {
            direction = (targetPosition - origin).normalized;
            return true;
        }

        direction = transform.forward;
        return false;
    }

    private bool GetMouseWorldPositionOnGround(out Vector3 worldPosition)
    {
        if (mainCamera == null)
        {
            worldPosition = Vector3.zero;
            return false;
        }

        var mouseScreenPosition = mousePositionAction.action.ReadValue<Vector2>();
        var ray = mainCamera.ScreenPointToRay(mouseScreenPosition);

        if (Physics.Raycast(ray, out RaycastHit hit, maxRaycastDistance, groundMask))
        {
            worldPosition = hit.point;
            return true;
        }

        worldPosition = Vector3.zero;
        return false;
    }
}