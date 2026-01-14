using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class PlayerData : IRuntimeData
// All the save-worthy player information
{
    public bool IsSandbox { get; private set; }

    private int _health;
    private int _xp;
    private int _stamina;
    private EnumWeaponType _equippedWeapon;
    private List<EnumPlayerStatusEffect> _activeStatusEffects;

    private int _playerCurrentAmmo;

    // new...
    private EnumPlayerArchetype _archetype;

    private int _aimSkill;
    private int _sneakSkill;
    private int _strengthSkill;
    private int _techSkill;
    private int _barterSkill;
    private int _agility;

    private Vector3 _lastPosition = Vector3.zero;

    // TODO: Update the SaveService conversion tools for these...

    ///////////////////
    // Public Access //
    ///////////////////

    // LIST FIELDS


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

    public int Stamina
    {
        get => _stamina;
        set
        {
            if (_stamina == value) return;
            _stamina = Mathf.Clamp(value, 0, Constants.MAX_PLAYER_STAMINA);
            DataTools.HandleOnDataChanged(this);
        }
    }
    
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

    // new...
    public int AimSkill
    {
        get => _aimSkill;
        set
        {
            if (value == _aimSkill) return;
            _aimSkill = Mathf.Clamp(value ,0, Constants.MAX_PLAYER_AIM);
            DataTools.HandleOnDataChanged(this);
        }
    }
    public int SneakSkill
    {
        get => _sneakSkill;
        set
        {
            if (value == _sneakSkill) return;
            _sneakSkill = Mathf.Clamp(value, 0, Constants.MAX_PLAYER_SNEAK);
            DataTools.HandleOnDataChanged(this);
        }
    }
    public int StrengthSkill
    {
        get => _strengthSkill;
        set
        {
            if (value == _strengthSkill) return;
            _strengthSkill = Mathf.Clamp(value, 0, Constants.MAX_PLAYER_STRENGTH);
            DataTools.HandleOnDataChanged(this);
        }
    }

    public int TechSkill
    {
        get => _techSkill;
        set
        {
            if (value == _techSkill) return;
            _techSkill = Mathf.Clamp(value, 0, Constants.MAX_PLAYER_TECH);
            DataTools.HandleOnDataChanged(this);
        }
    }
    public int BarterSkill
    {
        get => _barterSkill;
        set
        {
            if (value == _barterSkill) return;
            _barterSkill = Mathf.Clamp(value, 0, Constants.MAX_PLAYER_BARTER);
            DataTools.HandleOnDataChanged(this);
        }
    }
    public int Agility
    {
        get => _agility;
        set
        {
            if (value == _agility) return;
            _agility = Mathf.Clamp(value, 0, Constants.MAX_PLAYER_AGILITY);
            DataTools.HandleOnDataChanged(this);
        }
    }

    public EnumPlayerArchetype Archetype
    {
        get => _archetype;
        set
        {
            if (value == _archetype) return;
            _archetype = value;
            DataTools.HandleOnDataChanged(this);
        }
    }

    public Vector3 LastPosition
    {
        get => _lastPosition;
        set
        {
            if (value == _lastPosition) return;
            _lastPosition = value;
            DataTools.HandleOnDataChanged(this);
        }
    }

    public int PlayerCurrentAmmo
    {
        get => _playerCurrentAmmo;
        set
        {
            if (value == _playerCurrentAmmo) return;
            _playerCurrentAmmo = value;
            DataTools.HandleOnDataChanged(this);
        }
    }

    //////////////////
    // Constructors //
    //////////////////
    public PlayerData()
    {
        IsSandbox = false;
        Health = Constants.MAX_PLAYER_HEALTH;
        XP = 0;
        Stamina = Constants.MAX_PLAYER_STAMINA;
        EquippedWeapon = EnumWeaponType.NONE;
        _activeStatusEffects = new List<EnumPlayerStatusEffect>();

        Archetype = EnumPlayerArchetype.NONE;

        AimSkill = 0;
        SneakSkill = 0;
        StrengthSkill = 0;
        TechSkill = 0;
        BarterSkill = 0;
        Agility = 0;
        PlayerCurrentAmmo = 0;

        LastPosition = Vector3.zero;
    }
    public PlayerData(bool isSandbox)
    {
        IsSandbox = isSandbox;
        Health = Constants.MAX_PLAYER_HEALTH;
        XP = 0;
        Stamina = Constants.MAX_PLAYER_STAMINA;
        EquippedWeapon = EnumWeaponType.NONE;
        _activeStatusEffects = new List<EnumPlayerStatusEffect>();

        Archetype = EnumPlayerArchetype.NONE;

        AimSkill = 0;
        SneakSkill = 0;
        StrengthSkill = 0;
        TechSkill = 0;
        BarterSkill = 0;
        Agility = 0;
        PlayerCurrentAmmo = 0;

        LastPosition = Vector3.zero;
    }

    public PlayerData(PlayerData inData)
    {
        IsSandbox = false;
        Health = inData.Health;
        XP = inData.XP;
        Stamina = inData.Stamina;
        EquippedWeapon = inData.EquippedWeapon;
        _activeStatusEffects = inData.GetActiveStatusEffects();

        Archetype = inData.Archetype;

        AimSkill = inData.AimSkill;
        SneakSkill = inData.SneakSkill;
        StrengthSkill = inData.SneakSkill;
        TechSkill = inData.TechSkill;
        BarterSkill = inData.BarterSkill;
        Agility = inData.Agility;
        PlayerCurrentAmmo = inData.PlayerCurrentAmmo;

        LastPosition = inData.LastPosition;
    }
    public bool GetIsSandbox() { return IsSandbox; }
}
