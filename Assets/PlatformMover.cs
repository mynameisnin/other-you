using UnityEngine;

public class PlatformMover : MonoBehaviour
{
    public float moveDistance = 3f;      // 이동 거리
    public float moveSpeed = 2f;         // 이동 속도

    public Vector3 startPos;
    public Vector3 targetPos;
    public bool movingRight = true;

    void Start()
    {
        startPos = transform.position;
        targetPos = startPos + Vector3.right * moveDistance;
    }

    void Update()
    {
        if (movingRight)
        {
            transform.position = Vector3.MoveTowards(transform.position, targetPos, moveSpeed * Time.deltaTime);
            if (Vector3.Distance(transform.position, targetPos) < 0.01f)
                movingRight = false;
        }
        else
        {
            transform.position = Vector3.MoveTowards(transform.position, startPos, moveSpeed * Time.deltaTime);
            if (Vector3.Distance(transform.position, startPos) < 0.01f)
                movingRight = true;
        }
    }
    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            collision.transform.SetParent(transform);
        }
    }

    void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            collision.transform.SetParent(null);
        }
    }
}
