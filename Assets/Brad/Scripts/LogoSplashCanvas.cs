using UnityEngine;
using UnityEngine.UI;
using Tweens;
using Events;
using TMPro;

public class LogoSplashCanvas : MonoBehaviour, ICanvasUI
{
    [SerializeField] private TMP_Text mainText;
    [SerializeField] private TMP_Text subText;
    [SerializeField] private float staySeconds = 4f;
    [SerializeField] private float fadeDuration = 3f;
    [SerializeField] private Image frontPanel;
    [SerializeField] private RawImage pegi18;
    [SerializeField] private GameObject logo;

    private UISoundPlayer soundPlayer { get => UIController.Instance.UISoundPlayer; }

    private int phase = 0;


    private void Awake()
    {
        pegi18.gameObject.SetActive(false);
    }

    private void Start()
    {
        mainText.fontMaterial.SetFloat(TMProProperties.LIGHT_ANGLE, 0.0f);
        subText.fontMaterial.SetFloat(TMProProperties.LIGHT_ANGLE, 0.0f);
        frontPanel.color = new Color(0f, 0f, 0f, 1f);
        FadeIn();
    }

    private void FadeIn()
    {

        StartCoroutine(WaitThenPlaySound());

        Tween fadeTween = TweenService.GetFloatTween(gameObject, 1.0f, 0.0f, fadeDuration, EnumTweenEase.QUART, EnumTweenDirection.IN);
        fadeTween.StartTween();

        Tween lightingTween = TweenService.GetFloatTween(gameObject, 0.0f, 6.0f, (fadeDuration*0.99f));
        lightingTween.StartTween();

        lightingTween.OnValueUpdated += (value) =>
        {
            mainText.fontMaterial.SetFloat(TMProProperties.LIGHT_ANGLE, value.x);
            subText.fontMaterial.SetFloat(TMProProperties.LIGHT_ANGLE, value.x);
        };
        
        
        fadeTween.OnValueUpdated += (value) =>
        {
            Color newColor = new Color(0f, 0f, 0f, value.x);
            frontPanel.color = newColor;
        };
        fadeTween.OnFinished += () =>
        {
            StartCoroutine(WaitThenContinue());
        };





    }


    private void ShowPegi18()
    {
        frontPanel.color = new Color(0f, 0f, 0f, 1f);
        logo.SetActive(false);
        pegi18.gameObject.SetActive(true);
        Tween fadeTween = TweenService.GetFloatTween(gameObject, 1.0f, 0.0f, fadeDuration, EnumTweenEase.QUART, EnumTweenDirection.IN);
        fadeTween.OnValueUpdated += (value) =>
        {
            Color newColor = new Color(0f, 0f, 0f, value.x);
            frontPanel.color = newColor;
        };
        fadeTween.OnFinished += () =>
        {
            StartCoroutine(WaitThenShowMain());
        };
        fadeTween.StartTween();
    }

    private System.Collections.IEnumerator WaitThenShowMain()
    {
        yield return new WaitForSeconds(staySeconds);
        EventRelay.Instance.UIEvents.LogoSplashFinishedEvent.TriggerEvent();
    }


    System.Collections.IEnumerator WaitThenContinue()
    {
        yield return new WaitForSecondsRealtime(staySeconds);
        ShowPegi18();

        //EventRelay.Instance.UIEvents.LogoSplashFinishedEvent.TriggerEvent();
    }

    


    System.Collections.IEnumerator WaitThenPlaySound()
    {
        yield return new WaitForSecondsRealtime(0.5f);
        soundPlayer.PlayUISound(EnumUISound.LOGO_SOUND);
    }

    // Interface Methods //
    public EnumCanvasUIName GetCanvasName()
    {
        return EnumCanvasUIName.LOGO_SPLASH;
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
}
