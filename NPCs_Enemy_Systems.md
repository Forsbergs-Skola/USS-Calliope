# Enemy AI System Documentation

## Overview

This document describes the Enemy AI system implemented for the USS Calliope Unity project.  
The system provides modular, extensible, and data-driven behavior for all hostile NPCs, including perception, 
state-driven decision making, infection mechanics, enemy coordination, and combat interactions.

The design follows a component-based architecture that allows enemies to share common logic while 
supporting specialized behaviors (e.g. infected enemies, ranged attackers, callers).

## Architecture

The Enemy AI system follows a component-oriented architecture:

- **Perception Components**: Vision, detection checks, and awareness logic
- **Decision Components**: State machine controlling enemy behavior
- **Action Components**: Movement, attack, infection, and calling behaviors
- **Coordination Components**: Enemy-to-enemy communication system
- **Integration Components**: Events, objectives, audio, and player interaction

Each enemy is built by composing independent MonoBehaviour components on a single GameObject.

## Core Components

### 1. Enemy AI State Controller

**Location**: `Assets/Javi/Scripts/States/EnemyAIStateController.cs`

**Pattern**: Finite State Machine (FSM)

**Responsibilities**:
- Control high-level enemy behavior
- Switch between AI states based on perception and internal logic
- Act as the central coordinator for all enemy components

**Common States**:
- **Wandering**: Enemy patrols or remains idle
- **Attacking**: Enemy performs attacks
- **Watchful**: Enemy investigate the area where the player was seen
- **Dead**

**Key Features**:
- State transitions based on perception results

---

### 2. Enemy Perception System

**Components**:
**Location**: `Assets/Javi/Scripts/Utils/EnemyPerceptionSystem.cs`

**Responsibilities**:
- Determine whether the enemy can see the player
- Provide detection data to the AI state controller

**Detection Logic**:
- Distance check against view range
- Angle check using forward vector and view angle
- Raycast validation to ensure no obstacles block vision
- Layer masking to ignore irrelevant colliders

**Outputs**:
- Method: `CanSeePlayer` boolean

**Design Notes**:
- Fully reusable across enemy types


---

### 3. Infection System

**Location**: `Assets/Javi/Scripts/Infection/InfectionController.cs`

**Responsibilities**:
- Handle enemy infection logic
- Trigger phase changes via controller

**Key Features**:
- Infection progress value (0–100%)

**Gameplay Role**:
- Infection alters behavior, appearance, and abilities

---

### 4. Infection Phase Controller

**Location**: `Assets/Javi/Scripts/Phases/InfectionPhaseController.cs`

**Responsibilities**:
- Control infection stages
- Apply phase-based effects to the enemy

**Example Phases**:
- Phase Latent: Minor behavioral changes
- Phase Active: Aggressive or erratic behavior
- Phase Advance: Full transformation
- Phase FinalBoss: Full transformation

**Integration**:
- Can modify:
    - Movement speed
    - Attack patterns

---

### 5. Enemy Call System (Enemy-to-Enemy Communication)

#### Enemy Call System

**Location**: `Assets/Javi/Scripts/BattleCry/EnemyCallSystem.cs`

**Responsibilities**:
- Allow enemies to call nearby allies when engaging the player

**Logic**:
- Initial call based on probability chance
- Repeated calls while attacking, using a cooldown
- Calls are optional and configurable per enemy

#### Enemy Call Receiver

**Location**: `Assets/Javi/Scripts/EnemyCall/EnemyCallReceiver.cs`

**Responsibilities**:
- Receive call signals from other enemies
- React by switching to alert or chase state

**Features**:
- Distance-based response

**Gameplay Impact**:
- Encourages stealth and planning

---

### 6. Ranged Enemy & Projectile System

#### Enemy Projectile Spawner

**Location**: `Assets/Javi/Scripts/Attack/EnemyProjectileSpawner.cs`

**Responsibilities**:
- Spawn and launch projectiles toward the player
- Handle layer masking to ignore unwanted collisions

**Key Features**:
- Configurable target height offset (e.g. 1m on Y axis)
- LayerMask field to discard irrelevant colliders
- Projectile direction normalized toward player center mass

#### Enemy Projectile

**Responsibilities**:
- Move forward with constant speed
- Detect collision with valid targets
- Self-destruct on impact or timeout

---

### 7. Audio & Noise Interaction (Enemy Side)

**Responsibilities**:
- Enemies react to player-generated noise
- Audio sources configured with 3D spatial blending
- Hearing distance controlled via min/max distance

**Design Notes**:
- Integrates with player noise emission
- Supports stealth-based gameplay
- Can be expanded to directional hearing

---

## System Flow

### Enemy Detection Cycle

1. EnemyPerceptionSystem checks distance, angle, and raycast
2. Result passed to EnemyAIStateController
3. FSM transitions to Chase or Attack if player detected

### Combat & Coordination

1. Enemy enters Attacking state
2. EnemyCallSystem attempts initial call
3. Nearby enemies receive call and react
4. Ranged enemies spawn projectiles if applicable

---

## Data-Driven Design with ScriptableObjects

To ensure flexibility, reusability, and easy balancing, several enemy systems are implemented using Unity ScriptableObjects. 
This allows us to modify enemy behavior without changing code and enables clean separation between logic and data.

### Attack System as ScriptableObjects

Enemy attacks are implemented as ScriptableObjects.

**Responsibilities**:
- Define attack behavior and parameters
- Encapsulate attack-specific logic (damage, range, cooldown, effects)
- Allow enemies to share attacks without duplicating code

**Key Benefits**:
- Attacks can be reused across different enemy types
- Easy tuning and iteration directly from the Inspector

---

### Infection Phases as ScriptableObjects

Infection phases are also implemented as ScriptableObjects.

**Responsibilities**:
- Define a specific infection phase
- Define which attacks are available during each phase

**Key Features**:
- Each phase contains a list of Attack ScriptableObjects the enemy can perform
- Phase transitions are controlled by the Infection Phase Controller

---

### Patrol System with Zone ScriptableObjects

Enemies use a point-to-point patrol system driven by ScriptableObjects.

#### Patrol Zones

Patrol areas are defined using Patrol Zone ScriptableObjects.

**Responsibilities**:
- Group patrol points into logical navigation zones
- Allow reuse of patrol layouts across multiple enemies

#### Patrol Controller Integration

The Patrol Controller component assigned to the enemy references one or more Patrol Zone ScriptableObjects.

**Behavior**:
- Enemy selects patrol points from the assigned zones
- Enables consistent navigation behavior across multiple enemies
- Allows easy reassignment of patrol areas without modifying enemy prefabs

---

### Final Enemy Spawn Mechanics

The final enemy (boss) includes a special spawn mechanic.

**Behavior**:
- Can spawn up to three additional enemies
- Spawn locations are selected from nearby patrol points

---

## Design Patterns and Principles

1. **Finite State Machine** – Core enemy behavior control
2. **Component-Based Architecture** – Modular, reusable systems
3. **Separation of Concerns** – Clear responsibility boundaries
4. **Data-Driven Design** – Configurable via Inspector

---

## Future Extensibility

- Add hearing-based perception system
- Expand infection mutations and visual feedback
- Group tactics (flanking, formations)
- Difficulty-based AI parameter scaling
- Boss-specific AI controllers
