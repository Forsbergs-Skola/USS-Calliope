using UnityEngine;

public class PlatformMoving : MonoBehaviour
{
    public bool canMove;
    [SerializeField] float speed;
    [SerializeField] int startPoint;
    [SerializeField] Transform[] movePoints;
    public GameObject walls;

    [SerializeField] private GameObject centralCorridor;

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
            if (i == movePoints.Length - 1)
            {
                reverse = true;
                i--;
                return;
            }
            else if (i == 0)
            {
                reverse = false;
                i++;
                return;
            }
            if (reverse)
            {
                i--;
            }
            else
            {
                i++;
            }
        }
        if (canMove)
        {
            transform.position = Vector3.MoveTowards(transform.position, movePoints[i].position, speed * Time.deltaTime);
            centralCorridor.SetActive(false);
            walls.SetActive(true);
        }
        // if(canMove == true)
        // {
        //     walls.SetActive(true);
        // }
        else
        {
            walls.SetActive(false);
        }
    }
}
