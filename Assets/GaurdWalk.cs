using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GaurdWalk : MonoBehaviour
{
    public float moveSpeed = 2f;  // 이동 속도
    public Transform player; // 플레이어 오브젝트 직접 할당
    public Transform[] ignoreObjects; // 추가적으로 충돌을 무시할 오브젝트들
    public Collider2D obstacleCollider; // 벽 충돌 감지용 콜라이더
    public float rayLength = 0.5f; // Raycast 길이

    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;
    private Vector2 moveDirection = Vector2.right; // 초기 이동 방향 (오른쪽)

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        // **플레이어와 충돌 무시**
        if (player != null)
        {
            IgnoreCollisionWith(player.GetComponent<Collider2D>());
        }

        // **추가적인 오브젝트들과 충돌 무시**
        foreach (Transform obj in ignoreObjects)
        {
            if (obj != null)
            {
                Collider2D objCollider = obj.GetComponent<Collider2D>(); // Collider2D 가져오기
                IgnoreCollisionWith(objCollider);
            }
        }
    }

    void FixedUpdate()
    {
        // **Ray를 더 위쪽에서 발사하도록 위치 조정**
        Vector2 rayOrigin = (Vector2)transform.position + new Vector2(0, 0.5f); // 중심보다 위쪽

        // 장애물 감지 (벽만 감지)
        RaycastHit2D hitLeft = Physics2D.Raycast(rayOrigin, Vector2.left, rayLength, 1 << obstacleCollider.gameObject.layer);
        RaycastHit2D hitRight = Physics2D.Raycast(rayOrigin, Vector2.right, rayLength, 1 << obstacleCollider.gameObject.layer);

        // **Ray 시각화 - 위쪽에서 더 잘 보이도록 설정**
        Debug.DrawRay(rayOrigin, Vector2.left * rayLength, hitLeft ? Color.green : Color.red, 0.1f);
        Debug.DrawRay(rayOrigin, Vector2.right * rayLength, hitRight ? Color.green : Color.blue, 0.1f);

        // 장애물 감지 시 방향 반전
        if ((moveDirection.x > 0 && hitRight) || (moveDirection.x < 0 && hitLeft))
        {
            FlipDirection();
        }

        // 이동 적용
        rb.velocity = new Vector2(moveDirection.x * moveSpeed, rb.velocity.y);
    }

    void FlipDirection()
    {
        moveDirection.x *= -1; // 이동 방향 반전
        spriteRenderer.flipX = !spriteRenderer.flipX; // 스프라이트 반전
    }

    void IgnoreCollisionWith(Collider2D objCollider)
    {
        if (objCollider != null)
        {
            Collider2D enemyCollider = GetComponent<Collider2D>();
            if (enemyCollider != null)
            {
                Physics2D.IgnoreCollision(enemyCollider, objCollider, true);
            }
        }
    }
}
