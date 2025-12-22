using UnityEngine;

public class PlayerAimController : MonoBehaviour
{
    [SerializeField] private LayerMask groundMask;
    [SerializeField] private float aimHeightOffset = 1.2f;
    [SerializeField] private AttackInput attackInput;

    private Camera mainCamera;
    public bool IsAiming { get; private set; }
    private Vector2 lastMousePos;

    private void Awake()
    {
        mainCamera = Camera.main;
        if (!mainCamera) Debug.LogError("Main camera not found!");
    }

    private void OnEnable()
    {
        attackInput.AimStarted += OnAimStarted;
        attackInput.AimStopped += OnAimStopped;
        attackInput.MouseMoved += OnMouseMoved;
    }

    private void OnDisable()
    {
        attackInput.AimStarted -= OnAimStarted;
        attackInput.AimStopped -= OnAimStopped;
        attackInput.MouseMoved -= OnMouseMoved;
    }

    private void OnAimStarted() => IsAiming = true;
    private void OnAimStopped()  => IsAiming = false;

    private void OnMouseMoved(Vector2 mousePos)
    {
        lastMousePos = mousePos;
        if (IsAiming)
        {
            HandleRotation();
        }
    }

    private void HandleRotation()
    {
        if (!mainCamera) return;

        Ray ray = mainCamera.ScreenPointToRay(lastMousePos);
        Plane aimPlane = new Plane(Vector3.up, transform.position + Vector3.up * aimHeightOffset);

        if (aimPlane.Raycast(ray, out float enter))
        {
            Vector3 targetPosition = ray.GetPoint(enter);
            Vector3 lookDir = targetPosition - transform.position;
            lookDir.y = 0;

            if (lookDir.sqrMagnitude > 0.01f)
                transform.rotation = Quaternion.LookRotation(lookDir);
        }
    }

    public bool TryGetAimDirection(Vector3 origin, out Vector3 direction)
    {
        Ray ray = mainCamera.ScreenPointToRay(lastMousePos);
        Plane aimPlane = new Plane(Vector3.up, transform.position + Vector3.up * aimHeightOffset);

        if (aimPlane.Raycast(ray, out float enter))
        {
            Vector3 target = ray.GetPoint(enter);
            direction = (target - origin).normalized;
            return true;
        }

        direction = transform.forward;
        return false;
    }
}
