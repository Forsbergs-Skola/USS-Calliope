using UnityEngine;
using System.Collections.Generic;
using Unity.Cinemachine;

public class CameraFadeDetector : MonoBehaviour
{
    public Transform player;
    public LayerMask fadeLayer;
    private Camera mainCam;
    private List<FadeObject> fadedObjects = new List<FadeObject>();
    
    void Awake() => mainCam = Camera.main;
    
    void LateUpdate()
    {
        RestoreObjects(); //Restores all the previous frames faded objects

        Vector3 camPos = mainCam.transform.position;
        Vector3 target = player.position + Vector3.up * 1.2f;
        Vector3 dir = target - camPos;
        float dist = Vector3.Distance(camPos, player.position);

        RaycastHit[] hits = Physics.RaycastAll(camPos, dir, dist, fadeLayer); //Shoot a raycast from cam-player, detecting all objects blocking

        foreach (RaycastHit hit in hits)
        {
            Debug.Log("Hit: " + hit.collider.name);
            DrawHitPoint(hit);

            FadeObject fade = hit.collider.GetComponent<FadeObject>();
            if (fade != null)
            {
                fade.FadeOut();
                fadedObjects.Add(fade);
            }
        }
        
        Debug.DrawLine(camPos, player.position, Color.red);
    }

    void RestoreObjects()
    {
        foreach (FadeObject fade in fadedObjects)
            fade.FadeIn();
        
        fadedObjects.Clear();
    }
    
    void DrawHitPoint(RaycastHit hit)
    {
        Debug.DrawRay(hit.point, hit.normal * 0.3f, Color.green);
    }
    
}