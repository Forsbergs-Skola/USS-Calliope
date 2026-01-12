using UnityEngine;
using TMPro;
using Tweens;
using System.Linq;
using System.Collections.Generic;

public class ObjectiveCompleteAlert : MonoBehaviour
{
    private List<string> completedQuestIDs = new();
    private TMP_Text alertText;
    private const float DURATION = 4.0f;
    [SerializeField] private Color textColor = new Color(1f, 1f, 0f, 1f);

    private void Awake()
    {
        if (DataController.Instance == null)
        {
            Destroy(gameObject);
            return;
        }
        completedQuestIDs = GetAllCompletedQuestsCurrent();
        alertText = GetComponent<TMP_Text>();
        alertText.text = string.Empty;
    }
    private void OnEnable()
    {
        if (EventRelay.Instance == null) return;
        EventRelay.Instance.GameEvents.RuntimeDataUpdatedEvent.OnEventTriggered += HandleRuntimeUpdate;
    }
    private void OnDisable()
    {
        if (EventRelay.Instance == null) return;
        EventRelay.Instance.GameEvents.RuntimeDataUpdatedEvent.OnEventTriggered -= HandleRuntimeUpdate;
    }

    private List<string> GetAllCompletedQuestsCurrent()
    {
        ProgressionData pData = DataController.Instance.ProgressionRuntimeData.Value;
        List<string> idList = new List<string>(pData.ObjectivesAndStatusesDict.Where(pair => pair.Value == EnumObjectiveStatus.FINISHED).Select(pair => pair.Key).ToList());
        return idList;
    }


    private void HandleRuntimeUpdate(IRuntimeData data)
    {
        if (!(data is ProgressionData)) return;
        ProgressionData progData = data as ProgressionData;

        List<string> idsToAlert = new();


        IReadOnlyList<string> newCompletedIDs = GetAllCompletedQuestsCurrent();
        foreach (string id in newCompletedIDs)
        {
            if (completedQuestIDs.Contains(id)) continue;
            completedQuestIDs.Add(id);
            idsToAlert.Add(id);
        }
        if (idsToAlert.Count > 0)
        {
            ObjectiveSO obj = ObjectivesTracker.Instance.GetObjectiveWithID(idsToAlert[0]);
            HandleUI(obj.ObjectiveTitle);
        }
    }

    private void HandleUI(string objTitle)
    {
        alertText.text = $"{objTitle}: Complete";
        alertText.color = textColor;
        float _r = textColor.r;
        float _b = textColor.b;
        float _g = textColor.g;
        Tween fadeTween = TweenService.GetFloatTween(gameObject, 1.0f, 0.0f, DURATION, EnumTweenEase.CUBIC, EnumTweenDirection.OUT);
        fadeTween.OnValueUpdated += (value) =>
        {
            Color col = new Color(_r, _g, _b, value.x);
            alertText.color = col;
        };
        fadeTween.OnFinished += () =>
        {
            alertText.text = string.Empty;
            alertText.color = textColor;
        };
        fadeTween.StartTween();
    }



}
