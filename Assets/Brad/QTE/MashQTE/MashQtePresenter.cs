///////////////
// PRESENTER //
///////////////

//////////////////////////////////
// The presenter is the runtime //
// mediator of the data model   //
//////////////////////////////////

using UnityEngine;
using QTE;

public class MashQtePresenter : MonoBehaviour
{
    [SerializeField] private MashQteSO mashQteSO;

    public event System.Action<float> OnCurrentValueUpdated;
    public event System.Action OnFullyDrained;

    public void StartQTE()
    {
        mashQteSO.StartMasher();
    }

    public void StopQTE()
    {
        mashQteSO.StopMasher();
    }

    public void IngestMash()
    {
        if (mashQteSO.MashDampened) { Debug.Log("SORRY"); return; }
        mashQteSO.DampenMashInput(true);
        StartCoroutine(MashCooldown());
        mashQteSO.HandleMash();
    }

    private void Update()
    {
        if (!mashQteSO.IsRunning) return;
        float depleteThisInterval = (1 / mashQteSO.FullDepleteInterval) * Time.deltaTime;
        mashQteSO.Deplete(depleteThisInterval);

        if (mashQteSO.CurrentValue >= Mathf.Epsilon)
        {
            OnCurrentValueUpdated?.Invoke(mashQteSO.CurrentValue);
            return;
        }
        OnFullyDrained?.Invoke();
    }

    private System.Collections.IEnumerator MashCooldown()
    {
        yield return new WaitForSecondsRealtime(mashQteSO.MashInputCooldown);
        mashQteSO.DampenMashInput(false);
    }
}
