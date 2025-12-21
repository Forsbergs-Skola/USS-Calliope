using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerAimController : MonoBehaviour
{
    [SerializeField] private LayerMask groundMask;
    [SerializeField] private float maxRaycastDistance = 200f;
    [SerializeField] private float aimHeightOffset = 1.2f; 

    [Header("Input References")] 
    [SerializeField] private InputActionReference mousePositionAction;
    [SerializeField] private InputActionReference aimAction; 

    private Camera mainCamera;

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
        
        if (IsAiming)
        {
            HandleRotation();
        }
    }

    private void HandleRotation()
    {
        if (GetMouseWorldPositionOnAimPlane(out var targetPosition))
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
        if (GetMouseWorldPositionOnAimPlane(out var targetPosition))
        {
            direction = (targetPosition - origin).normalized;
            return true;
        }

        direction = transform.forward;
        return false;
    }

    private bool GetMouseWorldPositionOnAimPlane(out Vector3 worldPosition)
    {
        if (!mainCamera)
        {
            worldPosition = Vector3.zero;
            return false;
        }

        Vector2 mouseScreenPosition = mousePositionAction.action.ReadValue<Vector2>();
        Ray ray = mainCamera.ScreenPointToRay(mouseScreenPosition);

        float aimPlaneHeight = transform.position.y + aimHeightOffset;
        Plane aimPlane = new Plane(Vector3.up, new Vector3(0f, aimPlaneHeight, 0f));

        if (aimPlane.Raycast(ray, out float enter))
        {
            worldPosition = ray.GetPoint(enter);
            return true;
        }

        worldPosition = Vector3.zero;
        return false;
    }

}