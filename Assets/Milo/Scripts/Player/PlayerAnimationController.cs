using UnityEngine;


public class PlayerAnimationController : MonoBehaviour
{
    [SerializeField] private Animator animator;

    private static readonly int MoveXHash = Animator.StringToHash("MoveX");
    private static readonly int MoveYHash = Animator.StringToHash("MoveY");

    private void Reset()
    {
        animator = GetComponent<Animator>();
    }

    public void UpdateMovement(Vector2 moveInput)
    {
        moveInput = Vector2.ClampMagnitude(moveInput, 1f);

        float inputMagnitude = moveInput.magnitude; 
        animator.SetFloat("Speed", inputMagnitude);

        Debug.Log($"Anim MoveX: {moveInput.x}, MoveY: {moveInput.y}, Speed: {inputMagnitude}");

        animator.SetFloat(MoveXHash, moveInput.x, 0.1f, Time.deltaTime);
        animator.SetFloat(MoveYHash, moveInput.y, 0.1f, Time.deltaTime);
    }


    public void SetIdle()
    {
        animator.SetFloat(MoveXHash, 0f);
        animator.SetFloat(MoveYHash, 0f);
    }
}

