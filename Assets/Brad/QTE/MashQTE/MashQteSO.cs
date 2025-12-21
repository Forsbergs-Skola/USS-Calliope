///////////
// MODEL //
///////////

///////////////////////////////////////
// All the data. Everything there is //
// to know about the configuration   //
// and current state of the QTE      //
///////////////////////////////////////

using UnityEngine;
namespace QTE
{
    [CreateAssetMenu(fileName = "MashQteSO", menuName = "Quicktime Events/MashQteSO")]
    public class MashQteSO : ScriptableObject

    {
        [Range(0f, 1f)] [SerializeField] private float perMashRecovery = 0.2f;
        [SerializeField] private string qteName = string.Empty;
        [SerializeField] private float mashInputCooldown = 0.05f;
        [Range(1.0f, 30.0f)] [SerializeField] private float fullDepleteInterval = 10.0f; // seconds to go from 1 to zero
        private float currentValue = 1.0f;
        private bool mashDampened = false;
        private bool isRunning = false;

        public float PerMashRecovery { get => perMashRecovery; }
        public string QteName { get => qteName; }
        public float MashInputCooldown { get => mashInputCooldown; }
        public float FullDepleteInterval { get => fullDepleteInterval; }
        public float CurrentValue { get => currentValue; }
        public bool IsRunning { get => isRunning; }
        public bool MashDampened { get => mashDampened; }



        public void StartMasher()
        {
            currentValue = 1.0f;
            mashDampened = false;
            isRunning = true;
        }

        public void StopMasher()
        {
            isRunning = false;
        }

        public void Deplete(float DepleteAmount)
        {
            currentValue = Mathf.Max(0.0f, currentValue - DepleteAmount);
        }

        public void HandleMash()
        {
            currentValue = Mathf.Min(1.0f, currentValue + perMashRecovery);
        }
        public void DampenMashInput(bool _isDampened)
        {
            mashDampened = _isDampened;
        }

        private void OnValidate()
        {
            if (string.IsNullOrEmpty(qteName))
            {
                Debug.LogError("Give the QTE a unique name");
            }
        }
    }
}

