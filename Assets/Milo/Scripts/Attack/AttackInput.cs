using UnityEngine;
using UnityEngine.InputSystem;
using System;

public class AttackInput : MonoBehaviour
{
    [Header("Input References")]
    [SerializeField] private InputActionReference mousePosAction;
    [SerializeField] private InputActionReference aimAction;
    [SerializeField] private InputActionReference shootAction;
    
    public event Action<Vector2> MouseMoved;
    public event Action AimStarted;
    public event Action AimStopped;
    public event Action FireRequested;

    private void Awake()
    {
        Enable(mousePosAction);
        Enable(aimAction);
        Enable(shootAction);

        if (aimAction?.action != null)
        {
            Debug.Log("Aiming");
            aimAction.action.started += OnAimPerformed;
            aimAction.action.canceled  += OnAimCanceled;
        }

        if (shootAction?.action != null)
        {
            shootAction.action.performed += OnShootPerformed;
        }
    }

    private void OnDestroy()
    {
        if (aimAction?.action != null)
        {
            aimAction.action.started -= OnAimPerformed;
            aimAction.action.canceled  -= OnAimCanceled;
        }

        if (shootAction?.action != null)
        {
            shootAction.action.performed -= OnShootPerformed;
        }

        Disable(mousePosAction);
        Disable(aimAction);
        Disable(shootAction);
    }

    private void Update()
    {
        if (mousePosAction?.action != null)
        {
            Vector2 mousePos = mousePosAction.action.ReadValue<Vector2>();
            MouseMoved?.Invoke(mousePos);
        }
    }

    private static void Enable(InputActionReference reference)
    {
        if (reference?.action != null && !reference.action.enabled)
            reference.action.Enable();
    }

    private static void Disable(InputActionReference reference)
    {
        if (reference?.action != null && reference.action.enabled)
            reference.action.Disable();
    }
    
    // ── Callbacks ───────────────────────────────
    private void OnAimPerformed(InputAction.CallbackContext ctx)
    {
        
        AimStarted?.Invoke();
    }

    private void OnAimCanceled(InputAction.CallbackContext ctx)
    {
        
        AimStopped?.Invoke();
    }

    private void OnShootPerformed(InputAction.CallbackContext ctx)
    {
        
        FireRequested?.Invoke();
    }
}
