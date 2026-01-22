# Data Management

## Overview
This guide explains how to use the shared data and data events in our project. It assumes the current architecture: runtime ScriptableObjects, a DataController & an EventRelay service in the bootstrap scene.

## Core concepts

* Runtime data classes (e.g., `PlayerData`, `InventoryData`, `ProgressionData`) are plain C# classes that hold save-worthy state and know whether or not they’re in sandbox, i.e. unit testing, mode (`IsSandbox`).

* Runtime data assets (`PlayerRuntimeData`, `InventoryRuntimeData`, `ProgressionRuntimeData`) are ScriptableObjects that hold a runtime instance of those classes in their Value field.

* `DataController` exists in the bootstrap scene, owns the runtime assets, and sets them up for “real” games (non-sandbox).

* `EventRelay` [Event_System_Documentation.md](link) is the global event hub; when non-sandbox data changes, it fires `DataUpdatedEvent`.

## Where to put gameplay logic

* *DO NOT* add new logic into the ScriptableObjects themselves (other than the minimal setup they already have).

* *DO* put gameplay logic in MonoBehaviours (e.g., combat, UI, enemy AI) that reference the runtime data assets and operate on Value.

* *DO* keep save/load logic inside `SaveService` and initialization logic inside `DataController`.

## Using runtime data in gameplay (normal, non-sandbox game flow)

*In production gameplay (started via the bootstrap scene)...*

The `DataController` exists and will:

* Clear any old data when starting a new game.

* Create new `PlayerData`, `InventoryData`, `ProgressionData` instances (with `IsSandbox` = false)...

* Or assign loaded data from `SaveService`.

* Trigger `DataUpdatedEvent` once after initialization or load.

### Pattern for any system that needs player state:


    public class PlayerHealthUI : MonoBehaviour
    {
         private void OnEnable()
         {
            EventRelay.Instance.GameEvents.DataUpdatedEvent.OnEventTriggered += HandleDataUpdated;
            HandleDataUpdated(); // initial refresh
         }
        private void OnDisable()
        {
            EventRelay.Instance.GameEvents.DataUpdatedEvent.OnEventTriggered -= HandleDataUpdated;
        }
        private void HandleDataUpdated()
        {
            if (DataController.Instance == null) return;
            int health = DataController.Instance.PlayerRuntimeData.Value.Health;
            // Update UI with health
        }
    }

(for list fields, see Appendix 1, "Working with Collections Fields", [below](#appendix-1))

Key points:

* Always access runtime by one of the following means:
** Through `DataController.Instance.[Player/Inventory/Progression]RuntimeData.Value` in real game scenes. Here, assume `Value` is non-null once the game has started, but always null-check defensively.
** By serializing a reference to the appropriate runtime data SO in your script. Here, the data holder will detect whether or not it's in sandbox mode and initialize accordingly (see below). 

## Using runtime data in sandbox scenes

*Sandbox scenes are for independent testing without the bootstrap...*

* There is no `DataController`.

* The runtime asset will detect this and set its `Value.IsSandbox` to *true*.

* You can drag the runtime data ScriptableObjects from the Project tab directly into scripts and work with Value freely.

Example:

    public class TestPlayer : MonoBehaviour
    {
        [SerializeField] private PlayerRuntimeData playerData;

        private void Start()
        {
            StartCoroutine(TestTakingDamage());
        }

        private System.Collections.IEnumerator TestTakingDamage()
        {
            while (true)
            {
                yield return new WaitForSeconds(3f);
                playerData.Value.Health -= 10;
            }
        }
    }

(for list fields, see Appendix 1, "Working with Collections Fields", [below](#appendix-1))

Sandbox rules:

* In sandboxes, *do not* assume `DataController.Instance` exists.

* Changes still go through the same runtime data API, but `IsSandbox` == true, so they will not trigger global data-updated logic that relies on the controller.

## How data change events work
[Event_System_Documentation.md](EventRelay documentation)

`PlayerData`, `InventoryData` & `ProgressionData` (and other future data classes, as needed) call `DataTools.HandleOnDataChanged(this);` from their property setters when values change.

`DataTools` checks:

* If `data.GetIsSandbox()` is true → ignore.

* If `EventRelay.Instance` is null → ignore.

* Otherwise → call `EventRelay.Instance.GameEvents.DataUpdatedEvent.TriggerEvent();`

You can subcribe to `EventRelay.Instance.GameEvents.DataUpdatedEvent.OnEventTriggered` from anywhere in the game, then look at the updated data by reference to the `DataController`.

* ...for example `DataController.Instance.PlayerRuntimeData.Value.Health` to read the player's current health.

### Implications for devs:

* Always mutate data via properties/methods that call DataTools, for example: `playerData.Value.Health -= 10;`

* Do not add public fields and mutate them directly, or the event won’t fire.

* If you add new fields (e.g., XP, Level, etc.), implement them like Health:

    * Private backing field.
    * Public property that clamps/validates and calls `DataTools.HandleOnDataChanged(this);`
    * ...or just DM Brad

## Save / load responsibilities
To save the game, use the dedicated save path, for example from a menu:

    public class SaveMenu : MonoBehaviour
    {
        public void OnSaveButtonPressed()
        {
            var dc = DataController.Instance;
            if (dc == null) return;
            var player = dc.PlayerRuntimeData.Value;
            var inv    = dc.InventoryRuntimeData.Value;
            var prog   = dc.ProgressionRuntimeData.Value;
            // Optional: early-out if any are sandbox
            if (player.IsSandbox || inv.IsSandbox || prog.IsSandbox) return;
            SaveService.Save(player, inv, prog);
        }
    }

### SaveService is responsible for:

* Transforming GameData into SaveData.

* Writing to disk.

* Loading from disk and constructing GameData.

* Triggering SavedGameLoadedEvent(gameData).

`DataController` listens to `SavedGameLoadedEvent`, assigns the new instances into runtime assets, and fires `DataUpdatedEvent` once.

## Adding new runtime data

When you need new save-worthy data:

* Extend the data class (e.g., `PlayerData`) with:

    * A new private field.

    * A public property that validates and calls DataTools.HandleOnDataChanged(this);.

    * Update SaveData and the conversion helpers in SaveService.

* ...or just DM Brad

## Quick checklist for devs

Need player/quest/inventory state in a real scene?

* Use `DataController.Instance.[Player/Inventory/Progression]RuntimeData.Value`.

Need player/quest/inventory state in a sandbox scene?

* Drag the appropriate RuntimeData (ScriptableObject) asset into your script and use `.Value`.

Need to react to any data change?

* Subscribe to `EventRelay.Instance.GameEvents.DataUpdatedEvent.`

Adding a new data field?

* Backing field + property + `DataTools.HandleOnDataChanged(this);`.

* ...or DM Brad

Saving?

* Only save when all involved data objects have `IsSandbox` == false.

## Appendix 1
### Working with Collections Fields

Collection type fields (List & Dictionary) are a little different, because adding and removing items does dot trigger the setter logic.

For these, we provide custom public methods to add, remove & clear items, instead of exposing a public field. Player status effects, for example:

*On the back-end:*

    public enum EnumPlayerStatusEffect
    {
        BLEEDING,
        POISON,
        IN_STEALTH
        // add/remove more as needed
    }

    public class PlayerData : IRuntimeData
    {
        
        // ...
    
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
    
        // ...
    
    }

*Usage pattern, from outside the player prefab:*

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

*Usage pattern, from inside the player prefab:*

    public class TestPlayer : MonoBehaviour
    {
        [SerializeField] private PlayerRuntimeData playerData;

        // ...

        private void Blah()
        {
            /////////////////////////////////////////
            // GET A LIST OF ACTIVE STATUS EFFECTS //
            /////////////////////////////////////////
            playerData.Value.GetActiveStatusEffects();

            //////////////////////////////////////
            // ADD STATUS EFFECTS TO THE PLAYER //
            //////////////////////////////////////
            playerData.Value.AddActiveStatusEffect(EnumPlayerStatusEffect.IN_STEALTH);
            playerData.Value.AddActiveStatusEffect(EnumPlayerStatusEffect.BLEEDING);
            // ...etc

            ///////////////////////////////////////////
            // REMOVE STATUS EFFECTS FROM THE PLAYER //
            ///////////////////////////////////////////
            playerData.Value.RemoveActiveStatusEffect(EnumPlayerStatusEffect.POISON);

            //////////////////////////////
            // CLEAR ALL STATUS EFFECTS //
            //////////////////////////////
            playerData.Value.ClearAllActiveStatusEffects();
        }

    }