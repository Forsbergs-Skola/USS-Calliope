using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerAimController : MonoBehaviour
{
    [SerializeField] private LayerMask groundMask;
    [SerializeField] private float maxRaycastDistance = 200f;

    [Header("Input")] [SerializeField] private InputActionReference mousePositionAction;

    private Camera mainCamera;

    private void Awake()
    {
        if (mousePositionAction != null && mousePositionAction.action != null)
        {
            mousePositionAction.action.Enable();
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
        if (mousePositionAction != null && mousePositionAction.action != null)
        {
            mousePositionAction.action.Disable();
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