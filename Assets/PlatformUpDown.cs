using UnityEngine;

public class PlatformUpDown : MonoBehaviour
{
    public float moveDistance = 3f;      // 이동 거리
    public float moveSpeed = 2f;         // 이동 속도

    public Vector3 startPos;
    public Vector3 targetPos;
    public bool movingUp = true;

    void Start()
    {
        startPos = transform.position;
        targetPos = startPos + Vector3.up * moveDistance;
    }

    void Update()
    {
        if (movingUp)
        {
            transform.position = Vector3.MoveTowards(transform.position, targetPos, moveSpeed * Time.deltaTime);
            if (Vector3.Distance(transform.position, targetPos) < 0.01f)
                movingUp = false;
        }
        else
        {
            transform.position = Vector3.MoveTowards(transform.position, startPos, moveSpeed * Time.deltaTime);
            if (Vector3.Distance(transform.position, startPos) < 0.01f)
                movingUp = true;
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
