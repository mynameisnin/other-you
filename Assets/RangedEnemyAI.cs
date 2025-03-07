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
    private bool lastAttackWasHighAngle; // 마지막 공격 유형 저장

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
        lastAttackWasHighAngle = isHighAngle; // ?? 공격 방식 저장

        if (isHighAngle)
        {
            animator.SetTrigger("AttackHigh"); // 고각 공격 애니메이션
        }
        else
        {
            animator.SetTrigger("AttackStraight"); // 직선 공격 애니메이션
        }

        yield return new WaitForSeconds(0.5f); // 애니메이션 대기 시간

        //  애니메이션 이벤트에서 ShootArrowEvent()가 실행됨
        yield return new WaitForSeconds(attackCooldown);
        canAttack = true;
    }

    public void ShootArrowEvent()
    {
        ShootArrow(lastAttackWasHighAngle); //  마지막 공격 방식 전달
    }


    void ShootArrow(bool isHighAngle)
    {
        if (arrowPrefab == null || firePoint == null || player == null)
        {
            Debug.LogError(" 화살 프리팹, FirePoint 또는 Player가 설정되지 않음!");
            return;
        }

        GameObject arrow = Instantiate(arrowPrefab, firePoint.position, Quaternion.identity);

        if (arrow == null)
        {
            Debug.LogError(" 화살이 생성되지 않았음! 프리팹을 확인하세요.");
            return;
        }

        Arrow arrowScript = arrow.GetComponent<Arrow>();

        if (arrowScript == null)
        {
            Debug.LogError(" Arrow 스크립트가 감지되지 않음! 프리팹이 올바른지 확인하세요.", arrow);
            return;
        }

        Rigidbody2D arrowRb = arrow.GetComponent<Rigidbody2D>();

        if (arrowRb == null)
        {
            Debug.LogError(" Rigidbody2D가 화살 오브젝트에서 감지되지 않음! 화살 프리팹을 확인하세요.", arrow);
            return;
        }

        // ?? 플레이어 위치를 목표로 전달
        arrowScript.SetDirection(isFacingRight, isHighAngle, player.position);
    }



    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectRange);
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, straightShotRange);
    }
}