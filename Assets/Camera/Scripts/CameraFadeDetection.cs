using UnityEngine;
using System.Collections.Generic;
using Unity.Cinemachine;

public class CameraFadeDetector : MonoBehaviour
{
    public Transform player;
    public float raycastRadius;
    public LayerMask fadeLayer;
    private Camera mainCam;
    private List<FadeObject> fadedObjects = new List<FadeObject>();
    
    private HashSet<FadeObject> fadedSet = new HashSet<FadeObject>();
    
    
    void Awake() => mainCam = Camera.main;
    
    void LateUpdate()
    {
        RestoreObjects(); //Restores all the previous frames faded objects

        Vector3 camPosition = mainCam.transform.position;
        Vector3 target = player.position + Vector3.up * 1.2f;
        Vector3 direction = target - camPosition;
        //float distance = Vector3.Distance(camPosition, player.position);
        float distance = direction.magnitude;

        RaycastHit[] hits = Physics.SphereCastAll(camPosition, raycastRadius, direction.normalized, distance, fadeLayer, QueryTriggerInteraction.Ignore);
        
        //RaycastHit[] hits = Physics.RaycastAll(camPosition, direction.normalized, distance, fadeLayer);

        foreach (RaycastHit hit in hits)
        {
            //FadeObject fade = hit.collider.GetComponent<FadeObject>();
            //if (fade != null && fadedSet.Add(fade))
            //{
             //   fade.FadeOut();
             //   fadedObjects.Add(fade);
            //}
            
            var fadeGroup = hit.collider.GetComponentInParent<FadeObject>();
            if (fadeGroup != null && fadedSet.Add(fadeGroup))
            {
                fadeGroup.FadeOut();
                fadedObjects.Add(fadeGroup);
            }
            
        }
        
    }

    void RestoreObjects()
    {
        foreach (FadeObject fade in fadedObjects)
            fade.FadeIn();
        
        fadedObjects.Clear();
        fadedSet.Clear();
    }
    
    void DrawHitPoint(RaycastHit hit)
    {
        Debug.DrawRay(hit.point, hit.normal * 0.3f, Color.green);
    }
    
}