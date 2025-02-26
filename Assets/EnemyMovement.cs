using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMovement : MonoBehaviour
{
    private Animator enemyAnimator;
    private Rigidbody2D rb;

    public Transform player;
    public Transform groundCheck;
    public LayerMask groundLayer;
    public LayerMask playerLayer;
    public LayerMask obstacleLayer;
    public Transform attackBox; // 공격 박스 (Trigger Collider)
    public float attackBoxSize = 1.5f;

    [Header("Detection Settings")]
    public float detectionRange = 5f;
    public float attackRange = 1.5f;
    public float speed = 3f;
    public bool isFacingRight = true;
    private bool isChasing = false;
    private bool isAttacking = false;
    private bool isTurning = false;

    private bool isPatrolling = true;
    private float patrolTime = 2f;
    private float patrolTimer = 0f;
    private float patrolDirection = 1f; // -1이면 왼쪽, 1이면 오른쪽

    void Start()
    {
        enemyAnimator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        patrolTimer = patrolTime;
    }

    void Update()
    {
        if (!isAttacking && !isTurning)
        {
            DetectPlayer();
        }

        if (isChasing && !isAttacking && !isTurning)
        {
            ChasePlayer();
        }
        else if (isPatrolling && !isAttacking && !isTurning)
        {
            Patrol();
        }
    }

    void DetectPlayer()
    {
        RaycastHit2D hitRight = Physics2D.Raycast(transform.position, Vector2.right, detectionRange, playerLayer);
        RaycastHit2D hitLeft = Physics2D.Raycast(transform.position, Vector2.left, detectionRange, playerLayer);

        if (hitRight.collider != null)
        {
            isChasing = true;
            isPatrolling = false;
            if (!isFacingRight)
            {
                StartCoroutine(FlipAndTurn());
            }
        }
        else if (hitLeft.collider != null)
        {
            isChasing = true;
            isPatrolling = false;
            if (isFacingRight)
            {
                StartCoroutine(FlipAndTurn());
            }
        }
    }

    void ChasePlayer()
    {
        if (player == null) return;

        float distance = Vector2.Distance(transform.position, player.position);

        if (distance <= attackRange)
        {
            StartCoroutine(Attack());
        }
        else
        {
            enemyAnimator.SetBool("isWalking", true); // 걷기 애니메이션 실행

            Vector2 targetPosition = new Vector2(player.position.x, transform.position.y);
            transform.position = Vector2.MoveTowards(transform.position, targetPosition, speed * Time.deltaTime);

            if ((player.position.x > transform.position.x && !isFacingRight) ||
                (player.position.x < transform.position.x && isFacingRight))
            {
                StartCoroutine(FlipAndTurn());
            }
        }
    }

    IEnumerator Attack()
    {
        isAttacking = true;
        enemyAnimator.SetBool("isWalking", false);
        enemyAnimator.SetTrigger("Attack");

        yield return new WaitForSeconds(0.1f); // 애니메이션 시작 대기

        // 공격 애니메이션이 끝날 때까지 대기
        while (enemyAnimator.GetCurrentAnimatorStateInfo(0).IsName("Attack"))
        {
            yield return null; // 한 프레임 대기
        }

        // 공격 애니메이션이 끝난 후, 플레이어가 아직 공격 범위 내에 있는지 확인
        if (!CheckPlayerInAttackRange())
        {
            Debug.Log("플레이어가 범위를 벗어났으므로 이동 재개");
            isAttacking = false;
        }
        else
        {
            Debug.Log("플레이어가 범위 내에 있음, 다시 공격 가능");
            StartCoroutine(Attack()); // 다음 공격 시작
        }
    }

    // 플레이어가 현재 공격 범위 내에 있는지 확인하는 함수
    bool CheckPlayerInAttackRange()
    {
        Collider2D hit = Physics2D.OverlapCircle(attackBox.position, attackBoxSize, playerLayer);
        return hit != null;
    }

    void Patrol()
    {
        patrolTimer -= Time.deltaTime;

        if (patrolTimer <= 0)
        {
            patrolDirection *= -1f;
            StartCoroutine(FlipAndTurn());
            patrolTimer = patrolTime;
        }

        Vector2 patrolTarget = new Vector2(transform.position.x + patrolDirection * speed * Time.deltaTime, transform.position.y);

        if (ObstacleInFront() || !GroundAhead())
        {
            patrolDirection *= -1f;
            StartCoroutine(FlipAndTurn());
        }

        transform.position = patrolTarget;
        enemyAnimator.SetBool("isWalking", true);
    }

    IEnumerator FlipAndTurn()
    {
        isTurning = true;
        Flip(); // 방향 즉시 전환
        enemyAnimator.SetTrigger("Turn");

        yield return new WaitForSeconds(0.5f); // Turn 애니메이션 지속 시간

        isTurning = false;
    }

    bool ObstacleInFront()
    {
        Vector2 direction = isFacingRight ? Vector2.right : Vector2.left;
        RaycastHit2D hit = Physics2D.Raycast(transform.position, direction, 1f, obstacleLayer);
        return hit.collider != null;
    }

    bool GroundAhead()
    {
        return Physics2D.Raycast(groundCheck.position, Vector2.down, 1f, groundLayer);
    }

    void Flip()
    {
        isFacingRight = !isFacingRight;
        transform.Rotate(0f, 180f, 0f);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(attackBox.position, attackBoxSize); // 공격 범위 표시
        Gizmos.DrawRay(transform.position, Vector2.right * detectionRange);
        Gizmos.DrawRay(transform.position, Vector2.left * detectionRange);
    }
}
