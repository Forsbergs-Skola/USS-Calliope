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
    public string PLAYER_EquippedWeapon;


    /////////////////////
    // ProgressionData //
    /////////////////////
    public string PROGRESSION_defeatedEnemiesString;
    public string PROGRESSION_sceneName;

    public bool PROGRESSION_TalkedToBob;
    public bool PROGRESSION_TalkedToAlice;



}