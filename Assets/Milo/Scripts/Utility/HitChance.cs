
/*
 *  Three Type of Hit chances Red, Orange, Green
 *  Calculated depending on current stamina, health, movement, weapon impact range, weapon spread
 *  Before Calculating normalize values to 0.0 - 1.0
 */

public class HitChance
{
    public float MaxStamina;
    public float MaxHealth;
    
    public void FinalChance(PlayerState player)
    {
        float score = 1f;
        
        var staminaPercentage = player.CurrentStamina / player.MaxStamina;
        var healthPercentage = player.CurrentHealth / player.MaxHealth;
        
        score *= staminaPercentage;
        score *= healthPercentage;

        if (player.IsSprinting)
        {
            score *= 0.1f;
        }
        else if (player.Is)
        
    }

    public void RedHitChance()
    {
        
    }

    public void OrangeHitChance()
    {
        
    }

    public void GreenHitChance()
    {
        
    }
}
