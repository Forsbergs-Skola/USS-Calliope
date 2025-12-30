using UnityEngine;

public class HitChance
{
    public float GetHitChanceScore(PlayerState player, SO_WeaponType weapon, float distance)
    {
        var distanceRatio = Mathf.Clamp01(distance / weapon.ImpactRange);
        var score = weapon.DamageOverDistance.Evaluate(distanceRatio);

        // Force float math by adding (float)
        var staminaPercentage = (float)player.CurrentStamina / player.MaxStamina;
        var healthPercentage = (float)player.CurrentHealth / player.MaxHealth;
    
        score *= staminaPercentage;
        score *= healthPercentage;

        if (player.IsSprinting) score /= weapon.SprintInaccuracyMultiplier;
        else if (player.IsMoving) score /= weapon.MovementInaccuracyMultiplier;

        return Mathf.Clamp01(score);
    }
}