using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class PlayerData : IRuntimeData
// All the save-worthy player information
{
    public bool IsSandbox { get; private set; }

    /////////////////
    // Data Fields //
    /////////////////

    // LIST FIELDS

    private List<EnumPlayerStatusEffect> _activeStatusEffects;
    public List<EnumPlayerStatusEffect> GetActiveStatusEffects()
    {
        return new List<EnumPlayerStatusEffect>(_activeStatusEffects);
    }
    public void AddActiveStatusEffect(EnumPlayerStatusEffect _effect)
    {
        if (_activeStatusEffects.Contains(_effect)) return;
        _activeStatusEffects.Add(_effect);
        DataTools.HandleOnDataChanged(this);
    }
    public void RemoveActiveStatusEffect(EnumPlayerStatusEffect _effect)
    {
        if (!_activeStatusEffects.Contains(_effect)) return;
        _activeStatusEffects.Remove(_effect);
        DataTools.HandleOnDataChanged(this);
    }
    public void ClearAllActiveStatusEffects()
    {
        _activeStatusEffects.Clear();
        _activeStatusEffects = new List<EnumPlayerStatusEffect>();
        DataTools.HandleOnDataChanged(this);
    }

    // ATOMIC FIELDS

    private int _health;
    public int Health
    {
        get => _health;
        set
        {
            if (_health == value) return;
            _health = Mathf.Clamp(value, 0, Constants.MAX_PLAYER_HEALTH);
            DataTools.HandleOnDataChanged(this);
        }
    }

    private EnumWeaponType _equippedWeapon;
    public EnumWeaponType EquippedWeapon
    {
        get => _equippedWeapon;
        set
        {
            if (value == _equippedWeapon) return;
            _equippedWeapon = value;
            DataTools.HandleOnDataChanged(this);
        }
    }

    private int _xp;
    public int XP
    {
        get => _xp;
        set
        {
            if (_xp == value) return;
            _xp = Mathf.Clamp(value, 0, Constants.MAX_PLAYER_XP);
            DataTools.HandleOnDataChanged(this);
        }
    }

    // TODO... Other data fields

    //////////////////
    // Constructors //
    //////////////////
    public PlayerData()
    {
        IsSandbox = false;
        Health = Constants.MAX_PLAYER_HEALTH;
        XP = 0;
        EquippedWeapon = EnumWeaponType.NONE;
        _activeStatusEffects = new List<EnumPlayerStatusEffect>();
    }
    public PlayerData(bool isSandbox)
    {
        IsSandbox = isSandbox;
        Health = Constants.MAX_PLAYER_HEALTH;
        XP = 0;
        EquippedWeapon = EnumWeaponType.NONE;
        _activeStatusEffects = new List<EnumPlayerStatusEffect>();
    }

    public PlayerData(PlayerData inData)
    {
        IsSandbox = false;
        Health = inData.Health;
        XP = inData.XP;
        EquippedWeapon = inData.EquippedWeapon;
        _activeStatusEffects = inData.GetActiveStatusEffects();
    }
    public bool GetIsSandbox() { return IsSandbox; }
}
