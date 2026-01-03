using UnityEngine;

public static class HitChance
{
    public static float CurrentHitChanceScore { get; set; }

    public static float GetHitChanceScore(PlayerState player, SO_WeaponType weapon, float distance)
    {
        var distanceRatio = Mathf.Clamp01(distance / weapon.ImpactRange);
        var score = weapon.DamageOverDistance.Evaluate(distanceRatio);

        var staminaPercentage = (float)player.CurrentStamina / player.MaxStamina;
        var healthPercentage = (float)player.CurrentHealth / player.MaxHealth;
        if (staminaPercentage <= 0f) staminaPercentage = 0.1f;
        
        score *= staminaPercentage;
        score *= healthPercentage;

        if (player.IsSprinting) score *= 0.2f;
        else if (player.IsMoving) score *= 0.5f;

        return Mathf.Clamp01(score);
    }
}