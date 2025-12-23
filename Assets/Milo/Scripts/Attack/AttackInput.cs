using UnityEngine;
using UnityEngine.InputSystem;
using System;

public class AttackInput : MonoBehaviour
{
    [Header("Input References")]
    [SerializeField] private InputActionReference mousePosAction;
    [SerializeField] private InputActionReference aimAction;
    [SerializeField] private InputActionReference shootAction;
    
    [Header("Extras, later make full PlayerInput script")]
    // for the movement inaccuracy
    [SerializeField] private InputActionReference moveAction;
    [SerializeField] private InputActionReference sprintAction;

    public InputActionReference MoveAction => moveAction;
    public InputActionReference SprintAction => sprintAction;
    
    public event Action<Vector2> MouseMoved;
    public event Action AimStarted;
    public event Action AimStopped;
    public event Action FireStarted;
    public event Action FireStopped;

    private void Awake()
    {
        Enable(mousePosAction);
        Enable(aimAction);
        Enable(shootAction);

        if (aimAction?.action != null)
        {
            aimAction.action.started += OnAimPerformed;
            aimAction.action.canceled += OnAimCanceled;
        }

        if (shootAction?.action != null)
        {
            shootAction.action.started += OnShootStarted;
            shootAction.action.canceled += OnShootCanceled;
        }
    }

    private void OnDestroy()
    {
        if (aimAction?.action != null)
        {
            aimAction.action.started -= OnAimPerformed;
            aimAction.action.canceled -= OnAimCanceled;
        }

        if (shootAction?.action != null)
        {
            shootAction.action.started -= OnShootStarted;
            shootAction.action.canceled -= OnShootCanceled;
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
        if (reference?.action is { enabled: false })
            reference.action.Enable();
    }

    private static void Disable(InputActionReference reference)
    {
        if (reference?.action is { enabled: true })
            reference.action.Disable();
    }
    
    private void OnAimPerformed(InputAction.CallbackContext ctx)
    {
        
        AimStarted?.Invoke();
    }

    private void OnAimCanceled(InputAction.CallbackContext ctx)
    {
        
        AimStopped?.Invoke();
    }

    private void OnShootStarted(InputAction.CallbackContext ctx)
    {
        FireStarted?.Invoke();
    }

    private void OnShootCanceled(InputAction.CallbackContext ctx)
    {
        FireStopped?.Invoke();
    }
}
