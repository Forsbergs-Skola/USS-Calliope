using UnityEngine;
using UnityEngine.UI;
using Events;
using System.Collections.Generic;
using System.Linq;

public class WalkThrough : MonoBehaviour
{

    [SerializeField] private Button flashlightButton;
    [SerializeField] private Button discoverCentralCorridorButton;
    [SerializeField] private Button crewQuartersKeyButton;
    [SerializeField] private Button investigateMedBayButton;
    [SerializeField] private Button pistolButton;
    [SerializeField] private Button pistolAmmoPickup01;

    [SerializeField] private Button killEnemy01Button;



    private Dictionary<string, EnumObjectiveStatus> statusDict
    {
        get => new Dictionary<string, EnumObjectiveStatus>(DataController.Instance.ProgressionRuntimeData.Value.ObjectivesAndStatusesDict);
    }

    private Dictionary<string, int> consumablesDict
    {
        get => new Dictionary<string, int>(DataController.Instance.InventoryRuntimeData.Value.GetConsumableIDsAndQuantities());
    }

    private ProgressionData progData { get => DataController.Instance.ProgressionRuntimeData.Value; }
    private InventoryData invData { get => DataController.Instance.InventoryRuntimeData.Value; }


    private void Awake()
    {
        if (DataController.Instance == null) Destroy(gameObject);
    }

    private void OnEnable()
    {
        EventRelay.Instance.GameEvents.DataUpdatedEvent.OnEventTriggered += FixButtons;

        flashlightButton.onClick.AddListener(GetFlashlight);
        discoverCentralCorridorButton.onClick.AddListener(EnterCentralCorridor);
        crewQuartersKeyButton.onClick.AddListener(GetCrewQuartersKey);
        investigateMedBayButton.onClick.AddListener(InvestigateMedBay);
        pistolButton.onClick.AddListener(GetPistol);
        pistolAmmoPickup01.onClick.AddListener(PistolAmmoPickup01);

        killEnemy01Button.onClick.AddListener(KillEnemy01);
        



        crewQuartersKeyButton.gameObject.SetActive(false); // <-- temp

        FixButtons();
    }
    private void OnDisable()
    {
        EventRelay.Instance.GameEvents.DataUpdatedEvent.OnEventTriggered -= FixButtons;

        flashlightButton.onClick.RemoveAllListeners();
        discoverCentralCorridorButton.onClick.RemoveAllListeners();
        crewQuartersKeyButton.onClick.RemoveAllListeners();
        investigateMedBayButton.onClick.RemoveAllListeners();
        pistolButton.onClick.RemoveAllListeners();
        pistolAmmoPickup01.onClick.RemoveAllListeners();
        killEnemy01Button.onClick.RemoveAllListeners();
    }






    private void GetFlashlight()
    {
       invData.AddQuestItem(IDConstants.FLASHLIGHT_INV_ID);
    }
    private void EnterCentralCorridor()
    {
        progData.CentralCorridorDiscovered = true;
    }
    private void GetCrewQuartersKey()
    {
        invData.AddQuestItem(IDConstants.CREW_QUARTERS_KEY);
    }
    private void GetPistol()
    {
        invData.AddWeaponItem(IDConstants.PISTOL);
    }
    private void InvestigateMedBay()
    {
        DialogueController.Instance.StartConvoWithID(IDConstants.CONVERSATION_BOB_00);
    }
    private void PistolAmmoPickup01()
    {
        if (!consumablesDict.Keys.ToList<string>().Contains(IDConstants.PISTOL_AMMO))
        {
            invData.AddNewConsumable(IDConstants.PISTOL_AMMO, 10);
        }
    }
    private void KillEnemy01()
    {

        bool canKill = (invData.GetWeaponItemIDs().Contains(IDConstants.PISTOL) && consumablesDict.Keys.ToList<string>().Contains(IDConstants.PISTOL_AMMO));
        if (canKill)
        {
            Debug.Log("BANG!!!"); return;
        }
        else
        {
            Debug.Log("CLIKKK"); return;
        }



    }


    private void FixButtons()
    {
        flashlightButton.gameObject.SetActive(statusDict[IDConstants.OBJECTIVE_01_ID] == EnumObjectiveStatus.STARTED);

        discoverCentralCorridorButton.gameObject.SetActive(statusDict[IDConstants.OBJECTIVE_02_ID] == EnumObjectiveStatus.STARTED);
        investigateMedBayButton.gameObject.SetActive(statusDict[IDConstants.OBJECTIVE_04_ID] == EnumObjectiveStatus.STARTED);
        pistolButton.gameObject.SetActive(progData.BobContacted == true && !invData.GetWeaponItemIDs().Contains(IDConstants.PISTOL));
        pistolAmmoPickup01.gameObject.SetActive(progData.BobContacted == true && !consumablesDict.Keys.ToList<string>().Contains(IDConstants.PISTOL_AMMO));
        killEnemy01Button.gameObject.SetActive(statusDict[IDConstants.OBJECTIVE_05_ID] == EnumObjectiveStatus.STARTED);
      

    }






}
