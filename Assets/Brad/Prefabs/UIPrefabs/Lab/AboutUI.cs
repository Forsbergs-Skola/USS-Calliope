using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class AboutUI : MonoBehaviour
{
    [SerializeField] private Button nextButton;
    [SerializeField] private Button previousButton;
    [SerializeField] private Button mainButton;
    [SerializeField] private RawImage slideImage;

    [SerializeField] private List<string> slideNames;


    private int _slideIdx = -1;
    private int slideIdx
    {
        get => _slideIdx;
        set
        {
            _slideIdx = value % slideNames.Count;
            Resources.UnloadUnusedAssets();
            previousButton.gameObject.SetActive(_slideIdx != 0);
            nextButton.gameObject.SetActive(_slideIdx != slideNames.Count - 1);
            slideImage.texture = Resources.Load<Texture>($"{resPath}{slideNames[_slideIdx]}");
        }
    }

    private const string resPath = "AboutSlides/Readme/";

    private void OnEnable()
    {
        mainButton.onClick.AddListener(HandleMainPressed);
        nextButton.onClick.AddListener(HandleNextPressed);
        previousButton.onClick.AddListener(HandlePreviousPressed);

        slideIdx = 0;

    }
    private void OnDisable()
    {
        Resources.UnloadUnusedAssets();
        mainButton.onClick.RemoveAllListeners();
        nextButton.onClick.RemoveAllListeners();
        previousButton.onClick.RemoveAllListeners();
    }

    private void Start()
    {
        GetComponent<Canvas>().sortingOrder = 15;
    }


    private void HandleMainPressed()
    {
        Destroy(gameObject);
    }
    private void HandleNextPressed()
    {
        slideIdx++;
    }
    private void HandlePreviousPressed()
    {
        slideIdx--;
    }


}
