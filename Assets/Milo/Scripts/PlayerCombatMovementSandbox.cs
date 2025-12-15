using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class PlayerCombatPrototypeMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 5f;

    [Header("Input")]
    [SerializeField] private InputActionReference moveAction; // drag your Move action here in Inspector

    private CharacterController controller;
    private Vector2 moveInput;

    private void OnEnable()
    {
        moveAction.action.Enable();
    }

    private void OnDisable()
    {
        moveAction.action.Disable();
    }

    private void Awake()
    {
        controller = GetComponent<CharacterController>();
    }

    private void Update()
    {
        // Read input from the action
        moveInput = moveAction.action.ReadValue<Vector2>();

        // Convert 2D input to 3D movement
        Vector3 move = transform.right * moveInput.x + transform.forward * moveInput.y;

        controller.SimpleMove(move * moveSpeed);
    }
}