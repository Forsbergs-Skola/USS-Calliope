using UnityEngine;

public class TestPlayer : MonoBehaviour
{
    [SerializeField] private PlayerRuntimeData playerData;

    private void Start()
    {
        StartCoroutine(TakeDamageTest());
    }

    public void TakeDamage(int damageAmount)
    {
        playerData.Value.Health -= damageAmount;
    }

    private System.Collections.IEnumerator TakeDamageTest()
    {
        for (int i = 0; i< 10; i++)
        {
            yield return new WaitForSecondsRealtime(3f);
            TakeDamage(2);
        }
    }
}
