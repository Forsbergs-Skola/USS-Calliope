using UnityEngine;
using UnityEngine.UI;
using Tweens;
using Events;
using TMPro;

public class LogoSplashCanvas : MonoBehaviour, ICanvasUI
{
    [SerializeField] private TMP_Text mainText;
    [SerializeField] private float staySeconds = 2f;
    [SerializeField] private float fadeDuration = 2f;
    [SerializeField] private Image frontPanel;


    private void Start()
    {
        mainText.fontMaterial.SetFloat("_LightAngle", 0.0f);
        frontPanel.color = new Color(0f, 0f, 0f, 1f);
        FadeIn();
    }

    private void FadeIn()
    {
        Tween fadeTween = TweenService.GetFloatTween(gameObject, 1.0f, 0.0f, fadeDuration, EnumTweenEase.QUART, EnumTweenDirection.IN);
        fadeTween.StartTween();

        Tween lightingTween = TweenService.GetFloatTween(gameObject, 0.0f, 6.0f, (fadeDuration*0.95f));
        lightingTween.StartTween();

        lightingTween.OnValueUpdated += (value) =>
        {
            mainText.fontMaterial.SetFloat("_LightAngle", value.x);
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

    System.Collections.IEnumerator WaitThenContinue()
    {
        yield return new WaitForSecondsRealtime(staySeconds);
        EventRelay.Instance.UIEvents.LogoSplashFinishedEvent.TriggerEvent();
        //Debug.Log("FOO");
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
