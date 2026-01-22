# UI System Documentation

## Overview

This document describes the UI management system implementation for the USS Calliope Unity project. The system provides a centralized, singleton-based approach to managing Unity Canvas instances with support for multiple simultaneous canvases, foreground/background layering, prefab-based instantiation, and integration with the game's event system. The system enables clean separation between UI presentation and game logic.

## Architecture

The UI system follows a centralized controller pattern with interface-based canvas management:

- **Controller Layer**: `UIController` singleton manages all canvas lifecycle operations
- **Interface Layer**: `ICanvasUI` interface defines contract for canvas implementations
- **Canvas Layer**: Individual canvas implementations (HUD, Dialogue, Pause, etc.)
- **Supporting Systems**: Sound player, EventSystem management, scene-level handlers

## Core Components

### 1. UIController: Central Hub

#### `UIController`
Singleton that provides centralized management of all UI canvases.

**Location**: `Assets/Brad/Scripts/UI/UIController.cs`

**Pattern**: Singleton (inherits from `Singleton<UIController>`)

**Key Properties**:
- `canvasPrefabs` (List<StructCanvasUIPrefab>): Registry of canvas prefabs mapped to canvas names
- `uiSoundPlayer` (UISoundPlayer): Reference to UI sound system
- `FOREGROUND_SORT_ORDER` (const int = 10): Sorting order for foreground canvases
- `BACKGROUND_SORT_ORDER` (const int = 0): Sorting order for background canvases

**Core Functionality**:

1. **Canvas Discovery**: Scans scene root GameObjects to find active canvases implementing `ICanvasUI`
2. **Prefab Management**: Maintains registry of canvas prefabs for instantiation
3. **Sorting Order Management**: Controls which canvas appears on top using Unity Canvas sorting orders
4. **Lifecycle Management**: Handles showing, hiding, and removing canvases

**Public API Methods**:

- `ShowCanvas(EnumCanvasUIName canvasName)`: Instantiates and displays a canvas
  - Checks if canvas is already active (prevents duplicates)
  - Looks up prefab from registry
  - Instantiates prefab as root GameObject
  - Automatically foregrounds the new canvas

- `RemoveCanvas(EnumCanvasUIName canvasName)`: Destroys a canvas instance
  - Validates canvas exists before removal
  - Destroys the canvas GameObject
  - Automatically foregrounds another canvas if none remain foregrounded

- `ForegroundCanvas(EnumCanvasUIName canvasName)`: Brings a canvas to the front
  - Sets target canvas sorting order to `FOREGROUND_SORT_ORDER`
  - Sets all other canvases to `BACKGROUND_SORT_ORDER`
  - Ensures only one canvas is foregrounded at a time

- `GetIsCanvasUp(EnumCanvasUIName canvasName)`: Checks if a canvas is currently active
  - Returns boolean indicating canvas presence

- `ClearCanvases()`: Removes all active canvases
  - Useful for scene transitions
  - Destroys all canvas instances

- `GetReferenceToCanvas(EnumCanvasUIName canvasName)`: Gets interface reference to active canvas
  - Returns `ICanvasUI` interface for direct canvas access
  - Useful when canvas-specific methods need to be called

**Private Helper Methods**:

- `GetActiveCanvases()`: Scans scene for all active `ICanvasUI` implementations
- `GetIsCanvasActive()`: Checks if specific canvas type is active
- `GetCanvasPrefab()`: Looks up prefab from registry by canvas name
- `GetActiveCanvas()`: Gets interface reference to specific active canvas

**Scene Setup**: 
- Prefab located at `Assets/Brad/Prefabs/UIController.prefab`
- Instantiated in `Bootstrap.unity` scene
- Uses DontDestroyOnLoad (via Singleton base class) to persist across scenes

### 2. ICanvasUI Interface

#### `ICanvasUI`
Interface contract that all canvas implementations must follow.

**Location**: `Assets/Brad/Scripts/UI/UIController.cs`

**Required Methods**:

- `GetCanvasName()`: Returns the canvas's `EnumCanvasUIName` identifier
- `GetCanvas()`: Returns the Unity `Canvas` component
- `ForegroundCanvas(bool foregrounded)`: Called to set foreground/background state
- `GetSortingOrder()`: Returns current Canvas sorting order

**Purpose**: 
- Enables polymorphic canvas management
- Ensures consistent canvas behavior
- Allows `UIController` to work with any canvas type without knowing implementation details

**Implementation Pattern**: All canvas classes implement this interface and provide their specific `EnumCanvasUIName` value.

### 3. Canvas Name Enumeration

#### `EnumCanvasUIName`
Enumeration of all available canvas types.

**Location**: `Assets/Brad/Scripts/UI/UIController.cs`

**Values**:
- `MAIN_MENU`: Main menu screen
- `HUD`: Heads-up display (always-on gameplay UI)
- `DIALOGUE`: Dialogue conversation interface
- `PAUSE`: Pause menu
- `LOGO_SPLASH`: Logo splash screen
- `ABOUT`: About/info screen

**Usage**: Used throughout the system to identify canvas types for operations.

### 4. Canvas Prefab Structure

#### `StructCanvasUIPrefab`
Serializable structure mapping canvas names to prefabs.

**Location**: `Assets/Brad/Scripts/UI/UIController.cs`

**Fields**:
- `canvasName` (EnumCanvasUIName): Canvas identifier
- `canvasPrefab` (GameObject): Prefab reference

**Purpose**: Allows Inspector-based configuration of canvas prefabs in `UIController`.

### 5. Canvas Implementations

#### `Hud` (HUD Canvas)
Always-on gameplay UI displaying player status.

**Location**: `Assets/Brad/Scripts/UI/Hud.cs`

**Features**:
- Health and stamina sliders
- XP display
- Status effect icons
- Crouch indicator
- Death screen integration
- Reactive updates via `RuntimeDataUpdatedEvent`

**Data Integration**: 
- Subscribes to `RuntimeDataUpdatedEvent`
- Updates UI when `PlayerData` changes
- Dynamically manages status effect icons based on active effects

**Interface Implementation**: Returns `EnumCanvasUIName.HUD`

#### `DialogueCanvas`
Dialogue conversation interface.

**Location**: `Assets/Brad/Scripts/UI/DialogueCanvas.cs`

**Features**:
- Character portrait display
- Speaker name display
- Character-by-character text reveal
- Continue button
- Automatic game pausing when active

**Integration**: 
- Receives conversation data from `DialogueController`
- Triggers dialogue events for other systems
- Pauses game time when enabled

**Interface Implementation**: Returns `EnumCanvasUIName.DIALOGUE`

#### `MainMenu`
Main menu screen with game start options.

**Location**: `Assets/Brad/Scripts/UI/MainMenu.cs`

**Features**:
- New Game button
- Continue button (shown only if save exists)
- About button
- Quit button
- Cursor visibility management

**[Event_System_Documentation.md](Event) Integration**: 
- Triggers `newGamePressedEvent` and `loadGamePressedEvent`
- Integrates with save system to show/hide continue button

**Interface Implementation**: Returns `EnumCanvasUIName.MAIN_MENU`

#### `PauseCanvas`
Pause menu with game options.

**Location**: `Assets/Brad/Scripts/UI/PauseCanvas.cs`

**Features**:
- Return to main menu
- Save game functionality
- Resume/back button
- Toggle between Objectives and Inventory panels
- Automatic game pausing when active
- UI sound integration

**Panel Management**: 
- Contains `ObjectivesPanel` and `InventoryPanel` as child components
- Toggles visibility between panels
- Updates button text based on current panel

**Interface Implementation**: Returns `EnumCanvasUIName.PAUSE`

### 6. Supporting Panel Components

#### `ObjectivesPanel`
Displays active and completed objectives.

**Location**: `Assets/Brad/Scripts/UI/ObjectivesPanel.cs`

**Features**:
- Lists started objectives
- Lists finished objectives (with strikethrough)
- Dynamically instantiates `ObjectiveUIElement` prefabs
- Updates when panel is enabled

**Integration**: 
- Queries `ObjectivesTracker` for objective data
- Reads from `ProgressionRuntimeData` for status information

#### `InventoryPanel`
Displays player inventory items.

**Location**: `Assets/Brad/Scripts/UI/InventoryPanel.cs`

**Features**:
- Shows consumables with quantities
- Shows weapons
- Shows quest items
- Item name display on selection
- Scrollable list

**Integration**: 
- Queries `InventoryController` for item data
- Reads from `InventoryRuntimeData` for inventory contents
- Uses `InventoryItemUIPrefab` for item display

### 7. UI Sound System

#### `UISoundPlayer`
Manages UI sound effects.

**Location**: `Assets/Brad/Scripts/UI/UISoundPlayer.cs`

**Features**:
- Multiple AudioSource pool for concurrent sounds
- Sound catalog integration (`UISoundCatalogSO`)
- Automatic source selection (uses first available)

**Sound Types** (`EnumUISound`):
- `HOVER`: Button hover sound
- `PRESS`: Button press sound
- `PAUSE_SCREEN`: Pause menu sound
- `LOGO_SOUND`: Logo splash sound

**Usage**: 
- Accessed via `UIController.Instance.UISoundPlayer`
- Called by canvases for sound feedback

**Audio Source Pooling**: Uses multiple AudioSource components to allow overlapping sounds without interruption.

### 8. [Event_System_Documentation.md](EventSystem) Management

#### `EventSystemDDOL`
Manages Unity EventSystem across scenes.

**Location**: `Assets/Brad/Scripts/UI/EventSystemDDOL.cs`

**Pattern**: Singleton (inherits from `Singleton<EventSystemDDOL>`)

**Purpose**: 
- Ensures only one EventSystem exists per scene
- Destroys duplicate EventSystems when scenes load
- Prevents UI input conflicts

**Behavior**: 
- On scene load, finds all EventSystems
- Destroys any that aren't the singleton instance
- Ensures UI input works correctly

### 9. Scene-Level UI Handler

#### `SceneUIHandler`
Component for scene-specific UI setup.

**Location**: `Assets/Brad/Scripts/UI/SceneUIHandler.cs`

**Features**:
- Optional automatic HUD display on scene load
- Configurable via Inspector (`showHudOnLoad` flag)

**Usage**: 
- Attach to GameObject in game scenes
- Set `showHudOnLoad` to automatically show HUD when scene loads
- Useful for gameplay scenes that need HUD immediately

## System Flow

### Canvas Lifecycle

1. **Canvas Request**: System calls `UIController.Instance.ShowCanvas(canvasName)`
2. **Validation**: Controller checks if canvas already exists
3. **Prefab Lookup**: Controller finds prefab from registry
4. **Instantiation**: Prefab is instantiated as root GameObject
5. **Foregrounding**: New canvas is automatically brought to front
6. **Canvas Initialization**: Canvas `OnEnable()` runs, setting up UI state
7. **Removal**: System calls `UIController.Instance.RemoveCanvas(canvasName)`
8. **Cleanup**: Canvas `OnDisable()` runs, cleaning up resources
9. **Destruction**: Canvas GameObject is destroyed

### Foreground/Background Management

1. **Foreground Request**: `ForegroundCanvas()` is called
2. **Sorting Order Update**: Target canvas sorting order set to `FOREGROUND_SORT_ORDER` (10)
3. **Background Others**: All other canvases set to `BACKGROUND_SORT_ORDER` (0)
4. **Visual Result**: Foregrounded canvas appears on top

### Multiple Canvas Support

- Multiple canvases can exist simultaneously
- Only one canvas is foregrounded at a time
- Background canvases remain visible but behind foreground
- Useful for overlays (e.g., HUD behind Dialogue)

## Usage Examples

### Showing a Canvas

```csharp
// Show the pause menu
UIController.Instance.ShowCanvas(EnumCanvasUIName.PAUSE);

// Show dialogue canvas
UIController.Instance.ShowCanvas(EnumCanvasUIName.DIALOGUE);
```

### Removing a Canvas

```csharp
// Hide the pause menu
UIController.Instance.RemoveCanvas(EnumCanvasUIName.PAUSE);
```

### Checking Canvas State

```csharp
// Check if HUD is active
if (UIController.Instance.GetIsCanvasUp(EnumCanvasUIName.HUD))
{
    // HUD is displayed
}
```

### Accessing Canvas-Specific Functionality

```csharp
// Get reference to dialogue canvas
DialogueCanvas dialogueCanvas = UIController.Instance.GetReferenceToCanvas(EnumCanvasUIName.DIALOGUE) as DialogueCanvas;
if (dialogueCanvas != null)
{
    dialogueCanvas.LaunchConversation(conversationData);
}
```

### Clearing All Canvases

```csharp
// Remove all canvases (useful for scene transitions)
UIController.Instance.ClearCanvases();
```

### Playing UI Sounds

```csharp
// Play button hover sound
UIController.Instance.UISoundPlayer.PlayUISound(EnumUISound.HOVER);

// Play pause screen sound
UIController.Instance.UISoundPlayer.PlayUISound(EnumUISound.PAUSE_SCREEN);
```

## Creating a New Canvas

### Step 1: Create Canvas Script

```csharp
using UnityEngine;

public class MyNewCanvas : MonoBehaviour, ICanvasUI
{
    // Canvas-specific implementation
    
    // Interface Methods
    public EnumCanvasUIName GetCanvasName()
    {
        return EnumCanvasUIName.MAIN_MENU; // Use appropriate enum value
    }
    
    public Canvas GetCanvas()
    {
        return GetComponent<Canvas>();
    }
    
    public void ForegroundCanvas(bool foregrounded)
    {
        if (foregrounded) 
        { 
            GetComponent<Canvas>().sortingOrder = UIController.FOREGROUND_SORT_ORDER; 
        }
        else 
        { 
            GetComponent<Canvas>().sortingOrder = UIController.BACKGROUND_SORT_ORDER; 
        }
    }
    
    public int GetSortingOrder()
    {
        return GetComponent<Canvas>().sortingOrder;
    }
}
```

### Step 2: Add Enum Value (if needed)

If creating a new canvas type, add to `EnumCanvasUIName`:

```csharp
public enum EnumCanvasUIName
{
    MAIN_MENU,
    HUD,
    DIALOGUE,
    PAUSE,
    LOGO_SPLASH,
    ABOUT,
    MY_NEW_CANVAS  // Add new value
}
```

### Step 3: Create Canvas Prefab

1. Create GameObject with Canvas component
2. Attach your canvas script
3. Set up UI elements
4. Save as prefab

### Step 4: Register Prefab

1. Open `UIController` prefab or scene instance
2. Add entry to `canvasPrefabs` list
3. Set `canvasName` to your enum value
4. Assign prefab reference

### Step 5: Use the Canvas

```csharp
// Show your new canvas
UIController.Instance.ShowCanvas(EnumCanvasUIName.MY_NEW_CANVAS);
```

## Design Patterns and Principles

1. **Singleton Pattern**: `UIController` uses singleton for global access
2. **Interface Segregation**: `ICanvasUI` interface enables polymorphic canvas management
3. **Centralized Control**: Single entry point for all canvas operations
4. **Prefab-Based**: Canvases instantiated from prefabs for consistency
5. **Separation of Concerns**: Canvas implementations handle their own logic
6. **Loose Coupling**: Canvases don't need direct references to each other

## Key Features

### Multiple Simultaneous Canvases
- System supports multiple active canvases
- Foreground/background layering controls visibility
- Useful for overlays and layered UI

### Automatic Foregrounding
- New canvases automatically come to front
- Ensures user sees new UI immediately
- Prevents UI hidden behind other canvases

### Prefab-Based Instantiation
- All canvases instantiated from prefabs
- Ensures consistency across scenes
- Easy to update canvas designs

### Scene Persistence
- `UIController` persists across scenes (DDOL)
- Canvases are scene-specific (destroyed on scene change)
- Allows UI state management across scene transitions

### Event Integration
- Canvases can subscribe to game events
- Reactive UI updates based on game state
- Decoupled communication

### Sound Integration
- Centralized UI sound system
- Pooled AudioSources for performance
- Easy to add sound feedback

## Integration Points

### With Event System
- Canvases subscribe to events for reactive updates
- UI events trigger canvas state changes
- Decoupled communication between systems

### With Data System
- Canvases read from runtime data assets
- UI updates when data changes
- `RuntimeDataUpdatedEvent` triggers UI refreshes

### With Dialogue System
- `DialogueCanvas` integrates with dialogue system
- Dialogue events coordinate UI state
- Automatic game pausing during dialogue

### With Objectives System
- `ObjectivesPanel` displays objective data
- Queries `ObjectivesTracker` for information
- Updates when objectives change

### With Inventory System
- `InventoryPanel` displays inventory contents
- Queries `InventoryController` for item data
- Updates when inventory changes

### With Bootstrapper
- `Bootstrapper` manages initial UI state
- Coordinates scene transitions with UI
- Handles pause functionality

## Technical Notes

- **Canvas Discovery**: Uses `SceneManager.GetActiveScene().GetRootGameObjects()` to find canvases
- **Sorting Orders**: Uses Unity Canvas sorting order (0 = background, 10 = foreground)
- **Null Safety**: Methods include null checks and error logging
- **Memory Management**: Canvases clean up resources in `OnDisable()`
- **Performance**: Canvas discovery scans scene roots; efficient for typical UI counts
- **Scene Loading**: Canvases are destroyed on scene change unless using DDOL

## Dependencies

- Project's singleton base class (`Singleton<T>`)
- Unity Canvas system
- Unity EventSystem (managed by `EventSystemDDOL`)
- Project's event system (`EventRelay`)
- Project's data model (for reactive UI updates)
- Unity TextMeshPro (for text rendering)
- Unity UI system (Buttons, Sliders, etc.)

## Scene Setup

### Bootstrap Scene Configuration

The `UIController` is set up in `Bootstrap.unity`:

1. **UIController Prefab**: Instantiated from `Assets/Brad/Prefabs/UIController.prefab`
2. **DDOL**: Inherits from Singleton, automatically uses DontDestroyOnLoad
3. **Canvas Prefabs**: All canvas prefabs registered in Inspector
4. **UISoundPlayer**: Attached as child component with AudioSource pool

**Prefab Structure**:
```
UIController (Singleton)
└── UISoundPlayer
    ├── Source000 (AudioSource)
    ├── Source001 (AudioSource)
    └── ... (additional AudioSources)
```

### Canvas Prefab Requirements

For a canvas to work with the system:

1. **Root GameObject**: Must be at scene root (or will be when instantiated)
2. **Canvas Component**: Must have Unity Canvas component
3. **ICanvasUI Implementation**: Must implement `ICanvasUI` interface
4. **Correct Enum Value**: `GetCanvasName()` must return correct enum value
5. **Sorting Order Management**: `ForegroundCanvas()` must update Canvas sorting order

### EventSystem Setup

`EventSystemDDOL` ensures proper EventSystem management:

1. Instantiated in Bootstrap scene
2. Persists across scenes (DDOL)
3. Automatically removes duplicate EventSystems
4. Ensures UI input works correctly
