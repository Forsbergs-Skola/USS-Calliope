public interface IInfectedEntity
{
    float InfectionPercentage { get; }
    void IncreaseInfection(float amount);
}