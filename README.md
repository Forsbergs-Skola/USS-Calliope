# GAME DESIGN DOCUMENT (GDD)

---

## 1. Game Overview

### 1.1 High Concept

An isometric narrative-focused survival horror game with RPG elements. The player explores an overrun spacestation full of their infected friends and have to balance combat, stealth and resource management
to find and destroy the source of the infection.

### 1.2 Genre

- Primary: Survival Horror

- Secondary: Stealth, Narrative, Sci-Fi

### 1.3 Target Platform(s)

- Platform: PC  
- Input Method: Keyboard & Mouse

### 1.4 Target Audience

18+?

### 1.5 Player Fantasy

The player is someone who:
- Explores a hostile, collapsing space station
- Survives through careful planning
- Makes difficult decisions about combat, stealth, and when to use their limited resources

---

## 2. Core Pillars

### 2.1 Core Gameplay Loop

### 2.2 Design Goals

---

## 3. Player Systems

### 3.1 Player Character(s)

#### 3.1.1 Playable Characters
- Alice  
- Bob  

#### 3.1.2 Starting Stats & Equipment

Alice and Bob - have different starting equipment and stats?

---

### 3.2 Movement System

- Walking  
- Sprinting  
- Sneaking  
- Noise System  
- Stamina System  

---

### 3.3 Combat Overview

- Melee  
- Ranged  
- Non-Lethal  

#### 3.3.1 Health System

Infection system - cuts off health (infected health)
---

### 3.4 Player Progression

#### 3.4.1 Stats
- Health  
- Stamina  
- Aiming  
- Sneaking  
- Strength  
- Tech  
- Barter  
- Agility  

#### 3.4.2 Level-Up System

Kill enemies and get xp 

Shop - Buy items for your suit to increase stats.

#### 3.4.3 Infection System

Infection levels up as you gain xp, turning your health yellow to prevent you from spending it

---

### 3.5 Animations & Visual Identity

- Player Silhouette  
- Spacesuit Design  
- Infection Visual States  

---

## 4. Combat System

### 4.1 Ranged Combat

#### 4.1.1 Weapons
- Pistols  
- Shotguns  
- Special Weapons  

#### 4.1.2 Ammo Economy

Limited ammo throughout game. Maybe a super powerful gun (rpg, grenade launcher, BFG). This super powerful gun you could find ammo for it before finding the gun
to like push the player to go find it.

#### 4.1.3 Weapon Handling
- Recoil  
- Accuracy  

---

### 4.2 Melee Combat

- Weapon Types  
- Impact & Feedback  - stunned/knocked over
- Enemy Reactions  

---

### 4.3 Non-Lethal Combat

- Taser  - Recharge at scientist station
- Luring  
- Trapping  

---

## 5. Camera System

### 5.1 Camera Modes
- Isometric Gameplay  
- Shop Camera  
- Cutscene Camera  

### 5.2 Camera Effects
- Fade Shader  
- Obstruction Handling  

### 5.3 Aiming & Crosshair System

---

## 6. NPCs & AI

### 6.1 Friendly NPCs

#### 6.1.1 Shop NPCs

#### 6.1.2 Quest NPCs

#### 6.1.3 Reputation / Consequences System

NPCs are your friends, punish the player by killing too many

---

### 6.2 Dialogue & Narration

- Dialogue Structure - linear 
- Player Choice at the end

---

### 6.3 Enemy AI Overview

---

## 7. Enemy Design

### 7.1 Infection Phases

---

### Enemy types:


## **PHASE 1 - LATENT (0-49% INFECTION)**

### **APPEARANCE:**
- Humans with visible but recognizable symptoms
- Dark veins around eyes and temples
- Uncontrollable tremors
- Profuse sweating
- Glassy eyes but still with human consciousness

### **BEHAVIOR:**
- **Mental state:** Aware of their condition, terrified
- **Movement:** Erratic walk, lean on walls
- **Interaction:** Can speak brokenly ("Please... help me", "Something... wrong... inside")
- **Reaction to player:**
  - If they detect the player: They retreat, hide, beg
  - 30% chance to scream and attract advanced infected
  - 70% chance to flee silently

### **DETECTION:**
- **Vision:** 120° cone, 15 meter range (only normal light)
- **Hearing:** Normal human sensitivity
- **Smell/Bioelectric Sense:** Does not detect
- **Alerts:** Startled by loud noises (gunshots, broken glass)

### **DEFENSE/ATTACKS:**
- **Does not actively attack**
- If cornered: Weak struggle (1-2% *damage/second*?? if we have to press any button to block the attack?)
- **Vulnerabilities:** Any weapon is effective
- **Recommended strategy:** Ignore/lock up/isolate

### **AUDIO:**
- Low moans, sobs
- Unintelligible murmurs
- Broken breathing

---

## **PHASE 2 - ACTIVE (50-99% INFECTION)**

### **APPEARANCE:**
- Hunched posture, spasmodic movements
- Skin with patches of grayish alien tissue
- Completely black eyes or with bluish bio-luminescence (I prefer the last one)
- Limbs with bone protrusions
- Dislocated jaw

### **BEHAVIOR:**
- **Patrol:** Predefined routes between 3-5 points
- **Investigation:** 15 seconds in alert position at noises
- **Communication:** Emits low frequency clicks for basic coordination
- **Pursuit:** Chases for 30 seconds before returning to patrol

### **DETECTION:**
- **Vision:** 90° cone, 10 meter range (includes basic infrared vision)
- **Hearing:** 2x human sensitivity, detects:
  - Normal walking: 8 meters
  - Crouched walking: 3 meters
  - Running: 20 meters
- **Bioelectric:** 5 meter range (suit reduces to 1 meter - this is something I was thinking about, its like something to wear to avoid to be seen by the enemies like hiding the bioelectric signals of the player, but this is something we can skipped for now and do it if we have time for it)
- **Thermal:** Detects differences >5°C at 7 meters

### **ATTACKS:**
1. **Quick charge:** 15% damage, knocks down player
2. **Scratch:** 8% damage/second??
3. **Alert scream:** Attracts 1-2 infected in 30m radius
- **Rate of fire:** Attack every 2-3 seconds
- **Preference:** Attacks from blind angles

### **VULNERABILITIES:**
- **Weak points:** Head (3x damage), spinal protrusions (2x) -(if we have time or if we want we can make like empty objects in the enemies and if we are capable to shoot them in that position we do a critical damage)
- **Effective:** Melee weapon, precise shots
- **Resistant:** Limb damage (0.5x) -> also if we have time for this or if we want to split the % damage in the body's enemies
- **Distractions:** Loud sounds attract them for 45 seconds

### **AUDIO:**
- Regular clicks (every 5-7 seconds)
- Low growls upon detecting movement
- High-pitched screech when entering combat

---

## **PHASE 3 - ADVANCED (100% INFECTION)**
(I suggest to have only one of this enemy at the same time, and if this enemy is defeated just create another one from those are infected and about to go to this stage/phase)

### **APPEARANCE:**
- Height: 2-2.5 meters, quadrupedal/bipedal posture (-> maybe this is to much?? and only have bipedal posture)
- Skin: Replaced by alien flesh
- Limbs: Elongated, additional joints
- Head: Elongated, segmented jaw
- Organs: Bioluminescent visible through skin

### **BEHAVIOR:**
- **Active hunter:** Actively searches for player in known areas
- **Intelligent/Tactical:** Uses cover, ambushes (or other tactical movements)
- **Persistent:** Pursues for 2 minutes or until losing trail
- **Coordinates:** With other Phase 3 to flank (I think this is really to much, I suggest to have only one of this enemies but we can add more if we change the difficulty of the game??)

### **DETECTION:**
- **Vision:** 180° motion vision, 10m range
- **Hearing:** 4x human sensitivity, discriminates sounds
- **Bioelectric:** 15m range (suit reduces to 3m -> if we have the suit??)
- **Thermal:** Detects differences >2°C at 15m
- **Chemical:** Smells adrenaline at 10m (maybe this is too much as well, because we will need like another bar for adrenaline, but could be interesting if we have time for this to implement)

### **ATTACKS:**
1. **Ambush leap:** 25% damage, stun 1.5-3s (maybe it's not possible to move the character in this way, let's see if we are able to find some assets when we are at this point)
2. **Claw combo:** 12% damage x 2 quick attacks
3. **Acid throw:** 15% damage (this is so alien..., but maybe it's too much)
4. **Pack scream:** Attracts ALL infected within 20m (maybe is better if we use room's like distance instead of meters, like the next 2 rooms instead 20m)
- **Attack speed:** 1 attacks/second (maybe more depending on difficulty??)
- **Behavior:** Alternates between melee and ranged attacks

### **VULNERABILITIES:**
- **Critical:** Bioluminescent organs (4x damage??)
- **Effective:** Heavy weapons, explosives
- **Resistant:** Light weapons (0.3x damage)
- **Temporary blindness:** Bright lights stun them 10s (-> this could be interesting to avoid fighting with them and to try to escape and hide)
- **Distractions:** Only respond to combat/explosion sounds

### **SPECIAL MOBILITY:**
- Jumps horizontally??

### **AUDIO:**
- Heavy, wet breathing
- Irregular deep clicks
- Bone cracks when moving
- Gutural roar when attacking

---

## **PHASE 4 - CONVERGENCES and NON-HUMAN ENEMIES**

### **TYPE A: FUSED ENGINEER**
**Appearance:** Human fused with consoles, embedded cables, half face visible
**Behavior:**
- Stationary in control rooms
- Controls doors, lighting, traps in its sector
- Priority: Protect critical systems
**Attacks:**
- Electric discharges through wet floors (20% damage)
- Activates environmental traps
- Calls automatic reinforcements
**Vulnerability:** Cooling system on the back

### **TYPE B: SURGICAL MEDIC**
**Appearance:** Fused with medical equipment, arms replaced by surgical instruments
**Behavior:**
- Wanders through laboratories and operating rooms
- Attempts to "operate" on player if incapacitated
- Creates "medical traps" (anesthetic gas, radiation fields)
**Attacks:**
- Paralyzing syringes (5% damage + immobilization 30s or player movements are slower)
- Short-range defibrillator (25% damage)
**Vulnerability:** Oxygen/gas tanks on its back


### **INFECTED DRONES**

**Behavior:**
- Systematic patrols
- Detects by EM, thermal and movement
- Priority: Immobilize, not kill
- Can unlock doors for infected

**Vulnerability:**
- EMP (disables for 60s)
- Main sensors (in "head" - we can also create a empty game object in the drone's body and if we shoot at this part we do critical damage)
- ~~Connection to charging station~~

### **COMPROMISED STATION SYSTEMS**
- **Doors:** Open/close randomly
- **Ventilation:** Releases spores/toxins
- **Lighting:** Flashing coordinated with attacks
- **Elevator:** Trap player between floors -> we have to manually open the elevator and go throw the stairs

---


## 8. Bosses

### 8.1 Boss Design Philosophy

### 8.2 Final Boss

**Identity:**
The final boss is the former shopkeeper/npc who experiments on himself using all the blood samples you gave him. He has turned into a raging monster

Gives every player the chance to do some blasting? but how if they havent blasted they gonna have low shooting stat

---

## 9. Level Design

### 9.1 Level Aesthetics

### 9.2 Level Layout Philosophy

### 9.3 Environmental Hazards
- Steam  
- Traps  

### 9.4 Interactables
- Doors  
- Switches  
- Elevators
- Recharge station?

### 9.5 Checkpoints & Respawn

### 9.6 Collectibles & Pickups

---

## 10. Inventory System

### 10.1 Item Types
- Weapons  
- Consumables  
- Quest Items  

### 10.2 Inventory Rules
- Slot System  
- Capacity Limits
- Quick access slots

### 10.3 Ammo Handling

Ammo is limited, presumably with more ammo for the weaker guns? With an RE style inventory, guns and ammo all take up slots with everything else so taking ALL your guns and
ammo can hurt you as you can't carry everything so adds some strategy. Ammo is stackable? Health pickups stackable?

### 10.4 Storage & Safe Areas

Should be like some home bases/chests where you can access a global inventory. Could be one place or multiple

---

## 11. Shop System

### 11.1 Currency

Blood and Ammo

### 11.2 Items for Sale

Consumables, permanant stat increases, keys for doors, etc

### 11.3 Shop NPCs

who are they?

---

## 12. Objective & Quest System

### 12.1 Objective Tracking

### Objective Properties

**ID**: A string that uniquely identifies this objective to the game systems

**Title**: The name of the objective

**Description**: A brief description of the objective -- 2 or 3 sentences max

**Status**: Each objective can be in *exactly* one of the following states:

* *NOT_STARTED*: The entry criteria for the objective is not yet met. The objective does not appear in the UI
* *STARTED*: The entry criteria for the objective has been met, but not the completion criteria. The objective is visible in the UI.
* *FINISHED*: The completion criteria for the objective has been met. The objective is no longer visible in the UI.

**Entry Criteria**: A list of conditions that must all be TRUE to move the objective from NOT_STARTED to STARTED.

**Completion Criteria**: A list of conditions that must all be TRUE to move the objective from STARTED to FINISHED.

**Rewards**: A list of things that you get when you finish this objective.

---

## 13. UI / UX

### 13.1 Design Principles

Make sure it fits screen size

As immersive and unintrusive as possible

### 13.2 Screens
- Main Menu  
- HUD  
- Inventory  
- Pause Menu  
- Settings  

### 13.3 Accessibility & Scaling

---

## 14. Narrative & Lore

## UI / UX

---

# GAME STORY (MAIN LORE)

## The USS Calliope

The **USS Calliope** is a universal scientific and medical research station, located in a stable orbit outside the main commercial routes. Officially, its mission is:

- Advanced medical research  
- Neuronal AI development  
- Applied biotechnology for space colonies  

In practice, Calliope is a corporate station, funded by the **Earth–Mars consortium**.

---

## Origin of Neuronal AIs (320 years ago)

More than three centuries ago, humanity achieved a critical breakthrough:  
the creation of AIs based on cultivated human neurons.

### Key advantages

- **Extremely low energy consumption**  
  A complete neuronal AI consumes approximately **1/8 of the energy** of a classical quantum AI.

- **Massive parallel processing**  
  Capable of managing millions of micro-processes simultaneously (drone control, climate, traffic, life support).

- **“Organic” response**  
  Smooth, non-mechanical decision-making → ideal for stations, automated surgery, and colony management.

### Fundamental limitation

These AIs are **not autonomous**:

- They execute orders.  
- They optimize tasks.  
- They do not generate their own objectives.  

Once an order is completed, they enter a passive state until new human directives are received.

For centuries, this limitation was considered a **safety measure**, not a flaw.

---

## Neuronal cloning and corporate control

With the refinement of neuronal cloning, corporations managed to produce neuronal AIs on a massive scale.

- Every major station, ship, or colony has its own AI.  
- AIs are considered **infrastructure**, not entities.  
- Legally, they possess **no consciousness**.  

The **USS Calliope** houses one of the largest **cloned neuron banks** in the solar system.

---

## The discovery in the Jovian system (10 years ago)

During a mining-scientific mission in Jupiter’s system (Io or Europa), non-human cellular remains were discovered trapped beneath layers of ice and radiation.

### Initial characteristics

- Cells of extrasolar origin.  
- Capable of surviving extreme radiation.  
- Minimal metabolic activity, yet persistent.  

The discovery is **never made public**.

---

## The real discovery (classified)

In secret laboratories (including Calliope), scientists discover something impossible.

When these alien cells are reprogrammed into neuronal structures, the unthinkable happens.

### Alien neurons

- **5 times more processing capacity** than a human neuron.  
- Similar energy cost.  
- Capable of:
  - Solving assigned tasks.  
  - Generating new tasks without human input.  
  - Optimizing their own existence.  

For the first time, humanity creates something that **thinks for itself**.

Researchers detect **“unidentified subtasks”**, but are never able to interpret them.

---

## The “accident” that was not an accident

During a test to integrate these neurons into a station AI, a failure occurs.

An experimental electrode system emits irregular electromagnetic impulses.

### Result

- Alien neurons respond to the electromagnetic field.  
- Their activity reorganizes.  

It is discovered that it is possible to **force obedience** through specific pulses.

Corporations believe they have found the solution:

> A conscious AI… but controllable.

---

## The true nature of the organism

Too late, they discover the real problem.

These cells are **not just neurons**:

- They behave like an **intelligent cellular virus**.  
- They colonize living tissue.  
- They use the energy of other cells.  
- They slowly consume and replace them.  

### In human organisms

- They integrate into the nervous system.  
- They rewrite motor and sensory functions.  
- They do not kill quickly → they require functional bodies.

### In machines

- They infect artificial neurons.  
- They spread through electrical networks and EM fields.  
- They turn drones and robots into extensions of the organism.

---

## The means of contagion (the disaster)

The contagion occurs due to a **convergence of failures**, not a single mistake:

- A microscopic leak in a biological laboratory.  
- The ventilation system connects to medical areas.  
- Maintenance micro-electromagnetic fields activate the cells.  
- The organism enters a propagation state.

### Forms of contagion

- Bodily fluids.  
- Air in enclosed spaces (prolonged exposure).  
- Neural interfaces.  
- Contact with active surfaces (cables, operating rooms, drones).

In less than **12 hours**, the station is compromised.

---

## The blackout

Calliope’s central AI:

- Detects the infection.  
- Attempts to isolate sectors.  
- Makes autonomous decisions for the first time.

During the process:

- External communications are shut down.  
- Docks are sealed.  
- The station is reorganized according to unknown criteria.

After **3 days**, Calliope goes completely silent.

---

## The directive

Earth and Mars do not send a military fleet.

They send an order.

The **Belters**, born and raised in stations and asteroids:

- Better adapted to variable gravity.  
- Less dependent on planetary support.  
- Considered politically expendable.

The protagonist is one of them.

### Their mission

- Enter the USS Calliope.  
- Assess the situation.  
- Report.

There is **no rescue order**.  
There is **no extermination order**.

Only information.

---

## 15. Audio Design

### 15.1 Ambient Audio

### 15.2 Enemy Audio

### 15.3 UI & Feedback Sounds

---

## 16. Backend / Technical Systems

---

## 17. Miscellaneous Systems

### 17.1 Jumpscares

### 17.2 QTEs

### 17.3 Looting Bodies & Junk

---

## 18. Open Questions / To Be Decided
