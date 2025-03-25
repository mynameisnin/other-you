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
    private bool isStunned = false; //  스턴 상태 변수 추가

    private bool isPatrolling = true;
    private float patrolTime = 2f;
    private float patrolTimer = 0f;
    private float patrolDirection = 1f; // -1이면 왼쪽, 1이면 오른쪽
    public int attackDamage = 100; // 기본 공격 데미지

    private bool isBlocked = false; //  적이 앞에 막혀있는지 체크

    public Collider2D frontCollider; //  태그 감지용 트리거 콜라이더


    public List<string> ignoreEnemyNames = new List<string>(); // 무시할 적 이름 리스트
    void Start()
    {
        enemyAnimator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        patrolTimer = patrolTime;
        FindPlayer();
    }

    void Update()
    {
      
        if (isStunned || isBlocked) return; //  적이 막혀있으면 이동 X
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
        if (player == null || !player.gameObject.activeInHierarchy)
        {
            FindPlayer(); // 플레이어가 스위치되었거나 사라졌으면 다시 찾기
        }
    }
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

    }


    void DetectPlayer()
    {
        // 스턴 상태에서는 플레이어 감지 중단
        if (isStunned) return;

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
        if (player == null || isBlocked) return; //  앞에 적이 있으면 이동 X

        float distance = Vector2.Distance(transform.position, player.position);

        if (distance <= attackRange)
        {
            StartCoroutine(Attack());
        }
        else
        {
            enemyAnimator.SetBool("isWalking", true);
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
        // 스턴 상태에서는 공격 감지 중단
        if (isStunned) return false;

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
            patrolTimer = GetRandomPatrolTime(); // 무작위 타이머 설정
        }

        Vector2 patrolTarget = new Vector2(transform.position.x + patrolDirection * speed * Time.deltaTime, transform.position.y);

        if (ObstacleInFront() || !GroundAhead())
        {
            patrolDirection *= -1f;
            StartCoroutine(FlipAndTurn());
            patrolTimer = GetRandomPatrolTime(); // 무작위 타이머 재설정
        }

        transform.position = patrolTarget;
        enemyAnimator.SetBool("isWalking", true);
    }
    float GetRandomPatrolTime()
    {
        return Random.Range(1.5f, 6f); // 1.5초 ~ 3.5초 사이의 랜덤한 값 반환
    }
    public int GetDamage()
    {
        return attackDamage;
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
        RaycastHit2D hit = Physics2D.Raycast(transform.position, direction, 4f, obstacleLayer);

        if (hit.collider != null)
        {
          
            Gizmos.color = Color.red;
        }
        else
        {
      
            Gizmos.color = Color.green;
        }

        return hit.collider != null;
    }
    bool GroundAhead()
    {
        RaycastHit2D hit = Physics2D.Raycast(groundCheck.position, Vector2.down, 1f, groundLayer);
        if (hit.collider != null)
        {
            Debug.Log("지면 감지됨: " + hit.collider.gameObject.name);
        }
        else
        {
            Debug.Log("지면 없음");
        }
        return hit.collider != null;
    }
    public void CancelAttack()
    {
        isAttacking = false;
        enemyAnimator.ResetTrigger("Attack");
        enemyAnimator.SetTrigger("Parry"); // 패링 애니메이션 실행
        Debug.Log(" 적 공격이 패링됨! 패링 애니메이션 실행 및 스턴");

        rb.velocity = Vector2.zero;

        StartCoroutine(StunCoroutine(3f)); // 3초간 스턴
    }

    IEnumerator StunCoroutine(float stunDuration)
    {
        isStunned = true; //  스턴 상태 활성화
        isChasing = false;
        isAttacking = false;
        isPatrolling = false;

        enemyAnimator.SetBool("isWalking", false);
        rb.velocity = Vector2.zero;
        // X축 이동만 금지, Z축 회전은 유지
        rb.constraints = RigidbodyConstraints2D.FreezePositionX | RigidbodyConstraints2D.FreezeRotation;

        yield return new WaitForSeconds(stunDuration);

        isStunned = false; //  스턴 해제
        isChasing = true;
        isPatrolling = true;

        // Z축 회전은 계속 고정하고 X축 이동만 풀어줌
        rb.constraints = RigidbodyConstraints2D.FreezeRotation;

        Debug.Log(" 적 스턴 해제!");
    }

    void Flip()
    {
        isFacingRight = !isFacingRight;
        transform.Rotate(0f, 180f, 0f);
    }
    //  적이 감지되면 플레이어와 가까운 적만 이동할 수 있도록 설정
    private void OnTriggerStay2D(Collider2D other)
    {
        if (other.CompareTag("Enemy"))
        {
            // 특정 이름의 적이면 무시하고 이동 가능
            if (ignoreEnemyNames.Contains(other.gameObject.name))
            {
                Debug.Log($"무시된 적 발견: {other.gameObject.name} -> 이동 가능");
                isBlocked = false;
                return;
            }

            Debug.Log("앞에 적 감지됨!");

            float myDistance = Vector2.Distance(transform.position, player.position);
            float otherDistance = Vector2.Distance(other.transform.position, player.position);

            if (myDistance < otherDistance)
            {
                Debug.Log("내가 플레이어와 더 가까움 -> 이동 가능");
                isBlocked = false;
            }
            else
            {
                Debug.Log("내가 플레이어보다 멀음 -> 이동 멈춤");
                isBlocked = true;
                rb.velocity = Vector2.zero;
                enemyAnimator.SetBool("isWalking", false);
            }
        }
    }

    //  적이 사라지면 다시 이동 가능
    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Enemy"))
        {
            Debug.Log("앞에 있던 적이 사라짐! 이동 가능");
            isBlocked = false;
        }
    }


    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(attackBox.position, attackBoxSize); // 공격 범위 표시
        Gizmos.DrawRay(transform.position, Vector2.right * detectionRange);
        Gizmos.DrawRay(transform.position, Vector2.left * detectionRange);
        Gizmos.color = Color.blue;
        Gizmos.DrawRay(groundCheck.position, Vector2.down * 1f); // 지면 감지 Ray 확인
        Gizmos.color = Color.yellow;
        Gizmos.DrawRay(transform.position, isFacingRight ? Vector2.right * 1f : Vector2.left * 1f); // 장애물 감지 기즈모
    }
}
