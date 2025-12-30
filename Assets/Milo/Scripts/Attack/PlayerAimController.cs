using Unity.Cinemachine;
using UnityEngine;

public class PlayerAimController : MonoBehaviour
{
    [Header("Aim Settings")]
    [SerializeField] private LayerMask groundMask;
    [SerializeField] private LayerMask zombieLayer; // Added for detection
    [SerializeField] private float aimHeightOffset = 1.2f;
    [SerializeField] private float cameraOffsetDistance = 10f;
    
    [Header("Aim Camera Settings")]
    [SerializeField] private float holdThreshold = 0.2f; 
    [SerializeField] private float transitionSpeed = 5f; 
    [SerializeField] private float smoothSpeed = 3f;     

    [Header("References")]
    [SerializeField] private Transform crosshairTransform;
    [SerializeField] private CinemachineCamera cam;

    private AttackInput attackInput;
    private PlayerState playerState; // Added player state reference
    private CinemachineCameraOffset offsetExtension;
    private Camera mainCamera;
    private Vector2 lastMousePos;
    private Vector3 currentTargetOffset;
    private PlayerWeaponHandler weaponHandler;
    
    private HitChance hitLogic = new HitChance();
    private SpriteRenderer crosshairSprite;

    private float holdTimer = 0f;
    private float currentAimWeight = 0f; 
    private bool isHoldingButton = false;

    public bool IsAiming { get; private set; }

    private void Awake()
    {
        weaponHandler = GetComponent<PlayerWeaponHandler>();
        attackInput = GetComponent<AttackInput>();
        playerState = GetComponent<PlayerState>();
        mainCamera = Camera.main;
        
        if (cam)
        {
            offsetExtension = cam.GetComponent<CinemachineCameraOffset>();
            if (!offsetExtension) offsetExtension = cam.gameObject.AddComponent<CinemachineCameraOffset>();
        }

        if (crosshairTransform)
            crosshairSprite = crosshairTransform.GetComponent<SpriteRenderer>();

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
    private void OnAimInputStopped() { isHoldingButton = false; IsAiming = false; holdTimer = 0f; }

    private void Update()
    {
        if (isHoldingButton)
        {
            holdTimer += Time.deltaTime;
            if (holdTimer >= holdThreshold) IsAiming = true;
        }

        float targetWeight = IsAiming ? 1f : 0f;
        currentAimWeight = Mathf.MoveTowards(currentAimWeight, targetWeight, Time.deltaTime * transitionSpeed);

        UpdateAimLogic();
    }

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
                var lookDir = targetPosition - transform.position;
                lookDir.y = 0f;
                if (lookDir.sqrMagnitude > 0.01f) transform.rotation = Quaternion.LookRotation(lookDir);
                
                var pullVector = targetPosition - transform.position;
                pullVector.y = 0;
                desiredOffset = Vector3.ClampMagnitude(pullVector * 0.5f, cameraOffsetDistance) * currentAimWeight;

                var target = GetTargetNearMouse(targetPosition);
                if (target)
                {
                    var dist = Vector3.Distance(transform.position, target.transform.position);
                    var score = hitLogic.GetHitChanceScore(playerState, weaponHandler.CurrentWeapon, dist);
                    UpdateCrosshairColor(score);
                }
                else
                {
                    if (crosshairSprite) crosshairSprite.color = Color.white;
                }
            }

            if (crosshairTransform)
            {
                crosshairTransform.position = targetPosition + Vector3.up * 0.05f; 
                crosshairTransform.LookAt(mainCamera.transform);
                crosshairSprite.enabled = IsAiming;
            }
        }

        if (!offsetExtension) return;
        currentTargetOffset = Vector3.Lerp(currentTargetOffset, desiredOffset, Time.deltaTime * smoothSpeed);
        offsetExtension.Offset = currentTargetOffset;
    }
    
    private void UpdateCrosshairColor(float score)
    {
        if (!crosshairSprite) return;
        crosshairSprite.color = score switch
        {
            >= 0.8f => Color.green,
            >= 0.4f => new Color(1f, 0.5f, 0f),
            _ => Color.red
        };
    }

    private GameObject GetTargetNearMouse(Vector3 mouseWorldPos)
    {
        var targets = Physics.OverlapSphere(transform.position, weaponHandler.CurrentWeapon.ImpactRange, zombieLayer);
        GameObject bestTarget = null;
        var closestDistToMouse = 2.0f; 

        foreach (var col in targets)
        {
            var distToMouse = Vector3.Distance(col.transform.position, mouseWorldPos);
            if (!(distToMouse < closestDistToMouse)) continue;
            closestDistToMouse = distToMouse;
            bestTarget = col.gameObject;
        }
        return bestTarget;
    }

    public bool TryGetAimDirection(Vector3 origin, out Vector3 direction)
    {
        var ray = mainCamera.ScreenPointToRay(lastMousePos);
        var aimPlane = new Plane(Vector3.up, transform.position + Vector3.up * aimHeightOffset);

        if (aimPlane.Raycast(ray, out var enter))
        {
            var target = ray.GetPoint(enter);
            direction = (target - origin).normalized;
            return true;
        }

        direction = transform.forward;
        return false;
    }
}