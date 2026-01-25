# Objectives Tracking System Documentation

## Overview

This document describes the objectives tracking system implementation for the USS Calliope Unity project. The system provides a flexible, data-driven approach to managing game objectives (quests/missions) with support for prerequisites, completion criteria, status tracking, and automatic progression. The system integrates with the project's [data persistence layer](Data_Management_System_Documentation.md) and [event system](Event_System_Documentation.md).

## Architecture

The objectives system follows a modular architecture with clear separation of concerns:

- **Data Layer**: ScriptableObjects define objective definitions and completion criteria
- **Controller Layer**: `ObjectivesTracker` manages objective lifecycle, status tracking, and completion evaluation
- **Presentation Layer**: `ObjectivesPanel` and `ObjectiveUIElement` handle UI display (currently in early development)
- **Integration Layer**: [Event-driven](Event_System_Documentation.md) updates from `ProgressionData` trigger objective evaluation

## Core Components

### 1. ScriptableObject Data Structures

#### `ObjectiveSO`
Represents a single objective/quest definition.

**Location**: `Assets/Brad/Scripts/ObjectivesSystem/ObjectiveSO.cs`

**Properties**:
- `objectiveID` (string): Unique identifier (auto-generated GUID if empty)
- `objectiveTitle` (string): Display name for the objective
- `objectiveDescription` (string): Detailed description (TextArea support)
- `defaultStatus` (EnumObjectiveStatus): Initial status when objectives are reset
- `xpReward` (int): Experience points awarded upon completion
- `prerequisiteObjectiveIDs` (List<string>): IDs of objectives that must be finished before this one can start
- `completionCriteria` (CompletionCriteriaSO): Reference to the criteria that determines completion

**Features**:
- Automatic GUID generation in `OnValidate()` if `objectiveID` is empty
- Editor integration to mark asset as dirty when auto-generating IDs
- Support for prerequisite chains (objectives that depend on others)

**Status Enum**:
```csharp
public enum EnumObjectiveStatus
{
    NOT_STARTED,  // Objective exists but hasn't been activated
    STARTED,      // Objective is active and being tracked
    FINISHED      // Objective has been completed
}
```

#### `ObjectivesCatalogSO`
Central registry of all objectives in the game.

**Location**: `Assets/Brad/Scripts/ObjectivesSystem/ObjectivesCatalogSO.cs`

**Properties**:
- `objectives` (List<ObjectiveSO>): Collection of all objective definitions

**Purpose**: Provides a single point of reference for all objective content, enabling efficient lookup and initialization.

#### `CompletionCriteriaSO` (Abstract Base Class)
Abstract base class for defining completion conditions.

**Location**: `Assets/Brad/Scripts/ObjectivesSystem/CompletionCriterias/CompletionCriteriaSO.cs`

**Abstract Method**:
- `IsCriteriaMet()`: Returns true when the objective's completion condition is satisfied

**Design Pattern**: Uses the Strategy pattern, allowing different completion criteria implementations for different objective types.

**Example Implementations**:

1. **`KillAliceCompletionCriteria`**
   - Checks if "Alice" is in the defeated enemies list
   - Location: `Assets/Brad/Scripts/ObjectivesSystem/CompletionCriterias/KillAliceCompletionCriteria.cs`

2. **`KillBobCompletionCriteria`**
   - Checks if "Bob" is in the defeated enemies list
   - Location: `Assets/Brad/Scripts/ObjectivesSystem/CompletionCriterias/KillBobCompletionCriteria.cs`

3. **`KillTwoEnemiesCriteria`**
   - Checks if at least 2 enemies have been defeated
   - Location: `Assets/Brad/Scripts/ObjectivesSystem/CompletionCriterias/KillTwoEnemiesCriteria.cs`

**Extensibility**: New completion criteria can be created by inheriting from `CompletionCriteriaSO` and implementing `IsCriteriaMet()`. Each implementation can query any game state through `DataController.Instance.ProgressionRuntimeData.Value`.

**Note on Multiple Conditions**: While each `ObjectiveSO` references a single `CompletionCriteriaSO`, that criteria object's `IsCriteriaMet()` method can implement any complex boolean logic needed - checking multiple conditions, combining them with AND/OR operators, etc. The single reference is a structural design choice, not a functional limitation.

### 2. Controller: `ObjectivesTracker`

**Location**: `Assets/Brad/Scripts/ObjectivesSystem/ObjectivesTracker.cs`

**Pattern**: Singleton (inherits from `Singleton<ObjectivesTracker>`)

**Responsibilities**:
- Maintains reference to `ObjectivesCatalogSO` containing all objective definitions
- Manages objective status lifecycle (NOT_STARTED → STARTED → FINISHED)
- Evaluates completion criteria for active objectives
- Handles prerequisite checking and automatic objective activation
- Integrates with `ProgressionData` for persistence

**Key Methods**:

- `ResetObjectives()`: Initializes all objectives from catalog with their default statuses
  - Called when starting a new game
  - Creates status dictionary in `ProgressionData`

- `GetObjectiveWithID(string objID)`: Looks up objective definition by ID
  - Returns `ObjectiveSO` from catalog

- `StartObjectiveWithID(string objID)`: Transitions objective from NOT_STARTED to STARTED
  - Validates objective exists and is in correct state
  - Updates `ProgressionData` status dictionary
  - Triggers data change event

- `FinishObjectiveWithID(string objID)`: Transitions objective from STARTED to FINISHED
  - Validates objective exists and is in correct state
  - Updates `ProgressionData` status dictionary
  - Calls `GroomObjectives()` to check prerequisites

- `GroomObjectives(string finishedObjectiveID)`: Evaluates prerequisites after an objective finishes
  - Finds all NOT_STARTED objectives
  - Checks if their prerequisites are all FINISHED
  - Automatically starts objectives whose prerequisites are met
  - Enables automatic objective progression chains

**Event Integration**:

- Subscribes to `runtimeDataUpdatedEvent` (IRuntimeDataPayloadEvent)
- `IngestRuntimeDataUpdate()`: Called when `ProgressionData` changes
  - Filters for `ProgressionData` updates
  - Finds all STARTED objectives
  - Evaluates each objective's completion criteria
  - Automatically finishes objectives when criteria are met
  - In dev mode, logs objective statuses to console

**Development Mode**:
- `devMode` flag enables debug logging
- `DebugIncomingData()`: Logs all objective statuses when data updates

### 3. Data Model Integration: `ProgressionData`

**Location**: `Assets/Brad/Scripts/DataModel/ProgressionData.cs`

The objectives system integrates tightly with `ProgressionData`, which serves as the runtime state container.

**Objective Storage**:
- `ObjectivesAndStatusesDict` (IReadOnlyDictionary<string, EnumObjectiveStatus>): Maps objective IDs to their current status
- `UpdateObjectivesAndStatuses()`: Updates the status dictionary and triggers change events

**Change Notification**:
- When `ProgressionData` changes, it triggers `DataTools.HandleOnDataChanged()`
- This fires the `runtimeDataUpdatedEvent` that `ObjectivesTracker` listens to
- Creates a reactive loop: game state changes → data updates → objective evaluation → status changes → data updates

**Example Integration**:
- `DefeatEnemy(string enemyName)`: Adds enemy to defeated list and triggers data change
- Completion criteria can query `GetDefeatedEnemiesList()` to check conditions
- When enemy is defeated, objectives automatically evaluate and may complete

### 4. UI Components
[UI system documentation](UI_System_Documentation.md)

#### `ObjectivesPanel`
**Location**: `Assets/Brad/Scripts/UI/ObjectivesPanel.cs`

**Current State**: Basic structure in place, references `ProgressionRuntimeData`
- Intended to display active and completed objectives to the player

#### `ObjectiveUIElement`
**Location**: `Assets/Brad/Scripts/UI/ObjectiveUIElement.cs`

**Purpose**: Represents a single objective in the UI

**Methods**:
- `Configure(string titleStr, string descStr, bool finished)`: Sets up the UI element
  - Displays title and description
  - Applies strikethrough font style when objective is finished

**UI Elements**:
- `titleText` (TMP_Text): Objective title
- `desctiptionText` (TMP_Text): Objective description (note: typo in field name)

## System Flow

### Initialization

1. Game starts, `ObjectivesTracker` singleton is created
2. `ResetObjectives()` is called (typically by game initialization code)
3. All objectives from catalog are added to `ProgressionData` with their default statuses
4. Objectives with `defaultStatus == STARTED` are immediately active
5. Objectives with prerequisites are evaluated; those with met prerequisites are started

### Objective Lifecycle

1. **NOT_STARTED → STARTED**:
   - Triggered by `StartObjectiveWithID()` or automatic prerequisite evaluation
   - Objective becomes active and its completion criteria is evaluated on each data update

2. **STARTED → FINISHED**:
   - When `ProgressionData` changes, `ObjectivesTracker.IngestRuntimeDataUpdate()` is called
   - All STARTED objectives have their completion criteria evaluated
   - If `IsCriteriaMet()` returns true, objective is finished
   - `FinishObjectiveWithID()` updates status and triggers `GroomObjectives()`

3. **Prerequisite Evaluation**:
   - When an objective finishes, `GroomObjectives()` checks all NOT_STARTED objectives
   - For each, verifies all prerequisites are FINISHED
   - Automatically starts objectives whose prerequisites are satisfied

### Completion Criteria Evaluation

1. Game event occurs (e.g., enemy defeated, item collected)
2. `ProgressionData` is updated (e.g., `DefeatEnemy()` called)
3. Data change event is triggered
4. `ObjectivesTracker` receives update
5. All STARTED objectives are checked
6. Each objective's `CompletionCriteria.IsCriteriaMet()` is called
7. Criteria queries current game state from `ProgressionData`
8. If criteria met, objective is finished

## Asset Organization

Objective assets are organized in `Assets/Objectives/`:

- **Objectives Catalog**: `Assets/Objectives/_ObjectivesCatalog.asset` - Main registry
- **Objective Definitions**: Individual `ObjectiveSO` assets (e.g., `KillAlice.asset`, `KillBob.asset`)
- **Completion Criteria**: `Assets/Objectives/CompletionCriteria/` folder contains criteria ScriptableObjects

**Example Structure**:
```
Assets/Objectives/
├── _ObjectivesCatalog.asset
├── KillAlice.asset (ObjectiveSO)
├── KillBob.asset (ObjectiveSO)
├── KillTwoEnemies.asset (ObjectiveSO)
└── CompletionCriteria/
    ├── KillAliceCompletionCriteria.asset
    ├── KillBobCompletionCriteria.asset
    └── KillTwoEnemiesCriteria.asset
```

## Usage Examples

### Starting an Objective Manually
```csharp
ObjectivesTracker.Instance.StartObjectiveWithID("69dbaaf0-264f-4bdb-bcd8-739712e53ebc");
```

### Finishing an Objective Manually
```csharp
ObjectivesTracker.Instance.FinishObjectiveWithID("69dbaaf0-264f-4bdb-bcd8-739712e53ebc");
```

### Resetting All Objectives
```csharp
ObjectivesTracker.Instance.ResetObjectives();
```

### Creating a New Completion Criteria

1. Create a new script inheriting from `CompletionCriteriaSO`:
```csharp
[CreateAssetMenu(fileName = "MyCriteria", menuName = "Objectives/Criteria/MyCriteria")]
public class MyCriteria : CompletionCriteriaSO
{
    public override bool IsCriteriaMet()
    {
        if (DataController.Instance == null) return false;
        ProgressionData progData = DataController.Instance.ProgressionRuntimeData.Value;
        // Check your condition here
        return /* your condition */;
    }
}
```

2. Create the ScriptableObject asset in Unity
3. Assign it to an `ObjectiveSO`'s `completionCriteria` field

### Setting Up Prerequisites

In the Unity Inspector for an `ObjectiveSO`:
1. Add prerequisite objective IDs to the `prerequisiteObjectiveIDs` list
2. When those objectives finish, this objective will automatically start
3. Supports multiple prerequisites (all must be finished)

## Design Patterns and Principles

1. **Singleton Pattern**: `ObjectivesTracker` uses singleton for global access
2. **ScriptableObject Pattern**: Data-driven design separates content from code
3. **Strategy Pattern**: `CompletionCriteriaSO` allows different completion logic implementations
4. **Observer Pattern**: Event-driven updates enable reactive objective evaluation
5. **State Machine**: Objective status transitions follow strict rules (NOT_STARTED → STARTED → FINISHED)
6. **Dependency Graph**: Prerequisites create a directed acyclic graph of objective dependencies

## Key Features

### Automatic Progression
- Objectives automatically start when prerequisites are met
- No manual intervention needed for objective chains

### Reactive Evaluation
- Completion criteria are evaluated automatically when game state changes
- No polling required; event-driven architecture ensures efficiency

### Flexible Completion Criteria
- Abstract base class allows any completion logic
- Criteria can query any game state through `ProgressionData`
- Easy to extend with new criteria types

### Prerequisite Support
- Objectives can depend on other objectives
- Automatic activation when prerequisites complete
- Supports complex objective chains and branching paths

### Data Persistence
- Objective statuses are stored in `ProgressionData`
- Integrated with save/load system
- Statuses persist across game sessions

## Integration Points

### With [Data_Management_System_Documentation](Data) System
- Stores status in `ProgressionData.ObjectivesAndStatusesDict`
- Listens to `runtimeDataUpdatedEvent` for state changes
- Queries `ProgressionData` for completion criteria evaluation

### With [Event_System_Documentation.md](Event) System
- Uses `IRuntimeDataPayloadEvent` for reactive updates
- Could emit objective-specific events (currently placeholder)

### With [UI_System_Documentation.md](UI) System
- `ObjectivesPanel` and `ObjectiveUIElement` prepared for UI integration
- UI can query `ObjectivesTracker` for objective data
- UI can display status from `ProgressionData`

## Future Extensibility

The system is designed with extensibility in mind:

- **UI Integration**: `ObjectivesPanel` structure ready for full implementation
- **Event Emission**: Could emit events when objectives start/finish for other systems
- **XP Rewards**: `xpReward` field exists but reward distribution not yet implemented
- **Multiple Criteria**: Currently supports one criteria per objective; could be extended to support multiple
- **Objective Types**: Could add objective categories or types for filtering/grouping
- **Time Limits**: Could add optional time constraints for objectives
- **Objective Chains**: Prerequisites already support chains; could add visual representation

## Technical Notes

- Uses GUID-based IDs for unique objective identification
- Editor integration (`OnValidate`) provides automatic ID generation
- Status transitions are validated to prevent invalid state changes
- Dictionary-based status storage enables efficient lookups
- Reactive evaluation prevents unnecessary polling
- Completion criteria are evaluated every time `ProgressionData` changes (could be optimized with targeted events)

## Dependencies

- Project's data model (`ProgressionData`, `DataController`)
- Project's event system (`IRuntimeDataPayloadEvent`, `IRuntimeData`)
- Project's singleton base class
- Unity TextMeshPro for UI text rendering (in UI components)

