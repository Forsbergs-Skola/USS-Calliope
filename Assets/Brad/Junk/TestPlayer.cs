using UnityEngine;

public class TestPlayer : MonoBehaviour
{
    [SerializeField] private PlayerRuntimeData playerData;

    private void Start()
    {
        StartCoroutine(TakeDamageTest());
    }

    private System.Collections.IEnumerator TakeDamageTest()
    {
        for (int i = 0; i< 10; i++)
        {
            yield return new WaitForSecondsRealtime(3f);
            playerData.Value.Health -= 1;
        }
    }
}
