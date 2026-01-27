# Player Movement and Abilities System Documentation

## Overview

This document describes the Player Movement and Abilities system for the stealth-action game USS-Calliope.
The system provides third-person camera-relative movement with stamina management, stealth mode, double-tap directional dash, and consumable integration.
It uses an integrated component architecture on the Player GameObject with Rigidbody physics, Unity Input System, and PlayerData synchronization for UI and persistence.

## Architecture

The system follows a tightly coupled component architecture on the Player GameObject:

**Core Layer**: `PlayerController` handles input, movement physics, rotation, and coordination  
**Resource Layer**: `PlayerStamina` manages stamina drain, regeneration, and tired state  
**Ability Layer**: `CrouchInvisibility` and `DashAbility` provide stealth and mobility mechanics  
**Integration Layer**: `PlayerDataHandler` synchronization and `InventoryRuntimeData` for consumables

## Core Components

### 1. PlayerController

**Location**: `Assets/Olle/Scripts/PlayerController.cs`

**Pattern**: MonoBehaviour with required `Rigidbody`, integrates all movement systems

**Responsibilities**:
- Process movement, run, interact input via Unity Input System
- Camera-relative movement with configurable rotation styles (mouse aim or movement direction)
- Coordinate speed changes with stamina, dash, and crouch states
- Emit footstep noise based on movement type
- Handle consumable use (adrenaline, health packs)
- Manage movement lock and dash velocity override

**Key Properties**:
- Movement: `moveSpeed`, `runMoveSpeed`, `crouchMoveSpeed`, `rotationSpeed`
- Rotation: `mouse` (bool toggle for mouse vs movement rotation)
- Crouch: public `IsCrouching`
- Movement states: public `IsMoving`, `IsSprinting`, `IsDashing`, `MovementLocked`
- Events: `OnMoveEvent` (Action<Vector2>) for ability systems
- Inventory: `inventorySO` (InventoryRuntimeData), `pickupHandler` (PlayerPickupHandler)

**Key Methods**:
- `OnMove/OnRun/OnCrouch/OnInteract`: Input callbacks
- `StartDash(Vector2 dir, float speed)` / `EndDash()`: Dash velocity control
- `HandleConsumablePickup` / `TryDepleteAdrenaline` / `TryDepleteHealthPack`: Inventory integration
- `LockMovement()` / `UnlockMovement()`: State control
- `HandleNoise(bool isMoving, bool canSprint)`: Footstep timing
- `HandleRotation()`: Mouse raycast or movement direction rotation

### 2. PlayerStamina

**Location**: `Assets/Olle/Scripts/PlayerStamina.cs`

**Responsibilities**:
- Track stamina value and manage drain/regeneration cycles
- Determine sprint eligibility (`canSprint`) based on stamina and tired state
- Handle adrenaline rush effects (instant refill, no drain)
- Sync stamina state to PlayerData for UI persistence

**Key Properties**:
- Config: `maxStamina`, `drainPerSecond`, `regenAmount`, `regenInterval`, `regenDelay`
- Runtime: `currentStamina`, public `isTired`, public `AdrenalineRushActive`
- `Normalized`: public float (0-1 stamina percentage)

**Key Methods**:
- `Tick(float deltaTime, bool isTryingToSprint, bool blockRegen, out bool canSprint)`: Core update loop
- `UseStamina(float amount)`: Drain with bounds checking
- `HandleRegen(float deltaTime)`: Tick-based regeneration with delay
- `UpdateBackend(float stamina)`: Sync to PlayerData

### 3. CrouchInvisibility

**Location**: `Assets/Olle/Scripts/CrouchInvisibility.cs`

**Interface**: Integrated MonoBehaviour on Player GameObject

**Responsibilities**:
- Activate invisibility if stamina allows
- Periodic stamina drain during invisibility
- Visual transparency effect via MaterialPropertyBlock
- UI icon management and PlayerData stealth status sync

**Key Properties**:
- Config: `crouchTimeToInvisible`, `invisibleDuration`, `invisibleAlpha`
- Drain: `staminaTickAmount`, `staminaTickInterval`
- UI: `invisIcon` (Image), `iconGroup` (CanvasGroup)
- Visuals: `rendersToHide` (Renderer array)
- public `IsInvisible`

**Key Methods**:
- `SetInvisible(bool value)`: Toggle transparency and timers
- `HandleStaminaDrain()`: Periodic stamina cost
- `MakeTiredFromStealth()`: Set tired state with regen delay
- `UpdateTransparency()`: MaterialPropertyBlock alpha/blending
- `UpdateIcon()`: CanvasGroup alpha control

### 4. DashAbility

**Location**: `Assets/Olle/Scripts/DashAbility.cs`

**Requirements**: `[RequireComponent(typeof(PlayerController))]`, `[RequireComponent(typeof(PlayerStamina))]`

**Responsibilities**:
- Detect double-tap cardinal direction input
- Consume stamina and trigger directional dash
- Manage dash duration and state

**Key Properties**:
- Config: `dashSpeedMultiplier`, `dashDuration`, `doubleTapWindow`, `maxTapHoldTime`, `dashStaminaCost`

**Key Methods**:
- `HandleMoveInput(Vector2 moveInput)`: Tap detection via OnMoveEvent
- `RegisterTap(Vector2 dir)`: Double-tap validation
- `GetCardinalDirection(Vector2 input)`: Normalize to up/down/left/right
- `TryStartDash(Vector2 dashDir)`: Stamina check and PlayerController dash trigger

## System Flow

### Movement Update Cycle

**Update()**:
1. Skip if `MovementLocked` or `Time.timeScale == 0`
2. Convert input to camera-relative `_inputDir`
3. Call `PlayerStamina.Tick()` → get `canSprint`
4. Set `moveSpeed` based on crouch/sprint/tired/dash
5. `HandleNoise()`, animation update, `HandleRotation()`

**FixedUpdate()**:
1. Skip if locked/paused
2. If dashing: set Rigidbody velocity to `_dashVelocity`
3. Else: set horizontal velocity to `_inputDir * moveSpeed` or stop

### Stamina Cycle

**PlayerStamina.Tick()**:
1. Compute `canSprint = !isTired && stamina > 0 && isTryingToSprint`
2. If `canSprint` && !adrenaline: `UseStamina(drainPerSecond * deltaTime)`
3. Else if !blockRegen: `HandleRegen(deltaTime)`

### Invisibility Cycle

**CrouchInvisibility.Update()**:
1. If crouching: increment `_crouchTimer`
2. If timer ≥ threshold && stamina && !tired: `SetInvisible(true)`
3. If invisible: `HandleStaminaDrain()` + increment `_invisibleTimer`
4. If timer ≥ duration or stamina ≤ 0: `SetInvisible(false)`
5. On uncrouch: force `SetInvisible(false)`

### Dash Cycle

**DashAbility.HandleMoveInput()**:
1. On input start: record direction and `keyDownTime`
2. On input end: if short hold → `RegisterTap(dir)`
3. `RegisterTap()`: if same direction double-tap → `TryStartDash()`
4. `TryStartDash()`: deduct stamina → `PlayerController.StartDash()`
5. `DashAbility.Update()`: after `dashDuration` → `PlayerController.EndDash()`

## Asset Organization

```
Assets/Olle/Scripts/
├── PlayerController.cs
├── PlayerStamina.cs
├── CrouchInvisibility.cs
├── DashAbility.cs
├── PlayerPickupHandler.cs (internal class)
└── InventoryRuntimeData.cs (external reference)
```

**Player GameObject Components** (in scene):
- `PlayerController`
- `PlayerStamina` 
- `CrouchInvisibility`
- `DashAbility`
- `Rigidbody`
- `PlayerDataHandler`
- `NoiseEmitter`
- `PlayerAimController`
- `PlayerAnimationController`

## Scene Integration

**Player Prefab**: Contains all required components with serialized fields configured  
**Input Actions**: Move, Run, Crouch, Interact mapped to PlayerController callbacks  
**Camera**: Main Camera provides forward/right vectors for relative movement  
**PlayerDataHandler**: Syncs stamina, adrenaline, stealth status to runtime data

## Usage Example

```csharp
// From any script - use adrenaline consumable
player.GetComponent<PlayerController>().TryDepleteAdrenaline();

// Lock player during cutscene
player.GetComponent<PlayerController>().LockMovement();

// Subscribe to movement input
player.GetComponent<PlayerController>().OnMoveEvent += MyAbility.HandleMove;
```

## Design Patterns and Principles

**Component Integration**: All abilities on single GameObject for tight coordination  
**Event Delegation**: `OnMoveEvent` decouples input from abilities  
**State Synchronization**: PlayerData as single source of truth for UI/persistence  
**Resource Gating**: All abilities gated by stamina with consistent drain/regen rules  
**Configurable Thresholds**: Inspector fields for all timings, speeds, costs

## Future Extensibility

- Add `ApplyCrouchState()` for actual Y-scale crouching  
- Expand `NoiseEmitter` integration with more movement types  
- Add dash cooldown timer  
- Integrate with `PlayerHealthJavi` for more consumables  
- Add mouse sensitivity settings for rotation  
- Expand `PlayerPickupHandler` for more item types

## Technical Notes

- Uses `Rigidbody.linearVelocity` for physics-based movement with gravity preservation  
- Rotation uses `Quaternion.Slerp` for smooth camera-relative turning  
- Stamina regen uses discrete ticks for predictable timing  
- Invisibility uses `MaterialPropertyBlock` for efficient batch transparency  
- Dash uses cardinal directions only (up/down/left/right) for precise double-tap detection  
- Adrenaline coroutine preserves original speeds and normalizes movement post-rush  
- Reflection used in `MakeTiredFromStealth()` to sync stamina `_lastUseTime`  
- Input System callbacks handle `performed` for toggle actions (crouch)
