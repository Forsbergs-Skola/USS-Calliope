using UnityEngine;
using System.Collections.Generic;

public class HudTester : MonoBehaviour
{
    public void ToggleStatusEffect(string effectString)
    {

        List<EnumPlayerStatusEffect> effectsList = DataController.Instance.PlayerRuntimeData.Value.GetActiveStatusEffects();

        switch (effectString)
        {
            case "IN_STEALTH":

                if (effectsList.Contains(EnumPlayerStatusEffect.IN_STEALTH))
                {
                    DataController.Instance.PlayerRuntimeData.Value.RemoveActiveStatusEffect(EnumPlayerStatusEffect.IN_STEALTH);
                }
                else
                {
                    DataController.Instance.PlayerRuntimeData.Value.AddActiveStatusEffect(EnumPlayerStatusEffect.IN_STEALTH);
                }
                return;
            case "BLEEDING":
                if (effectsList.Contains(EnumPlayerStatusEffect.BLEEDING))
                {
                    DataController.Instance.PlayerRuntimeData.Value.RemoveActiveStatusEffect(EnumPlayerStatusEffect.BLEEDING);
                }
                else
                {
                    DataController.Instance.PlayerRuntimeData.Value.AddActiveStatusEffect(EnumPlayerStatusEffect.BLEEDING);
                }
                return;
            case "POISON":
                if (effectsList.Contains(EnumPlayerStatusEffect.POISON))
                {
                    DataController.Instance.PlayerRuntimeData.Value.RemoveActiveStatusEffect(EnumPlayerStatusEffect.POISON);
                }
                else
                {
                    DataController.Instance.PlayerRuntimeData.Value.AddActiveStatusEffect(EnumPlayerStatusEffect.POISON);
                }
                return;

            default: return;
        }
    }
}
