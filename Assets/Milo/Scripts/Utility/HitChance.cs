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
        if (player.CurrentHealth <= 20) healthNormalized = 0.0f;

        score *= staminaNormalized + 0.2f;
        score *= healthNormalized + 0.3f;

        if (player.IsSprinting) score *= 0.2f;
        else if (player.IsMoving) score *= 0.7f;


        CurrentHitChanceScore = Mathf.Clamp01(score);
        return CurrentHitChanceScore;
    }

    private static float DistanceRatio(SO_WeaponType weapon, float distance)
    {
        float distanceRatio = Mathf.Clamp01(distance / weapon.ImpactRange);

        return weapon.DamageOverDistance.Evaluate(distanceRatio);
    }
}