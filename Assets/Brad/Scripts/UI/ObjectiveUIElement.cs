using UnityEngine;
using TMPro;

public class ObjectiveUIElement : MonoBehaviour
{
    [SerializeField] private TMP_Text titleText;
    [SerializeField] private TMP_Text desctiptionText;

    public void Configure(string titleStr, string descStr, bool finished = false)
    {
        titleText.text = titleStr;
        desctiptionText.text = descStr;
        if (finished)
        {
            titleText.fontStyle = FontStyles.Strikethrough;
            desctiptionText.fontStyle = FontStyles.Strikethrough;
        }
    }




}
