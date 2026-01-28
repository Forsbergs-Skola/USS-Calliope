# Player Footstep System (TDD Summary)

---

## Responsibilities & Data Flow

- **`PlayerFootstepInterval`**
  - Reads high‑level locomotion state from `PlayerController` (moving / walking / sprinting / crouching) plus stamina.
  - Maintains an internal timer and decides **when** a new step event should be triggered.
  - Calls `PlayerFootstepAudio.PlayFootstep()` whenever a step should produce sound.

- **`PlayerFootstepAudio`**
  - Owns a bank of footstep clips and decides **which clip** to use for a given step.
  - Applies small pitch variation and a simple “don’t repeat the last clip” rule.
  - Delegates playback to the pooled audio system.

- **`AudioPoolManager` / `PlayerAudioPoolHandler`**
  - Provide a reusable pool of `AudioSource` instances.
  - Guarantee each footstep sound is played on a pooled source that is returned to the pool after playback finishes.

---

## PlayerFootstepInterval – Timing Rules

- **Movement‑driven cadence**
  - **Given** the player is not moving  
  - **Then** no timer progresses and no footsteps are fired.
  - **Given** the player starts or continues moving  
  - **Then** an internal step timer advances with `Time.deltaTime` and periodically triggers `PlayFootstep()` when it exceeds the current step interval.

- **Step interval selection**
  - **Given** the player is crouching  
  - **Then** the base step interval comes from the controller’s **crouch** step interval.
  - **Given** the player is sprinting **and** not tired  
  - **Then** the base step interval comes from the **run** step interval.
  - **Otherwise**  
  - **Then** the base step interval comes from the **walk** step interval.
  - A configurable `speedScale` multiplies this base interval to tune overall cadence.

- **First‑step behavior**
  - **Given** the player has just transitioned from not‑moving to moving  
  - **Then** the timer is “primed” so the first footstep happens shortly after movement starts, not instantly and not with a long delay.

---

## PlayerFootstepAudio – Clip & Pitch Rules

- **Clip selection**
  - **Given** there are no clips configured  
  - **Then** a step request is a no‑op (no sound played).
  - **Given** at least one clip is configured  
  - **Then** a clip index is chosen at random, with a rule that avoids picking the same index as the immediately previous step when multiple clips exist.

- **Pitch variation**
  - **Given** a base pitch of 1.0 and a configured pitch‑variation range  
  - **Then** each step’s pitch is randomly offset within that range (e.g. 1.0 ± a small delta) to avoid “machine‑gun” repetition.

- **Delegation to audio pool**
  - **Given** a clip and final pitch/volume have been selected  
  - **Then** the component requests a pooled `AudioSource` from `AudioPoolManager` and asks its `PlayerAudioPoolHandler` to play the clip with those parameters.

---

## Pooled Audio – Lifetime Rules

- **Pool manager**
  - **Given** the game starts with `AudioPoolManager` present  
  - **Then** it constructs an initial pool of inactive `AudioSource` instances and exposes a global `Instance`.
  - **Given** a caller needs to play a sound  
  - **Then** `GetSource()` returns an active `AudioSource`, pulling from the queue or instantiating a new one if necessary.
  - **Given** a sound has finished playing on a pooled source  
  - **Then** `ReturnToPool()` deactivates that GameObject and re‑queues the `AudioSource` for reuse.

- **Per‑source handler**
  - **Given** `PlayerAudioPoolHandler.Play(clip, volume, pitch)` is called on a pooled source  
  - **Then** the handler configures the underlying `AudioSource`, starts playback, and runs a coroutine that waits until the clip is finished before returning the source to the pool.

---

## Invariants

- Footstep sounds are **only** emitted while the controller reports movement.
- Step cadence always reflects the current locomotion mode (walk / run / crouch) and stamina state.
- Consecutive steps avoid replaying the **exact same** clip when multiple options exist.
- Audio playback for footsteps **does not allocate new `AudioSource`s per step**; all sounds use the shared pool and return to it once complete.

