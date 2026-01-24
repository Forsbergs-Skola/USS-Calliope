using UnityEngine;
using Events;
using Tweens;
using UnityEngine.UI;

public class EndCredits : MonoBehaviour
{
    [SerializeField] private EmptyPayloadEvent thankYouComplete;
    [SerializeField] private RectTransform creditsXform;
    [SerializeField] private RawImage fp;


    private void OnEnable()
    {
        thankYouComplete.OnEventTriggered += HandleOnThankYouComplete;
        fp.color = new Color(1f, 1f, 1f, 0f);
    }
    private void OnDisable()
    {
        thankYouComplete.OnEventTriggered -= HandleOnThankYouComplete;
    }



    public void HandleOnThankYouComplete()
    {
        Debug.Log("BEGIN SCROLL");

        float startY = creditsXform.position.y;
        Vector3 startPos = new Vector3(creditsXform.position.x, creditsXform.position.y, creditsXform.position.z);

        Tween moveTween = TweenService.GetFloatTween(gameObject, 0f, 2000f, 10f);
        moveTween.OnValueUpdated += (value) =>
        {
            float newY = startY + value.x;
            Vector3 newPos = new Vector3(startPos.x, newY, startPos.z);
            creditsXform.position = newPos;
        };
        moveTween.OnFinished += () =>
        {
            FadeInFP();
        };
        moveTween.StartTween();
    }

    private void FadeInFP()
    {
        Debug.Log("FOOOO");


        Tween fadeTween = TweenService.GetFloatTween(gameObject, 0f, 1f, 3f, EnumTweenEase.CUBIC, EnumTweenDirection.IN);
        fadeTween.OnValueUpdated += (value) =>
        {
            fp.color = new Color(1f, 1f, 1f, value.x);
        };
        fadeTween.StartTween();
    }


}
