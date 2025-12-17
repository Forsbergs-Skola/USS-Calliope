using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    public InputActionReference moveAction;
    public float moveSpeed = 5f;
    public float rotationSpeed = 10f;
    public float isoYawDegrees = 45f;
    private CharacterController cc;

    void Awake() => cc = GetComponent<CharacterController>();
    void OnEnable() => moveAction.action.Enable();
    void OnDisable() => moveAction.action.Disable();

    void Update()
    {
        Vector2 input = moveAction.action.ReadValue<Vector2>();

        Vector3 inputDir = new Vector3(input.x, 0, input.y);

        if (inputDir.sqrMagnitude < 0.001f)
            return;

        Quaternion isoRotation = Quaternion.Euler(0, isoYawDegrees, 0);
        Vector3 moveDir = isoRotation * inputDir;
        moveDir.Normalize();

        cc.Move(moveDir * (moveSpeed * Time.deltaTime));
        
        Quaternion targetRotation = Quaternion.LookRotation(moveDir, Vector3.up);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
    }
}
