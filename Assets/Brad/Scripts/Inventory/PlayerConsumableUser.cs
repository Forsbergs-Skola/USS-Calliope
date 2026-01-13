using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerConsumableUser : MonoBehaviour
{

    private const float INPUT_DAMP = 0.1f;
    private bool inputDampened = false;
    private Olle.Scripts.PlayerController pController;

    private void Awake()
    {
        pController = GetComponent<Olle.Scripts.PlayerController>();
    }

    private void Update()
    {
        if (inputDampened) return;

        // F key is the Adrenaline input
        if (Keyboard.current.fKey.wasPressedThisFrame)
        {

            StartCoroutine(StartInputCooldown());
            HandleAdrenalineInput();
            return;
        }

        // H key is Health Pack Input
        if (Keyboard.current.hKey.wasPressedThisFrame)
        {
            StartCoroutine(StartInputCooldown());
            HandleHealthPackInput();
            return;
        }

        // and so on...

    }

    private void HandleAdrenalineInput()
    {
        pController.TryDepleteAdrenaline();
    }
    private void HandleHealthPackInput()
    {
        //
    }

    private System.Collections.IEnumerator StartInputCooldown()
    {
        yield return new WaitForSecondsRealtime(INPUT_DAMP);
        inputDampened = false;
    }

}
