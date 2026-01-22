# TweenService Documentation

## Overview

This document describes the TweenService implementation for the USS Calliope Unity project. The system provides a lightweight, MonoBehaviour-based tweening solution for animating values over time. It supports float, Vector2, and Vector3 interpolation with multiple easing functions and directions, enabling smooth animations for UI elements, transforms, and other game objects.

## Architecture

The tweening system follows a service-based architecture with component-based execution:

- **Service Layer**: `TweenService` static class provides factory methods for creating tweens
- **Component Layer**: `Tween` MonoBehaviour handles per-frame updates and value interpolation
- **Interface Layer**: `ITween` interface defines tween control operations
- **Easing System**: Built-in easing functions for smooth animation curves

## Core Components

### 1. TweenService: Static Factory

#### `TweenService`
Static service class that creates and configures tween instances.

**Location**: `Assets/Brad/TweenService/TweenService.cs`

**Namespace**: `Tweens`

**Key Constants**:
- `MINIMUM_DURATION` (const float = 0.01f): Minimum allowed tween duration

**Factory Methods**:

#### `GetFloatTween()`
Creates a tween for animating a single float value.

**Signature**:
```csharp
public static Tween GetFloatTween(
    GameObject owner,
    float startValue,
    float endValue,
    float duration,
    EnumTweenEase ease = EnumTweenEase.LINEAR,
    EnumTweenDirection direction = EnumTweenDirection.IN_OUT,
    bool autodestruct = true
)
```

**Parameters**:
- `owner`: GameObject that owns the tween (tween GameObject becomes child)
- `startValue`: Starting float value
- `endValue`: Target float value
- `duration`: Animation duration in seconds
- `ease`: Easing function type (default: LINEAR)
- `direction`: Easing direction (default: IN_OUT)
- `autodestruct`: Whether to destroy tween GameObject when complete (default: true)

**Returns**: `Tween` instance ready to be started

**Internal Implementation**: Converts float to Vector3 (x component) for unified interpolation

#### `GetVector2Tween()`
Creates a tween for animating a Vector2 value.

**Signature**:
```csharp
public static Tween GetVector2Tween(
    GameObject owner,
    Vector2 startValue,
    Vector2 endValue,
    float duration,
    EnumTweenEase ease = EnumTweenEase.LINEAR,
    EnumTweenDirection direction = EnumTweenDirection.IN_OUT,
    bool autodestruct = true
)
```

**Parameters**: Similar to `GetFloatTween()`, but uses Vector2 for start/end values

**Returns**: `Tween` instance

**Internal Implementation**: Converts Vector2 to Vector3 (x, y components) for interpolation

#### `GetVector3Tween()`
Creates a tween for animating a Vector3 value.

**Signature**:
```csharp
public static Tween GetVector3Tween(
    GameObject owner,
    Vector3 startValue,
    Vector3 endValue,
    float duration,
    EnumTweenEase ease = EnumTweenEase.LINEAR,
    EnumTweenDirection direction = EnumTweenDirection.IN_OUT,
    bool autodestruct = true
)
```

**Parameters**: Similar to other methods, but uses Vector3 for start/end values

**Returns**: `Tween` instance

**Internal Implementation**: Uses Vector3 directly for interpolation

#### `GetEaseFunction()`
Returns an easing function based on ease type and direction.

**Signature**:
```csharp
public static System.Func<float, float> GetEaseFunction(
    EnumTweenEase ease, 
    EnumTweenDirection direction
)
```

**Returns**: Function that takes normalized time (0-1) and returns eased value (0-1)

**Purpose**: Used internally by `Tween` to calculate interpolation curves

### 2. Easing Types

#### `EnumTweenEase`
Enumeration of available easing function types.

**Location**: `Assets/Brad/TweenService/TweenService.cs`

**Values**:
- `LINEAR`: Constant speed (no easing)
- `QUAD`: Quadratic easing (t²)
- `CUBIC`: Cubic easing (t³)
- `QUART`: Quartic easing (t⁴)
- `SINE`: Sinusoidal easing (smooth curves)

**Easing Curves**:
- **LINEAR**: Linear interpolation, constant velocity
- **QUAD**: Accelerates/decelerates smoothly (quadratic curve)
- **CUBIC**: Stronger acceleration/deceleration (cubic curve)
- **QUART**: Very strong acceleration/deceleration (quartic curve)
- **SINE**: Smooth, natural motion (sine wave)

### 3. Easing Directions

#### `EnumTweenDirection`
Enumeration of easing direction types.

**Location**: `Assets/Brad/TweenService/TweenService.cs`

**Values**:
- `IN`: Ease in (slow start, fast end)
- `OUT`: Ease out (fast start, slow end)
- `IN_OUT`: Ease in and out (slow start and end, fast middle)

**Behavior**:
- **IN**: Animation starts slowly and accelerates
- **OUT**: Animation starts quickly and decelerates
- **IN_OUT**: Animation starts slowly, speeds up in middle, slows at end

### 4. Tween Component

#### `Tween`
MonoBehaviour component that executes the tween animation.

**Location**: `Assets/Brad/TweenService/Tween.cs`

**Namespace**: `Tweens`

**Interface**: Implements `ITween`

**Key Properties**:
- `OnValueUpdated` (System.Action<Vector3>): Event fired each frame with current interpolated value
- `OnFinished` (System.Action): Event fired when tween completes

**Private Fields**:
- `owner`: GameObject that owns the tween
- `startValue`: Starting Vector3 value
- `endValue`: Target Vector3 value
- `duration`: Total animation duration
- `elapsed`: Time elapsed since start
- `running`: Whether tween is currently active
- `autodestruct`: Whether to destroy GameObject on completion
- `easeFunc`: Easing function delegate

**Methods**:

- `Initialize()`: Sets up tween parameters
  - Validates duration (enforces minimum)
  - Stores all parameters
  - Gets easing function from `TweenService`
  - Called by factory methods

- `StartTween()`: Begins the tween animation
  - Sets `running` flag to true
  - Tween begins updating in `FixedUpdate()`

- `PauseTween()`: Pauses the tween
  - Sets `running` flag to false
  - Animation stops but state is preserved

- `SetRepeat(bool value)`: Placeholder for repeat functionality (TODO)

- `ResetTween()`: Placeholder for reset functionality (TODO)

**Update Loop**:
- Uses `FixedUpdate()` for frame-independent timing
- Calculates completion ratio (0-1)
- Applies easing function
- Interpolates between start and end values
- Invokes `OnValueUpdated` event with current value
- Checks for completion and invokes `OnFinished` event
- Auto-destructs if enabled

**Lifecycle**:
- GameObject created by factory method
- Parented to owner GameObject
- Initialized with parameters
- Started via `StartTween()`
- Updates in `FixedUpdate()`
- Destroys itself when complete (if autodestruct enabled)

### 5. ITween Interface

#### `ITween`
Interface defining tween control operations.

**Location**: `Assets/Brad/TweenService/TweenService.cs`

**Methods**:
- `StartTween()`: Begin animation
- `PauseTween()`: Pause animation
- `SetRepeat(bool value)`: Set repeat behavior
- `ResetTween()`: Reset to initial state

**Purpose**: Provides consistent API for tween control operations

## System Flow

### Creating a Tween

1. **Factory Call**: Code calls `TweenService.GetFloatTween()` (or Vector2/Vector3 variant)
2. **GameObject Creation**: Service creates new GameObject named "TweenFloat"
3. **Component Addition**: `Tween` component added to GameObject
4. **Initialization**: `Initialize()` called with parameters
5. **Parenting**: Tween GameObject parented to owner GameObject
6. **Return**: `Tween` instance returned to caller

### Executing a Tween

1. **Start**: `StartTween()` called, sets `running = true`
2. **Update Loop**: `FixedUpdate()` runs each frame
3. **Time Tracking**: `elapsed` incremented by `Time.deltaTime`
4. **Progress Calculation**: Completion ratio calculated (elapsed / duration)
5. **Easing**: Easing function applied to completion ratio
6. **Interpolation**: `Vector3.Lerp()` between start and end values
7. **Event Firing**: `OnValueUpdated` invoked with current value
8. **Completion Check**: If complete, `OnFinished` invoked
9. **Cleanup**: GameObject destroyed if `autodestruct` is true

### Event Subscription Pattern

1. **Create Tween**: Get tween instance from factory
2. **Subscribe to Events**: Attach handlers to `OnValueUpdated` and/or `OnFinished`
3. **Start Tween**: Call `StartTween()`
4. **Handle Updates**: `OnValueUpdated` fires each frame with interpolated value
5. **Handle Completion**: `OnFinished` fires when animation completes

## Usage Examples

### Basic Float Tween

```csharp
// Fade alpha from 1.0 to 0.0 over 2 seconds
Tween fadeTween = TweenService.GetFloatTween(
    gameObject, 
    1.0f, 
    0.0f, 
    2.0f, 
    EnumTweenEase.QUART, 
    EnumTweenDirection.OUT
);

fadeTween.OnValueUpdated += (value) =>
{
    // value.x contains the interpolated float
    Color newColor = image.color;
    newColor.a = value.x;
    image.color = newColor;
};

fadeTween.StartTween();
```

### Vector3 Position Tween

```csharp
// Move object from current position to target over 1 second
Vector3 startPos = transform.position;
Vector3 targetPos = new Vector3(10, 5, 0);

Tween moveTween = TweenService.GetVector3Tween(
    gameObject,
    startPos,
    targetPos,
    1.0f,
    EnumTweenEase.CUBIC,
    EnumTweenDirection.IN_OUT
);

moveTween.OnValueUpdated += (value) =>
{
    transform.position = value;
};

moveTween.OnFinished += () =>
{
    Debug.Log("Movement complete!");
};

moveTween.StartTween();
```

### Multiple Simultaneous Tweens

```csharp
// Fade panel and animate text lighting simultaneously
Tween fadeTween = TweenService.GetFloatTween(
    gameObject, 
    1.0f, 
    0.0f, 
    3.0f, 
    EnumTweenEase.QUART, 
    EnumTweenDirection.IN
);

Tween lightingTween = TweenService.GetFloatTween(
    gameObject, 
    0.0f, 
    6.0f, 
    2.97f  // Slightly shorter for timing
);

// Update panel alpha
fadeTween.OnValueUpdated += (value) =>
{
    Color newColor = new Color(0f, 0f, 0f, value.x);
    frontPanel.color = newColor;
};

// Update text lighting
lightingTween.OnValueUpdated += (value) =>
{
    mainText.fontMaterial.SetFloat("_LightAngle", value.x);
    subText.fontMaterial.SetFloat("_LightAngle", value.x);
};

// Handle completion
fadeTween.OnFinished += () =>
{
    Debug.Log("Fade complete!");
};

fadeTween.StartTween();
lightingTween.StartTween();
```

### Pausing and Resuming

```csharp
Tween myTween = TweenService.GetFloatTween(gameObject, 0f, 1f, 2f);

// Start
myTween.StartTween();

// Pause later
myTween.PauseTween();

// Resume
myTween.StartTween();
```

## Real-World Example: LogoSplashCanvas

The `LogoSplashCanvas` demonstrates practical tween usage:

**Location**: `Assets/Brad/Scripts/LogoSplashCanvas.cs`

**Use Case**: Logo splash screen with fade-in and text lighting effects

**Implementation**:

```csharp
private void FadeIn()
{
    // Fade panel from opaque to transparent
    Tween fadeTween = TweenService.GetFloatTween(
        gameObject, 
        1.0f,      // Start: fully opaque
        0.0f,      // End: fully transparent
        fadeDuration, 
        EnumTweenEase.QUART, 
        EnumTweenDirection.IN
    );
    
    // Animate text lighting angle
    Tween lightingTween = TweenService.GetFloatTween(
        gameObject, 
        0.0f,      // Start: no lighting
        6.0f,      // End: full lighting
        fadeDuration * 0.99f  // Slightly faster
    );
    
    // Update lighting on text materials
    lightingTween.OnValueUpdated += (value) =>
    {
        mainText.fontMaterial.SetFloat(TMProProperties.LIGHT_ANGLE, value.x);
        subText.fontMaterial.SetFloat(TMProProperties.LIGHT_ANGLE, value.x);
    };
    
    // Update panel alpha
    fadeTween.OnValueUpdated += (value) =>
    {
        Color newColor = new Color(0f, 0f, 0f, value.x);
        frontPanel.color = newColor;
    };
    
    // Handle completion
    fadeTween.OnFinished += () =>
    {
        StartCoroutine(WaitThenContinue());
    };
    
    // Start both tweens
    fadeTween.StartTween();
    lightingTween.StartTween();
}
```

**Key Points**:
- Two tweens run simultaneously
- Different durations create timing effects
- Events update UI elements each frame
- Completion triggers next phase of animation

## Design Patterns and Principles

1. **Factory Pattern**: `TweenService` creates tween instances
2. **Component Pattern**: `Tween` is a MonoBehaviour component
3. **Observer Pattern**: Events (`OnValueUpdated`, `OnFinished`) notify subscribers
4. **Service Locator**: Static service class provides easy access
5. **Separation of Concerns**: Service creates, component executes

## Key Features

### Multiple Value Types
- Supports float, Vector2, and Vector3 interpolation
- Unified internal representation (Vector3)
- Easy to extend for other types

### Flexible Easing
- Multiple easing functions (LINEAR, QUAD, CUBIC, QUART, SINE)
- Three directions (IN, OUT, IN_OUT)
- Smooth, professional animations

### Event-Driven Updates
- `OnValueUpdated` fires each frame with current value
- `OnFinished` fires on completion
- Enables reactive UI updates

### Automatic Cleanup
- Auto-destruct option removes GameObject when complete
- Prevents memory leaks
- Optional manual management

### Frame-Independent Timing
- Uses `FixedUpdate()` for consistent timing
- Works correctly with time scale changes
- Smooth animations regardless of framerate

### Owner-Based Lifecycle
- Tween GameObject parented to owner
- Automatically destroyed if owner is destroyed
- Prevents orphaned tweens

## Technical Notes

### Duration Validation
- Minimum duration enforced (`MINIMUM_DURATION = 0.01f`)
- Prevents division by zero errors
- Warns if duration too low

### Value Extraction
- Float tweens: Use `value.x` from Vector3
- Vector2 tweens: Use `value.x` and `value.y`
- Vector3 tweens: Use `value` directly

### Easing Functions
- All easing functions normalize input (0-1) and output (0-1)
- Applied to completion ratio before interpolation
- Mathematical formulas provide smooth curves

### Update Timing
- Uses `FixedUpdate()` instead of `Update()`
- Ensures consistent timing regardless of framerate
- May cause slight delay if FixedUpdate rate is low

### Memory Management
- Tween GameObjects are lightweight
- Auto-destruct prevents accumulation
- Owner destruction cleans up automatically

## Dependencies

- Unity MonoBehaviour system
- Unity GameObject/Transform system
- Unity Time system (`Time.deltaTime`)
- Unity Mathf utilities

## Known Limitations / Future Work

1. **Repeat Functionality**: `SetRepeat()` is not yet implemented (TODO)
2. **Reset Functionality**: `ResetTween()` is not yet implemented (TODO)
3. **Reverse Playback**: No built-in reverse/rewind functionality
4. **Delay Support**: No start delay option
5. **Callbacks**: Limited callback options (only OnFinished, no OnStart)
6. **Chaining**: No built-in tween chaining/sequencing
7. **Color Tweens**: No direct Color/RGBA tween support (must use Vector3/Vector4)
8. **Rotation Tweens**: No Quaternion tween support
9. **Path Animation**: No curve/path following support
10. **Performance**: Creates GameObject per tween (could use object pooling)

## Best Practices

### Event Cleanup
Always unsubscribe from events if tween persists:

```csharp
Tween myTween = TweenService.GetFloatTween(...);
myTween.OnValueUpdated += HandleUpdate;
myTween.OnFinished += HandleFinish;

// Later, if needed:
myTween.OnValueUpdated -= HandleUpdate;
myTween.OnFinished -= HandleFinish;
```

### Owner Selection
Use appropriate owner GameObject:

```csharp
// Good: Owner is the object being animated
Tween tween = TweenService.GetFloatTween(gameObject, ...);

// Good: Owner is UI panel being faded
Tween tween = TweenService.GetFloatTween(panelObject, ...);
```

### Easing Selection
Choose easing based on animation type:

- **LINEAR**: UI elements, simple movements
- **QUAD**: General purpose, smooth motion
- **CUBIC**: Stronger emphasis, dramatic effects
- **QUART**: Very dramatic, attention-grabbing
- **SINE**: Natural, organic motion

### Direction Selection
Choose direction based on animation intent:

- **IN**: Elements appearing, fading in
- **OUT**: Elements disappearing, fading out
- **IN_OUT**: General purpose, smooth start and end

### Duration Guidelines
- **UI Animations**: 0.2-0.5 seconds
- **Transitions**: 0.5-1.0 seconds
- **Dramatic Effects**: 1.0-3.0 seconds
- **Background Animations**: 2.0+ seconds

## Integration Points

### With UI System
- Used extensively in `LogoSplashCanvas` for fade effects
- Can animate any UI element property
- Works with Canvas-based UI

### With Game Objects
- Can animate Transform properties
- Works with any GameObject
- Owner-based lifecycle ensures cleanup

### With Materials
- Can animate material properties (as shown in LogoSplashCanvas)
- Useful for shader parameter animation
- Enables dynamic visual effects

## Performance Considerations

### GameObject Creation
- Each tween creates a GameObject
- Lightweight but adds overhead for many tweens
- Consider object pooling for high-frequency use

### Update Overhead
- `FixedUpdate()` runs even when paused
- Multiple tweens multiply update calls
- Generally negligible for typical use cases

### Memory Usage
- Minimal per-tween memory footprint
- Auto-destruct prevents accumulation
- Owner destruction ensures cleanup

## Extension Ideas

### Custom Easing Functions
Could extend `GetEaseFunction()` to support:
- Custom curves
- Bounce effects
- Elastic effects
- Back effects

### Additional Value Types
Could add support for:
- Color/RGBA tweens
- Quaternion rotation tweens
- Rect tweens

### Advanced Features
Could implement:
- Tween chaining/sequencing
- Parallel tween groups
- Delay support
- Repeat functionality
- Reverse playback
