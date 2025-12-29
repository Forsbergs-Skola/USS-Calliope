public interface IPhaseBehavior
{
    void OnEnterPhase(SO_InfectionPhaseData data);
    void OnExitPhase();
    void Tick();
}