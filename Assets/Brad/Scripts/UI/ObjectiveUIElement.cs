using UnityEngine;
using TMPro;

public class ObjectiveUIElement : MonoBehaviour
{
    [SerializeField] private TMP_Text titleText;
    [SerializeField] private TMP_Text desctiptionText;
    [SerializeField] private Color startedTextColor = new Color(1f, 1f, 0f, 1f);
    [SerializeField] private Color finishedTextColor = new Color(.5f, .5f, .5f, 1f);


    public void Configure(string titleStr, string descStr, bool finished = false)
    {
        titleText.text = titleStr;
        desctiptionText.text = descStr;
        titleText.color = startedTextColor;
        desctiptionText.color = startedTextColor;
        if (finished)
        {
            titleText.fontStyle = FontStyles.Strikethrough;
            desctiptionText.fontStyle = FontStyles.Strikethrough;
            titleText.color = finishedTextColor;
            desctiptionText.color = finishedTextColor;
        }
    }
}
