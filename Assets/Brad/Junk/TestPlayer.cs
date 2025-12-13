using UnityEngine;

public class TestPlayer : MonoBehaviour
{
    [SerializeField] private PlayerRuntimeData myData;

    private void Start()
    {
        StartCoroutine(Test());
    }


    private System.Collections.IEnumerator Test()
    {
        for (int i = 0; i < 10; i++)
        {
            yield return new WaitForSeconds(3.0f);
            myData.Value.Health -= 10;
        }
    }
}
