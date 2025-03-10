using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Arrow : MonoBehaviour
{
    public float straightSpeed = 10f; // 직선 속도
    public float arcSpeed = 5f; // 포물선 속도
    public float arcHeight = 2f; // 포물선 높이


    private Rigidbody2D rb;
    private bool isHighAngle;
    private Vector2 targetDirection;
    private bool isFacingRight;
    public int damage = 10;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();

        if (rb == null)
        {
            Debug.LogError(" Rigidbody2D가 없음! Rigidbody2D를 추가하세요.", this);
        }
    }
    


    void StraightShot()
    {
        if (rb == null)
        {
            Debug.LogError("Rigidbody2D가 설정되지 않음! Arrow 프리팹을 확인하세요.");
            return;
        }

        rb.velocity = targetDirection * straightSpeed;

        //  화살 방향에 맞게 회전
        float angle = Mathf.Atan2(rb.velocity.y, rb.velocity.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, angle);
    }
    public void SetDirection(bool facingRight, bool highAngle, Vector2 targetPosition)
    {
        isFacingRight = facingRight;
        isHighAngle = highAngle;

        targetDirection = facingRight ? Vector2.right : Vector2.left;

        if (isHighAngle)
        {
            StartCoroutine(ArcShot(targetPosition)); // ?? 목표 위치 전달
        }
        else
        {
            StraightShot();
        }
    }

    IEnumerator ArcShot(Vector2 targetPosition)
    {
        if (rb == null)
        {
            Debug.LogError("Rigidbody2D가 설정되지 않음! Arrow 프리팹을 확인하세요.");
            yield break;
        }

        Vector2 startPos = transform.position;
        Vector2 direction = targetPosition - startPos; // 목표 위치까지의 벡터
        float distanceX = Mathf.Abs(direction.x); // 수평 거리
        float distanceY = direction.y; // 수직 거리

        float gravity = Mathf.Abs(Physics2D.gravity.y); // 중력 값

        // ?? 목표까지 도달하는 시간을 `arcSpeed`로 조정 (arcSpeed가 클수록 빠름)
        float timeToTarget = distanceX / arcSpeed; // 속도를 기반으로 비행 시간 조절

        // ?? 초기 수직 속도 계산 (arcSpeed 반영)
        float initialVelocityY = (distanceY + (0.5f * gravity * timeToTarget * timeToTarget)) / timeToTarget;

        // ?? 화살 속도 조절 가능
        rb.velocity = new Vector2(targetDirection.x * arcSpeed, initialVelocityY);

        while (true)
        {
            // ?? 현재 속도 방향으로 회전 적용
            float angle = Mathf.Atan2(rb.velocity.y, rb.velocity.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0, 0, angle);

            yield return null;
        }
    }





    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player")) // 플레이어 충돌
        {
            // 플레이어에게 대미지 주기
            HurtPlayer player = collision.GetComponent<HurtPlayer>();
            if (player != null)
            {
                int damage = 10; // 화살 기본 대미지 (원하는 값으로 변경 가능)
                player.TakeDamage(damage);
            }

            // 화살 제거
            Destroy(gameObject);
        }
        else if (collision.CompareTag("isGround")) // 땅과 충돌하면 제거
        {
            Destroy(gameObject);
        }
    }

}
