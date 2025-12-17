using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class PlayerCombatPrototypeMovement : MonoBehaviour
{
    
    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 5f;

    [Header("Input")]
    [SerializeField] private InputActionReference moveAction;
    [SerializeField] private InputActionReference mousePositionAction; // NEW: Used for rotation

    private CharacterController controller;
    private Vector2 moveInput;
    private Camera mainCamera;

    private void OnEnable()
    {
        moveAction.action.Enable();
        if (mousePositionAction != null && mousePositionAction.action != null)
        {
            mousePositionAction.action.Enable();
        }
    }

    private void OnDisable()
    {
        moveAction.action.Disable();
        if (mousePositionAction != null && mousePositionAction.action != null)
        {
            mousePositionAction.action.Disable();
        }
    }

    private void Awake()
    {
        controller = GetComponent<CharacterController>();
        mainCamera = Camera.main;
        if (mainCamera == null)
        {
            Debug.LogError("Main Camera must be tagged 'MainCamera'. Rotation will be disabled.");
        }
    }

    private void Update()
    {
        moveInput = moveAction.action.ReadValue<Vector2>();
        Vector3 move = transform.right * moveInput.x + transform.forward * moveInput.y;
        controller.SimpleMove(move * moveSpeed);

        if (mainCamera != null && mousePositionAction != null && mousePositionAction.action != null)
        {
            Vector2 mouseScreenPosition = mousePositionAction.action.ReadValue<Vector2>();
            
            Ray ray = mainCamera.ScreenPointToRay(mouseScreenPosition);
            
            Plane groundPlane = new Plane(Vector3.up, transform.position.y); 
            
            if (groundPlane.Raycast(ray, out float distance))
            {
                Vector3 targetPoint = ray.GetPoint(distance);
                
                Vector3 lookDirection = targetPoint - transform.position;
                lookDirection.y = 0;
                
                if (lookDirection.sqrMagnitude > 0.01f)
                {
                    Quaternion targetRotation = Quaternion.LookRotation(lookDirection);
                    transform.rotation = targetRotation;
                }
            }
        }
    }
}