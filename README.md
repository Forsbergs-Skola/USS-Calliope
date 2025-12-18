## Player System

### Movement:

Walking and Sprinting

Sneaking

Stamina bar and system

### Combat: 

melee and ranged and non lethal (see below)

Health Bar

### Stats

Two ways to get stronger:

Shop - Buy items for your suit to increase stats.

Kill enemies and get xp 

### Animations/Aesthetic

Spacesuit? Infected?

## Combat System

### Ranged

#### Guns

Pistol, Shotgun, ???

Limited ammo throughout game. Maybe a super powerful gun (rpg, grenade launcher, BFG). This super powerful gun you could find ammo for it before finding the gun
to like push the player to go find it.

Need to decide on the inventory system; are we doing weight limit? Or like a physical, RE style inventory? 

Ammo is limited, presumably with more ammo for the weaker guns? With an RE style inventory, guns and ammo all take up slots with everything else so taking ALL your guns and
ammo can hurt you as you can't carry everything so adds some strategy. 

ammo, instead of one ammo type per weapon we could have "junk ammo" with high spread low damage?, easy to implement but could be a stretch.

### Melee

What are the melee weapons? 

Melee combat should feel weighty, but can't be too strong. Maybe the enemies get stunned/knocked over, or it takes some difficulty to kill enemies with it

### Non-Lethal

Taser?

Do we have non-lethal weapons OR do we focus on luring/trapping enemies as the Non-Letal


## Camera System


Camera modes: isometric for player, can switch to other modes for shop/cutscenes

Fade shader: fades objects blocking the player

Crosshair/aiming system?

## NPCs & AI

NPCs are your friends, punish the player by killing too many

### Friendly NPC types:

Shop NPC(s)

Quest NPC(s)

### Enemy types:

[Need to know what kind of NPCs we have, their mechanics and how to defeat them. So far we have just standard zombie-esque NPCs and an Invincible NPC that cannot move when the player is looking at it

Evolution mechanic]

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

### Bosses:

Just one boss? Final Boss?

## Shop System

what is currency?

what do they sell?

who are they?

Player buys items/armour/suit upgrades that increase stats. Pay with blood and lose health? Pay with ammo and lose ammo?

Suit upgrade limit slots? (requires UI)


## Level System

Aesthetics

Level layout

Checkpoints/Respawn?

Environmental hazards: list here

Collectibles/Pickups

Interactables (doors, switches)



## Inventory System


Item types (weapons, consumables, quest items)

Ammo is stackable? Health pickups stackable?

Weight / capacity limits: either weight limit or physical inventory (slots like resident evil)

Ammo is limited, presumably with more ammo for the weaker guns? With an RE style inventory, guns and ammo all take up slots with everything else so taking ALL your guns and
ammo can hurt you as you can't carry everything so adds some strategy. 

Should be like some home bases/chests where you can access a global inventory. Could be one place or multiple

## Stats/LevelUp System

Suit upgrades purchased from shop increase stats only when equipped. Bought with health (which permanently decreases health bar) and ammo

Killing enemies gets xp, get enough to permanently increase stats (like health)

How else do we level up? Do we level up sneaking by sneaking? or is that TOO much to keep track off and just level it up with shop


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

Screens:

Main menu

HUD

Inventory screen

Pause menu

Settings

Make sure it fits screen size

As immersive and unintrusive as possible

## Backend System
