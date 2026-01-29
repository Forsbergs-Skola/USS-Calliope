## Player Vision and Enemy Visibility System Documentation

### Overview

This document describes the Player Vision and Enemy Visibility system.  
The system renders a vision cone for the player and highlights enemies that are within this cone and line-of-sight, supporting stealth readability and tactical planning.  
It uses procedural mesh generation and visibility checks to drive both rendering and potential gameplay hooks.

### Architecture

The system is split into two primary components:

- **Logic Layer**: `PlayerVisionLogic` handles detection, line-of-sight checks, and enemy visibility state  
- **Rendering Layer**: `VisionConeMesh` generates and updates the visual mesh for the vision cone

### Core Components

#### 1. PlayerVisionLogic

**Location**: `Assets/Camera/PlayerVisionLogic.cs`

**Pattern**: MonoBehaviour on the Player GameObject.

**Responsibilities**:
- Define the player’s vision parameters (range, angle, height)  
- Perform detection sweeps to find visible enemies  
- Maintain and expose a list of currently visible enemies  
- Coordinate with `VisionConeMesh` for cone orientation and size

**Key Properties**:
- Config:  
  - `visionRange`  
  - `visionAngle` (in degrees)  
  - `visionHeightOffset` (from player origin)  
  - `detectionInterval` (seconds between checks)  
  - `enemyLayerMask`, `obstacleLayerMask`  
- References:  
  - `visionConeMesh` (VisionConeMesh)  
  - Optional list/registry of potential enemy targets  
- Runtime:  
  - `VisibleEnemies` (read-only list or HashSet<EnemyBase>)  
  - Last detection timestamp

**Key Methods**:
- `Update()`: Periodically trigger detection when `Time.time >= _nextCheckTime`  
- `ScanForVisibleEnemies()`:  
  1. Collect potential enemies within `visionRange` (Physics.OverlapSphere or maintained list)  
  2. For each candidate, test angle vs player forward  
  3. Perform raycast(s) against `obstacleLayerMask` to confirm line-of-sight  
  4. Update `VisibleEnemies` set  
- `IsInVisionCone(Vector3 worldPos)`: Utility to determine if a world point is inside cone (ignoring obstacles)  
- `NotifyEnemyVisibilityChanged(EnemyBase enemy, bool isVisible)`: Optional event dispatch for AI/UX hooks

#### 2. VisionConeMesh

**Location**: `Assets/Camera/VisionConeMesh.cs`

**Pattern**: MonoBehaviour with `MeshFilter` and `MeshRenderer`, controlled by `PlayerVisionLogic`.

**Responsibilities**:
- Generate a cone/wedge mesh at runtime based on vision parameters  
- Update mesh as range/angle change (e.g. due to abilities)  
- Align the cone with the player’s forward direction and position

**Key Properties**:
- Config:  
  - `segments` (resolution of the cone arc)  
- References:  
  - `meshFilter`  
  - `meshRenderer`  
- Runtime:  
  - Cached `Mesh` instance  
  - Current range/angle used for generation

**Key Methods**:
- `Initialize()`: Create and assign a new Mesh to the `MeshFilter`  
- `UpdateCone(float range, float angle)`: Rebuild vertices/triangles for the cone mesh  
- `LateUpdate()`: Align cone transform to player/camera orientation, apply `visionHeightOffset`  
- `SetVisible(bool visible)`: Enable/disable cone rendering (e.g. in UI modes)

### System Flow

**Detection Cycle (`PlayerVisionLogic.Update`)**:
1. Every `detectionInterval`, call `ScanForVisibleEnemies`  
2. For each potential enemy:  
   - Check distance ≤ `visionRange`  
   - Compute angle between player forward and direction to enemy  
   - If angle ≤ `visionAngle * 0.5f`, proceed  
   - Raycast from eye position to enemy center using `obstacleLayerMask`  
   - If unobstructed, mark enemy as visible  
3. Maintain `VisibleEnemies` set, send events for enter/exit visibility

**Vision Cone Rendering**:
1. `PlayerVisionLogic` passes `visionRange` and `visionAngle` to `VisionConeMesh.UpdateCone`  
2. `VisionConeMesh` generates a fan of vertices from origin out to arc at `visionRange`  
3. `LateUpdate` aligns cone with player forward, offset by `visionHeightOffset`  
4. Material can be configured (e.g. semi-transparent color, stencil, or minimap-only layer)

### Asset Organization

- `Assets/Camera/PlayerVisionLogic.cs`  
- `Assets/Camera/VisionConeMesh.cs`  
- Optional:  
  - Shared materials for the vision cone (e.g. `VisionCone_Mat`)

### Scene Integration

- Player or camera rig GameObject:  
  - Add `PlayerVisionLogic`  
  - Add child object `VisionCone` with `MeshFilter`, `MeshRenderer`, and `VisionConeMesh`  
- Configure:  
  - Vision range/angle in Inspector  
  - Layers: enemies on `Enemy` layer, obstacles on `Default`/`Environment` layer  
  - Cone material (e.g. additive, emissive, or UI-only)

### Usage Example

```csharp
// Query if a specific enemy is currently visible to the player
if (playerVisionLogic.VisibleEnemies.Contains(enemy))
{
    enemy.Highlight(true);
}

// Ability temporarily extends vision range
playerVisionLogic.SetVisionRange(playerVisionLogic.VisionRange * 1.5f);
visionConeMesh.UpdateCone(playerVisionLogic.VisionRange, playerVisionLogic.VisionAngle);
```

### Design Principles

- **Visual-Gameplay Alignment**: The rendered cone matches the actual detection logic  
- **Performance-Aware**: Uses interval-based scans rather than every frame if needed  
- **Extensible**: Parameters can be modified by abilities (e.g. gadgets, adrenaline vision)  
- **Decoupled Rendering**: Vision logic is independent of the exact mesh/visual style
