using UnityEngine;


/*
 * Milo:
 * To fetch runtime data for my HitChance calculation but can be useful for other things
 */

public class PlayerState : MonoBehaviour
{
    [SerializeField] private PlayerStamina stamina;
    [SerializeField] private PlayerHealthJavi health;
    [SerializeField] private AttackInput input;

    public float CurrentHealth => health.GetCurrentHealth();
    public float MaxHealth => health.GetMaxHealth();
    public float CurrentStamina => stamina.currentStamina;
    public float MaxStamina => stamina.maxStamina;
    
    public bool IsMoving => input.MoveAction.action.ReadValue<Vector2>().sqrMagnitude > 0.01f;
    public bool IsSprinting => input.SprintAction.action.ReadValue<float>() > 0.01f;
    
        
}
