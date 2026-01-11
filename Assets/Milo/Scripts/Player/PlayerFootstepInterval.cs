using UnityEngine;

public class PlayerFootstepInterval : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Olle.Scripts.PlayerController playerController;
    [SerializeField] private PlayerFootstepAudio footstepAudio;

    [Header("Settings")]
    [Range(0.8f, 1.5f)]
    [SerializeField] private float speedScale = 1.15f;

    private float _stepTimer;
    private bool _wasMoving;

    private PlayerStamina playerStamina;


    private void Awake()
    {
        playerStamina = playerController.GetComponent<PlayerStamina>();
    }

    private void Update()
    {
        bool isMoving = playerController.IsMoving;

        if (!isMoving)
        {
            _stepTimer = 0f;
            _wasMoving = false;
            return;
        }

        if (!_wasMoving)
        {
            _stepTimer = GetCurrentInterval() * 0.2f;
            _wasMoving = true;
        }

        _stepTimer += Time.deltaTime;

        float currentInterval = GetCurrentInterval() * speedScale;

        if (_stepTimer >= currentInterval)
        {
            footstepAudio?.PlayFootstep();
            _stepTimer -= currentInterval;
        }
    }

    private float GetCurrentInterval()
    {
        if (playerController.IsCrouching) return playerController.crouchStepInterval;
        if (playerController.IsSprinting && playerStamina.isTired == false) return playerController.runStepInterval;
        return playerController.walkStepInterval;
    }
}