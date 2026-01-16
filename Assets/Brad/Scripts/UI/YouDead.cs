using UnityEngine;
using UnityEngine.UI;
public class YouDead : MonoBehaviour
{
    [SerializeField] private Button mainButton;
    [SerializeField] private Button quitButton;

    private void OnEnable()
    {
        mainButton.onClick.AddListener(MainPressed);
        quitButton.onClick.AddListener(QuitPressed);
        Bootstrapper.Instance.PauseGame(true);
        Cursor.visible = true;
    }
    private void OnDisable()
    {
        mainButton.onClick.RemoveAllListeners();
        quitButton.onClick.RemoveAllListeners();
        Bootstrapper.Instance.PauseGame(false);
    }

    private void QuitPressed()
    {
        Application.Quit();
    }
    private void MainPressed()
    {
        if (Bootstrapper.Instance != null) { Bootstrapper.Instance.ReturnToMain(); }
    }


}
