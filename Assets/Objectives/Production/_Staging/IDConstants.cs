using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;

public static class IDConstants
{
    /////////////////////
    // INVENTORY ITEMS //
    /////////////////////
    
    // QUEST ITEMS //
    public const string FLASHLIGHT_INV_ID = "6d81adca-c9f4-44bd-8523-ce3f820faa19";
    public const string CREW_QUARTERS_KEY = "3d92ec05-bb06-4964-b865-815664154bda";
    public const string INFECTED_SAMPLE = "66603036-9a3d-4d39-9ee4-1539d00cbb6b";
    public const string BRIDGE_KEY = "05be5693-3432-4da3-9e32-89df470151c9";
    public const string STATION_DATA = "e5f91c28-d50f-43a2-b167-a86c559f44cd";

    // WEAPONS //
    public const string PISTOL = "e0d787fd-841a-4d18-80aa-a714d392ec94";
    public const string PISTOL_MK7 = "6ebdcfdc-4e88-49d7-954a-143f52bdba91";
    public const string SHOTGUN_RATTLESNAKE_V = "82e898b7-6a7b-41cd-996b-e1907dc4981a";
    public const string TASER = "3f4dd43a-e99b-4c1b-9c6d-3712e4113175";
    public const string STEELPIPE = "bbb47cb0-3019-4cc0-9392-b1f78e038035";

    // CONSUMABLES //
    public const string PISTOL_AMMO = "45723e17-4905-4e9b-8214-e0fd74a87e13";
    // public const string PISTOL_MK7_AMMO = "45723e17-4905-4e9b-8214-e0fd74a87e13";
    public const string BATTERIES = "6164535a-3b6b-426b-9e1a-b57d180f9eba";
    public const string SHOTGUN_AMMO = "9c776512-d310-40af-ae44-d51b07abe9b6";
    public const string ADRENALINE = "f6d080bd-dd76-413e-b14a-c1574ab16fba";
    public const string HEALTH_PACK = "7c3dc583-e342-4708-a4ba-67e35fa17fde";
    
    
    //public const string HEALTH_PACK = "ABC123";



    ////////////////
    // OBJECTIVES //
    ////////////////

    public const string OBJECTIVE_01_ID = "e58ad942-7804-4300-96a0-9d491166dc4f"; //Flashlight
    public const string OBJECTIVE_02_ID = "4553e647-8ac9-4a60-a82d-4c2b25dc402f"; //Central Corridor
    public const string OBJECTIVE_03_ID = "9260c37a-3ab7-4bb8-810c-26343331fc1d";
    public const string OBJECTIVE_04_ID = "f7051371-8cfd-4c9c-acf8-6700da31cd66";
    public const string OBJECTIVE_05_ID = "c85d2b1e-29eb-418f-a70a-0976131276b8";
    public const string OBJECTIVE_06_ID = "7f07fe96-6982-49fe-8027-32367337eb1e"; // Gain access to the crew quarters
    // 07
    // 08
    // 09

    ///////////////////
    // CONVERSATIONS //
    ///////////////////

    public const string CONVERSATION_BOB_00 = "50791dc5-7571-4ef8-930c-08a63d7ebec5";
    public const string CONVERSATION_BOB_01 = "1fe47f51-40fc-4ef2-b03f-b6d2208e927c";
    public const string CONVERSATION_BOB_03 = "4790b499-362a-4574-9739-bf5366148028";

    public const string CONVERSATION_WHAT_THAT_NOISE = "58fba2a0-d48a-4037-81d8-e7c8987bf1bd";
    public const string CONVERSATION_BOB_FINAL = "57442e47-c215-4a48-acfb-57c24e94dec4";

    public const string BIG_NOISE_LINE = "ce8065e4-7a78-4d2d-959f-e38f02d5ed9c";



    public static List<string> GetAllWeapons()
    {
        List<string> allWeapons = new List<string>();
        allWeapons.Add(PISTOL);
        allWeapons.Add(PISTOL_MK7);
        allWeapons.Add(TASER);
        allWeapons.Add(STEELPIPE);
        allWeapons.Add(SHOTGUN_RATTLESNAKE_V);
        
        // etc..
        return allWeapons;
    }

}
