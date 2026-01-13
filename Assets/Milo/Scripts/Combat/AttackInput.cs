using UnityEngine;
using UnityEngine.InputSystem;
using System;

public class AttackInput : MonoBehaviour
{
    [Header("Input References")]
    [SerializeField] private InputActionReference mousePosAction;
    [SerializeField] private InputActionReference aimAction;
    [SerializeField] private InputActionReference shootAction;
    [SerializeField] private InputActionReference switchWeaponAction;
    [SerializeField] private InputActionReference reloadAction;
    [SerializeField] private InputActionReference unEquipWeaponAction;
    [SerializeField] private InputActionReference lockOnAction;

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
    
    public event Action SwitchWeaponTriggered;
    
    public event Action ReloadTriggered;

    public event Action UnEquipWeaponTriggered;

    private void Awake()
    {
        Enable(mousePosAction);
        Enable(aimAction);
        Enable(shootAction);
        Enable(switchWeaponAction); 
        Enable(unEquipWeaponAction);
        Enable(lockOnAction);

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

        if (switchWeaponAction?.action != null)
        {
            switchWeaponAction.action.performed += OnSwitchWeaponPerformed;
        }
        
        if (reloadAction?.action != null)
        {
            reloadAction.action.Enable();
            reloadAction.action.performed += OnReloadPerformed;
        }

        if (unEquipWeaponAction?.action != null)
        {
            unEquipWeaponAction.action.performed += OnUnEquipWeapon;
        }
            
        if (lockOnAction?.action != null)
        {
            lockOnAction.action.performed += OnLockOnPerformed;
            lockOnAction.action.canceled += OnLockCanceled;
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
        
        if (switchWeaponAction?.action != null)
        {
            switchWeaponAction.action.performed -= OnSwitchWeaponPerformed;
        }
        
        if (reloadAction?.action != null)
        {
            reloadAction.action.performed -= OnReloadPerformed;
            reloadAction.action.Disable();
        }

        if (unEquipWeaponAction?.action != null)
        {
            unEquipWeaponAction.action.performed -= OnUnEquipWeapon;
        }

        if (lockOnAction?.action != null)
        {
            lockOnAction.action.performed -= OnLockOnPerformed;
            lockOnAction.action.canceled -= OnLockCanceled;
        }

        Disable(mousePosAction);
        Disable(aimAction);
        Disable(shootAction);
        Disable(switchWeaponAction);
        Disable(unEquipWeaponAction);
        Disable(lockOnAction);
    }

    public Vector2 GetMousePosition()
    {
        if (mousePosAction?.action == null)
            return Vector2.zero;

        return mousePosAction.action.ReadValue<Vector2>();
    }

    private void Update()
    {
        if (mousePosAction?.action == null) return;
        var mousePos = mousePosAction.action.ReadValue<Vector2>();
        MouseMoved?.Invoke(mousePos);
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
    
    private void OnSwitchWeaponPerformed(InputAction.CallbackContext ctx)
    {
        SwitchWeaponTriggered?.Invoke();
    }
    
    private void OnReloadPerformed(InputAction.CallbackContext ctx)
    {
        ReloadTriggered?.Invoke();
    }

    private void OnUnEquipWeapon(InputAction.CallbackContext ctx)
    {
        UnEquipWeaponTriggered?.Invoke();
    }

    private void OnLockOnPerformed(InputAction.CallbackContext ctx)
    {
        // Implement lock-on functionality here
    }

    private void OnLockCanceled(InputAction.CallbackContext ctx)
    {
        // Implement unlock functionality here
    }   
}
