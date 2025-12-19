using UnityEngine;

public enum EnumPlayerStatusEffect
{
    BLEEDING,
    POISON,
    IN_STEALTH
    // add/remove more as needed
}

public enum EnumAttackType
{
    MELEE,
    RANGED
}

public enum EnumWeaponType
{
    NONE,
    PISTOL,
    SHOTGUN
    // add/remove more as needed
}

public enum EnumInventoryItem
{
    FLASHLIGHT,
    PISTOL,
    SHOTGUN,
    RIFLE
    // add or remove more as needed
}

public enum EnumInventoryResource
{
    PISTOL_AMMO,
    RIFLE_AMMO,
    SHOTGUN_AMMO,
    HEALTH_PACK
    // add or remove more as needed
}

public enum EnumObjective
{
    //TALK_TO_BOB,
    //TALK_TO_ALICE,
    //DO_A_LITTLE_DANCE,
    //MAKE_A_LITTLE_LOVE,
    //GET_DOWN_TONIGHT
    ENTER_THE_LAB,
    TALK_TO_ALICE,
    DEFEAT_ALICE,
    DEFEAT_FOUR_ENEMIES
}
public enum EnumObjectiveStatus
{
    NOT_STARTED,
    STARTED,
    FINISHED
}

public enum EnumProgressionField
{
    TALKED_TO_ALICE, 
    LAB_DISCOVERED,
    //TALKED_TO_BOB,
    DEFEATED_ENEMIES_COUNT,
    DEFEATED_ENEMIES
}

public enum EnumValueComparison
{
    //IS_TRUE,
    //IS_FALSE,
    GREATER_OR_EQUAL,
    LESS_OR_EQUAL,
    EQUAL,
    NOT_EQUAL,
    CONTAINS
}
/*
public enum EnumComparisonType
{
    BOOL,
    INT,
    FLOAT,
    STRING_LIST
}
*/
