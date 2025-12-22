using Unity.Cinemachine;
using UnityEngine;

public class PlayerAimController : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private LayerMask groundMask;
    [SerializeField] private float aimHeightOffset = 1.2f;
    [SerializeField] private float cameraOffsetDistance = 5f;
    
    [Header("Timing & Delay")]
    [Tooltip("How long to hold RMB before camera starts pulling (Click vs Hold)")]
    [SerializeField] private float holdThreshold = 0.2f; 
    [Tooltip("How fast the player enters the combat stance")]
    [SerializeField] private float transitionSpeed = 5f; 
    [Tooltip("How 'heavy' the camera feels when following the mouse")]
    [SerializeField] private float smoothSpeed = 3f;     

    [Header("References")]
    [SerializeField] private AttackInput attackInput;
    [SerializeField] private Transform crosshairTransform;
    [SerializeField] private CinemachineCamera vCam;
    
    private CinemachineCameraOffset offsetExtension;
    private Camera mainCamera;
    private Vector2 lastMousePos;
    private Vector3 currentTargetOffset;
    
    // Internal Logic State
    private float holdTimer = 0f;
    private float currentAimWeight = 0f; 
    private bool isHoldingButton = false;

    public bool IsAiming { get; private set; }

    private void Awake()
    {
        mainCamera = Camera.main;
        if (!mainCamera) Debug.LogError("Main camera not found!");
        
        if (vCam != null)
        {
            offsetExtension = vCam.GetComponent<CinemachineCameraOffset>();
            if (offsetExtension == null)
                offsetExtension = vCam.gameObject.AddComponent<CinemachineCameraOffset>();
        }

        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Confined;
    }

    private void OnEnable()
    {
        if (attackInput == null) return;
        attackInput.AimStarted += OnAimInputStarted;
        attackInput.AimStopped += OnAimInputStopped;
        attackInput.MouseMoved += (pos) => lastMousePos = pos;
    }

    private void OnDisable()
    {
        if (attackInput == null) return;
        attackInput.AimStarted -= OnAimInputStarted;
        attackInput.AimStopped -= OnAimInputStopped;
    }

    private void OnAimInputStarted() => isHoldingButton = true;

    private void OnAimInputStopped()
    {
        // If they released the button before the threshold, it's a Click
        if (isHoldingButton && holdTimer < holdThreshold)
        {
            PerformQuickClickAction();
        }

        isHoldingButton = false;
        IsAiming = false;
        holdTimer = 0f;
    }

    private void PerformQuickClickAction()
    {
        // This is where you'd trigger your Zomboid-style Context Menu
        Debug.Log("Zomboid Quick Click: Opening Interaction Menu...");
    }

    private void Update()
    {
        // 1. Determine if we have held long enough to be 'Aiming'
        if (isHoldingButton)
        {
            holdTimer += Time.deltaTime;
            if (holdTimer >= holdThreshold)
            {
                IsAiming = true;
            }
        }

        // 2. Transition the weight (0 to 1) for a smooth 'stance' delay
        float targetWeight = IsAiming ? 1f : 0f;
        currentAimWeight = Mathf.MoveTowards(currentAimWeight, targetWeight, Time.deltaTime * transitionSpeed);

        UpdateAimLogic();
    }

    private void UpdateAimLogic()
    {
        if (!mainCamera) return;

        Ray ray = mainCamera.ScreenPointToRay(lastMousePos);
        Plane aimPlane = new Plane(Vector3.up, transform.position + Vector3.up * aimHeightOffset);

        Vector3 desiredOffset = Vector3.zero;

        if (aimPlane.Raycast(ray, out float enter))
        {
            Vector3 targetPosition = ray.GetPoint(enter);
            
            if (IsAiming)
            {
                // Rotate Player towards target
                Vector3 lookDir = targetPosition - transform.position;
                lookDir.y = 0f;
                if (lookDir.sqrMagnitude > 0.01f)
                    transform.rotation = Quaternion.LookRotation(lookDir);
                
                // Calculate the 'Pull' vector from player to mouse
                Vector3 pullVector = targetPosition - transform.position;
                pullVector.y = 0;

                // Move camera toward mouse, scaled by our current stance weight
                desiredOffset = Vector3.ClampMagnitude(pullVector * 0.5f, cameraOffsetDistance) * currentAimWeight;
            }

            // Update Crosshair position
            if (crosshairTransform)
            {
                crosshairTransform.position = targetPosition + Vector3.up * 0.05f; 
                crosshairTransform.LookAt(mainCamera.transform);
            }
        }

        // 3. Apply the final offset to Cinemachine
        if (offsetExtension != null)
        {
            // Ensure CinemachineCameraOffset is set to 'World Space' in Inspector
            currentTargetOffset = Vector3.Lerp(currentTargetOffset, desiredOffset, Time.deltaTime * smoothSpeed);
            offsetExtension.Offset = currentTargetOffset;
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