NoiseEmitter System Documentation

## Overview

This document describes the **NoiseEmitter** system for **USS-Calliope**.

The system generates expanding particle rings at the emitter's position to visualize sound propagation from **player movement** (walk, run, crouch) or **object interactions** (e.g., breaking boxes).

Enemies within the radius receive a **HeardPlayer** alert, switching to **investigation/chase mode** until line-of-sight is lost.

## Core Components

**NoiseEmitter** (on player or interactables): Handles radius-based emission and enemy alerting.

**EnemyChase** (on enemies): Reacts to `HeardPlayer(Vector3 noisePos)` by setting chase state and targeting noise location.

**ParticleSystem** (assigned to `ringParticles`): Visual ring effect scaled to match radius.

## Key Features

- **Movement-Tied Emission**: `EmitWalk()`, `EmitRun()`, `EmitCrouch()` trigger from **PlayerController** based on state (e.g., footstep intervals).
- **Radius Variation**: **Walk (5f)**, **Run (9f)**, **Crouch (2f)** — configurable in Inspector.
- **Enemy Detection**: `Physics.OverlapSphere` finds **EnemyChase** components on **enemyMask** layer.
- **Visual Feedback**: Ring starts at emitter position, scales to **radius * 2f** diameter.
- **Reusable**: Works on player (**PlayerNoiseEmitter** renamed) or objects like destructible boxes with **Interactable**.

## Usage Flow

1. **Emission Trigger** (e.g., PlayerController `Update()`): Call `EmitRun()` on movement.
2. **EmitRing(radius)**:
   - Scale/emit particle ring.
   - `OverlapSphere(transform.position, radius, enemyMask)`.
   - `enemy.HeardPlayer(transform.position)` for each hit.
3. **Enemy Reaction**: **EnemyChase** sets `investigating = true`, moves to noise pos, checks for player sight.

## Inspector Setup

**NoiseEmitter**:
- `walkRadius` (**5f**), `runRadius` (**9f**), `crouchRadius` (**2f**).
- `enemyMask`: **Enemy** layer only.
- `ringParticles`: Drag **ParticleSystem** prefab (**Circle** shape, **Billboard**, fade-out module).

**ParticleSystem**:
- **Shape**: Circle (0 radius).
- **Renderer**: **Billboard** mode, ring sprite texture.
- **Main**: Start Size (**0**), Max Particles (**1**), non-looping.

## Dependencies

- **Unity Physics** (`OverlapSphere`, **LayerMask**).
- **EnemyChase.HeardPlayer(Vector3)** method.
- Player tagged **"Player"**; enemies on custom **"Enemy"** layer.
- Optional: **Interactable.Trigger()** calls `EmitRun()` on objects.

## Integration Examples

**Player (PlayerController)**:
```
if (isRunning && noiseTimer > runNoiseInterval) {
    GetComponent<NoiseEmitter>().EmitRun();
    noiseTimer = 0f;
}
````
**Destructible Box (Interactable)**:
```
public void Trigger() {
    GetComponent<NoiseEmitter>().EmitRun();  // Loud break noise
    Destroy(gameObject);
}
```
