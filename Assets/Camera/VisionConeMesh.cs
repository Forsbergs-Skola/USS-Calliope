using UnityEngine;

public class PlayerVisionConeMesh : MonoBehaviour
{
    
    public PlayerVisionLogic visionLogicScript;
    public int raycastAmount = 30; //how many raycasts per frame, smaller = blockier and larger = smoother
    public LayerMask obstacleLayer;

    private Mesh visionConeMesh;

    void Awake()
    {
        visionConeMesh = new Mesh();
        visionConeMesh.name = "Vision Cone Mesh";
        GetComponent<MeshFilter>().mesh = visionConeMesh;
    }

    void LateUpdate() => DrawCone();

    void DrawCone()
    {
        int vertexCount = raycastAmount + 2;

        Vector3[] vertices = new Vector3[vertexCount];
        int[] triangles = new int[raycastAmount * 3];
        
        vertices[0] = Vector3.zero; //Cone centre

        float angleStep = visionLogicScript.viewAngle / raycastAmount; //The angle between each of the raycasts
        float startAngle = -visionLogicScript.viewAngle * 0.5f; //The start angle is the leftmost edge

        for (int i = 0; i <= raycastAmount; i++)
        {
            float angle = startAngle + angleStep * i;
            Vector3 localDirection = GetDirectionFromAngle(angle);

            float raycastDistance = visionLogicScript.viewDistance;
            
            //Check if the raycast is blocked by walls
            if (Physics.Raycast(transform.position, transform.TransformDirection(localDirection), out RaycastHit hit, visionLogicScript.viewDistance, obstacleLayer))
                raycastDistance = hit.distance;
            
            vertices[i + 1] = localDirection * raycastDistance;
        }

        int triangleIndex = 0;
        for (int i = 0; i < raycastAmount; i++) //This loop builds the triangles of the mesh
        {
            triangles[triangleIndex++] = 0; 
            triangles[triangleIndex++] = i + 1; 
            triangles[triangleIndex++] = i + 2;
        }

        visionConeMesh.Clear();
        visionConeMesh.vertices = vertices;
        visionConeMesh.triangles = triangles;
        visionConeMesh.RecalculateNormals();
    }

    Vector3 GetDirectionFromAngle(float angleInDegrees)
    {
        float radians = angleInDegrees * Mathf.Deg2Rad;
        return new Vector3(Mathf.Sin(radians), 0f, Mathf.Cos(radians));
    }
}
