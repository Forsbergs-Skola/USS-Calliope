//////////
// VIEW //
//////////

//////////////////////////////////////////
// This is just a throwaway example     //
// The actual "View" class would be the //
// QTE object itself, as it appears to  //
// the player on screen.                //
//                                      //
// Stick this and the MashQTEPresenter  //
// component on the QTE prefab          //
//////////////////////////////////////////

using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;

public class MashQTEExample : MonoBehaviour
{

    [SerializeField] private TMP_Text mashText;

    private MashQtePresenter mashPresenter;
    private void Awake()
    {
        mashPresenter = GetComponent<MashQtePresenter>();
    }

    private void Start()
    {
        mashPresenter.StartQTE();
    }

    private void OnEnable()
    {
        mashPresenter.OnCurrentValueUpdated += HandleQteUpdated;
        mashPresenter.OnFullyDrained += HandleQteFalied;
    }
    private void OnDisable()
    {
        mashPresenter.OnCurrentValueUpdated -= HandleQteUpdated;
        mashPresenter.OnFullyDrained -= HandleQteFalied;
    }


    private void Update()
    {
        // Throwaway "Controller" logic
        if (Keyboard.current.spaceKey.wasPressedThisFrame)
        {
            mashPresenter.IngestMash();
        }
    }

    private void HandleQteUpdated(float value)
    {
        mashText.text = value.ToString();
    }

    private void HandleQteFalied()
    {
        mashPresenter.StopQTE();
        mashText.text = "0";
        Debug.Log("FART SOUND");
    }
}
