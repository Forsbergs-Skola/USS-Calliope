# Data Management System Documentation

## Overview

This document describes the data management and save system implementation for the USS Calliope Unity project. The system provides a centralized, event-driven approach to managing runtime game state with support for sandbox testing, automatic change notifications, and persistent save/load functionality. The architecture separates runtime data classes from ScriptableObject containers, enabling flexible testing and production workflows.

## Architecture

The data management system follows a layered architecture with clear separation of concerns:

- **Data Layer**: Plain C# classes (`PlayerData`, `InventoryData`, `ProgressionData`) hold save-worthy state
- **Container Layer**: ScriptableObject runtime assets (`PlayerRuntimeData`, `InventoryRuntimeData`, `ProgressionRuntimeData`) hold runtime instances
- **Controller Layer**: `DataController` singleton manages data lifecycle and initialization
- **Service Layer**: `SaveService` handles persistence and serialization
- **Tool Layer**: `DataTools` provides utilities and automatic event triggering

## Core Components

### 1. Runtime Data Classes

Runtime data classes are plain C# classes that implement `IRuntimeData` and hold save-worthy game state.

#### `IRuntimeData` Interface
Base interface for all runtime data classes.

**Location**: `Assets/Brad/Scripts/DataModel/SaveService.cs`

**Methods**:
- `GetIsSandbox()`: Returns whether the data instance is in sandbox (testing) mode

**Purpose**: Enables polymorphic handling of different data types and sandbox detection

#### `PlayerData`
Holds all save-worthy player information.

**Location**: `Assets/Brad/Scripts/DataModel/PlayerData.cs`

**Key Properties**:
- `Health` (float): Current player health (clamped to max)
- `MaxHealth` (float): Maximum player health
- `Stamina` (float): Current player stamina
- `XP` (int): Player experience points
- `EquippedWeapon` (EnumWeaponType): Currently equipped weapon
- `LastPosition` (Vector3): Last saved player position
- `PlayerCurrentAmmo` (int): Current ammo count
- `OnAdrenaline` (bool): Whether player is on adrenaline

**Status Effects Management**:
- `GetActiveStatusEffects()`: Returns copy of active status effects list
- `AddActiveStatusEffect()`: Adds status effect (prevents duplicates)
- `RemoveActiveStatusEffect()`: Removes status effect
- `ClearAllActiveStatusEffects()`: Clears all status effects

**Design Pattern**: 
- Private backing fields with public properties
- Properties call `DataTools.HandleOnDataChanged(this)` on modification
- List operations use dedicated methods to trigger change events

#### `InventoryData`
Holds all save-worthy inventory information.

**Location**: `Assets/Brad/Scripts/DataModel/InventoryData.cs`

**Key Features**:
- Weapon item management
- Quest item management
- Consumable items with quantities (Dictionary)
- Exhausted pickup tracking

**Design Pattern**: Similar to `PlayerData` - private fields, public methods for collection manipulation

#### `ProgressionData`
Holds all save-worthy progression information.

**Location**: `Assets/Brad/Scripts/DataModel/ProgressionData.cs`

**Key Features**:
- Objective status tracking (Dictionary<string, EnumObjectiveStatus>)
- Defeated enemies list
- Exhausted breakables list
- Unlocked terminal world IDs
- Scene name tracking
- Boolean flags for story progression (DataDelivered, BobContacted, etc.)

**Design Pattern**: Similar to other data classes with read-only dictionary access

### 2. Runtime Data Assets (ScriptableObjects)

Runtime data assets are ScriptableObjects that contain references to runtime data instances.

#### `PlayerRuntimeData`
ScriptableObject container for `PlayerData` instance.

**Location**: `Assets/Brad/Scripts/DataModel/RuntimeDataAssets/PlayerRuntimeData.cs`

**Properties**:
- `Value` (PlayerData): The runtime data instance (non-serialized)

**Behavior**:
- `OnEnable()`: Initializes `Value` to null
- If `DataController.Instance` is null (sandbox mode), creates new `PlayerData(true)` (sandbox instance)
- If `DataController.Instance` exists, `Value` is set by `DataController`

**Purpose**: Enables Inspector assignment and sandbox testing

#### `InventoryRuntimeData`
ScriptableObject container for `InventoryData` instance.

**Location**: `Assets/Brad/Scripts/DataModel/RuntimeDataAssets/InventoryRuntimeData.cs`

**Behavior**: Similar to `PlayerRuntimeData` - auto-initializes in sandbox mode

#### `ProgressionRuntimeData`
ScriptableObject container for `ProgressionData` instance.

**Location**: `Assets/Brad/Scripts/DataModel/RuntimeDataAssets/ProgressionRuntimeData.cs`

**Behavior**: Similar to other runtime data assets

### 3. DataController: Central Hub

#### `DataController`
Singleton that manages runtime data lifecycle and initialization.

**Location**: `Assets/Brad/Scripts/DataModel/DataController.cs`

**Pattern**: Singleton (inherits from `Singleton<DataController>`)

**Properties**:
- `PlayerRuntimeData` (PlayerRuntimeData): Reference to player runtime data asset
- `ProgressionRuntimeData` (ProgressionRuntimeData): Reference to progression runtime data asset
- `InventoryRuntimeData` (InventoryRuntimeData): Reference to inventory runtime data asset

**Key Methods**:

- `InitializeRuntimeData()`: Called when new game starts
  - Clears existing save file
  - Wipes all runtime data
  - Creates new instances of all data classes (non-sandbox)
  - Assigns to runtime data assets
  - Resets objectives via `ObjectivesTracker`
  - Triggers `DataUpdatedEvent`

- `LoadSavedData(GameData gameData)`: Called when saved game is loaded
  - Wipes existing runtime data
  - Assigns loaded data to runtime data assets
  - Triggers `DataUpdatedEvent`
  - Loads the saved scene

**Event Subscriptions**:
- `NewGameStartedEvent`: Triggers `InitializeRuntimeData()`
- `SavedGameLoadedEvent`: Triggers `LoadSavedData()`

**Scene Setup**: 
- Prefab located at `Assets/Brad/Prefabs/DataController.prefab`
- Instantiated in `Bootstrap.unity` scene
- Uses DontDestroyOnLoad (via Singleton base class) to persist across scenes

### 4. SaveService: Persistence Layer

#### `SaveService`
Static service class that handles save/load operations.

**Location**: `Assets/Brad/Scripts/DataModel/SaveService.cs`

**Key Constants**:
- `saveFilePath`: Path to save file (`Application.persistentDataPath/save.json`)

**Public Methods**:

- `SaveExists()`: Checks if save file exists on disk

- `Save(PlayerData, InventoryData, ProgressionData)`: Saves game state
  - Clears existing save
  - Creates `GameData` from parameters
  - Converts `GameData` to `SaveData` (JSON-friendly format)
  - Writes JSON to disk
  - Triggers `GameSavedEvent`

- `Load()`: Loads game state from disk
  - Checks if save exists
  - Reads JSON from disk
  - Converts `SaveData` to `GameData`
  - Triggers `SavedGameLoadedEvent` with `GameData`

- `ClearSave()`: Deletes save file from disk

**Conversion Methods**:
- `GameDataToSaveData()`: Converts runtime data to serializable format
  - Handles list serialization via `StringListWrapper`
  - Converts enums to strings
  - Handles Dictionary serialization (objectives, consumables)

- `GetPlayerDataFromSaveData()`: Reconstructs `PlayerData` from `SaveData`
- `GetInventoryDataFromSaveData()`: Reconstructs `InventoryData` from `SaveData`
- `GetProgressionDataFromSaveData()`: Reconstructs `ProgressionData` from `SaveData`

**Serialization Strategy**:
- Uses Unity's `JsonUtility` for JSON serialization
- Lists wrapped in `StringListWrapper` for JSON compatibility
- Dictionaries converted to parallel lists (keys and values)
- Enums serialized as strings

### 5. DataTools: Utility Layer

#### `DataTools`
Static utility class providing data management helpers.

**Location**: `Assets/Brad/Scripts/DataModel/SaveService.cs`

**Key Methods**:

- `HandleOnDataChanged(IRuntimeData data)`: Called automatically when data changes
  - Checks if data is sandbox (returns early if true)
  - Checks if `EventRelay.Instance` exists (returns early if null)
  - Triggers `DataUpdatedEvent` (empty payload)
  - Triggers `RuntimeDataUpdatedEvent` (with data object)

**Purpose**: Provides automatic event triggering for data changes, enabling reactive systems

**List Utilities**:
- `GetWrapperizedStringList()`: Wraps list in `StringListWrapper` for JSON serialization
- `GetStringListFromJson()`: Extracts list from JSON string

### 6. Data Structures

#### `GameData`
Container class for all runtime data during save/load operations.

**Location**: `Assets/Brad/Scripts/DataModel/SaveService.cs`

**Properties**:
- `playerData` (PlayerData)
- `inventoryData` (InventoryData)
- `progressionData` (ProgressionData)

**Purpose**: Groups all data for save/load operations

#### `SaveData`
Serializable class for JSON persistence.

**Location**: `Assets/Brad/Scripts/DataModel/SaveData.cs`

**Properties**: 
- All fields prefixed with data type (e.g., `PLAYER_Health`, `INVENTORY_weaponsString`)
- String fields for serialized lists and dictionaries
- Primitive fields for atomic values

**Purpose**: JSON-friendly format for disk storage

#### `StringListWrapper`
Utility class for serializing lists to JSON.

**Location**: `Assets/Brad/Scripts/DataModel/SaveService.cs`

**Properties**:
- `strings` (List<string>): The list to serialize

**Purpose**: Unity's `JsonUtility` doesn't serialize generic lists directly; wrapper enables serialization

## System Flow

### New Game Initialization

1. User starts new game (triggers `NewGameStartedEvent`)
2. `DataController` receives event
3. `InitializeRuntimeData()` called
4. Save file cleared
5. Runtime data wiped (set to null)
6. New data instances created (non-sandbox)
7. Data assigned to runtime data assets
8. Objectives reset
9. `DataUpdatedEvent` triggered
10. Systems react to data update

### Save Game Flow

1. User triggers save (e.g., from pause menu)
2. System gets data from `DataController.Instance`
3. Validates data is not sandbox
4. `SaveService.Save()` called with data instances
5. `GameData` created from parameters
6. `GameData` converted to `SaveData`
7. `SaveData` serialized to JSON
8. JSON written to disk
9. `GameSavedEvent` triggered

### Load Game Flow

1. User triggers load
2. `SaveService.Load()` called
3. Save file existence checked
4. JSON read from disk
5. JSON deserialized to `SaveData`
6. `SaveData` converted to `GameData`
7. `SavedGameLoadedEvent` triggered with `GameData`
8. `DataController` receives event
9. `LoadSavedData()` called
10. Runtime data wiped
11. Loaded data assigned to runtime data assets
12. `DataUpdatedEvent` triggered
13. Scene loaded from `ProgressionData.SceneName`

### Data Change Flow

1. Code modifies data property (e.g., `playerData.Health = 50`)
2. Property setter called
3. Value validated/clamped
4. `DataTools.HandleOnDataChanged(this)` called
5. `DataTools` checks sandbox mode (skips if sandbox)
6. `DataTools` checks `EventRelay` existence (skips if null)
7. `DataUpdatedEvent` triggered
8. `RuntimeDataUpdatedEvent` triggered with data object
9. Subscribed systems react to change

## Usage Examples

### Accessing Runtime Data (Production)

```csharp
public class PlayerHealthUI : MonoBehaviour
{
    private void OnEnable()
    {
        if (EventRelay.Instance == null) return;
        EventRelay.Instance.GameEvents.DataUpdatedEvent.OnEventTriggered += HandleDataUpdated;
        HandleDataUpdated(); // Initial refresh
    }
    
    private void OnDisable()
    {
        if (EventRelay.Instance == null) return;
        EventRelay.Instance.GameEvents.DataUpdatedEvent.OnEventTriggered -= HandleDataUpdated;
    }
    
    private void HandleDataUpdated()
    {
        if (DataController.Instance == null) return;
        float health = DataController.Instance.PlayerRuntimeData.Value.Health;
        float maxHealth = DataController.Instance.PlayerRuntimeData.Value.MaxHealth;
        // Update UI with health values
    }
}
```

### Accessing Runtime Data (Sandbox)

```csharp
public class TestPlayer : MonoBehaviour
{
    [SerializeField] private PlayerRuntimeData playerData;
    
    private void Start()
    {
        // In sandbox, Value is auto-initialized
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
```

### Modifying Data

```csharp
// Atomic field modification
DataController.Instance.PlayerRuntimeData.Value.Health = 50f;

// List modification (use dedicated methods)
DataController.Instance.PlayerRuntimeData.Value.AddActiveStatusEffect(EnumPlayerStatusEffect.BLEEDING);
DataController.Instance.PlayerRuntimeData.Value.RemoveActiveStatusEffect(EnumPlayerStatusEffect.POISON);

// Dictionary modification (use dedicated methods)
DataController.Instance.ProgressionRuntimeData.Value.DefeatEnemy("Alice");
```

### Saving Game

```csharp
public class SaveMenu : MonoBehaviour
{
    public void OnSaveButtonPressed()
    {
        var dc = DataController.Instance;
        if (dc == null) return;
        
        var player = dc.PlayerRuntimeData.Value;
        var inv = dc.InventoryRuntimeData.Value;
        var prog = dc.ProgressionData.Value;
        
        // Optional: validate not sandbox
        if (player.GetIsSandbox() || inv.GetIsSandbox() || prog.GetIsSandbox()) return;
        
        SaveService.Save(player, inv, prog);
    }
}
```

### Loading Game

```csharp
public class LoadMenu : MonoBehaviour
{
    public void OnLoadButtonPressed()
    {
        if (!SaveService.SaveExists()) return;
        SaveService.Load(); // Triggers SavedGameLoadedEvent, DataController handles the rest
    }
}
```

### Reacting to Data Changes

```csharp
public class ObjectivesTracker : MonoBehaviour
{
    private void OnEnable()
    {
        EventRelay.Instance.GameEvents.RuntimeDataUpdatedEvent.OnEventTriggered += OnDataUpdated;
    }
    
    private void OnDisable()
    {
        EventRelay.Instance.GameEvents.RuntimeDataUpdatedEvent.OnEventTriggered -= OnDataUpdated;
    }
    
    private void OnDataUpdated(IRuntimeData data)
    {
        if (data is ProgressionData progressionData)
        {
            // React to progression data changes
            EvaluateObjectives(progressionData);
        }
    }
}
```

## Design Patterns and Principles

1. **Singleton Pattern**: `DataController` uses singleton for global access
2. **ScriptableObject Pattern**: Runtime data assets enable Inspector assignment and sandbox testing
3. **Observer Pattern**: Event-driven updates enable reactive systems
4. **Encapsulation**: Private backing fields with public properties ensure controlled access
5. **Separation of Concerns**: Data classes, containers, controller, and service have distinct responsibilities
6. **Sandbox Mode**: Enables testing without full game initialization

## Key Features

### Automatic Change Notifications
- Data modifications automatically trigger events
- Systems can react to any data change
- No manual event triggering required

### Sandbox Support
- Runtime data assets auto-initialize in sandbox mode
- Enables independent testing without `DataController`
- Sandbox changes don't trigger global events

### Type-Safe Access
- Strongly typed data classes
- Compile-time checking
- IntelliSense support

### Persistent Storage
- JSON-based save system
- Platform-independent save location
- Automatic serialization/deserialization

### Event Integration
- Integrates with project's event system
- `DataUpdatedEvent` for general notifications
- `RuntimeDataUpdatedEvent` for specific data type notifications

## Integration Points

### With [Event](Event_System_Documentation.md) System
- `DataTools.HandleOnDataChanged()` triggers events automatically
- `DataController` subscribes to game events
- `SaveService` triggers save/load events

### With [Objectives](Objectives_Tracking_System_Documentation.md) System
- `ObjectivesTracker` subscribes to `RuntimeDataUpdatedEvent`
- Reacts to `ProgressionData` changes
- Queries objective status from `ProgressionData`

### With [UI](UI_System_Documentation) System
- UI systems subscribe to `DataUpdatedEvent`
- Reactive UI updates based on data changes
- HUD updates when player data changes

### With Inventory System
- `InventoryController` queries `InventoryData`
- Inventory changes trigger data events
- UI updates reactively

### With [Dialogue](Dialogue_System_Documentation.md) System
- Dialogue system can query progression data
- Story flags stored in `ProgressionData`
- Enables conditional dialogue

## Technical Notes

### Sandbox Detection
- Runtime data assets check `DataController.Instance == null` in `OnEnable()`
- If null, creates sandbox instance (`new PlayerData(true)`)
- Sandbox instances have `IsSandbox = true`

### Change Event Suppression
- Sandbox data changes don't trigger events
- Prevents test data from affecting production systems
- `DataTools.HandleOnDataChanged()` checks `GetIsSandbox()`

### List Serialization
- Unity's `JsonUtility` doesn't serialize generic lists
- `StringListWrapper` used as workaround
- Lists converted to/from JSON strings

### Dictionary Serialization
- Dictionaries serialized as parallel lists (keys and values)
- Reconstructed during load
- Maintains key-value relationships

### Enum Serialization
- Enums serialized as strings
- `Enum.TryParse()` used for deserialization
- Case-insensitive parsing for robustness

### Non-Serialized Fields
- Runtime data `Value` fields marked `[System.NonSerialized]`
- Prevents Unity from serializing runtime instances
- Data only persists through save/load system

## Dependencies

- Project's singleton base class (`Singleton<T>`)
- Project's event system (`EventRelay`, `GameEvents`)
- Unity ScriptableObject system
- Unity JSON serialization (`JsonUtility`)
- Unity file I/O (`System.IO`)

## Best Practices

### Data Modification
Always use properties or dedicated methods:

```csharp
// Good: Uses property setter
playerData.Health = 50f;

// Good: Uses dedicated method
playerData.AddActiveStatusEffect(EnumPlayerStatusEffect.BLEEDING);

// Bad: Direct field access (won't trigger events)
playerData._health = 50f; // Compiler error - field is private
```

### Null Checking
Always check for `DataController.Instance`:

```csharp
if (DataController.Instance == null) return;
var health = DataController.Instance.PlayerRuntimeData.Value.Health;
```

### Event Subscription
Always unsubscribe in `OnDisable()`:

```csharp
private void OnEnable()
{
    EventRelay.Instance.GameEvents.DataUpdatedEvent.OnEventTriggered += HandleUpdate;
}

private void OnDisable()
{
    EventRelay.Instance.GameEvents.DataUpdatedEvent.OnEventTriggered -= HandleUpdate;
}
```

### Sandbox Testing
Use serialized references in sandbox scenes:

```csharp
[SerializeField] private PlayerRuntimeData playerData; // Drag asset in Inspector
// Value auto-initializes in sandbox mode
```

### Adding New Data Fields
Follow the established pattern:

1. Add private backing field
2. Add public property with validation
3. Call `DataTools.HandleOnDataChanged(this)` in setter
4. Update `SaveData` class
5. Update `SaveService` conversion methods

### Collection Fields
Use dedicated methods, not direct list access:

```csharp
// Good: Uses dedicated method
playerData.AddActiveStatusEffect(effect);

// Bad: Direct list manipulation (won't trigger events)
playerData.GetActiveStatusEffects().Add(effect); // Returns copy, won't work anyway
```

## Working with Collection Fields

Collection fields (List and Dictionary) require special handling because adding/removing items doesn't trigger property setters.

### Pattern for Collection Fields

**Backend Implementation**:

```csharp
public class PlayerData : IRuntimeData
{
    private List<EnumPlayerStatusEffect> _activeStatusEffects;
    
    // Getter returns copy (prevents external modification)
    public List<EnumPlayerStatusEffect> GetActiveStatusEffects()
    {
        return new List<EnumPlayerStatusEffect>(_activeStatusEffects);
    }
    
    // Dedicated add method
    public void AddActiveStatusEffect(EnumPlayerStatusEffect effect)
    {
        if (_activeStatusEffects.Contains(effect)) return;
        _activeStatusEffects.Add(effect);
        DataTools.HandleOnDataChanged(this); // Trigger event
    }
    
    // Dedicated remove method
    public void RemoveActiveStatusEffect(EnumPlayerStatusEffect effect)
    {
        if (!_activeStatusEffects.Contains(effect)) return;
        _activeStatusEffects.Remove(effect);
        DataTools.HandleOnDataChanged(this); // Trigger event
    }
    
    // Dedicated clear method
    public void ClearAllActiveStatusEffects()
    {
        _activeStatusEffects.Clear();
        _activeStatusEffects = new List<EnumPlayerStatusEffect>();
        DataTools.HandleOnDataChanged(this); // Trigger event
    }
}
```

**Usage Pattern (Production)**:

```csharp
// Get list (returns copy)
List<EnumPlayerStatusEffect> effects = DataController.Instance.PlayerRuntimeData.Value.GetActiveStatusEffects();

// Add effect
DataController.Instance.PlayerRuntimeData.Value.AddActiveStatusEffect(EnumPlayerStatusEffect.IN_STEALTH);

// Remove effect
DataController.Instance.PlayerRuntimeData.Value.RemoveActiveStatusEffect(EnumPlayerStatusEffect.POISON);

// Clear all
DataController.Instance.PlayerRuntimeData.Value.ClearAllActiveStatusEffects();
```

**Usage Pattern (Sandbox)**:

```csharp
[SerializeField] private PlayerRuntimeData playerData;

private void TestStatusEffects()
{
    // Same API works in sandbox
    playerData.Value.AddActiveStatusEffect(EnumPlayerStatusEffect.BLEEDING);
    playerData.Value.RemoveActiveStatusEffect(EnumPlayerStatusEffect.POISON);
}
```

## Scene Setup

### Bootstrap Scene Configuration

The `DataController` is set up in `Bootstrap.unity`:

1. **DataController Prefab**: Instantiated from `Assets/Brad/Prefabs/DataController.prefab`
2. **DDOL**: Inherits from Singleton, automatically uses DontDestroyOnLoad
3. **Runtime Data Assets**: All runtime data assets assigned in Inspector
4. **Event Subscriptions**: Subscribes to `NewGameStartedEvent` and `SavedGameLoadedEvent`

**Runtime Data Assets**:
- `PlayerRuntimeData` asset assigned
- `ProgressionRuntimeData` asset assigned
- `InventoryRuntimeData` asset assigned

This structure ensures the data system is available throughout the game's lifetime, persisting across scene loads.
