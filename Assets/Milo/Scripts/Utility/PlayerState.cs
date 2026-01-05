using UnityEngine;
using Olle.Scripts;


public class PlayerState : MonoBehaviour
{


    [SerializeField] private PlayerStamina stamina;
    [SerializeField] private PlayerHealthJavi health;
    [SerializeField] private AttackInput input;
    [SerializeField] private Olle.Scripts.PlayerController controller;

    public float CurrentHitChanceScore => HitChance.CurrentHitChanceScore;
    public float CurrentHealth => health.GetCurrentHealth();
    public float MaxHealth => health.GetMaxHealth();
    public float CurrentStamina => stamina.currentStamina;
    public float MaxStamina => stamina.maxStamina;

    public bool IsMoving => controller != null && controller.IsMoving;
    public bool IsSprinting => controller != null && controller.IsSprinting;
    public bool IsCrouching => controller != null && controller.IsCrouching;

}
