## Minimap and Room Discovery System Documentation

### Overview

This document describes the Minimap system.  
The system manages a dedicated minimap camera, switching between views, and progressively reveals rooms on the minimap as the player explores.  
It uses room triggers to light up/map rooms, minimize performance overhead, and provide clear player feedback.

### Architecture

The system is split into three collaborating components:

- **Camera Layer**: `MapCameraSwitch` swaps between the normal isometric camera and the full map camera and handles level toggling  
- **Room Visualization Layer**: `MinimapRoomLightUp` controls the color of individual room labels on the minimap  
- **Trigger Layer**: `MiniMapRoomTrigger` detects player entry and switches which room label is active

### Core Components

#### 1. MapCameraSwitch

**Location**: `Assets/Camera/Minimap/MapCameraSwitch.cs`

**Pattern**: MonoBehaviour on a minimap manager object, wired to two Cinemachine cameras and input actions.

**Responsibilities**:
- Toggle between the regular isometric gameplay camera and the map camera  
- Lock and unlock player movement when entering/exiting the map  
- Enable/disable a minimap room container while the map is open  
- Toggle between two ship levels when the map is open

**Key Properties**:
- Cinemachine cameras:  
  - `isoCam` (`CinemachineCamera`): normal isometric camera  
  - `mapCam` (`CinemachineCamera`): third-person map camera  
- Input:  
  - `toggleMapAction` (`InputActionReference`): opens/closes the map  
  - `toggleLevelAction` (`InputActionReference`): swaps active level while map is open  
- Player:  
  - `player` (`Olle.Scripts.PlayerController`): used to lock/unlock movement and orient the player  
- Level visuals:  
  - `LevelOne`, `LevelTwo` (`GameObject`): containers for the two level layouts  
  - `LevelOneActive` (`bool`, private): tracks which level is currently shown  
- Map UI:  
  - `MinimapRoom` (`GameObject`): root object for minimap room UI  
- State:  
  - `mapOpen` (`bool`): whether the map is currently open

**Key Methods**:
- `Start()`: Initializes `mapOpen` to false and sets initial camera priorities (`isoCam` active, `mapCam` inactive).  
- `OnEnable()` / `OnDisable()`: Subscribes/unsubscribes `OnToggleMap` and `OnToggleLevel` to the respective input actions.  
- `ToggleMap()`: Flips `mapOpen` and calls `EnterMap()` or `ExitMap()` accordingly.  
- `ApplyLevelToggle()`: Activates `LevelOne` when `LevelOneActive` is true and `LevelTwo` when false.  
- `EnterMap()`: Raises `mapCam` priority, lowers `isoCam` priority, enables `MinimapRoom`, locks player movement, orients the player to face right, and applies the current level toggle.  
- `ExitMap()`: Returns priority to `isoCam`, disables `MinimapRoom`, and starts `UnlockDelay()`.  
- `UnlockDelay()`: Coroutine that waits 0.8 seconds before calling `player.UnlockMovement()`.

#### 2. MinimapRoomLightUp

**Location**: `Assets/Camera/Minimap/MinimapRoomLightUp.cs`

**Pattern**: MonoBehaviour on a TextMeshProUGUI element representing a room node on the minimap.

**Responsibilities**:
- Represent whether a specific room is currently active/highlighted on the minimap  
- Update the color of the attached TextMeshPro label when the active state changes

**Key Properties**:
- Visual configuration:  
  - `inactiveColor` (`Color`): color used when the room is not the current one  
  - `activeColor` (`Color`): color used when the room is the current/active one  
- Runtime:  
  - `tmp` (`TextMeshProUGUI`): cached reference to the label component  
  - `isActive` (`bool`): whether this room is currently highlighted

**Key Methods**:
- `Awake()`: Caches the `TextMeshProUGUI` component and applies the initial color.  
- `OnEnable()`: Ensures `tmp` is cached and reapplies the current color.  
- `SetActive(bool active)`: Sets `isActive` and calls `ApplyColor()`.  
- `ApplyColor()`: Sets `tmp.color` to `activeColor` when `isActive` is true, otherwise to `inactiveColor`.

#### 3. MiniMapRoomTrigger

**Location**: `Assets/Camera/Minimap/MiniMapRoomTrigger.cs`

**Pattern**: MonoBehaviour on a trigger volume associated with a specific minimap room node. Lives on empty game objects in the level.

**Responsibilities**:
- Detect when the player enters a room trigger volume  
- Deactivate the previously active room node and activate this room’s node

**Key Properties**:
- `mapNode` (`MinimapRoomLightUp`): the room node to activate when the player is inside this trigger  
- `currentRoom` (`MinimapRoomLightUp`, static): tracks the globally active room node

**Key Methods**:
- `OnTriggerEnter(Collider other)`:  
  - Returns early if `other` is not tagged `"Player"`.  
  - If `currentRoom` is already `mapNode`, does nothing.  
  - If `currentRoom` is not null, calls `currentRoom.SetActive(false)`.  
  - Sets `currentRoom = mapNode` and, if not null, calls `currentRoom.SetActive(true)`.

### System Flow

**Minimap Camera Flow (`MapCameraSwitch`)**:
1. At startup, `isoCam` is active and `mapCam` is inactive.  
2. When the map input (`toggleMapAction`) is performed, `ToggleMap()` flips `mapOpen`.  
3. `EnterMap()` activates the map camera, shows `MinimapRoom`, locks player movement, rotates the player to face right, and ensures the correct level (LevelOne/LevelTwo) is shown.  
4. `ExitMap()` returns to the isometric camera, hides `MinimapRoom`, and starts a short delay before unlocking player movement.

**Room Highlight Flow (`MiniMapRoomTrigger` + `MinimapRoomLightUp`)**:
1. Player enters a trigger with `MiniMapRoomTrigger`:  
   - If this trigger’s `mapNode` is already the `currentRoom`, nothing changes.  
   - Otherwise, the previous `currentRoom` (if any) is set inactive, and the new `mapNode` is set active.  
2. `MinimapRoomLightUp.SetActive(true/false)` updates the corresponding label color via `ApplyColor()`, visually indicating which room is current.

### Asset Organization

- `Assets/Camera/Minimap/MapCameraSwitch.cs`  
- `Assets/Camera/Minimap/MinimapRoomLightUp.cs`  
- `Assets/Camera/Minimap/MiniMapRoomTrigger.cs`  
- Associated scene objects:  
  - Two Cinemachine cameras (`isoCam`, `mapCam`)  
  - Level containers (`LevelOne`, `LevelTwo`)  
  - Minimap room UI root (`MinimapRoom`)  
  - Trigger volumes per room with `MiniMapRoomTrigger` and associated `MinimapRoomLightUp`

### Scene Integration

- Map manager object:  
  - Add `MapCameraSwitch` and assign `isoCam`, `mapCam`, `player`, `LevelOne`, `LevelTwo`, `MinimapRoom`, and input actions.  
- Player:  
  - Must have the `"Player"` tag so that `MiniMapRoomTrigger` can recognize it.  
- Rooms:  
  - Each room has:  
    - A `MinimapRoomLightUp` on a TextMeshProUGUI label in the minimap UI  
    - A trigger collider in the 3D scene with `MiniMapRoomTrigger` referencing that label

### Usage Example

```csharp
// Manually open the map from another script
public MapCameraSwitch mapCameraSwitch;

void ShowMap()
{
    if (mapCameraSwitch != null)
    {
        mapCameraSwitch.ToggleMap();
    }
}
```

### Design Principles

- **Player Feedback**: Only one room label is fully highlighted at a time, clearly indicating the player’s current location.  
- **Separation of Concerns**: Camera/map opening logic, room label visuals, and trigger detection are handled in separate components.  
- **Minimal Overhead**: The system relies on simple color changes and trigger events, keeping logic lightweight and focused on readability.
