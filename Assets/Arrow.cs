using System.Collections;
using UnityEngine;

public class Arrow : MonoBehaviour
{
    public float lifetime = 3f;
    private Rigidbody2D rb;
    private bool hasHit = false;

    public float arrowSpeed = 10f; // 화살 속도 (직선 발사용)
    public float arcForceX = 6f;   // 포물선 발사 - X축 힘
    public float arcForceY = 8f;   // 포물선 발사 - Y축 힘

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        if (rb == null)
        {
            Debug.LogError("화살에 Rigidbody2D가 없음!");
            return;
        }

        Destroy(gameObject, lifetime); // 일정 시간 후 제거
    }

    // 화살 발사 방향을 설정하는 함수
    public void SetDirection(bool isRight, bool isHighAngle)
    {
        if (rb == null) return;

        float direction = isRight ? 1f : -1f;

        if (isHighAngle)
        {
            // 포물선으로 발사 (즉시 속도 적용)
            rb.velocity = new Vector2(direction * arcForceX, arcForceY);
        }
        else
        {
            // 직선으로 발사
            rb.velocity = new Vector2(direction * arrowSpeed, 0f);
        }
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (hasHit) return; // 이미 충돌했으면 더 이상 처리 안 함

        if (collision.CompareTag("Player"))
        {
            Debug.Log("플레이어 맞음!");
            hasHit = true;
            Destroy(gameObject); // 플레이어 맞으면 삭제
        }
        else if (collision.CompareTag("Wall") || collision.CompareTag("Ground"))
        {
            Debug.Log("벽이나 바닥에 충돌!");
            Destroy(gameObject); // 벽과 바닥에 맞으면 삭제
        }
    }
}
