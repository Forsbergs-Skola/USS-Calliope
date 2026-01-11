using UnityEngine;

namespace Olle.Scripts
{
    public class PlayerAnimationController : MonoBehaviour
    {
        [SerializeField] private Animator animator;
        [SerializeField] private float smoothing = 0.1f;

        private static readonly int MoveXHash = Animator.StringToHash("MoveX");
        private static readonly int MoveYHash = Animator.StringToHash("MoveY");
        private static readonly int SpeedHash = Animator.StringToHash("Speed");
        private static readonly int SprintHash = Animator.StringToHash("IsSprinting");
        private static readonly int TurnLeftHash = Animator.StringToHash("TurnLeft90");
        private static readonly int TurnRightHash = Animator.StringToHash("TurnRight90");

        private void Reset()
        {
            animator = GetComponent<Animator>();
        }

        public void UpdateMovement(Vector2 moveInput, bool isSprinting)
        {
            if (animator == null) return;

            float inputMagnitude = moveInput.magnitude;
            animator.SetFloat(SpeedHash, inputMagnitude);

            Vector2 clampedInput = Vector2.ClampMagnitude(moveInput, 1f);

            animator.SetFloat(MoveXHash, clampedInput.x, smoothing, Time.deltaTime);
            animator.SetFloat(MoveYHash, clampedInput.y, smoothing, Time.deltaTime);

            animator.SetBool(SprintHash, isSprinting);
        }

        public void SetIdle()
        {
            if (animator == null) return;

            animator.SetFloat(MoveXHash, 0f);
            animator.SetFloat(MoveYHash, 0f);
            animator.SetFloat(SpeedHash, 0f);
            animator.SetBool(SprintHash, false);
        }

        public void PlayTurnAnimation(float angle)
        {
            if (animator == null) return;

            if (angle > 45f)
                animator.SetTrigger(TurnRightHash);
            else if (angle < -45f)
                animator.SetTrigger(TurnLeftHash);
        }

        public void Punch()
        {
            animator.SetTrigger("PunchTrigger");
        }
    }

    // In the animation controller, weapontype int determines which weapon is equipped, 0 = unarmed, 1 = pistol, 2 = rifle/shotgun 3 = Melee
}
