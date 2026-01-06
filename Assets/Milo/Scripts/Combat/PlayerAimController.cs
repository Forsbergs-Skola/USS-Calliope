using UnityEngine;
using UnityEngine.UI;
using Unity.Cinemachine;

public class PlayerAimController : MonoBehaviour
{
    [SerializeField] private LayerMask groundMask;
    [SerializeField] private LayerMask enemyLayer;

    [Header("Aim Camera Settings")]
    [SerializeField] private float holdThreshold = 0.2f;
    [SerializeField] private float transitionSpeed = 5f;
    [SerializeField] private float smoothSpeed = 3f;
    [SerializeField] private float cameraOffsetDistance = 10f;

    [Header("UI")]
    [SerializeField] private Image crosshairImage;

    [SerializeField] private CinemachineCamera cineMachineCam;

    private AttackInput attackInput;
    private PlayerState playerState;
    private CinemachineCameraOffset offsetExtension;
    private Camera mainCamera;
    private Vector2 lastMousePos;
    private Vector3 currentTargetOffset;
    private PlayerWeaponHandler weaponHandler;
    private Animator animator;

    private float holdTimer = 0f;
    private float currentAimWeight = 0f;
    private bool isHoldingButton = false;

    private int unEquippedAnim = 0;

    private const float AimHeightOffset = 0.56f;

    public bool IsAiming { get; private set; }

    private void Awake()
    {
        weaponHandler = GetComponent<PlayerWeaponHandler>();
        attackInput = GetComponent<AttackInput>();
        playerState = GetComponent<PlayerState>();
        animator = GetComponent<Animator>();
        mainCamera = Camera.main;

        if (cineMachineCam)
        {
            offsetExtension = cineMachineCam.GetComponent<CinemachineCameraOffset>();
            if (!offsetExtension)
                offsetExtension = cineMachineCam.gameObject.AddComponent<CinemachineCameraOffset>();
        }

        if (crosshairImage)
            crosshairImage.enabled = false;

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
        isHoldingButton = false;
        IsAiming = false;
        holdTimer = 0f;
        if (crosshairImage) crosshairImage.enabled = false;
    }

    private void Update()
    {
        lastMousePos = attackInput.GetMousePosition();

        if (isHoldingButton)
        {
            holdTimer += Time.deltaTime;
            if (holdTimer >= holdThreshold) IsAiming = true;
        }

        float targetWeight = IsAiming ? 1f : 0f;
        currentAimWeight = Mathf.MoveTowards(currentAimWeight, targetWeight, Time.deltaTime * transitionSpeed);

        UpdateAimLogic();

        if (!IsAiming)
        {
            int currentAnimation = animator.GetInteger("WeaponType");
            if (currentAnimation == 0) return;
            animator.SetInteger("WeaponType", (int)unEquippedAnim);
        }
    }

    private void UpdateAimLogic()
    {
        if (!mainCamera) return;

        var ray = mainCamera.ScreenPointToRay(lastMousePos);
        var aimPlane = new Plane(Vector3.up, transform.position + Vector3.up * AimHeightOffset);

        Vector3 targetPosition;
        Vector3 desiredOffset = Vector3.zero;

        if (aimPlane.Raycast(ray, out float enter))
        {
            targetPosition = ray.GetPoint(enter);
        }
        else
        {
            return;
        }

        bool hitEnemy = Physics.Raycast(ray, out RaycastHit enemyHit, 100f, enemyLayer);

        if (IsAiming)
        {
            if (weaponHandler.CurrentWeaponData != null)
            {
                animator.SetInteger("WeaponType", (int)weaponHandler.CurrentWeaponData.TypeOfWeapon);
            }
                
            var lookDir = targetPosition - transform.position;
            lookDir.y = 0f;
            if (lookDir.sqrMagnitude > 0.01f)
                transform.rotation = Quaternion.LookRotation(lookDir);

            var pullVector = targetPosition - transform.position;
            pullVector.y = 0f;

            desiredOffset = Vector3.ClampMagnitude(pullVector * 0.5f, cameraOffsetDistance) * currentAimWeight;

            if (hitEnemy)
            {
                var dist = Vector3.Distance(transform.position, enemyHit.collider.bounds.center);
                var score = HitChance.GetHitChanceScore(playerState, weaponHandler.CurrentWeaponData, dist);
                UpdateCrosshairColor(score);
            }
            else
            {
                if (crosshairImage) crosshairImage.color = Color.white;
            }
        }

        UpdateCrosshairPosition(targetPosition);

        if (offsetExtension)
        {
            currentTargetOffset = Vector3.Lerp(currentTargetOffset, desiredOffset, Time.deltaTime * smoothSpeed);
            offsetExtension.Offset = currentTargetOffset;
        }
    }

    private void UpdateCrosshairColor(float score)
    {
        if (!crosshairImage) return;

        crosshairImage.color = score switch
        {
            >= 0.8f => Color.green,
            >= 0.4f => Color.yellow,
            _ => Color.red
        };
    }

    private void UpdateCrosshairPosition(Vector3 worldPos)
    {
        if (!crosshairImage) return;

        crosshairImage.enabled = IsAiming;
        if (!IsAiming) return;

        Vector3 screenPos = mainCamera.WorldToScreenPoint(worldPos);

        screenPos.x = Mathf.Clamp(screenPos.x, 0, Screen.width);
        screenPos.y = Mathf.Clamp(screenPos.y, 0, Screen.height);

        crosshairImage.transform.position = screenPos;
    }

    public bool TryGetAimDirection(Vector3 origin, out Vector3 direction)
    {
        var ray = mainCamera.ScreenPointToRay(lastMousePos);
        var aimPlane = new Plane(Vector3.up, transform.position + Vector3.up * AimHeightOffset);

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