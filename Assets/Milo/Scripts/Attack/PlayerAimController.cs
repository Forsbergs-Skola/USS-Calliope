using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.Serialization;

public class PlayerAimController : MonoBehaviour
{
    [Header("Aim Settings")]
    [SerializeField] private LayerMask groundMask;
    [SerializeField] private float aimHeightOffset = 1.2f;
    [SerializeField] private float cameraOffsetDistance = 10f;
    
    [Header("Aim Camera Settings")]
    [SerializeField] private float holdThreshold = 0.2f; 
    [Tooltip("How fast the player enters the combat stance")]
    [SerializeField] private float transitionSpeed = 5f; 
    [Tooltip("How 'heavy' the camera feels when following the mouse")]
    [SerializeField] private float smoothSpeed = 3f;     

    [Header("References")]
    [SerializeField] private Transform crosshairTransform;
    [SerializeField] private CinemachineCamera cam;
    
    private AttackInput attackInput;
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
        
        attackInput = GetComponent<AttackInput>();
        
        mainCamera = Camera.main;
        if (!mainCamera) Debug.LogError("Main camera not found!");
        
        if (cam)
        {
            offsetExtension = cam.GetComponent<CinemachineCameraOffset>();
            if (!offsetExtension)
                offsetExtension = cam.gameObject.AddComponent<CinemachineCameraOffset>();
        }

        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Confined;
    }

    private void OnEnable()
    {
        if (!attackInput) return;
        attackInput.AimStarted += OnAimInputStarted;
        attackInput.AimStopped += OnAimInputStopped;
        attackInput.MouseMoved += (pos) => lastMousePos = pos;
    }

    private void OnDisable()
    {
        if (!attackInput) return;
        attackInput.AimStarted -= OnAimInputStarted;
        attackInput.AimStopped -= OnAimInputStopped;
    }

    private void OnAimInputStarted() => isHoldingButton = true;

    private void OnAimInputStopped()
    {
        if (isHoldingButton && holdTimer < holdThreshold) return;
        
        isHoldingButton = false;
        IsAiming = false;
        holdTimer = 0f;
    }

    private void Update()
    {
        if (isHoldingButton)
        {
            holdTimer += Time.deltaTime;
            if (holdTimer >= holdThreshold)
            {
                IsAiming = true;
            }
        }

        float targetWeight = IsAiming ? 1f : 0f;
        currentAimWeight = Mathf.MoveTowards(currentAimWeight, targetWeight, Time.deltaTime * transitionSpeed);

        UpdateAimLogic();
    }

    // Refactor this spaghetti
    
    private void UpdateAimLogic()
    {
        if (!mainCamera) return;

        var ray = mainCamera.ScreenPointToRay(lastMousePos);
        var aimPlane = new Plane(Vector3.up, transform.position + Vector3.up * aimHeightOffset);

        var desiredOffset = Vector3.zero;

        if (aimPlane.Raycast(ray, out var enter))
        {
            var targetPosition = ray.GetPoint(enter);
            
            if (IsAiming)
            {
                Vector3 lookDir = targetPosition - transform.position;
                lookDir.y = 0f;
                if (lookDir.sqrMagnitude > 0.01f)
                    transform.rotation = Quaternion.LookRotation(lookDir);
                
                Vector3 pullVector = targetPosition - transform.position;
                pullVector.y = 0;

                desiredOffset = Vector3.ClampMagnitude(pullVector * 0.5f, cameraOffsetDistance) * currentAimWeight;
            }

            if (crosshairTransform)
            {
                crosshairTransform.position = targetPosition + Vector3.up * 0.05f; 
                crosshairTransform.LookAt(mainCamera.transform);
            }
        }

        if (!offsetExtension) return;
        currentTargetOffset = Vector3.Lerp(currentTargetOffset, desiredOffset, Time.deltaTime * smoothSpeed);
        offsetExtension.Offset = currentTargetOffset;
    }

    public bool TryGetAimDirection(Vector3 origin, out Vector3 direction)
    {
        var ray = mainCamera.ScreenPointToRay(lastMousePos);
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