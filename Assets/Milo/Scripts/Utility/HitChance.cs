using UnityEngine;

public static class HitChance
{
    public static float CurrentHitChanceScore { get; private set; }

    public static float GetHitChanceScore(PlayerState player, SO_WeaponType weapon, float distance)
    {
       
        var score = DistanceRatio(weapon, distance);

        var staminaNormalized = Mathf.Clamp01(
            (float)player.CurrentStamina / player.MaxStamina);

        var healthNormalized = Mathf.Clamp01(
            (float)player.CurrentHealth / player.MaxHealth);

        if (player.CurrentStamina == 0) staminaNormalized = 0.1f;

        score *= staminaNormalized + 0.2f;
        score *= healthNormalized + 0.3f;

        if (player.IsSprinting()) score *= weapon.SprintInaccuracyMultiplier;
        else if (player.IsMoving()) score *= weapon.MovementInaccuracyMultiplier;


        if (player.CurrentHealth <= 15) score = 1f;

        CurrentHitChanceScore = Mathf.Clamp01(score);
        return CurrentHitChanceScore;
    }

    private static float DistanceRatio(SO_WeaponType weapon, float distance)
    {
        if (weapon == null) return 50f;
        float distanceRatio = Mathf.Clamp01(distance / weapon.ImpactRange);

        return weapon.DamageOverDistance.Evaluate(distanceRatio);
    }
}