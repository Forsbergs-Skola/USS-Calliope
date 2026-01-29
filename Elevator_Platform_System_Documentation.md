# Elevator Platform System Documentation

## Table of Contents

- [Overview](#overview)
- [Design Goals](#design-goals)
- [Core Features](#core-features)
- [System Architecture](#system-architecture)
- [Player Experience Flow](#player-experience-flow)
- [Technical Specifications](#technical-specifications)
- [Inspector Configuration](#inspector-configuration)
- [Integration Points](#integration-points)
- [Future Improvements](#future-improvements)

---

## Overview

The **Elevator Platform System** manages the player's cinematic transition into the final boss encounter. It delivers a secure, immersive experience that prevents exploits, backtracking, and accidental deaths while maintaining narrative tension and player agency.

### Core Pillars
This system reinforces essential survival horror mechanics:

| Pillar | Implementation |
|--------|----------------|
| **Commitment** | One-way gate prevents return to previous areas |
| **High Tension** | Cinematic descent builds dramatic intensity |
| **No Escape** | Player fully locked into boss encounter once activated |
| **Immersion** | Invisible safety systems, no UI or button prompts |

---

## Design Goals

- ✅ Create a clear **point of no return** before the final boss fight
- ✅ Prevent player death from falling during platform movement
- ✅ Maintain immersion by avoiding UI elements and button interactions
- ✅ Lock level progression without hard cutscenes or forced narratives
- ✅ Ensure smooth, frame-rate independent platform movement
- ✅ Provide configurable waypoints and movement speed

---

## Core Features

### 1. Platform Movement

**Behavior:**
- Moves smoothly between predefined waypoints using `Vector3.MoveTowards()`
- Frame-rate independent motion via `Time.deltaTime`
- Waypointarray-based traversal system
- Automatic reversal at endpoint boundaries

**Configuration:**
- `speed` - Controls platform movement velocity (Inspector configurable)
- `startPoint` - Initial waypoint index (default: 0)
- `movePoints[]` - Array of Transform waypoints

### 2. Automatic Activation

**Trigger Mechanism:**
- Player enters trigger zone (handled by `TriggerPlatform.cs`)
- No button press or UI interaction required
- Sets `canMove = true` to initiate descent
- Maintains immersive pacing throughout descent

**Benefits:**
- Natural level discovery without explicit interaction cues
- Preserves cinematic tension during critical moment
- Reduces cognitive load—player enters elevator, descent begins

### 3. Safety Systems

#### Dynamic Safety Walls
- **Invisible colliders** that activate only during platform movement
- Prevents player from falling off platform perimeter during descent
- Automatically deactivate when platform stops
- Reference: `walls` GameObject in `PlatformMoving.cs`

**Activation Logic:**
```
When canMove = true  → walls.SetActive(true)
When canMove = false → walls.SetActive(false)
```

#### Permanent Edge Barriers
- **Static colliders** around platform perimeter
- Provide fall prevention during idle/waiting states
- Always active unless explicitly disabled
- Maintains safety when platform is stationary

#### One-Way Boss Lock (Point of No Return)
Once platform descent begins:
- **Invisible collider blocks the entrance** (`liftBlock` GameObject)
- Player cannot return to previous areas/central corridor
- Gate remains locked even if elevator returns to starting position
- Reinforces narrative stakes and commitment

**Result:** Player is fully committed to final boss encounter. No escape, no backtracking.

---

## System Architecture

### Component Overview

```
PlatformMoving (Main Controller)
├── TriggerPlatform (Activation)
├── ParentPlatform (Player Binding)
└── Safety Colliders (Walls)
```

### `PlatformMoving.cs`

**Primary Responsibilities:**
- Manages platform movement logic between waypoints
- Controls safety collider activation/deactivation
- Locks/unlocks level progression elements
- Tracks movement state and direction

**Movement Logic:**
```csharp
if (Vector3.Distance(transform.position, movePoints[i].position) < 0.01f)
{
    // Waypoint reached
    canMove = false;
    // Handle direction reversal at endpoints
}

if (canMove)
{
    // Move toward current waypoint
    transform.position = Vector3.MoveTowards(
        transform.position, 
        movePoints[i].position, 
        speed * Time.deltaTime
    );
    
    // Activate safety systems
    walls.SetActive(true);
    centralCorridor.SetActive(false);
    liftBlock.SetActive(true);
}
else
{
    // Deactivate safety systems when stopped
    walls.SetActive(false);
    centralCorridor.SetActive(true);
    stairwayPlatformCollider.SetActive(true);
}
```

### `TriggerPlatform.cs`

**Primary Responsibility:**
- Detects player entry into elevator trigger zone
- Initiates platform descent

**Activation Logic:**
```csharp
void OnTriggerEnter(Collider other)
{
    platform.canMove = true;  // Begin descent
}
```

**Notes:**
- Uses `OnTriggerEnter()` for seamless detection
- Assumes trigger collider has `isTrigger = true`
- Requires `TriggerPlatform` attached to same GameObject as `PlatformMoving`

### `ParentPlatform.cs`

**Primary Responsibility:**
- Binds player to platform during movement
- Ensures stable, aligned motion with platform

**Parenting Logic:**
```csharp
OnCollisionEnter() → SetParent(platform.transform)
OnCollisionExit()  → SetParent(null)
```

**Notes:**
- Uses physics-based collision detection
- Requires collider on both platform and player
- Ensures player moves with platform without jitter

---

## Player Experience Flow

### Step-by-Step Progression

```
1. DISCOVERY
   └─ Player explores level naturally
   └─ Encounters elevator platform

2. ENTRY
   └─ Player steps onto platform (collision triggers parenting)
   └─ No UI prompts, no button press required

3. ACTIVATION
   └─ Player reaches trigger zone
   └─ Descent automatically initiates
   └─ Cinematic tension builds

4. DESCENT
   └─ Safety walls invisibly activate
   └─ Previous area access blocked (liftBlock)
   └─ Smooth movement to boss level
   └─ Player feels committed, no escape

5. ARRIVAL
   └─ Platform reaches final waypoint
   └─ Safety systems deactivate
   └─ Boss encounter begins
   └─ Point of no return fully established
```

---

## Technical Specifications

### Performance Characteristics

| Metric | Value | Notes |
|--------|-------|-------|
| **Update Frequency** | Every frame | Smooth motion via `Time.deltaTime` |
| **Distance Threshold** | 0.01f units | Fine-grained waypoint detection |
| **Physics Checks** | Collision/Trigger | Standard Unity physics |
| **Active GameObjects** | 1-4 (dynamic) | Minimal overhead |
| **Memory Footprint** | Negligible | Only waypoint array overhead |

### Dependencies

| Component | Type | Purpose |
|-----------|------|---------|
| `Rigidbody` | Physics | Required for collision detection |
| `Collider` | Physics | Required for parenting and triggers |
| `Transform` | Core | Waypoint positioning |

---

## Inspector Configuration

### PlatformMoving Inspector Setup

```
Script: PlatformMoving
├─ Can Move (bool) [readonly during gameplay]
├─ Speed (float) [default: 2.0]
├─ Start Point (int) [default: 0]
├─ Move Points (Transform[])
│  ├─ Element 0: [Waypoint 1]
│  ├─ Element 1: [Waypoint 2]
│  └─ Element N: [Waypoint N]
├─ Walls (GameObject)
├─ Central Corridor (GameObject)
├─ Lift Block (GameObject)
└─ Stairway Platform Collider (GameObject)
```

### Recommended Values

**Speed Tuning:**
- `speed = 2.0` - Moderate descent (cinematic feel)
- `speed = 1.0` - Slow, ominous descent
- `speed = 4.0` - Rapid descent (emergency)

**Waypoint Count:**
- Minimum: 2 (top and bottom)
- Recommended: 3-5 (smooth transition with intermediate stops)
- Maximum: 10 (avoid performance issues)

### GameObject References

**Critical Setup:**
1. Ensure all GameObjects in Inspector are assigned
2. Verify colliders have `isTrigger = true` for trigger zones
3. Ensure `movePoints` array is fully populated
4. Test waypoint transitions in Scene view before runtime

---

## Integration Points

### Interaction with Other Systems

#### Event System
- **Current:** Direct GameObject references
- **Future:** Consider EventChannel integration for loose coupling

#### UI System
- **Exclusion:** Intentionally no UI during descent
- **Maintenance:** Ensure HUD is hidden if needed

#### Audio System
- **Integration Point:** `PlatformMoving.canMove` state change
  - Could trigger elevator motor sounds
  - Could queue alarm/warning audio

#### Dialogue System
- **Integration Point:** Pre-descent dialogue before player enters trigger
  - NPCs could warn player about point of no return

---

## Future Improvements

### Planned Enhancements

#### Architecture Improvements
- [ ] **State Machine Conversion** - Replace `canMove` boolean with State Machine pattern
  - States: `Idle`, `Ascending`, `Descending`, `Locked`
  - Clearer state transitions and behavior logic

- [ ] **Event System Integration** - Replace direct GameObject references
  - Use EventChannels for platform movement events
  - Decouple safety system activation from core logic
  - Example: `OnPlatformDescentBegan`, `OnPointOfNoReturnReached`

#### Gameplay Enhancements
- [ ] **Audio Integration**
  - Elevator motor sounds during descent
  - Alarm/warning sirens before activation
  - Metal creaking and mechanical sounds

- [ ] **Lighting Changes**
  - Fade corridor lights as player descends
  - Introduce boss-level lighting atmosphere
  - Dynamic intensity changes based on descent progress

- [ ] **Visual Effects**
  - Particle effects for elevator activation
  - Screen shake/camera movement during descent
  - Fog/darkness effects as player descends

- [ ] **Waypoint Visualization**
  - Gizmos in Editor to preview waypoint path
  - Debug visualization during gameplay

#### Robustness Improvements
- [ ] **Multiple Platform Support** - Handle multiple elevators in level
- [ ] **Player Count Validation** - Ensure only intended player enters
- [ ] **Graceful Error Handling** - Handle missing GameObjects references
- [ ] **Animation Support** - Integrate with Animator for smooth transitions

---

## Related Documentation

- [Technical Design Documentation Index](Technical_Design_Documentation_Index.md)
- [Event System Documentation](Event_System_Documentation.md)
- [UI System Documentation](UI_System_Documentation.md)

---

## Credits & Version History

| Version | Date | Author | Notes |
|---------|------|--------|-------|
| 1.0 | Jan 29, 2026 | Abdessamad | Initial implementation and documentation |
| 1.1 (Planned) | TBD | - | Event system integration |
| 1.2 (Planned) | TBD | - | Audio and visual enhancements |
