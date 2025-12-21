using UnityEngine;

public class SceneUIHandler : MonoBehaviour
{
    [SerializeField] private bool showHudOnLoad = true;

    private void Start()
    {
        if (showHudOnLoad)
        {
            UIController.Instance.ShowCanvas(EnumCanvasUIName.HUD);
        }
    }

}
