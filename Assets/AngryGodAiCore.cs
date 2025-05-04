using UnityEngine;
using System.Collections;
using System.Collections.Generic; // Gizmos 디버깅용

/// <summary>
/// 보스 AI 시스템 클래스.
/// 플레이어 추적, 거리 기반 공격 결정(백대쉬 예측 포함), 무작위 공격 패턴 실행,
/// 공격 중 추격 대쉬, 대쉬/백대쉬(상승 포함) 및 방향 전환 애니메이션 제어를 담당합니다.
/// 백대쉬 이동은 애니메이션 이벤트로 시작됩니다. Flip 시 공격 콜라이더 Offset도 반전됩니다.
/// </summary>
public class AngryGodAiCore : MonoBehaviour
{
    #region 변수 선언

    // --- 컴포넌트 참조 ---
    private Animator animator;
    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;

    // --- 플레이어 관련 ---
    [Header("플레이어 참조")]
    private Transform target; // 가장 가까운 타겟
    private SpriteRenderer targetSpriteRenderer; // 타겟의 스프라이트 렌더러 저장

    // --- AI 행동 설정 ---
    [Header("AI 행동 설정")]
    [Tooltip("플레이어를 탐지하기 시작하는 최대 거리")]
    public float detectRange = 10f;
    [Tooltip("이 거리 안에 플레이어가 들어오면 공격을 시작")]
    public float attackRange = 2.5f;
    [Tooltip("플레이어가 이 거리보다 가까워지고 특정 조건을 만족하면 백대쉬 실행")]
    public float backdashRange = 1.5f;
    [Tooltip("공격 중 플레이어가 이 거리보다 멀어지면 추격 대쉬 시작")]
    public float chaseDashTriggerRange = 3.0f;

    // --- 이동 설정 ---
    [Header("이동 설정")]
    [Tooltip("플레이어 추적 시 기본 이동 속도")]
    [SerializeField] private float moveSpeed = 2f; // 추격 대쉬 계산용

    // --- 대쉬 설정 ---
    [Header("대쉬 설정")]
    [Tooltip("전방/후방 대쉬 시 이동할 기본 거리")]
    public float dashDistance = 3f;
    [Tooltip("전방/후방 대쉬가 지속되는 시간")]
    public float dashDuration = 0.3f;
    [Tooltip("백대쉬 시 위쪽으로 이동하는 정도 (0: 수평, 1: 45도 위)")]
    [Range(0f, 1f)]
    [SerializeField] private float backdashUpwardFactor = 0.3f;
    [Tooltip("공격 중 추격 대쉬 시 이동 속도 배율 (moveSpeed 기준)")]
    public float chaseDashSpeedMultiplier = 1.5f;
    [Tooltip("공격 중 추격 대쉬 지속 시간 (초)")]
    public float chaseDashDuration = 0.2f;

    // --- 대쉬 이펙트 ---
    [Header("대쉬 이펙트")]
    [Tooltip("대쉬 시 사용할 Trail Renderer 컴포넌트")]
    [SerializeField] private TrailRenderer dashTrail;
    [Tooltip("잔상이 생성되는 간격 (초)")]
    [SerializeField] private float afterImageInterval = 0.05f;
    [Tooltip("잔상이 사라지기까지 걸리는 시간 (초)")]
    [SerializeField] private float afterImageLifetime = 0.5f;

    // --- 공격 설정 ---
    [Header("공격 설정")]
    [Tooltip("공격 실행 후 다음 행동까지의 대기 시간")]
    public float attackCooldown = 0.8f;
    [Tooltip("공격 애니메이션 시작 후 실제 데미지 판정이 발생하는 시점 (초)")]
    [SerializeField] private float attackHitTiming = 0.5f;
    [Tooltip("공격 판정에 사용할 BoxCollider2D가 있는 자식 오브젝트")]
    [SerializeField] private Transform attackBoxObject; // 공격 박스 오브젝트 참조
    private BoxCollider2D attackCollider; // 공격 박스 콜라이더 참조

    // --- 내부 상태 변수 ---
    private bool isActing = false;
    private bool facingRight = true;
    private bool isDashing = false;
    private bool isChaseDashing = false;
    private Coroutine stopAttackMovementCoroutine = null; // ★ 추가: 공격 전진 멈춤 코루틴 참조
    #endregion

    #region 유니티 생명주기 메서드

    void Start()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        if (animator == null || rb == null || spriteRenderer == null) { Debug.LogError("필수 컴포넌트 없음!", this); this.enabled = false; return; }
        if (dashTrail != null) dashTrail.emitting = false; else Debug.LogWarning("Dash Trail 없음", this);

        // 공격 콜라이더 초기화 및 확인
        if (attackBoxObject != null)
        {
            attackCollider = attackBoxObject.GetComponent<BoxCollider2D>();
            if (attackCollider == null) Debug.LogError("Attack Box Object에 BoxCollider2D가 없습니다!", attackBoxObject);
        }
        else Debug.LogError("Attack Box Object가 할당되지 않았습니다!", this);

        rb.constraints = RigidbodyConstraints2D.FreezeRotation;
        // facingRight = !spriteRenderer.flipX; // 초기 방향 설정
    }

    void Update()
    {
        if (isActing || isChaseDashing) return; // 현재 행동 중이면 중단

        FindClosestTarget(); // 타겟 찾기
        if (target == null) return; // 타겟 없으면 중단

        FlipTowardsTarget(); // 타겟 방향 보기 (Collider Offset 반전 포함)
        float distance = Vector2.Distance(transform.position, target.position); // 거리 계산

        // --- 행동 결정 로직 ---
        // 1. 백대쉬 조건 확인 및 애니메이션 시작
        if (distance < backdashRange && IsPlayerFacingBoss() && CanPredictPlayerAttack())
        {
            isActing = true;
            rb.velocity = Vector2.zero;
            FlipTowardsTarget(true); // 방향 고정
            animator.SetTrigger("Backdash"); // 애니메이션 시작 (이동은 이벤트로)
        }
        // 2. 공격 조건
        else if (distance < attackRange)
        {
            if (distance >= backdashRange) StartCoroutine(AttackRoutine()); // 적정 거리면 공격
            // else: 매우 가깝지만 백대쉬 조건 안 맞으면 대기
        }
        // 3. 접근 대쉬 조건
        else if (distance < detectRange)
        {
            StartCoroutine(DashRoutine(1)); // 전방 대쉬 (즉시 이동)
        }
        // 4. 범위 밖
        // else { /* Idle 상태 처리 */ }
    }

    #endregion

    #region 타겟 찾기 및 방향 전환
    // 가장 가까운 활성화된 플레이어 타겟과 그 SpriteRenderer 찾기
    void FindClosestTarget()
    {
        GameObject adam = GameObject.FindWithTag("Player");
        GameObject deva = GameObject.FindWithTag("DevaPlayer");
        Transform closestTarget = null; SpriteRenderer closestSprite = null; float minDist = Mathf.Infinity;
        if (adam != null && adam.activeInHierarchy) { float dist = Vector2.Distance(transform.position, adam.transform.position); if (dist < minDist) { minDist = dist; closestTarget = adam.transform; closestSprite = adam.GetComponent<SpriteRenderer>(); } }
        if (deva != null && deva.activeInHierarchy) { float dist = Vector2.Distance(transform.position, deva.transform.position); if (dist < minDist) { closestTarget = deva.transform; closestSprite = deva.GetComponent<SpriteRenderer>(); } }
        target = closestTarget; targetSpriteRenderer = closestSprite;
    }

    // 타겟 방향으로 스프라이트 및 공격 콜라이더 Offset 뒤집기
    void FlipTowardsTarget(bool forceFlip = false)
    {
        if (target == null) return;
        bool shouldFaceRight = (target.position.x > transform.position.x);

        if (facingRight != shouldFaceRight || forceFlip)
        {
            facingRight = shouldFaceRight;

            // 1. 스프라이트 반전
            if (spriteRenderer != null)
                spriteRenderer.flipX = !facingRight;

            // 2. BoxCollider2D Offset 반전
            if (attackCollider != null)
            {
                Vector2 offset = attackCollider.offset;
                offset.x = Mathf.Abs(offset.x) * (facingRight ? 1 : -1); // 절대값에 방향 곱하기
                attackCollider.offset = offset;
            }
        }
    }
    #endregion

    #region 행동 코루틴 (대쉬, 공격, 추격 대쉬)

    /// <summary>
    /// 일반 전방 대쉬 또는 백대쉬(상승 포함)를 실행하는 코루틴.
    /// </summary>
    IEnumerator DashRoutine(int direction) // 1 = 앞으로, -1 = 뒤로
    {
        isActing = true;
        isDashing = true;
        animator.SetTrigger(direction == 1 ? "Dash" : "Backdash");

        // 이펙트 시작
        if (dashTrail != null) dashTrail.emitting = true;
        StartCoroutine(LeaveAfterImage());

        // --- ★★★ 대쉬 방향 및 거리 계산 수정 ★★★ ---
        Vector2 dashDir;
        // ★★★ 수정: 전방/후방 모두 기본적으로 dashDistance 사용 ★★★
        float distanceToMove = dashDistance; // 기본 이동 거리를 먼저 설정

        if (direction == 1) // 전방 대쉬
        {
            if (target != null)
            {
                // 이동 방향은 타겟 방향
                dashDir = ((Vector2)target.position - rb.position).normalized;
                FlipTowardsTarget(true); // 타겟 방향 보기
                // ★ 삭제: 목표 지점 계산 및 거리 재계산 로직 제거
                // Vector2 targetStopPosition = (Vector2)target.position - dirToPlayer * attackRange;
                // distanceToMove = Vector2.Distance(rb.position, targetStopPosition);
                // float dot = Vector2.Dot(dirToPlayer, targetStopPosition - rb.position);
                // if (dot <= 0 || distanceToMove < 0.1f) distanceToMove = 0.1f;
            }
            else // 타겟 없으면 현재 방향으로 기본 거리만큼
            {
                dashDir = facingRight ? Vector2.right : Vector2.left;
                // distanceToMove = dashDistance; // 이미 위에서 설정됨
            }
        }
        else // 백대쉬 (direction == -1)
        {
            Vector2 backwardDir = facingRight ? Vector2.left : Vector2.right;
            Vector2 upwardDir = Vector2.up * backdashUpwardFactor;
            dashDir = (backwardDir + upwardDir).normalized;
            // distanceToMove = dashDistance; // 백대쉬는 항상 고정 거리 (위에서 설정됨)
        }
        // --- 계산 로직 끝 ---

        // --- 이동 실행 ---
        // 속도 계산 (설정된 거리와 시간 사용)
        float currentDashSpeed = (dashDuration > 0.01f) ? distanceToMove / dashDuration : 0;
        rb.velocity = dashDir * currentDashSpeed;

        yield return new WaitForSeconds(dashDuration);

        // --- 종료 처리 ---
        rb.velocity = Vector2.zero;
        isDashing = false;
        if (dashTrail != null) dashTrail.emitting = false;
        yield return new WaitForSeconds(0.3f); // 딜레이
        isActing = false;
        yield break;
    }


    /// <summary>
    /// 애니메이션 이벤트에서 호출될 함수. 실제 백대쉬 이동을 시작합니다.
    /// </summary>
    public void TriggerBackdashMovementFromAnim()
    {
        if (!isActing || isDashing || isChaseDashing) return; // 중복/상태 오류 방지
        StartCoroutine(ExecuteDashMovement(-1)); // 백대쉬 이동 시작
    }

    /// <summary>
    /// 실제 대쉬/백대쉬 이동 및 종료 처리를 담당하는 코루틴.
    /// </summary>
    private IEnumerator ExecuteDashMovement(int direction) // -1: 백대쉬
    {
        isDashing = true; // 이펙트 시작용 플래그

        if (dashTrail != null) dashTrail.emitting = true;
        StartCoroutine(LeaveAfterImage());

        // 방향 및 거리 계산
        Vector2 dashDir;
        float distanceToMove = dashDistance; // 백대쉬는 고정 거리
        if (direction == -1)
        {
            Vector2 backwardDir = facingRight ? Vector2.left : Vector2.right;
            Vector2 upwardDir = Vector2.up * backdashUpwardFactor;
            dashDir = (backwardDir + upwardDir).normalized;
        }
        else { yield break; } // 이 함수는 백대쉬 전용으로 가정


        float currentDashSpeed = (dashDuration > 0.01f) ? distanceToMove / dashDuration : 0;
        rb.velocity = dashDir * currentDashSpeed;
        yield return new WaitForSeconds(dashDuration);
        rb.velocity = Vector2.zero;
        isDashing = false; // 이펙트 종료용 플래그

        if (dashTrail != null) dashTrail.emitting = false;

        // TODO: 백대쉬 후 소환 로직 필요 시 여기에 추가 (BossSummoner 사용 시)
        // bool startedSummoning = false;
        // if (direction == -1 && summoner != null) { startedSummoning = summoner.TryStartSummon(); }
        // if (!startedSummoning) { ... }

        yield return new WaitForSeconds(0.3f); // 행동 후 딜레이
        isActing = false; // ★ 중요: 모든 행동 종료
        yield break;
    }

    /// <summary>
    /// 기본 공격 코루틴 (추격 대쉬 포함).
    /// </summary>
    IEnumerator AttackRoutine()
    {
        isActing = true;
        rb.velocity = Vector2.zero;
        FlipTowardsTarget(true);
        animator.SetTrigger("NomalAttack");

        float timeElapsed = 0f;
        while (timeElapsed < attackHitTiming && isActing)
        {
            if (target != null && !isChaseDashing) { float currentDistance = Vector2.Distance(transform.position, target.position); if (currentDistance > chaseDashTriggerRange) { StartCoroutine(ChaseDashDuringAttack()); } }
            yield return new WaitForSeconds(0.1f);
            timeElapsed += 0.1f;
            if (!IsPlayerValid()) { isActing = false; if (isChaseDashing) { /* 추격 중단 */ } yield break; }
        }

        float remainingTime = attackHitTiming - timeElapsed;
        if (remainingTime > 0) { yield return new WaitForSeconds(remainingTime); }
        while (isChaseDashing) { yield return null; }

        if (isActing) PerformAttackHit();

        yield return new WaitForSeconds(attackCooldown);
        isActing = false;
        yield break;
    }

    /// <summary>
    /// 공격 중 짧은 추격 대쉬 코루틴.
    /// </summary>
    private IEnumerator ChaseDashDuringAttack()
    {
        isChaseDashing = true;
        isDashing = true;
        if (dashTrail != null) dashTrail.emitting = true;
        StartCoroutine(LeaveAfterImage());
        Vector2 chaseDir = Vector2.zero;
        if (IsPlayerValid()) { chaseDir = ((Vector2)target.position - rb.position).normalized; FlipTowardsTarget(true); }
        else { isChaseDashing = false; isDashing = false; if (dashTrail != null) dashTrail.emitting = false; yield break; }
        float chaseSpeed = moveSpeed * chaseDashSpeedMultiplier;
        rb.velocity = chaseDir * chaseSpeed;
        yield return new WaitForSeconds(chaseDashDuration);
        if (isActing) rb.velocity = Vector2.zero; // 공격 중일 때만 멈춤
        isDashing = false;
        isChaseDashing = false;
        if (dashTrail != null) dashTrail.emitting = false;
        yield break;
    }

    // 실제 공격 판정 실행 함수
    void PerformAttackHit()
    {
        Debug.Log("보스 공격 판정!");
        // TODO: 실제 판정 로직 구현 필요.
        // 예시: 애니메이션 이벤트로 attackCollider.enabled = true/false 제어 후,
        //       attackCollider에 붙은 스크립트에서 OnTriggerEnter2D로 처리
    }

    #endregion

    #region 예측 및 보조 함수
    // 플레이어가 보스를 향하고 있는지 확인
    bool IsPlayerFacingBoss()
    {
        if (target == null || targetSpriteRenderer == null) return false;
        bool playerIsFacingRight = !targetSpriteRenderer.flipX;
        bool playerIsRightOfBoss = target.position.x > transform.position.x;
        return (!playerIsRightOfBoss && !playerIsFacingRight) || (playerIsRightOfBoss && playerIsFacingRight);
    }
    // 플레이어 공격 예측 (현재는 항상 true)
    bool CanPredictPlayerAttack() { return true; }
    // 플레이어 참조 유효성 확인
    bool IsPlayerValid() { return target != null && target.gameObject.activeInHierarchy && targetSpriteRenderer != null; }
    // 사용 안 함
    void ValidateAttackBoxes() { }
    #endregion

    #region 대쉬 이펙트 로직
    // 대쉬 중 잔상 생성 코루틴
    private IEnumerator LeaveAfterImage()
    {
        while (isDashing) { CreateAfterImage(); yield return new WaitForSeconds(afterImageInterval); }
        yield break;
    }
    // 잔상 게임 오브젝트 생성 및 설정
    void CreateAfterImage()
    {
        GameObject afterImage = new GameObject("AfterImage_Boss");
        SpriteRenderer sr = afterImage.AddComponent<SpriteRenderer>();
        sr.sprite = spriteRenderer.sprite;
        sr.color = new Color(1f, 0.2f, 0.2f, 0.7f);
        sr.flipX = spriteRenderer.flipX;
        sr.sortingLayerName = spriteRenderer.sortingLayerName;
        sr.sortingOrder = spriteRenderer.sortingOrder - 1;
        afterImage.transform.position = transform.position;
        afterImage.transform.localScale = transform.localScale;
        StartCoroutine(FadeOutAndDestroy(sr));
    }
    // 잔상 페이드 아웃 및 자동 파괴 코루틴
    private IEnumerator FadeOutAndDestroy(SpriteRenderer sr)
    {
        if (sr == null) yield break;
        float fadeDuration = afterImageLifetime; Color originalColor = sr.color; float elapsed = 0f;
        while (elapsed < fadeDuration) { if (sr == null) yield break; elapsed += Time.deltaTime; float alpha = Mathf.Lerp(originalColor.a, 0, elapsed / fadeDuration); sr.color = new Color(originalColor.r, originalColor.g, originalColor.b, alpha); yield return null; }
        if (sr != null && sr.gameObject != null) Destroy(sr.gameObject);
        yield break;
    }
    #endregion



    /// <summary>
    /// 공격 애니메이션 도중 이벤트로 호출됨. 짧은 전진 이동.
    /// </summary>
    public void TriggerAttackLunge()
    {
        if (!isActing || isDashing || isChaseDashing) return;

        Vector2 moveDir = facingRight ? Vector2.right : Vector2.left;
        float lungeDistance = 1.5f;
        float lungeDuration = 0.15f;
        float lungeSpeed = lungeDistance / lungeDuration;

        StartCoroutine(AttackLungeRoutine(moveDir, lungeSpeed, lungeDuration));
    }
    private IEnumerator AttackLungeRoutine(Vector2 moveDir, float speed, float duration)
    {
        isDashing = true;
        if (dashTrail != null)
            dashTrail.emitting = true;

        StartCoroutine(LeaveAfterImage());

        rb.velocity = moveDir * speed;
        yield return new WaitForSeconds(duration);
        rb.velocity = Vector2.zero;

        if (dashTrail != null)
            dashTrail.emitting = false;

        isDashing = false;
    }

    #region Gizmos
    // 씬 뷰에서 AI 범위 및 상태 시각화
    void OnDrawGizmos()
    {
        // 범위 시각화
        Gizmos.color = Color.red; Gizmos.DrawWireSphere(transform.position, attackRange);
        Gizmos.color = Color.yellow; Gizmos.DrawWireSphere(transform.position, backdashRange);
        Gizmos.color = Color.Lerp(Color.red, Color.yellow, 0.5f); Gizmos.DrawWireSphere(transform.position, chaseDashTriggerRange);
        Gizmos.color = Color.cyan; Gizmos.DrawWireSphere(transform.position, detectRange);

        // 공격 박스 시각화 (BoxCollider2D 기준)
        if (attackCollider != null)
        {
            Gizmos.color = Color.magenta;
            // BoxCollider2D의 월드 좌표 바운드 사용
            Bounds bounds = attackCollider.bounds;
            Gizmos.DrawWireCube(bounds.center, bounds.size);
        }

        // 상태 시각화 (게임 실행 중)
        if (Application.isPlaying)
        {
            if (target != null) { Gizmos.color = Color.white; Gizmos.DrawLine(transform.position, target.position); Gizmos.color = IsPlayerFacingBoss() ? Color.green : Color.gray; Gizmos.DrawCube(transform.position + Vector3.up * 1.5f, Vector3.one * 0.2f); }
            // bool isSummoningNow = (summoner != null && summoner.IsSummoning); // 소환 기능 사용 시
            if (isActing /*|| isSummoningNow*/) { if (isChaseDashing) Gizmos.color = Color.magenta; else if (isDashing) Gizmos.color = Color.blue; /* else if(isSummoningNow) Gizmos.color = Color.white; */ else Gizmos.color = Color.Lerp(Color.red, Color.black, 0.5f); } else { Gizmos.color = Color.green; }
            Gizmos.DrawSphere(transform.position + Vector3.down * 0.5f, 0.15f);
        }
    }
    #endregion
}