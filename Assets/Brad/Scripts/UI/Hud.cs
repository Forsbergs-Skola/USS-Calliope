using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Linq;
using Events;
using TMPro;


[System.Serializable]
public struct StructStatusEffectIcon
{
    public EnumPlayerStatusEffect effect;
    public GameObject iconPrefab;
}

public class Hud : MonoBehaviour, ICanvasUI
{

    

    [SerializeField] private IRuntimeDataPayloadEvent runtimeDataUpdatedEvent;
    [SerializeField] private Slider healthSlider;
    [SerializeField] private GridLayoutGroup statusEffectsGrid;
    [SerializeField] private TMP_Text xpText;

    [Header("Prefabs")]
    [SerializeField] private List<StructStatusEffectIcon> effectIcons;

    private List<EnumPlayerStatusEffect> statusEffectsList = new List<EnumPlayerStatusEffect>();


    //[SerializeField] private Dictionary<EnumPlayerStatusEffect, GameObject> iconPrefabs;

    private void OnEnable()
    {
        runtimeDataUpdatedEvent.OnEventTriggered += HandleRuntimeDataUpdatedEvent;

        if (DataController.Instance != null)
        {
            FixHudPlayerData(DataController.Instance.PlayerRuntimeData.Value);
        }

        ClearStatusEffectIcons();
        //TestAddStatusEffectIcons();

    }
    private void OnDisable()
    {
        runtimeDataUpdatedEvent.OnEventTriggered -= HandleRuntimeDataUpdatedEvent;
    }

    private void HandleRuntimeDataUpdatedEvent(IRuntimeData data)
    {
        switch (data)
        {
            case PlayerData:
                FixHudPlayerData(data as PlayerData);
                return;

            default: return;
        }
    }


    private void FixHudPlayerData(PlayerData playerData)
    {
        int max = Constants.MAX_PLAYER_HEALTH;
        int health = playerData.Health;
        healthSlider.maxValue = max;
        healthSlider.value = health;

        int xp = playerData.XP;
        xpText.text = $"XP: {xp.ToString()}";
        
        List<EnumPlayerStatusEffect> inStatusEffectsList = playerData.GetActiveStatusEffects();
        if (!AreStatusEffectListsSame(inStatusEffectsList, statusEffectsList))
        {
            FixStatusEffectUI(inStatusEffectsList);
        }
    }

    private void FixStatusEffectUI(List<EnumPlayerStatusEffect> seList)
    {
        ClearStatusEffectIcons();
        if (seList.Count <= 0) return;
        statusEffectsList = new List<EnumPlayerStatusEffect>(seList);
        foreach(EnumPlayerStatusEffect effect in statusEffectsList)
        {
            GameObject? iconPrefab = GetIconPrefabByEffect(effect);
            if (iconPrefab != null)
            {
                GameObject iconObj = Instantiate(iconPrefab, statusEffectsGrid.transform);
            }
        }
    }

    private GameObject? GetIconPrefabByEffect(EnumPlayerStatusEffect _effect)
    {
        GameObject iconPrefab = null;

        foreach(StructStatusEffectIcon iconStruct in effectIcons)
        {
            if(iconStruct.effect == _effect)
            {
                iconPrefab = iconStruct.iconPrefab;
                break;
            }
        }

        return iconPrefab;
    }

    private bool AreStatusEffectListsSame(List<EnumPlayerStatusEffect> listA, List<EnumPlayerStatusEffect> listB)
    {
        return listA.OrderBy(f => f).SequenceEqual(listB.OrderBy(f => f));
    }

    private void ClearStatusEffectIcons()
    {
        statusEffectsList.Clear();
        Transform xform = statusEffectsGrid.transform;
        if (xform.childCount <= 0) return;
        foreach(Transform child in xform)
        {
            Destroy(child.gameObject);
        }
    }

    private void TestAddStatusEffectIcons()
    {
        /*
        GameObject stealthIcon = Instantiate(stealthIconPrefab, statusEffectsGrid.transform);
        GameObject poisonIcon = Instantiate(poisonIconPrefab, statusEffectsGrid.transform);
        GameObject bleedingIcon = Instantiate(bleedingIconPrefab, statusEffectsGrid.transform);
        */
    }


    // Interface Methods //

    public EnumCanvasUIName GetCanvasName()
    {
        return EnumCanvasUIName.HUD;
    }
    public Canvas GetCanvas()
    {
        return GetComponent<Canvas>();
    }
    public void ForegroundCanvas(bool foregrounded)
    {
        if (foregrounded) { GetComponent<Canvas>().sortingOrder = UIController.FOREGROUND_SORT_ORDER; }
        else { GetComponent<Canvas>().sortingOrder = UIController.BACKGROUND_SORT_ORDER; }
    }
    public int GetSortingOrder()
    {
        return GetComponent<Canvas>().sortingOrder;
    }

    /*
    private enum EnumFruit
    {
        APPLE,
        PEAR,
        ORANGE
    }
    private List<EnumFruit> listA = new List<EnumFruit>();
    private List<EnumFruit> listB = new List<EnumFruit>();
    private void Test()
    {
        listA.Add(EnumFruit.APPLE);
        listA.Add(EnumFruit.PEAR);
        listA.Add(EnumFruit.ORANGE);

        listB.Add(EnumFruit.PEAR);
        listB.Add(EnumFruit.ORANGE);
        listB.Add(EnumFruit.APPLE);

        bool bothListsHaveTheSameItems = listA.OrderBy(f => f).SequenceEqual(listB.OrderBy(f => f));

        Debug.Log(bothListsHaveTheSameItems);

    }
    */



}
