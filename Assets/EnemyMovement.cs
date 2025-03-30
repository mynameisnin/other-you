using System.Collections; 
using System.Collections.Generic; 
using UnityEngine;

public class EnemyMovement : MonoBehaviour // 적의 이동 및 행동을 제어하는 스크립트
{
    private Animator enemyAnimator; // 애니메이터 컴포넌트 참조
    private Rigidbody2D rb; // Rigidbody2D 컴포넌트 참조

    public Transform player; // 플레이어 위치 참조
    public Transform groundCheck; // 지면 체크 포인트
    public LayerMask groundLayer; // 지면 레이어 설정
    public LayerMask playerLayer; // 플레이어 레이어 설정
    public LayerMask obstacleLayer; // 장애물 레이어 설정
    public Transform attackBox; // 공격 판정 박스 위치
    public float attackBoxSize = 1.5f; // 공격 범위 크기

    [Header("Detection Settings")] // 인스펙터 구분용 헤더
    public float detectionRange = 5f; // 플레이어 탐지 거리
    public float attackRange = 1.5f; // 공격 가능 거리
    public float speed = 3f; // 이동 속도
    public bool isFacingRight = true; // 캐릭터가 오른쪽을 보고 있는지 여부
    private bool isChasing = false; // 플레이어 추적 중인지 여부
    private bool isAttacking = false; // 공격 중인지 여부
    private bool isTurning = false; // 방향 전환 중인지 여부
    private bool isStunned = false; // 스턴 상태 여부

    private bool isPatrolling = true; // 순찰 중인지 여부
    private float patrolTime = 2f; // 순찰 시간 간격
    private float patrolTimer = 0f; // 순찰 타이머
    private float patrolDirection = 1f; // 순찰 방향 (1: 오른쪽, -1: 왼쪽)
    public int attackDamage = 100; // 공격 데미지 값

    private bool isBlocked = false; // 앞에 적이 있어 막혀 있는지 여부

    public Collider2D frontCollider; // 앞에 있는 콜라이더 (태그 감지용)

    public List<string> ignoreEnemyNames = new List<string>(); // 무시할 적 이름 리스트
    void Start()
    {
        enemyAnimator = GetComponent<Animator>(); // 애니메이터 초기화
        rb = GetComponent<Rigidbody2D>(); // 리지드바디 초기화
        patrolTimer = patrolTime; // 순찰 타이머 초기 설정
        FindPlayer(); // 플레이어 객체 찾기
    }

    void Update()
    {
        if (isStunned || isBlocked) return; // 스턴 상태나 막혀 있으면 실행 중단
        if (!isAttacking && !isTurning)
        {
            DetectPlayer(); // 플레이어 감지 시도
        }

        if (isChasing && !isAttacking && !isTurning)
        {
            ChasePlayer(); // 추적 행동
        }
        else if (isPatrolling && !isAttacking && !isTurning)
        {
            Patrol(); // 순찰 행동
        }
        if (player == null || !player.gameObject.activeInHierarchy)
        {
            FindPlayer(); // 플레이어가 바뀌었거나 비활성화된 경우 다시 찾기
        }
    }

    void FindPlayer()
    {
        GameObject adam = GameObject.FindGameObjectWithTag("AdamCamPosition"); // 아담 태그 찾기
        GameObject deba = GameObject.FindGameObjectWithTag("DevaCamPosition"); // 데바 태그 찾기

        if (adam != null && adam.activeInHierarchy)
        {
            player = adam.transform; // 아담을 타겟으로 설정
        }
        else if (deba != null && deba.activeInHierarchy)
        {
            player = deba.transform; // 데바를 타겟으로 설정
        }
    }

    void DetectPlayer()
    {
        if (isStunned) return; // 스턴 상태이면 감지 중단

        RaycastHit2D hitRight = Physics2D.Raycast(transform.position, Vector2.right, detectionRange, playerLayer); // 오른쪽 감지
        RaycastHit2D hitLeft = Physics2D.Raycast(transform.position, Vector2.left, detectionRange, playerLayer); // 왼쪽 감지

        if (hitRight.collider != null)
        {
            isChasing = true; // 추적 시작
            isPatrolling = false; // 순찰 중지
            if (!isFacingRight)
            {
                StartCoroutine(FlipAndTurn()); // 방향 전환
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
        if (player == null || isBlocked) return; // 타겟 없음 또는 이동 차단 시 중단

        float distance = Vector2.Distance(transform.position, player.position); // 플레이어와 거리 측정

        if (distance <= attackRange)
        {
            StartCoroutine(Attack()); // 공격 시작
        }
        else
        {
            enemyAnimator.SetBool("isWalking", true); // 걷기 애니메이션 실행
            Vector2 targetPosition = new Vector2(player.position.x, transform.position.y); // 수평 위치만 이동
            transform.position = Vector2.MoveTowards(transform.position, targetPosition, speed * Time.deltaTime); // 이동

            if ((player.position.x > transform.position.x && !isFacingRight) ||
                (player.position.x < transform.position.x && isFacingRight))
            {
                StartCoroutine(FlipAndTurn()); // 방향 전환 필요 시 실행
            }
        }
    }

    IEnumerator Attack()
    {
        isAttacking = true; // 공격 상태 설정
        enemyAnimator.SetBool("isWalking", false); // 걷기 중지
        enemyAnimator.SetTrigger("Attack"); // 공격 트리거 실행

        yield return new WaitForSeconds(0.1f); // 애니메이션 대기

        while (enemyAnimator.GetCurrentAnimatorStateInfo(0).IsName("Attack"))
        {
            yield return null; // 공격 애니메이션 끝날 때까지 대기
        }

        if (!CheckPlayerInAttackRange())
        {
            Debug.Log("플레이어가 범위를 벗어났으므로 이동 재개");
            isAttacking = false; // 공격 종료
        }
        else
        {
            Debug.Log("플레이어가 범위 내에 있음, 다시 공격 가능");
            StartCoroutine(Attack()); // 연속 공격
        }
    }

    bool CheckPlayerInAttackRange()
    {
        if (isStunned) return false; // 스턴 시 공격 불가

        Collider2D hit = Physics2D.OverlapCircle(attackBox.position, attackBoxSize, playerLayer); // 공격 범위 확인
        return hit != null; // 범위 내에 플레이어 있으면 true
    }

    void Patrol()
    {
        patrolTimer -= Time.deltaTime; // 순찰 타이머 감소

        if (patrolTimer <= 0)
        {
            patrolDirection *= -1f; // 방향 반전
            StartCoroutine(FlipAndTurn()); // 회전 애니메이션 실행
            patrolTimer = patrolTime;
            patrolTimer = GetRandomPatrolTime(); // 랜덤 시간 설정
        }

        Vector2 patrolTarget = new Vector2(transform.position.x + patrolDirection * speed * Time.deltaTime, transform.position.y); // 이동할 위치 계산

        if (ObstacleInFront() || !GroundAhead())
        {
            patrolDirection *= -1f; // 반전
            StartCoroutine(FlipAndTurn()); // 방향 전환
            patrolTimer = GetRandomPatrolTime(); // 타이머 재설정
        }

        transform.position = patrolTarget; // 이동
        enemyAnimator.SetBool("isWalking", true); // 걷기 애니메이션
    }

    float GetRandomPatrolTime()
    {
        return Random.Range(1.5f, 6f); // 랜덤 순찰 시간 반환
    }

    public int GetDamage()
    {
        return attackDamage; // 공격 데미지 반환
    }

    IEnumerator FlipAndTurn()
    {
        isTurning = true; // 방향 전환 중
        Flip(); // 실제 방향 전환
        enemyAnimator.SetTrigger("Turn"); // 회전 애니메이션

        yield return new WaitForSeconds(0.5f); // 애니메이션 대기

        isTurning = false; // 전환 완료
    }

    bool ObstacleInFront()
    {
        Vector2 direction = isFacingRight ? Vector2.right : Vector2.left; // 바라보는 방향 설정
        RaycastHit2D hit = Physics2D.Raycast(transform.position, direction, 4f, obstacleLayer); // 장애물 탐지

        if (hit.collider != null)
        {
            Gizmos.color = Color.red; // 감지됨
        }
        else
        {
            Gizmos.color = Color.green; // 없음
        }

        return hit.collider != null; // 장애물 여부 반환
    }

    bool GroundAhead()
    {
        RaycastHit2D hit = Physics2D.Raycast(groundCheck.position, Vector2.down, 1f, groundLayer); // 지면 감지
        if (hit.collider != null)
        {
            Debug.Log("지면 감지됨: " + hit.collider.gameObject.name);
        }
        else
        {
            Debug.Log("지면 없음");
        }
        return hit.collider != null; // 지면 존재 여부 반환
    }

    public void CancelAttack()
    {
        isAttacking = false; // 공격 취소
        enemyAnimator.ResetTrigger("Attack"); // 공격 트리거 리셋
        enemyAnimator.SetTrigger("Parry"); // 패링 애니메이션 실행
        Debug.Log(" 적 공격이 패링됨! 패링 애니메이션 실행 및 스턴");

        rb.velocity = Vector2.zero; // 이동 멈춤

        StartCoroutine(StunCoroutine(3f)); // 3초 스턴
    }

    IEnumerator StunCoroutine(float stunDuration)
    {
        isStunned = true; // 스턴 시작
        isChasing = false;
        isAttacking = false;
        isPatrolling = false;

        enemyAnimator.SetBool("isWalking", false); // 걷기 중지
        rb.velocity = Vector2.zero; // 이동 정지
        rb.constraints = RigidbodyConstraints2D.FreezePositionX | RigidbodyConstraints2D.FreezeRotation; // 이동 제한

        yield return new WaitForSeconds(stunDuration); // 스턴 대기

        isStunned = false; // 스턴 해제
        isChasing = true;
        isPatrolling = true;

        rb.constraints = RigidbodyConstraints2D.FreezeRotation; // 이동 허용, 회전 고정

        Debug.Log(" 적 스턴 해제!");
    }

    void Flip()
    {
        isFacingRight = !isFacingRight; // 방향 토글
        transform.Rotate(0f, 180f, 0f); // 시각적 회전
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (other.CompareTag("Enemy")) // 다른 적 감지 시
        {
            if (ignoreEnemyNames.Contains(other.gameObject.name)) // 무시 대상이면 통과
            {
                Debug.Log($"무시된 적 발견: {other.gameObject.name} -> 이동 가능");
                isBlocked = false;
                return;
            }

            Debug.Log("앞에 적 감지됨!");

            float myDistance = Vector2.Distance(transform.position, player.position); // 내 거리
            float otherDistance = Vector2.Distance(other.transform.position, player.position); // 상대 거리

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

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Enemy")) // 적이 사라졌을 때
        {
            Debug.Log("앞에 있던 적이 사라짐! 이동 가능");
            isBlocked = false;
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(attackBox.position, attackBoxSize); // 공격 범위 시각화
        Gizmos.DrawRay(transform.position, Vector2.right * detectionRange); // 오른쪽 탐지 시각화
        Gizmos.DrawRay(transform.position, Vector2.left * detectionRange); // 왼쪽 탐지 시각화
        Gizmos.color = Color.blue;
        Gizmos.DrawRay(groundCheck.position, Vector2.down * 1f); // 지면 감지 시각화
        Gizmos.color = Color.yellow;
        Gizmos.DrawRay(transform.position, isFacingRight ? Vector2.right * 1f : Vector2.left * 1f); // 장애물 감지 시각화
    }
}
