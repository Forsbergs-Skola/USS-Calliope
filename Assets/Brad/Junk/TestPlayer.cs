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

    private void Blah()
    {
        /////////////////////////////////////////
        // GET A LIST OF ACTIVE STATUS EFFECTS //
        /////////////////////////////////////////
        playerData.Value.GetActiveStatusEffects();

        //////////////////////////////////////
        // ADD STATUS EFFECTS TO THE PLAYER //
        //////////////////////////////////////
        playerData.Value.AddActiveStatusEffect(EnumPlayerStatusEffect.IN_STEALTH);
        DataController.Instance.PlayerRuntimeData.Value.AddActiveStatusEffect(EnumPlayerStatusEffect.BLEEDING);
        // ...etc

        ///////////////////////////////////////////
        // REMOVE STATUS EFFECTS FROM THE PLAYER //
        ///////////////////////////////////////////
        playerData.Value.RemoveActiveStatusEffect(EnumPlayerStatusEffect.POISON);

        //////////////////////////////
        // CLEAR ALL STATUS EFFECTS //
        //////////////////////////////
        playerData.Value.ClearAllActiveStatusEffects();
    }

}
