# The Combat System

This folder contains Milo’s player combat gameplay stack: input → aiming → weapon handling → attack execution → hit processing, plus supporting utilities like hit chance and ballistic spread.

## Combat Architecture (high level)

- **Input (`AttackInput`)**: Raises C# events for aim/fire/reload/weapon switching + exposes move/sprint actions used for accuracy.
- **Aiming (`PlayerAimController`)**: Computes aim direction from the mouse, rotates player while aiming, drives crosshair visuals (color + noise), and applies Cinemachine aim offset.
- **Weapon orchestration (`PlayerWeaponHandler`)**: Equips/unequips weapons, manages ammo via `AmmoModel`/inventory, enforces cooldowns/reload restrictions, and triggers attacks.
- **Execution (`PerformAttack`)**: Performs raycasts (hitscan/taser), sphere casts (melee), delegates unarmed attacks, applies ballistic spread, and forwards hits to `ImpactProcessor`.
- **Hit processing (`ImpactProcessor`)**: Applies damage/stun based on weapon type and hit target components.
- **Accuracy (`HitChance`, `BallisticsUtility`)**: Computes a normalized hit chance score and turns that into visual + ballistic inaccuracy.

## Key Scripts (by responsibility)

### Input

- `Scripts/Combat/AttackInput.cs`
  - Wraps Unity Input System `InputActionReference`s.
  - Emits events:
    - `AimStarted`, `AimStopped`
    - `FireStarted`, `FireStopped`
    - `SwitchWeaponTriggered`
    - `ReloadTriggered`
    - `UnEquipWeaponTriggered`
    - `MouseMoved(Vector2)` (every frame)
  - Exposes `MoveAction` and `SprintAction` used for inaccuracy/movement tracking.

### Aiming + Crosshair feedback

- `Scripts/Combat/PlayerAimController.cs`
  - Converts mouse position to a world target using a ground-aligned plane above the player.
  - Maintains `IsAiming` using a **hold threshold** (must hold aim for a short time to enter aim mode).
  - While aiming:
    - Rotates the player to face the target on the horizontal plane.
    - Updates animator `WeaponType` based on the currently equipped weapon.
    - Updates crosshair:
      - **Color** indicates hit chance: green (high), yellow (medium), red (low).
      - **Noise/jitter** increases as hit chance decreases (visualizing inaccuracy).
    - Applies Cinemachine camera offset (`CinemachineCameraOffset`) for aim pull.
  - Provides `TryGetAimDirection(origin, out direction)` for other systems (notably `PerformAttack`).

### Weapon handling (equip, ammo, fire modes)

- `Scripts/Combat/WeaponHandler.cs` (`PlayerWeaponHandler`)
  - Subscribes to `AttackInput` and routes actions:
    - **Unarmed**: can attack without a weapon.
    - **Melee**: executes without requiring a valid hitscan aim.
    - **Ranged**: requires aiming; supports **semi-auto** and **automatic fire** (coroutine loop).
  - Uses:
    - `WeaponCooldown` to gate firing rate.
    - `WeaponReload` to block shooting while reloading.
    - `AmmoModel` to track current magazine ammo.
    - Inventory runtime data (if available) to source/return ammo on equip/unequip.
  - Instantiates the equipped weapon’s model prefab under `firePoint` and sets some local offsets/scales.

### Attack execution

- `Scripts/Combat/PerformAttack.cs`
  - `SetCurrentWeapon(SO_WeaponType)` sets weapon context and initializes `ImpactProcessor`.
  - `Execute()` determines which attack path to run:
    - **Hitscan**: raycast per pellet (`PelletCount`) → `ImpactProcessor.ProcessHit`
    - **Non-lethal (taser)**: raycast → `ImpactProcessor.ProcessTase`
    - **Melee**: `Physics.SphereCastAll` → `ImpactProcessor.ProcessMeleeHit`
    - **Unarmed**: delegates to `UnarmedAttack.Attack(...)`
  - Ballistic spread:
    - If `SO_WeaponType.HasBallistics` is true, shot direction is perturbed with `BallisticsUtility.GetGaussianSpread(...)`.
  - Movement tracking:
    - For hitscan weapons, tracks time spent moving to inform spread (via input move action).

### Hit processing (damage/stun)

- `Scripts/Combat/ImpactProcessor.cs`
  - `ProcessHit(RaycastHit)`:
    - Computes damage at distance using `SO_WeaponType.GetDamageAtDistance(distance)`.
    - Looks for `IDamageable` in parents and calls `TakeDamage(...)`.
  - `ProcessTase(RaycastHit)`:
    - If weapon is `NonLethal`, finds `EnemyStunController` and applies stun for `StunEffectTime`.
  - `ProcessMeleeHit(...)`:
    - Applies weapon damage to `IDamageable`.
  - `ProcessUnarmedHit(...)`:
    - Applies delayed damage (coroutine) and plays punch audio at hit point.

## Hit Chance System

### Player state adapter

- `Scripts/Utility/PlayerState.cs`
  - A thin facade exposing combat-relevant state:
    - Health (`CurrentHealth`, `MaxHealth`)
    - Stamina (`CurrentStamina`, `MaxStamina`)
    - Movement/sprint state via `AttackInput` (`IsMoving()`, `IsSprinting()`)
    - Crouch state (`IsCrouching`)
  - Exposes `CurrentHitChanceScore` (reads `HitChance.CurrentHitChanceScore`).

### Hit chance scoring

- `Scripts/Utility/HitChance.cs`
  - Calculates a normalized score \[0..1\] and caches it:
    - `HitChance.CurrentHitChanceScore`
  - Factors:
    - **Distance** (normalized by `SO_WeaponType.ImpactRange`, shaped by `SO_WeaponType.DamageOverDistance` curve)
    - **Stamina** and **health** (normalized; with small offsets to avoid hard zeros)
    - **Movement/sprint multipliers** from the weapon
    - Special-case: if health ≤ 15, score is forced to 1 (assist behavior)
  - Used for:
    - Crosshair color + jitter in `PlayerAimController`
    - Spread/intensity decisions for ballistic weapons (indirectly via `PerformAttack`)

## Combat Utilities

### Ballistics (spread)

- `Scripts/Utility/BallisticsUtility.cs`
  - `GetGaussianSpread(forward, spreadStandardDeviation)`:
    - Uses Box–Muller transform to generate Gaussian offsets.
    - Applies offsets in right/up axes relative to `forward`.
    - Produces a clustered “real gun” spread around the aim direction.

### Cooldown gating

- `Scripts/Combat/WeaponCooldown.cs`
  - Simple coroutine-based fire-rate limiter:
    - `CanFire()` returns whether firing is currently allowed.
    - `StartCooldown(waitTime)` disables firing until timer elapses.

## Weapon Data (ScriptableObjects)

- `Scripts/Combat/Data/SO_WeaponType.cs`
  - Defines weapon behavior and tuning:
    - Attack category: Hitscan / NonLethal / Melee / Flashlight
    - Damage, fire rate, semi-auto vs auto
    - Ammo usage, mag size, reload time
    - Impact range, pellet count, ballistics spread deviation
    - Damage-over-distance curve (`DamageOverDistance`)
    - Movement/sprint inaccuracy multipliers
    - Stun duration for taser weapons
    - Melee reach/radius/force
    - Audio clips + visuals (model prefab, muzzle flash)
  - Provides helpers:
    - `GetDamageAtDistance(distance)`
    - `ShouldTrackMovement()` (currently for hitscan)

## End-to-end Example (what happens when you shoot)

1. Player holds aim → `AttackInput` raises aim events → `PlayerAimController` enters aiming after hold threshold.
2. `PlayerAimController` computes aim direction, rotates player, updates crosshair color/noise using `HitChance`.
3. Player presses fire → `AttackInput.FireStarted` → `PlayerWeaponHandler` checks:
   - unarmed/melee vs ranged
   - aiming requirement (ranged)
   - reload and cooldown gates
   - ammo availability (ranged)
4. `PlayerWeaponHandler` calls `PerformAttack.Execute()`.
5. `PerformAttack` raycasts/spherecasts (optionally with ballistic spread) and forwards hits to `ImpactProcessor`.
6. `ImpactProcessor` applies damage/stun to appropriate target components.

