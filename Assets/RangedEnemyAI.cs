using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RangedEnemyAI : MonoBehaviour
{
    public Transform player; // 플레이어 위치
    public Transform firePoint; // 화살 발사 위치
    public GameObject arrowPrefab; // 화살 프리팹

    public float detectRange = 7f; // 감지 범위
    public float straightShotRange = 3f; // 직선 발사 범위
    public float attackCooldown = 2f; // 공격 쿨타임

    private Animator animator;
    private bool canAttack = true;
    private bool isFacingRight = true;

    void Start()
    {
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        DetectPlayer();
        FlipDirection();
    }

    void DetectPlayer()
    {
        Collider2D playerInRange = Physics2D.OverlapCircle(transform.position, detectRange, LayerMask.GetMask("Player"));

        if (playerInRange != null && canAttack)
        {
            float distanceToPlayer = Vector2.Distance(transform.position, player.position);

            if (distanceToPlayer <= straightShotRange)
            {
                StartCoroutine(AttackRoutine(false)); // 직선 공격
            }
            else
            {
                StartCoroutine(AttackRoutine(true)); // 포물선 공격
            }
        }
    }

    void FlipDirection()
    {
        if (player == null) return;

        bool playerOnRight = player.position.x > transform.position.x;
        if (playerOnRight && !isFacingRight)
        {
            Flip();
        }
        else if (!playerOnRight && isFacingRight)
        {
            Flip();
        }
    }

    void Flip()
    {
        isFacingRight = !isFacingRight;
        transform.Rotate(0f, 180f, 0f);

        animator.SetTrigger("Flip");
    }

    IEnumerator AttackRoutine(bool isHighAngle)
    {
        canAttack = false;
        if (isHighAngle)
        {
            animator.SetTrigger("AttackHigh"); // 고각 공격 애니메이션
        }
        else
        {
            animator.SetTrigger("AttackStraight"); // 직선 공격 애니메이션
        }

        yield return new WaitForSeconds(0.5f);

        ShootArrow(isHighAngle);
        yield return new WaitForSeconds(attackCooldown);
        canAttack = true;
    }

    void ShootArrow(bool isHighAngle)
    {
        if (arrowPrefab == null || firePoint == null)
        {
            Debug.LogError(" 화살 프리팹 또는 FirePoint가 설정되지 않음!");
            return;
        }

        GameObject arrow = Instantiate(arrowPrefab, firePoint.position, Quaternion.identity); //  여러 개의 화살 생성 가능

        Arrow arrowScript = arrow.GetComponent<Arrow>();

        if (arrowScript != null)
        {
            arrowScript.SetDirection(isFacingRight, isHighAngle);
        }
        else
        {
            Debug.LogError(" Arrow 스크립트가 할당되지 않음!");
        }
    }
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectRange);
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, straightShotRange);
    }
}