using UnityEngine;




[System.Serializable]
public class SaveData
// JSON friendly data store
{   
    /////////////////
    // Player Data //
    /////////////////
    public string PLAYER_ActiveStatusEffectsString;

    public int PLAYER_Health;
    public int PLAYER_XP;
    public int PLAYER_Stamina;
    public string PLAYER_EquippedWeapon;
    public float PLAYER_PosX;
    public float PLAYER_PosY;
    public float PLAYER_PosZ;



    /////////////////////
    // ProgressionData //
    /////////////////////
    /*
    public string PROGRESSION_defeatedEnemiesString;
    public string PROGRESSION_sceneName;

    public bool PROGRESSION_TalkedToBob;
    public bool PROGRESSION_TalkedToAlice;
    */

    public string PROGRESSION_ObjectiveIDs;
    public string PROGRESSION_ObjectiveStatuses;
    public string PROGRESSION_EnemiesDefeated;
    public string PROGRESSION_SceneName;

    public bool PROGRESSION_DataDelivered;
    public bool PROGRESSION_CentralCorridorDiscovered;
    public bool PROGRESSION_BobContacted;
    public bool PROGRESSION_CrewQuartersUnlocked;
    public bool PROGRESSION_AlicaAndBobFuneralHeld;

    ///////////////////
    // InventoryData //
    ///////////////////

    public string INVENTORY_weaponsString;
    public string INVENTORY_questItemsString;
    public string INVENTORY_consumablesItemsString;
    public string INVENTORY_consumablesValuesString;



}