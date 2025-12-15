using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 5f;
    public float runMoveSpeed = 8f;
    public float crouchMoveSpeed = 2f;

    [Header("Crouch")]
    public float crouchScaleY = 0.5f;

    [Header("View")]
    public Camera mainCamera;
    public LayerMask groundMask = ~0;

    [Header("Noise")]
    public float walkStepInterval = 0.4f;
    public float runStepInterval  = 0.25f;
    public float crouchStepInterval = 0.6f;

    Rigidbody rb;
    Vector3 inputDir;
    Vector2 moveInput;
    bool wantsToRun;
    bool isCrouching;

    float defaultScaleY;
    float defaultMoveSpeed;

    PlayerNoiseEmitter noise;
    float noiseTimer;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        if (mainCamera == null)
            mainCamera = Camera.main;

        defaultScaleY = transform.localScale.y;
        defaultMoveSpeed = moveSpeed;

        noise = GetComponent<PlayerNoiseEmitter>();
    }

    // These will be called from PlayerInput Events
    public void OnMove(InputAction.CallbackContext ctx)
    {
        moveInput = ctx.ReadValue<Vector2>();
    }

    public void OnRun(InputAction.CallbackContext ctx)
    {
        wantsToRun = ctx.ReadValue<float>() > 0.5f;
    }

    public void OnCrouch(InputAction.CallbackContext ctx)
    {
        if (ctx.performed)
        {
            isCrouching = !isCrouching;
            ApplyCrouchState();
        }
    }

    void Update()
    {
        Vector3 move = new Vector3(moveInput.x, 0f, moveInput.y);
        move = Vector3.ClampMagnitude(move, 1f);

        Vector3 camForward = mainCamera.transform.forward;
        camForward.y = 0f;
        camForward.Normalize();

        Vector3 camRight = mainCamera.transform.right;
        camRight.y = 0f;
        camRight.Normalize();

        inputDir = camForward * move.z + camRight * move.x;

        bool isMoving = inputDir.sqrMagnitude > 0.01f;

        if (!isCrouching && wantsToRun && isMoving)
            moveSpeed = runMoveSpeed;
        else if (isCrouching)
            moveSpeed = crouchMoveSpeed;
        else
            moveSpeed = defaultMoveSpeed;

        if (isMoving && noise != null)
        {
            noiseTimer += Time.deltaTime;

            if (!isCrouching && wantsToRun)
            {
                if (noiseTimer >= runStepInterval)
                {
                    noise.EmitRun();
                    noiseTimer = 0f;
                }
            }
            else if (isCrouching)
            {
                if (noiseTimer >= crouchStepInterval)
                {
                    noise.EmitCrouch();
                    noiseTimer = 0f;
                }
            }
            else
            {
                if (noiseTimer >= walkStepInterval)
                {
                    noise.EmitWalk();
                    noiseTimer = 0f;
                }
            }
        }
        else
        {
            noiseTimer = 0f;
        }
    }

    void FixedUpdate()
    {
        if (inputDir.sqrMagnitude > 0.0001f)
        {
            Vector3 targetPos = rb.position + inputDir * moveSpeed * Time.fixedDeltaTime;
            rb.MovePosition(targetPos);
        }

        Ray ray = mainCamera.ScreenPointToRay(Mouse.current.position.ReadValue());
        if (Physics.Raycast(ray, out RaycastHit hit, 1000f, groundMask))
        {
            Vector3 lookPos = hit.point;
            lookPos.y = rb.position.y;

            Vector3 lookDir = lookPos - rb.position;
            if (lookDir.sqrMagnitude > 0.0001f)
            {
                Quaternion targetRot = Quaternion.LookRotation(lookDir);
                rb.MoveRotation(targetRot);
            }
        }
    }

    void ApplyCrouchState()
    {
        Vector3 scale = transform.localScale;
        scale.y = isCrouching ? defaultScaleY * crouchScaleY : defaultScaleY;
        transform.localScale = scale;
    }
}
