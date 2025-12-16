using UnityEngine;
using System.Collections.Generic;

public class TestDialogueTrigger : MonoBehaviour
{

    [SerializeField] private string npcName = "Alice";




    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;
        if (DataController.Instance == null) return;

        DataController.Instance.ProgressionRuntimeData.Value.TalkedToAlice = true;

        if (DataController.Instance.ProgressionRuntimeData.Value.TalkedToBob)
        {
            // "I see you already met Bob"
        }
        else
        {
            // you should meet bob
        }

        return;

        ///////////////////////////////////
        // INTERACTING WITH A LIST FIELD //
        ///////////////////////////////////
        // Call the appropriate Add or Remove method. Don't operate on the list itself

        // To get a list of defeated enemies:
        List<string> defealtedEnemies = DataController.Instance.ProgressionRuntimeData.Value.GetDefeatedEnemies();
        // ^^ this will return a copy of the defeated enemies list for your internal use, not a reference to the source

        // When a new enemy is defeated
        DataController.Instance.ProgressionRuntimeData.Value.AddDefeatedEnemy("Joe");
        // ^^ the DataController will manage the list, and trigger the appropriate backend events

        // To "undefeat" a single enemy (we propbably won't need this, but here it is anyway):
        DataController.Instance.ProgressionRuntimeData.Value.RemoveDefeatedEnemy("Joe");

        //////////////////////////////////////
        // INTERACTING WITH AN ATOMIC FIELD //
        //////////////////////////////////////

        // Has the player talked to Bob?
        bool talkedToBob = DataController.Instance.ProgressionRuntimeData.Value.TalkedToBob;

        // When the player talks to Bob:
        DataController.Instance.ProgressionRuntimeData.Value.TalkedToBob = true;
        // ^^ here, you can sset the public value directly, and the setter function on the
        // backend will take care of the rest.

        /////////////////////////////////////////
        // GET A LIST OF ACTIVE STATUS EFFECTS //
        /////////////////////////////////////////
        DataController.Instance.PlayerRuntimeData.Value.GetActiveStatusEffects();

        //////////////////////////////////////
        // ADD STATUS EFFECTS TO THE PLAYER //
        //////////////////////////////////////
        DataController.Instance.PlayerRuntimeData.Value.AddActiveStatusEffect(EnumPlayerStatusEffect.IN_STEALTH);
        DataController.Instance.PlayerRuntimeData.Value.AddActiveStatusEffect(EnumPlayerStatusEffect.BLEEDING);
        // ...etc

        ///////////////////////////////////////////
        // REMOVE STATUS EFFECTS FROM THE PLAYER //
        ///////////////////////////////////////////
        DataController.Instance.PlayerRuntimeData.Value.RemoveActiveStatusEffect(EnumPlayerStatusEffect.POISON);

        //////////////////////////////
        // CLEAR ALL STATUS EFFECTS //
        //////////////////////////////
        DataController.Instance.PlayerRuntimeData.Value.ClearAllActiveStatusEffects();


    }
}
