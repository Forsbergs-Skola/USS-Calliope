using UnityEngine;

public class PlayerStamina : MonoBehaviour
{
    public float maxStamina = 100f;
    public float drainPerSecond = 20f;
    public float regenAmount = 5f;
    public float regenInterval = 0.5f;
    public float regenDelay = 2f;

    private float _currentStamina;
    public float currentStamina
    {
        get
        {
            return _currentStamina;
        }
        set
        {
            _currentStamina = value;
            UpdateBackend(_currentStamina);
        }
    }


    public bool isTired;
    public bool adrenalineRushActive;
    float _lastUseTime;
    float _regenTimer;

    void Awake()
    {
        currentStamina = maxStamina;
    }

    public void Tick(float deltaTime, bool isTryingToSprint, bool blockRegen, out bool canSprint)
    {
        canSprint = !isTired && currentStamina > 0f && isTryingToSprint;

        if (canSprint && !adrenalineRushActive)
        {
            float amount = drainPerSecond * deltaTime;
            UseStamina(amount);
        }
        else
        {
            if (!blockRegen)
                HandleRegen(deltaTime);
        }
    }

    void UseStamina(float amount)
    {
        float before = currentStamina;

        currentStamina -= amount;
        if (currentStamina < 0f)
            currentStamina = 0f;
        
        _lastUseTime = Time.time;
        _regenTimer = 0f;
        
        if (currentStamina <= 0f)
            isTired = true;
        /*
        if (!Mathf.Approximately(before, currentStamina))
            Debug.Log($"Stamina DRAIN: {currentStamina:0}/{maxStamina}", this);
        */
    }

    void HandleRegen(float deltaTime)
    {
        if (currentStamina >= maxStamina)
        {
            currentStamina = maxStamina;
            _regenTimer = 0f;
            
            if (isTired)
                isTired = false;

            return;
        }

        if (Time.time - _lastUseTime < regenDelay)
        {
            _regenTimer = 0f;
            return;
        }

        _regenTimer += deltaTime;
        if (_regenTimer >= regenInterval)
        {
            _regenTimer = 0f;

            float before = currentStamina;

            currentStamina += regenAmount;
            if (currentStamina > maxStamina)
                currentStamina = maxStamina;
            
            if (currentStamina >= maxStamina)
                isTired = false;

            // if (!Mathf.Approximately(before, currentStamina))
             //   Debug.Log($"Stamina REGEN: {currentStamina:0}/{maxStamina}", this);
        }
    }
    
    public float Normalized => currentStamina / maxStamina;
    public bool AdrenalineRushActive 
    { 
        get => adrenalineRushActive; 
        set 
        { 
            adrenalineRushActive = value;
            if (adrenalineRushActive)
            {
                currentStamina = maxStamina;
                isTired = false;
            }
            UpdateBackend(currentStamina);
        } 
    }

    private void UpdateBackend(float _stamina)
    {
        if (TryGetPlayerData(out PlayerData pData))
        {
            pData.Stamina = _stamina;
            pData.OnAdrenaline = adrenalineRushActive;
        }
    }

    private bool TryGetPlayerData(out PlayerData pData)
    {
        if (TryGetComponent<PlayerDataHandler>(out PlayerDataHandler pDataHandler))
        {
            pData = pDataHandler.RuntimeData.Value;
            return true;
        }
        pData = null;
        return false;
    }

}
