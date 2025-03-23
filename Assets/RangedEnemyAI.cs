using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RangedEnemyAI : MonoBehaviour
{
    public Transform player; // 플레이어 위치 (씬 전환 시 다시 찾아야 함)
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
        FindPlayer(); // ?? 플레이어를 찾아서 할당
    }

    void Update()
    {
        if (player == null)
        {
            FindPlayer(); // ?? 씬이 바뀌어도 플레이어를 다시 찾음
        }

        DetectPlayer();
        FlipDirection();
    }

    // ?? 플레이어 찾기 메서드 추가
    void FindPlayer()
    {
        GameObject adam = GameObject.FindGameObjectWithTag("AdamCamPosition");
        GameObject deba = GameObject.FindGameObjectWithTag("DevaCamPosition");

        if (adam != null && adam.activeInHierarchy)
        {
            player = adam.transform;
        }
        else if (deba != null && deba.activeInHierarchy)
        {
            player = deba.transform;
        }
        else
        {
            Debug.LogWarning("플레이어를 찾을 수 없음! 'AdamCamPosition' 또는 'DevaCamPosition' 태그 확인 필요.");
        }
    }

    void DetectPlayer()
    {
        if (player == null) return; //  플레이어가 없으면 실행 안 함

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
        lastAttackWasHighAngle = isHighAngle; // 공격 방식 저장

        if (isHighAngle)
        {
            animator.SetTrigger("AttackHigh"); // 고각 공격 애니메이션
        }
        else
        {
            animator.SetTrigger("AttackStraight"); // 직선 공격 애니메이션
        }

        yield return new WaitForSeconds(0.5f); // 애니메이션 대기 시간

        yield return new WaitForSeconds(attackCooldown);
        canAttack = true;
    }

    public void ShootArrowEvent()
    {
        if (player == null) return; // ?? 플레이어가 없으면 화살 발사 안 함
        ShootArrow(lastAttackWasHighAngle);
    }

    void ShootArrow(bool isHighAngle)
    {
        if (arrowPrefab == null || firePoint == null || player == null)
        {
            
            return;
        }

        GameObject arrow = Instantiate(arrowPrefab, firePoint.position, Quaternion.identity);

        if (arrow == null)
        {
           
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
