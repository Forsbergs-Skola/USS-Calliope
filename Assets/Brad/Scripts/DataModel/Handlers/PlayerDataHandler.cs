using UnityEngine;

public class PlayerDataHandler : MonoBehaviour
{
    [SerializeField] private PlayerRuntimeData runtimeData;
    public PlayerRuntimeData RuntimeData { get => runtimeData; }

    private void Start()
    {
        StartCoroutine(TestRoutine());
    }

    private void TakeDamage(int damageAmount)
    {
        runtimeData.Value.Health -= damageAmount;
    }
    private void GainXP(int xpAmount)
    {
        runtimeData.Value.XP += xpAmount;
    }

    private System.Collections.IEnumerator TestRoutine()
    {
        for (int i = 0; i < 10; i++)
        {
            yield return new WaitForSecondsRealtime(1f);
            TakeDamage(2);
            GainXP(1);
        }
    }

}
