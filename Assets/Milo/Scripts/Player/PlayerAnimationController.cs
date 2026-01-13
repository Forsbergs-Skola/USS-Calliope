using UnityEngine;

    public class PlayerAnimationController : MonoBehaviour
    {
        [SerializeField] private Animator animator;
        [SerializeField] private float smoothing = 0.1f;

        private static readonly int MoveXHash = Animator.StringToHash("MoveX");
        private static readonly int MoveYHash = Animator.StringToHash("MoveY");
        private static readonly int SpeedHash = Animator.StringToHash("Speed");
        private static readonly int SprintHash = Animator.StringToHash("IsSprinting");

        public Animator Animator { get => animator; set => animator = value; }
        public float Smoothing { get => smoothing; set => smoothing = value; }

        public static int MoveXHash1 => MoveXHash;

         public static int MoveYHash1 => MoveYHash;

        public static int SpeedHash1 => SpeedHash;

        public static int SprintHash1 => SprintHash;

        private void Reset()
        {
            Animator = GetComponent<Animator>();
        }

        public void UpdateMovement(Vector2 moveInput, bool isSprinting)
        {
            if (Animator == null) return;

            float inputMagnitude = moveInput.magnitude;
            Animator.SetFloat(SpeedHash1, inputMagnitude);

            Vector2 clampedInput = Vector2.ClampMagnitude(moveInput, 1f);

            Animator.SetFloat(MoveXHash1, clampedInput.x, Smoothing, Time.deltaTime);
            Animator.SetFloat(MoveYHash1, clampedInput.y, Smoothing, Time.deltaTime);

            Animator.SetBool(SprintHash1, isSprinting);
        }

        public void SetIdle()
        {
            if (Animator == null) return;

            Animator.SetFloat(MoveXHash1, 0f);
            Animator.SetFloat(MoveYHash1, 0f);
            Animator.SetFloat(SpeedHash1, 0f);
            Animator.SetBool(SprintHash1, false);
        }

        public void Punch()
        {
            Animator.SetTrigger("PunchTrigger");
        }
    }

