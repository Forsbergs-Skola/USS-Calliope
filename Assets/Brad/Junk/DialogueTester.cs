using UnityEngine;
using UnityEngine.UI;

public class DialogueTester : MonoBehaviour
{
    [SerializeField] private Button testConvoButton;
    [SerializeField] private string testConvoID;

    private void OnEnable()
    {
        testConvoButton.onClick.AddListener(LaunchConvo);
    }
    private void OnDisable()
    {
        testConvoButton.onClick.RemoveAllListeners();
    }

    private void LaunchConvo()
    {
        // do this from anything that needs to trigger a dialogue...
        DialogueController.Instance.StartConvoWithID(testConvoID);
    }

}
