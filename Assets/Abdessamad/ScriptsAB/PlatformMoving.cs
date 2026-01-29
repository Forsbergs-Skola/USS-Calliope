using UnityEngine;

public class PlatformMoving : MonoBehaviour
{
    public bool canMove;
    [SerializeField] float speed;
    [SerializeField] int startPoint;
    [SerializeField] Transform[] movePoints;
    [SerializeField] GameObject walls;

    [SerializeField] private GameObject centralCorridor;
    [SerializeField] GameObject liftBlock;
    [SerializeField] GameObject stairwayPlatformCollider;

    int i;
    bool reverse;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        transform.position = movePoints[startPoint].position;
        i = startPoint;
    }

    // Update is called once per frame
    void Update()
    {
        if (Vector3.Distance(transform.position, movePoints[i].position) < 0.01f)
        {
            canMove = false;
            
            if (i == 0)
            {
                i++;
                return;
            }
        }
        if (canMove)
        {
            transform.position = Vector3.MoveTowards(transform.position, movePoints[i].position, speed * Time.deltaTime);
            walls.SetActive(true);
            centralCorridor.SetActive(false);
            liftBlock.SetActive(true);
            stairwayPlatformCollider.SetActive(false);
        }
        else
        {
            walls.SetActive(false);
            centralCorridor.SetActive(true);
            stairwayPlatformCollider.SetActive(true);
        }
    }
}
