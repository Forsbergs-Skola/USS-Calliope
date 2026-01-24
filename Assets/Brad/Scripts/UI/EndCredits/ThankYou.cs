using UnityEngine;
using TMPro;
using Events;
using Tweens;

public class ThankYou : MonoBehaviour
{

    private const float WAIT_TIME = 2.5f;

    [SerializeField] private TMP_Text thankYouText;
    [SerializeField] private TMP_Text gameNameText;
    [SerializeField] private EmptyPayloadEvent thankYouComplete;



    private void Start()
    {
        thankYouText.gameObject.SetActive(false);
        gameNameText.gameObject.SetActive(false);
    }

    public void Execute()
    {
        StartCoroutine(ThankYouRoutine());
    }

    private System.Collections.IEnumerator ThankYouRoutine()
    {
        thankYouText.gameObject.SetActive(true);
        RectTransform rXform = thankYouText.gameObject.GetComponent<RectTransform>();
        Vector3 startPos = new Vector3(rXform.position.x, rXform.position.y, rXform.position.z);
        float startY = rXform.position.y;
        yield return new WaitForSeconds(WAIT_TIME);
        Tween moveTween = TweenService.GetFloatTween(gameObject, 0f, 160f, WAIT_TIME * 0.5f);

        moveTween.OnValueUpdated += (value) =>
        {
            float newY = startY + value.x;
            Vector3 newPos = new Vector3(startPos.x, newY, startPos.z);
            rXform.position = newPos;
        };
        moveTween.OnFinished += () =>
        {
            gameNameText.gameObject.SetActive(true);
            StartCoroutine(ContinueRoutine());
        };
        moveTween.StartTween();
    }

    private System.Collections.IEnumerator ContinueRoutine()
    {
        yield return new WaitForSeconds(WAIT_TIME);
        thankYouComplete.TriggerEvent();
    }





}
