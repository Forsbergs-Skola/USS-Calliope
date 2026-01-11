using UnityEngine;

public interface IMovementAgent
{
    float BaseSpeed { get; }
    float CurrentSpeed { get; }
    SimpleMovementAgent.MovementState CurrentState { get; }
    
    void SetMovementState(SimpleMovementAgent.MovementState state);
    void SetCustomSpeedMultiplier(float multiplier);
    void AddSpeedModifier(object source, float multiplier);
    void RemoveSpeedModifier(object source);
    void MoveTo(Vector3 position);
}