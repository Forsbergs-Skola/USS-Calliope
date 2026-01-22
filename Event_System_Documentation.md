# Event System Documentation

## Overview

This document describes the event system implementation for the USS Calliope Unity project. The system provides a decoupled, event-driven architecture for communication between game systems using Unity ScriptableObjects as event channels. This design enables loose coupling, making systems easier to maintain, test, and extend.

## Architecture

The event system follows a centralized relay pattern with categorized event channels:

- **Event Channels**: ScriptableObject-based event definitions that act as communication channels
- **Event Relay**: Singleton hub that provides centralized access to all event categories
- **Event Categories**: Organized groups of related events (GameEvents, UIEvents, PlayerEvents)
- **Payload Types**: Support for various data types passed with events (empty, string, int, bool, float, custom types)

## Core Components

### 1. Event Channel Base Classes

All event channels inherit from `ScriptableObject` and are located in the `Events` namespace. They follow a consistent pattern: expose a public event delegate and provide a `TriggerEvent()` method.

#### `EmptyPayloadEvent`
Event channel that carries no data payload.

**Location**: `Assets/Brad/Scripts/EventScripts/EmptyPayloadEvent.cs`

**Properties**:
- `OnEventTriggered` (event System.Action): Event delegate with no parameters

**Methods**:
- `TriggerEvent()`: Invokes the event with no payload

**Use Cases**: Simple notifications that don't require data (e.g., "Game Saved", "New Game Started")

**Example Usage**:
```csharp
// Subscribe
EventRelay.Instance.GameEvents.NewGameStartedEvent.OnEventTriggered += HandleNewGame;

// Trigger
EventRelay.Instance.GameEvents.NewGameStartedEvent.TriggerEvent();

// Unsubscribe
EventRelay.Instance.GameEvents.NewGameStartedEvent.OnEventTriggered -= HandleNewGame;
```

#### `StringPayloadEvent`
Event channel that carries a string payload.

**Location**: `Assets/Brad/Scripts/EventScripts/StringPayloadEvent.cs`

**Properties**:
- `OnEventTriggered` (event System.Action<string>): Event delegate with string parameter

**Methods**:
- `TriggerEvent(string payload)`: Invokes the event with string data

**Use Cases**: Events that need to pass identifiers, names, or text data (e.g., dialogue conversation IDs, item names)

#### `IntPayloadEvent`
Event channel that carries an integer payload.

**Location**: `Assets/Brad/Scripts/EventScripts/IntPayloadEvent.cs`

**Properties**:
- `OnEventTriggered` (event System.Action<int>): Event delegate with int parameter

**Methods**:
- `TriggerEvent(int payload)`: Invokes the event with integer data

**Use Cases**: Events that need to pass numeric values (e.g., scores, counts, IDs)

#### `BoolPayloadEvent`
Event channel that carries a boolean payload.

**Location**: `Assets/Brad/Scripts/EventScripts/BoolPayloadEvent.cs`

**Properties**:
- `OnEventTriggered` (event System.Action<bool>): Event delegate with bool parameter

**Methods**:
- `TriggerEvent(bool payload)`: Invokes the event with boolean data

**Use Cases**: Events that need to pass state flags (e.g., enabled/disabled, on/off)

#### `FloatPayloadEvent`
Event channel that carries a float payload.

**Location**: `Assets/Brad/Scripts/EventScripts/FloatPayloadEvent.cs`

**Properties**:
- `OnEventTriggered` (event System.Action<float>): Event delegate with float parameter

**Methods**:
- `TriggerEvent(float payload)`: Invokes the event with float data

**Use Cases**: Events that need to pass decimal values (e.g., health percentages, distances)

#### `GameDataPayloadEvent`
Event channel that carries a `GameData` payload.

**Location**: `Assets/Brad/Scripts/EventScripts/GameDataPayloadEvent.cs`

**Properties**:
- `OnEventTriggered` (event System.Action<GameData>): Event delegate with GameData parameter

**Methods**:
- `TriggerEvent(GameData payload)`: Invokes the event with GameData

**Use Cases**: Loading saved games, passing complete game state

**GameData Structure**:
```csharp
public class GameData
{
    public PlayerData playerData;
    public InventoryData inventoryData;
    public ProgressionData progressionData;
}
```

#### `IRuntimeDataPayloadEvent`
Event channel that carries an `IRuntimeData` payload.

**Location**: `Assets/Brad/Scripts/EventScripts/IRuntimeDataPayloadEvent.cs`

**Properties**:
- `OnEventTriggered` (event System.Action<IRuntimeData>): Event delegate with IRuntimeData parameter

**Methods**:
- `TriggerEvent(IRuntimeData payload)`: Invokes the event with runtime data

**Use Cases**: Notifying systems when runtime data changes (e.g., `PlayerData`, `ProgressionData`, `InventoryData`)

**IRuntimeData Interface**:
```csharp
public interface IRuntimeData
{
    bool GetIsSandbox();
}
```

**Note**: `PlayerData`, `ProgressionData`, and `InventoryData` all implement `IRuntimeData`, allowing this event to handle any runtime data type.

### 2. Event Category Classes

Event categories group related events together and are attached as MonoBehaviour components to child GameObjects of the EventRelay.

#### `GameEvents`
Contains events related to game state and data management.

**Location**: `Assets/Brad/Scripts/EventScripts/GameEvents.cs`

**Events**:
- `NewGameStartedEvent` (EmptyPayloadEvent): Fired when a new game begins
- `SavedGameLoadedEvent` (GameDataPayloadEvent): Fired when a saved game is loaded
- `GameSavedEvent` (EmptyPayloadEvent): Fired when the game is saved
- `DataUpdatedEvent` (EmptyPayloadEvent): Fired when any runtime data changes
- `RuntimeDataUpdatedEvent` (IRuntimeDataPayloadEvent): Fired when runtime data changes, includes the changed data
- `ItemPickupEvent` (StringPayloadEvent): Fired when an item is picked up (payload: item ID/name)

**Usage Pattern**: Systems subscribe to these events to react to game state changes. For example, `DataController` listens to `NewGameStartedEvent` to initialize runtime data.

#### `UIEvents`
Contains events related to user interface interactions and state.

**Location**: `Assets/Brad/Scripts/EventScripts/UIEvents.cs`

**Events**:
- `LogoSplashFinishedEvent` (EmptyPayloadEvent): Fired when logo splash screen completes
- `DialogueLineStartedEvent` (StringPayloadEvent): Fired when a dialogue line begins (payload: line ID)
- `DialogueLineFinishedEvent` (StringPayloadEvent): Fired when a dialogue line completes (payload: line ID)
- `DialogueConvoStartedEvent` (StringPayloadEvent): Fired when a dialogue conversation begins (payload: conversation ID)
- `DialogueConvoFinishedEvent` (StringPayloadEvent): Fired when a dialogue conversation ends (payload: conversation ID)

**Usage Pattern**: UI systems use these events to coordinate interface updates and transitions.

#### `PlayerEvents`
Currently a placeholder for player-related events.

**Location**: `Assets/Brad/Scripts/EventScripts/PlayerEvents.cs`

**Status**: Structure in place, ready for future player-specific events (e.g., player death, level up, status effect changes)

### 3. Event Relay: Central Hub

#### `EventRelay`
Singleton that provides centralized access to all event categories.

**Location**: `Assets/Brad/Scripts/EventScripts/EventRelay.cs`

**Pattern**: Singleton (inherits from `Singleton<EventRelay>`)

**Properties**:
- `GameEvents` (GameEvents): Access to game-related events
- `PlayerEvents` (PlayerEvents): Access to player-related events
- `UIEvents` (UIEvents): Access to UI-related events

**Architecture**:
- EventRelay GameObject contains child GameObjects for each event category
- Each child GameObject has the corresponding MonoBehaviour component (GameEvents, UIEvents, PlayerEvents)
- Event channel ScriptableObjects are assigned in the Inspector to these components
- Singleton pattern ensures global access: `EventRelay.Instance.GameEvents.NewGameStartedEvent`

**Scene Setup**: 
- Prefab located at `Assets/Brad/Prefabs/EventRelay.prefab`
- Instantiated in `Bootstrap.unity` scene
- Uses DontDestroyOnLoad (via Singleton base class) to persist across scenes

## Event Channel Assets

Event channels are created as ScriptableObject assets in Unity and organized by category:

**Location**: `Assets/EventChannels/`

**Organization**:
```
Assets/EventChannels/
├── GameEvents/
│   ├── NewGameStartedEvent.asset
│   ├── SavedGameLoadedEvent.asset
│   ├── GameSavedEvent.asset
│   ├── DataUpdatedEvent.asset
│   ├── RuntimeDataUpdatedEvent.asset
│   └── ItemPickupEvent.asset
├── UIEvents/
│   ├── LogoSplashFinishedEvent.asset
│   ├── DialogueLineStartedEvent.asset
│   ├── DialogueLineFinishedEvent.asset
│   ├── DialogueConvoStartedEvent.asset
│   └── DialogueConvoFinishedEvent.asset
└── PlayerEvents/
    └── TestEmptyEvent.asset
```

**Creating New Event Channels**:
1. Right-click in Project window → Create → Event Channels → [PayloadType]PayloadEvent
2. Name the asset appropriately (e.g., "PlayerDiedEvent")
3. Assign to the appropriate event category component in EventRelay prefab
4. Expose through the category class if needed

## System Flow

### Event Subscription Pattern

1. **In `OnEnable()` or `Start()`**: Subscribe to events
```csharp
private void OnEnable()
{
    EventRelay.Instance.GameEvents.NewGameStartedEvent.OnEventTriggered += HandleNewGame;
}
```

2. **In `OnDisable()` or `OnDestroy()`**: Unsubscribe from events
```csharp
private void OnDisable()
{
    EventRelay.Instance.GameEvents.NewGameStartedEvent.OnEventTriggered -= HandleNewGame;
}
```

**Critical**: Always unsubscribe to prevent memory leaks and errors when objects are destroyed.

### Event Triggering Pattern

1. **Direct Trigger**: Call `TriggerEvent()` on the event channel
```csharp
EventRelay.Instance.GameEvents.NewGameStartedEvent.TriggerEvent();
```

2. **With Payload**: Pass data when triggering
```csharp
EventRelay.Instance.GameEvents.ItemPickupEvent.TriggerEvent("Pistol_01");
```

3. **Through Data Tools**: Runtime data changes automatically trigger events
```csharp
// In ProgressionData, PlayerData, or InventoryData
DataTools.HandleOnDataChanged(this);
// This automatically triggers:
// - DataUpdatedEvent
// - RuntimeDataUpdatedEvent (with the data object)
```

### Reactive Data Updates

The system includes automatic event triggering for runtime data changes:

**Location**: `Assets/Brad/Scripts/DataModel/SaveService.cs` (DataTools class)

**Method**: `DataTools.HandleOnDataChanged(IRuntimeData data)`

**Behavior**:
- Called automatically when runtime data properties change
- Triggers `DataUpdatedEvent` (empty payload)
- Triggers `RuntimeDataUpdatedEvent` (with the changed data object)
- Skips triggering if data is sandbox mode
- Skips if EventRelay doesn't exist

**Integration**: Runtime data classes (`PlayerData`, `ProgressionData`, `InventoryData`) call this method in their property setters and list modification methods.

## Usage Examples

### Subscribing to an Event

```csharp
public class MySystem : MonoBehaviour
{
    private void OnEnable()
    {
        EventRelay.Instance.GameEvents.NewGameStartedEvent.OnEventTriggered += Initialize;
    }
    
    private void OnDisable()
    {
        EventRelay.Instance.GameEvents.NewGameStartedEvent.OnEventTriggered -= Initialize;
    }
    
    private void Initialize()
    {
        Debug.Log("New game started!");
    }
}
```

### Subscribing to an Event with Payload

```csharp
public class DialogueSystem : MonoBehaviour
{
    private void OnEnable()
    {
        EventRelay.Instance.UIEvents.DialogueConvoStartedEvent.OnEventTriggered += OnConvoStarted;
    }
    
    private void OnDisable()
    {
        EventRelay.Instance.UIEvents.DialogueConvoStartedEvent.OnEventTriggered -= OnConvoStarted;
    }
    
    private void OnConvoStarted(string conversationID)
    {
        Debug.Log($"Conversation {conversationID} started");
    }
}
```

### Triggering an Event

```csharp
// Simple event
EventRelay.Instance.GameEvents.GameSavedEvent.TriggerEvent();

// Event with payload
EventRelay.Instance.GameEvents.ItemPickupEvent.TriggerEvent("HealthPack_01");
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

1. **Observer Pattern**: Event channels implement the observer pattern, allowing multiple subscribers to react to events
2. **ScriptableObject Pattern**: Event channels are ScriptableObjects, enabling:
   - Asset-based configuration
   - Inspector assignment
   - Persistence across play sessions
   - Easy testing and debugging
3. **Singleton Pattern**: EventRelay uses singleton for global access
4. **Centralized Hub Pattern**: All events accessed through single entry point
5. **Loose Coupling**: Systems communicate through events without direct references
6. **Separation of Concerns**: Event categories organize related events

## Key Features

### Decoupled Communication
- Systems don't need direct references to each other
- Easy to add new systems without modifying existing code
- Reduces circular dependencies

### Type Safety
- Strongly typed payloads prevent runtime errors
- Compile-time checking for event signatures
- IntelliSense support for event access

### Inspector Configuration
- Event channels assigned in Unity Inspector
- No code changes needed to wire up events
- Visual debugging of event connections

### Multiple Subscribers
- Any number of systems can subscribe to the same event
- Each subscriber receives the event independently
- Order of execution is not guaranteed (use priority systems if needed)

### Automatic Data Events
- Runtime data changes automatically trigger events
- Reduces boilerplate code
- Ensures consistency in data change notifications

## Integration Points

### With Data System
- `DataTools.HandleOnDataChanged()` automatically triggers events
- `RuntimeDataUpdatedEvent` carries the changed data object
- Systems can react to any data change without polling

### With Dialogue System
- Dialogue system triggers events at conversation/line boundaries
- Other systems can react to dialogue events (e.g., quest triggers, cutscenes)

### With Objectives System
- Objectives tracker subscribes to `RuntimeDataUpdatedEvent`
- Automatically evaluates objectives when data changes
- Enables reactive objective completion

### With UI System
- UI events coordinate interface updates
- Canvas management reacts to game state events
- Menu systems respond to user input events

## Event Channel Creation Guide

### Creating a New Event Channel

1. **Choose Payload Type**: Determine what data (if any) the event needs to carry
   - No data → `EmptyPayloadEvent`
   - Simple type → `StringPayloadEvent`, `IntPayloadEvent`, `BoolPayloadEvent`, `FloatPayloadEvent`
   - Complex data → Create custom payload event (see below)

2. **Create ScriptableObject Asset**:
   - Right-click in Project → Create → Event Channels → [Type]PayloadEvent
   - Name appropriately (e.g., "PlayerDiedEvent")

3. **Assign to Event Category**:
   - Open EventRelay prefab
   - Select appropriate category GameObject (GameEvents, UIEvents, PlayerEvents)
   - Drag event channel asset to appropriate field in Inspector

4. **Expose Through Category Class** (if needed):
   - Add field to category class (e.g., `GameEvents.cs`)
   - Add property getter
   - Assign in Inspector

### Creating a Custom Payload Event

For events that need custom data types:

```csharp
using UnityEngine;
namespace Events
{
    [CreateAssetMenu(fileName = "MyCustomPayloadEvent", menuName = "Event Channels/MyCustomPayloadEvent")]
    public class MyCustomPayloadEvent : ScriptableObject
    {
        public event System.Action<MyCustomType> OnEventTriggered;
        public void TriggerEvent(MyCustomType payload)
        {
            OnEventTriggered?.Invoke(payload);
        }
    }
}
```

## Best Practices

1. **Always Unsubscribe**: Prevent memory leaks by unsubscribing in `OnDisable()` or `OnDestroy()`

2. **Null Checks**: Check for EventRelay.Instance existence before accessing
```csharp
if (EventRelay.Instance == null) return;
EventRelay.Instance.GameEvents.NewGameStartedEvent.TriggerEvent();
```

3. **Event Naming**: Use clear, descriptive names that indicate when the event fires
   - Good: `DialogueConvoFinishedEvent`
   - Bad: `DialogueEvent`

4. **Payload Clarity**: Choose payload types that clearly communicate intent
   - Use strings for IDs and names
   - Use enums for state values
   - Use custom types for complex data

5. **Category Organization**: Place events in appropriate categories
   - Game state → GameEvents
   - UI interactions → UIEvents
   - Player actions → PlayerEvents

6. **Avoid Over-Subscription**: Don't subscribe to events you don't need
   - Reduces unnecessary processing
   - Improves performance
   - Makes code intent clearer

## Technical Notes

- **Thread Safety**: Event system is not thread-safe; use from Unity's main thread only
- **Event Order**: Subscriber execution order is not guaranteed
- **Null Safety**: Uses null-conditional operator (`?.`) for safe invocation
- **Memory**: Event delegates hold references; ensure proper cleanup
- **Performance**: Event invocation is fast; overhead is minimal for typical use cases
- **ScriptableObject Persistence**: Event channels persist across play sessions, maintaining subscriptions

## Dependencies

- Project's singleton base class (`Singleton<T>`)
- Unity ScriptableObject system
- `Events` namespace for event channel classes
- Data model classes (`GameData`, `IRuntimeData`, etc.)

## Known Limitations / Future Work

1. **Event Ordering**: No built-in priority system for subscriber execution order
2. **Event History**: No logging or history of triggered events (useful for debugging)
3. **Conditional Subscriptions**: No built-in way to conditionally subscribe/unsubscribe
4. **Event Validation**: No validation that required subscribers exist before triggering
5. **Performance Monitoring**: No built-in performance metrics for event system usage
6. **Editor Tools**: Could add custom editor tools for visualizing event connections
7. **Event Batching**: No support for batching multiple events together

## Scene Setup

### Bootstrap Scene Configuration

The EventRelay is set up in `Bootstrap.unity`:

1. **EventRelay Prefab**: Instantiated from `Assets/Brad/Prefabs/EventRelay.prefab`
2. **DDOL**: Inherits from Singleton, automatically uses DontDestroyOnLoad
3. **Event Channels**: All event channel ScriptableObjects are assigned in Inspector
4. **Category Components**: GameEvents, UIEvents, PlayerEvents components attached to child GameObjects

**Hierarchy Structure**:
```
EventRelay (Singleton)
├── GameEvents (GameEvents component)
├── UIEvents (UIEvents component)
└── PlayerEvents (PlayerEvents component)
```

This structure ensures the event system is available throughout the game's lifetime, persisting across scene loads.
