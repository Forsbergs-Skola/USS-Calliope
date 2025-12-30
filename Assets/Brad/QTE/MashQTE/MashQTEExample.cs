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
using UnityEngine.UI;

public class MashQTEExample : MonoBehaviour
{

    [SerializeField] private TMP_Text mashText;
    [SerializeField] private TMP_Text promtText;
    [SerializeField] private Slider qteSlider;
    [SerializeField] private Button startQTEButton;

    private MashQtePresenter mashPresenter;
    private float defaultPromptFontSize = 36f;



    private void Awake()
    {
        mashPresenter = GetComponent<MashQtePresenter>();
    }

    private void Start()
    {
        qteSlider.gameObject.SetActive(false);
        mashText.gameObject.SetActive(false);
        startQTEButton.gameObject.SetActive(true);
        promtText.gameObject.SetActive(false);
        defaultPromptFontSize = promtText.fontSize;
    }

    private void OnEnable()
    {
        mashPresenter.OnCurrentValueUpdated += HandleQteUpdated;
        mashPresenter.OnFullyDrained += HandleQteFalied;
        startQTEButton.onClick.AddListener(LaunchQTE);

        

    }
    private void OnDisable()
    {
        mashPresenter.OnCurrentValueUpdated -= HandleQteUpdated;
        mashPresenter.OnFullyDrained -= HandleQteFalied;
        startQTEButton.onClick.RemoveAllListeners();
    }


    private void Update()
    {
        // Throwaway "Controller" logic
        if (Keyboard.current.spaceKey.wasPressedThisFrame)
        {
            mashPresenter.IngestMash();
            promtText.color = Color.yellow;
            promtText.fontSize = defaultPromptFontSize * 1.1f;
        }
        if (Keyboard.current.spaceKey.wasReleasedThisFrame)
        {
            promtText.color = Color.white;
            promtText.fontSize = defaultPromptFontSize;
        }
    }

    private void LaunchQTE()
    {
        startQTEButton.gameObject.SetActive(false);
        qteSlider.gameObject.SetActive(true);        
        promtText.gameObject.SetActive(true);
        mashText.gameObject.SetActive(true);
        mashPresenter.StartQTE();
    }

    private void HandleQteUpdated(float value)
    {
        mashText.text = value.ToString();
        qteSlider.value = value;

        Color newTextColor = new Color(1f - value, value, 0f);
        mashText.color = newTextColor;
    }

    private void HandleQteFalied()
    {
        mashPresenter.StopQTE();
        mashText.text = "0";
        Debug.Log("FART SOUND");
    }
}
