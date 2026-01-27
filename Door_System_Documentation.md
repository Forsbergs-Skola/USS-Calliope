# Doors and Terminal System Documentation

## Overview
This document describes the Door and Terminal interaction system for the stealth-action Unity project.

The system provides automatic sliding doors that respond to player proximity, with configurable locking mechanisms (keycard, terminal, keypad).

Terminals provide puzzle-based unlocking via player interaction, integrating with inventory, progression data, and visual/audio feedback.

## Architecture
The system uses a trigger-based interaction architecture.

**Door Layer:** SlidingDoor handles movement, locking, and inventory unlocking.

**Terminal Layer:** Terminal provides interaction UI, progression gating, and door activation.

**Integration Layer:** InventoryRuntimeData, ProgressionRuntimeData for persistent state.

## Core Components

### SlidingDoor
**Location:** `Assets/Olle/Scripts/SlidingDoor.cs`

**Responsibilities:**
- Automatic opening/closing based on player proximity when unlocked.
- Multiple lock types with inventory/progression integration.
- Smooth position lerping between open/closed markers.
- Door hiding/showing based on Y position threshold.
- Keycard unlocking via inventory check.

**Key Properties:**
- Movement: `doorTransform`, `lerpSpeed`, `hideThresholdY`.
- Markers: `OpenMarker`, `ClosedMarker` (Transform positions).
- Locking: `lockType` (DoorLockType enum), `isLocked`.
- Integration: `keyID` (string), `inventoryRuntime`, `openedWithTerminalWorldID`, `progressionRuntimeData`.

**Key Methods:**
- `SetPlayerInTrigger(bool InTrigger)`: Called by player trigger.
- `TryUnlock()`: Check inventory for keyID, unlock if found, trigger progression.
- `UnlockDoor()` / `LockDoor()`: Toggle `isLocked`.
- `UnlockAndBecomeFreeDoor()`: Unlock and set lockType = None.

### DoorLockType Enum
- **None:** Always open on proximity.
- **Keycard:** Requires matching keyID in inventory.
- **Terminal:** Unlocked via progression data.
- **KeypadCode:** Reserved for future implementation.

### Terminal
**Location:** `Assets/Olle/Scripts/Terminal.cs`

**Responsibilities:**
- Player detection and interaction prompt.
- Single-use activation that unlocks associated door.
- Visual feedback via lock indicator color change.
- Audio feedback and progression data persistence.
- Input System integration for interact action.

**Key Properties:**
- Input: `inputActionsAsset`.
- Door/UI: `lockedDoor` (SlidingDoor), `lockIndicator` (Renderer).
- Colors: `lockedColor`, `unlockedColor`.
- UI: `pressEText` (GameObject).
- Integration: `terminalWorldID`, `progressionRuntimeData`.
- Audio: `audioClip`, `audioSource`.

**Key Methods:**
- `OnInteract(InputAction.CallbackContext)`: Activation logic.
- `OnTriggerEnter/Exit(Collider)`: Player range detection.

## System Flow

### Door Operation
**Start():**
- If `lockType == Terminal`: Check `progressionRuntimeData` for `openedWithTerminalWorldID`.
- If found: `UnlockAndBecomeFreeDoor()`.
- Validate `keyID` for keycard doors.

**Update():**
- Compute `shouldBeOpen = !isLocked && _playerInTrigger`.
- Lerp `doorTransform.position` toward `OpenMarker` or `ClosedMarker`.
- Auto-hide door when `position.y < hideThresholdY` (`SetActive(false)`).
- Auto-show when returning above threshold.

### Terminal Activation
**OnTriggerEnter/Exit:**
- Set `playerInRange` true/false for tagged "Player".

**OnInteract (performed):**
- If `!playerInRange || activated`: return.
- Set `activated = true`.
- Destroy `pressEText`.
- Set `lockIndicator.material.color = unlockedColor`.
- Play `audioClip` via `audioSource`.
- Call `lockedDoor.UnlockAndBecomeFreeDoor()`.
- Add `terminalWorldID` to `progressionRuntimeData.Value.unlockedTerminalWorldIDs`.
- Log activation.

## Asset Organization
```
Assets/Olle/Scripts/
├── SlidingDoor.cs
├── Terminal.cs
└── DoorLockType enum (in SlidingDoor.cs)
```

## Scene Setup

**SlidingDoor GameObject:**
- Collider (Trigger)
- `doorTransform` (child with MeshRenderer)
- `OpenMarker` (empty Transform)
- `ClosedMarker` (empty Transform)

**Terminal GameObject:**
- Collider (Trigger)
- `lockIndicator` (child Renderer)
- `pressEText` prefab instance
- AudioSource

## Scene Integration
- **Player Trigger:** Player GameObject must have Collider tagged "Player".
- **Input Actions:** "Player/Interact" or "Interact" action mapped to E key.
- **Runtime Data:** InventoryRuntimeData and ProgressionRuntimeData singletons required.
- **Progression Handler:** KeyedDoorProgressionHandler.HandleDoorUnlocked(keyID) called on keycard unlock.

## Usage Example
```csharp
// Scene setup - assign in inspector:
slidingDoor.inventoryRuntime = GameObject.Find("InventoryManager").GetComponent<InventoryRuntimeData>();
slidingDoor.keyID = "keycard_blue_001";

// Terminal auto-links to door and sets terminalWorldID = "terminal_corridor_01"

// Player approaches → door slides if unlocked
// Player presses E at terminal → unlocks door permanently
```

## Design Patterns
- **State-Driven Movement:** `isLocked` + proximity → target position.
- **Data-Driven Unlocking:** Inventory/progression SOs as persistent gates.
- **Single Responsibility:** Doors handle movement, terminals handle interaction.
- **Visual Hierarchy:** Color-coded indicators, hide/show optimization.
- **Audio Integration:** One-shot feedback on activation.

## Future Extensibility
- Implement DoorLockType.KeypadCode with code entry UI.
- Add multi-stage terminals requiring multiple interactions.
- Support group doors (multiple panels).
- Add door close delay timer.
- Integrate with alarm/guard alert system on failed unlock.
- Expand KeyedDoorProgressionHandler for quest tracking.

## Technical Notes
- Uses `Vector3.Lerp` with `Time.deltaTime` for frame-rate independent smooth movement.
- Door hiding via `SetActive(false)` at `hideThresholdY` for performance.
- Terminal input uses fallback action map search ("Player/Interact" or "Interact").
- Progression data prevents terminal re-activation across sessions.
- Inventory check uses `GetQuestItemIDs()` list containment.
- Audio uses `PlayOneShot` for non-looping feedback.
- Debug logs provide unlock feedback during development.

## Dependencies
- `InventoryRuntimeData` singleton for keycard inventory.
- `ProgressionRuntimeData` singleton for terminal progression.
- Unity Input System for interact action.
- Player GameObject tagged "Player" with Collider.
- `KeyedDoorProgressionHandler` static method for keycard progression.
