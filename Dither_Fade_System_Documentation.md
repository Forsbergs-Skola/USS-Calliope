## Camera Fade and Obstruction System Documentation

### Overview

This document describes the Camera Fade and Custom Dither Shader system.  
The system handles dynamic fading of level geometry and props between the camera and player, using a custom URP-cloned opaque fade shader that supports dithering.  
It ensures the player remains visible in tight spaces while preserving scene readability and performance.

### Architecture

The system is built around a camera-centric detection and per-object fade control:

- **Detection Layer**: `CameraFadeDetection` raycasts/spherecasts from camera to player and detects obstructing objects  
- **Fade Control Layer**: `FadeObject` components receive fade commands and drive material properties  
- **Environment Layer**: `StairsFade` provides stair-specific behavior and grouping for multi-mesh staircases and catwalks. Can be applied to other objects.
- **Rendering Layer**: `OpaqueFade_URP` shader exposes properties for alpha/dither fade while remaining in the opaque URP pass

### Core Components

#### 1. OpaqueFade_URP Shader

**Location**: `Assets/Camera/OpaqueFade_URP.shader`

**Pattern**: Custom URP-compatible surface shader cloned by hand to be as close to the standard URP Lit shader, extending it for dither control.

**Responsibilities**:
- Render geometry in the opaque queue while supporting camera-driven transparency  
- Provide smooth or dithered fade-out/in based on a `_Fade` property  
- Preserve lighting, shadows, and existing material properties where possible

**Key Properties**:
- `_Fade` (float 0–1): 0 = fully visible, 1 = fully invisible/dithered  
- `_DitherScale`: Controls size of the dither pattern  
- `_BaseColor`: Base albedo with tint support  
- Exposed the standard URP properties: metallic, smoothness, normal map, etc.

**Key Behaviors**:
- When `_Fade > 0`, reduce effective alpha and/or apply screen-space/world-space dither  
- Keep object in opaque render queue for batching and shadow consistency  
- Optional clamping to prevent complete invisibility if desired (e.g. min alpha)

#### 2. FadeObject

**Location**: `Assets/Camera/FadeObject.cs`

**Pattern**: MonoBehaviour attached to any object that should fade when occluding the player.

**Responsibilities**:
- Manage runtime fade state for a single object (or group of renderers)  
- Drive `_Fade` (and related) shader properties via `MaterialPropertyBlock`  
- Provide time-based interpolation for fade-in/fade-out  
- Expose a simple API for `CameraFadeDetection` and other systems

**Key Properties**:
- Config:  
  - `fadeOutDuration`, `fadeInDuration`  
  - `targetFadeAmount` (e.g. 0.6–0.9)  
- References:  
  - `renderers` (Renderer[])  
- Runtime:  
  - `CurrentFade` (read-only)  
  - `IsFaded` (bool)

**Key Methods**:
- `FadeTo(float target, float duration)`: Begin interpolating to target fade value  
- `FadeOut()`: Convenience for fading to `targetFadeAmount`  
- `FadeIn()`: Convenience for fading back to 0  
- `ApplyFade()`: Push current fade value to all renderers via `MaterialPropertyBlock`  
- `OnBecameInvisible` / `OnDisable`: Ensure fade state is reset when object is destroyed/disabled

#### 3. CameraFadeDetection

**Location**: `Assets/Camera/CameraFadeDetection.cs`

**Pattern**: MonoBehaviour on the camera (or a camera rig root) responsible for detecting occluders.

**Responsibilities**:
- Detect objects between the camera and the player that should be faded  
- Track currently faded objects and restore them when no longer occluding  
- Provide tuning for raycast type, distance, and layer masks

**Key Properties**:
- References:  
  - `cameraTransform`  
  - `playerTransform`  
- Config:  
  - `layerMask` for fadeable geometry  
  - `rayRadius` (0 → raycast, >0 → spherecast)  
  - `maxDistance`  
- Runtime:  
  - `currentObstructions` (HashSet<FadeObject>)  
  - `previousObstructions` (HashSet<FadeObject>)

**Key Methods**:
- `Update()`:  
  1. Cast (ray/sphere) from camera to player  
  2. Collect all `FadeObject` on hit colliders  
  3. Fade in objects no longer in the obstruction set  
  4. Fade out objects newly detected  
- `GetFadeObject(Collider col)`: Find or cache `FadeObject` on the hit object hierarchy  
- `ClearAllFades()`: Force all tracked objects to fade in (e.g. on scene unload or camera disable)

#### 4. StairsFade

**Location**: `Assets/Camera/StairsFade.cs`

**Pattern**: MonoBehaviour attached to be used on a catwalk so you can see under it.

**Responsibilities**:
- Provide grouped fade behavior for multi-part staircases that can occlude the player  
- Integrate with triggers or camera logic for special stair handling (e.g. always fade while on stairs)  
- Optionally override default fade amounts/durations for stair meshes

**Key Properties**:
- References:  
  - `stairFadeObjects` (FadeObject[])  
  - Optional `triggerCollider` for entering/exiting stairs  
- Config:  
  - `autoFadeWhenPlayerOnStairs` (bool)  
  - Stair-specific fade durations/targets

**Key Methods**:
- `OnTriggerEnter(Collider other)` / `OnTriggerExit(Collider other)`:  
  - If player enters, fade all `stairFadeObjects` out  
  - If player exits and not needed by line-of-sight, fade them back in  
- `SetStairsFaded(bool faded)`: Batch call `FadeOut`/`FadeIn` on all child `FadeObject`s  
- Optional helper for `CameraFadeDetection` to tag stair objects differently (e.g. different fade target)

### System Flow

**Per-Frame Camera Update (`CameraFadeDetection.Update`)**:
1. Compute vector from camera to player, clamp to `maxDistance`  
2. Perform ray/sphere cast using `layerMask`  
3. Build new set of obstructions (`newObstructions`)  
4. For each `FadeObject` in `previousObstructions` but not in `newObstructions`: call `FadeIn()`  
5. For each `FadeObject` in `newObstructions` but not in `previousObstructions`: call `FadeOut()`  
6. Swap sets for next frame

**FadeObject Update (if using coroutine or manual update)**:
1. Interpolate `CurrentFade` toward target based on duration  
2. Call `ApplyFade()` to update `MaterialPropertyBlock` on each renderer  
3. When close enough to target, snap and stop interpolation

**Stairs Fade Flow**:
- When player enters stair trigger: `StairsFade` sets stair meshes to faded state  
- When player exits: either immediately fade in, or defer to `CameraFadeDetection` if still directly occluding

### Asset Organization

- `Assets/Camera/OpaqueFade_URP.shader`  
- `Assets/Camera/FadeObject.cs`  
- `Assets/Camera/CameraFadeDetection.cs`  
- `Assets/Camera/StairsFade.cs`

### Scene Integration

- Main Camera (or rig root) has `CameraFadeDetection` configured with:  
  - Player transform reference  
  - Layer mask for fadeable geometry  
- Any mesh to be faded:  
  - Uses `OpaqueFade_URP` material  
  - Has `FadeObject` on the same GameObject or parent  
- Staircases:  
  - Stair root with `StairsFade`  
  - Child meshes with `FadeObject`  
  - Optional trigger collider under the same root

### Usage Example

```csharp
// Force fade a specific object (e.g. during a scripted sequence)
var fadeObj = someWall.GetComponent<FadeObject>();
fadeObj.FadeOut();

// Reset all fades when switching cameras
cameraFadeDetection.ClearAllFades();
```

### Design Principles

- **Non-invasive**: Uses `MaterialPropertyBlock` so multiple objects can share materials safely  
- **Camera-centric**: All decisions are made from the camera→player line of sight  
- **Configurable**: All durations, fade amounts, and layer masks are Inspector-driven  
- **Shader Reuse**: Based on URP Lit to keep consistent visuals and lighting

