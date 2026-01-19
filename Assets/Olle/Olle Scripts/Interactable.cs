using UnityEngine;

namespace Olle.Scripts
{
    public class Interactable : MonoBehaviour
    {
        public void Trigger(PlayerController player)
        {
            // Makes the RUN noise
            var noise = GetComponent<NoiseEmitter>();
            if (noise != null)
            {
                noise.EmitRun();
            }

            if (TryGetComponent<BreakableProgressionDataHandler>(out BreakableProgressionDataHandler handler))
            {
                handler.UpdateBackend();
            }

            Destroy(gameObject);
        }
    }
}