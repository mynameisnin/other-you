using UnityEngine;
using System.Collections;

/// <summary>
/// 보스 AI 시스템 클래스.
/// 플레이어 추적, 거리 기반 공격 결정(백대쉬 예측 포함), 무작위 공격 패턴 실행,
/// 공격 중 추격 대쉬, 대쉬/백대쉬(상승 포함) 및 방향 전환 애니메이션 제어를 담당합니다.
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
    private SpriteRenderer targetSpriteRenderer; // ★ 추가: 타겟의 스프라이트 렌더러 저장

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
    [SerializeField] private float moveSpeed = 2f; // 일반 이동 속도 (추격 대쉬 계산용)

    // --- 대쉬 설정 ---
    [Header("대쉬 설정")]
    [Tooltip("전방/후방 대쉬 시 이동할 총 거리")]
    public float dashDistance = 3f;
    [Tooltip("전방/후방 대쉬가 지속되는 시간")]
    public float dashDuration = 0.3f;
    [Tooltip("백대쉬 시 위쪽으로 이동하는 정도 (0: 수평, 1: 45도 위)")]
    [Range(0f, 1f)]
    [SerializeField] private float backdashUpwardFactor = 0.3f; // 백대쉬 상승 각도 조절
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

    // --- 내부 상태 변수 ---
    private bool isActing = false;        // 현재 행동(공격 또는 일반/백 대쉬) 중인지 여부
    private bool facingRight = true;      // 현재 바라보는 방향 (true: 오른쪽)
    private bool isDashing = false;       // 현재 모든 종류의 대쉬(일반, 백, 추격) 중인지 (이펙트 제어용)
    private bool isChaseDashing = false;    // 현재 공격 중 추격 대쉬 중인지 여부

    #endregion

    #region 유니티 생명주기 메서드

    void Start()
    {
        // 필수 컴포넌트 가져오기 및 확인
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        if (animator == null || rb == null || spriteRenderer == null)
        {
            Debug.LogError($"필수 컴포넌트 누락! (Animator: {animator == null}, Rigidbody2D: {rb == null}, SpriteRenderer: {spriteRenderer == null})", this);
            this.enabled = false;
            return;
        }
        if (dashTrail != null) dashTrail.emitting = false;
        else Debug.LogWarning("Dash Trail Renderer가 할당되지 않았습니다.", this);

        rb.constraints = RigidbodyConstraints2D.FreezeRotation;
        // facingRight = !spriteRenderer.flipX; // 초기 방향 설정 필요 시
    }

    void Update()
    {
        // 현재 행동 중이면 중단
        if (isActing || isChaseDashing) return;

        // 타겟 찾기 (및 스프라이트 렌더러 저장)
        FindClosestTarget();
        if (target == null) return; // 타겟 없으면 중단

        // 타겟 방향으로 몸 돌리기
        FlipTowardsTarget();

        // 거리 계산
        float distance = Vector2.Distance(transform.position, target.position);

        // --- 행동 결정 로직 ---
        // 1. 백대쉬 조건
        if (distance < backdashRange && IsPlayerFacingBoss() && CanPredictPlayerAttack())
        {
            StartCoroutine(DashRoutine(-1)); // 백대쉬
        }
        // 2. 공격 조건 (백대쉬 조건 불충족 시)
        else if (distance < attackRange)
        {
            // 백대쉬 거리 안에 있지만 시선/예측 불만족 시 처리
            if (distance < backdashRange)
            {
                // TODO: 공격 대신 다른 행동 (대기, 방어 등) 추가 가능
                // 현재: 아무것도 안 하고 다음 프레임에 다시 판단
                // Debug.Log("백대쉬 조건 불충족, 공격 보류");
            }
            else // 적절한 공격 거리
            {
                StartCoroutine(AttackRoutine()); // 공격
            }
        }
        // 3. 접근 대쉬 조건
        else if (distance < detectRange)
        {
            StartCoroutine(DashRoutine(1)); // 전방 대쉬
        }
        // 4. 범위 밖 (현재는 아무 행동 안 함)
        // else { /* Idle 상태 처리 */ }
    }

    #endregion

    #region 타겟 찾기 및 방향 전환

    // 가장 가까운 활성화된 플레이어 타겟과 그 SpriteRenderer 찾기
    void FindClosestTarget()
    {
        GameObject adam = GameObject.FindWithTag("Player");
        GameObject deva = GameObject.FindWithTag("DevaPlayer");
        Transform closestTarget = null;
        SpriteRenderer closestSprite = null; // ★ 추가
        float minDist = Mathf.Infinity;

        // Adam 체크
        if (adam != null && adam.activeInHierarchy)
        {
            float dist = Vector2.Distance(transform.position, adam.transform.position);
            if (dist < minDist)
            {
                minDist = dist;
                closestTarget = adam.transform;
                closestSprite = adam.GetComponent<SpriteRenderer>(); // ★ 추가
            }
        }
        // Deva 체크
        if (deva != null && deva.activeInHierarchy)
        {
            float dist = Vector2.Distance(transform.position, deva.transform.position);
            if (dist < minDist)
            {
                closestTarget = deva.transform;
                closestSprite = deva.GetComponent<SpriteRenderer>(); // ★ 추가
            }
        }
        target = closestTarget;
        targetSpriteRenderer = closestSprite; // ★ 추가: 찾은 스프라이트 저장
    }

    // 타겟 방향으로 스프라이트 뒤집기
    void FlipTowardsTarget(bool forceFlip = false)
    {
        if (target == null) return;
        bool shouldFaceRight = (target.position.x > transform.position.x);
        if (facingRight != shouldFaceRight || forceFlip)
        {
            facingRight = shouldFaceRight;
            if (spriteRenderer != null) spriteRenderer.flipX = !facingRight;
        }
    }
    #endregion

    #region 행동 코루틴 (대쉬, 공격, 추격 대쉬)

    /// <summary>
    /// 일반 전방 대쉬 또는 백대쉬(상승 포함)를 실행하는 코루틴.
    /// </summary>
    IEnumerator DashRoutine(int direction)
    {
        isActing = true;
        isDashing = true;
        animator.SetTrigger(direction == 1 ? "Dash" : "Backdash");

        // 이펙트 시작
        if (dashTrail != null) dashTrail.emitting = true;
        StartCoroutine(LeaveAfterImage());

        // 방향 계산
        Vector2 dashDir;
        if (direction == 1) // 전방
        {
            if (target != null) { dashDir = ((Vector2)target.position - rb.position).normalized; FlipTowardsTarget(true); }
            else { dashDir = facingRight ? Vector2.right : Vector2.left; }
        }
        else // 후방 (백대쉬)
        {
            Vector2 backwardDir = facingRight ? Vector2.left : Vector2.right;
            Vector2 upwardDir = Vector2.up * backdashUpwardFactor;
            dashDir = (backwardDir + upwardDir).normalized;
        }

        // 이동 실행
        float currentDashSpeed = dashDistance / dashDuration;
        rb.velocity = dashDir * currentDashSpeed;

        yield return new WaitForSeconds(dashDuration);

        // 종료 처리
        rb.velocity = Vector2.zero;
        isDashing = false;
        if (dashTrail != null) dashTrail.emitting = false;
        yield return new WaitForSeconds(0.3f); // 딜레이
        isActing = false;
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

        // 공격 중 거리 체크 및 추격 대쉬
        float timeElapsed = 0f;
        while (timeElapsed < attackHitTiming && isActing)
        {
            if (target != null && !isChaseDashing)
            {
                float currentDistance = Vector2.Distance(transform.position, target.position);
                if (currentDistance > chaseDashTriggerRange)
                {
                    StartCoroutine(ChaseDashDuringAttack());
                }
            }
            yield return new WaitForSeconds(0.1f);
            timeElapsed += 0.1f;
            if (!IsPlayerValid()) // 타겟 유효성 재확인
            {
                isActing = false;
                if (isChaseDashing) { isDashing = false; isChaseDashing = false; if (dashTrail != null) dashTrail.emitting = false; rb.velocity = Vector2.zero; }
                yield break;
            }
        }

        // 공격 판정 시점까지 대기
        float remainingTime = attackHitTiming - timeElapsed;
        if (remainingTime > 0) { yield return new WaitForSeconds(remainingTime); }
        while (isChaseDashing) { yield return null; } // 추격 대쉬 완료 대기

        // 공격 판정
        if (isActing) PerformAttackHit();

        // 쿨다운 및 종료
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

        // 이펙트 시작
        if (dashTrail != null) dashTrail.emitting = true;
        StartCoroutine(LeaveAfterImage());

        // 이동 로직
        Vector2 chaseDir = Vector2.zero;
        if (IsPlayerValid()) // 타겟 유효성 확인
        {
            chaseDir = ((Vector2)target.position - rb.position).normalized;
            FlipTowardsTarget(true);
        }
        else { isChaseDashing = false; isDashing = false; if (dashTrail != null) dashTrail.emitting = false; yield break; }

        float chaseSpeed = moveSpeed * chaseDashSpeedMultiplier;
        rb.velocity = chaseDir * chaseSpeed;

        yield return new WaitForSeconds(chaseDashDuration);

        // 종료 처리
        if (isActing) rb.velocity = Vector2.zero; // 공격이 계속 중일 때만 멈춤
        isDashing = false;
        isChaseDashing = false;
        if (dashTrail != null) dashTrail.emitting = false;
        yield break;
    }

    // 실제 공격 판정 실행 함수
    void PerformAttackHit()
    {
        Debug.Log("보스 공격 판정!");
        // TODO: 실제 판정 로직 구현
    }

    #endregion

    #region 예측 및 보조 함수

    // 플레이어가 보스를 향하고 있는지 확인
    bool IsPlayerFacingBoss()
    {
        // ★ 변경: 저장된 targetSpriteRenderer 사용
        if (target == null || targetSpriteRenderer == null) return false;

        bool playerIsFacingRight = !targetSpriteRenderer.flipX;
        bool playerIsRightOfBoss = target.position.x > transform.position.x;
        return (!playerIsRightOfBoss && !playerIsFacingRight) || (playerIsRightOfBoss && playerIsFacingRight);
    }

    // 플레이어 공격 예측 (현재는 항상 true)
    bool CanPredictPlayerAttack()
    {
        // TODO: 예측 로직 강화
        return true;
    }

    // 플레이어 참조 유효성 확인
    bool IsPlayerValid()
    {
        // targetSpriteRenderer도 함께 확인 (FindClosestTarget에서 같이 설정되므로)
        return target != null && target.gameObject.activeInHierarchy && targetSpriteRenderer != null;
    }

    // AttackBoxes 배열 유효성 검사 (현재 사용 안 함)
    void ValidateAttackBoxes() { }

    #endregion

    #region 대쉬 이펙트 로직

    // 대쉬 중 잔상 생성 코루틴
    private IEnumerator LeaveAfterImage()
    {
        while (isDashing)
        {
            CreateAfterImage();
            yield return new WaitForSeconds(afterImageInterval);
        }
        yield break;
    }

    // 잔상 게임 오브젝트 생성 및 설정
    void CreateAfterImage()
    {
        GameObject afterImage = new GameObject("AfterImage_Boss");
        SpriteRenderer sr = afterImage.AddComponent<SpriteRenderer>();
        sr.sprite = spriteRenderer.sprite;
        sr.color = new Color(1f, 0.2f, 0.2f, 0.7f); // 붉은색 잔상
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
        float fadeDuration = afterImageLifetime;
        Color originalColor = sr.color;
        float elapsed = 0f;
        while (elapsed < fadeDuration)
        {
            if (sr == null) yield break;
            elapsed += Time.deltaTime;
            float alpha = Mathf.Lerp(originalColor.a, 0, elapsed / fadeDuration);
            sr.color = new Color(originalColor.r, originalColor.g, originalColor.b, alpha);
            yield return null;
        }
        if (sr != null && sr.gameObject != null) Destroy(sr.gameObject);
        yield break;
    }
    #endregion

    #region Gizmos

    // 씬 뷰에서 AI 범위 및 상태 시각화 (항상 표시)
    void OnDrawGizmos()
    {
        // 범위 시각화
        Gizmos.color = Color.red; Gizmos.DrawWireSphere(transform.position, attackRange);
        Gizmos.color = Color.yellow; Gizmos.DrawWireSphere(transform.position, backdashRange);
        Gizmos.color = Color.Lerp(Color.red, Color.yellow, 0.5f); Gizmos.DrawWireSphere(transform.position, chaseDashTriggerRange);
        Gizmos.color = Color.cyan; Gizmos.DrawWireSphere(transform.position, detectRange);

        // 상태 시각화 (게임 실행 중)
        if (Application.isPlaying)
        {
            // 타겟 라인
            if (target != null)
            {
                Gizmos.color = Color.white;
                Gizmos.DrawLine(transform.position, target.position);
                // 플레이어 시선 표시
                Gizmos.color = IsPlayerFacingBoss() ? Color.green : Color.gray;
                Gizmos.DrawCube(transform.position + Vector3.up * 1.5f, Vector3.one * 0.2f);
            }
            // 현재 행동 상태 표시
            if (isActing) { if (isChaseDashing) Gizmos.color = Color.magenta; else if (isDashing) Gizmos.color = Color.blue; else Gizmos.color = Color.Lerp(Color.red, Color.black, 0.5f); }
            else { Gizmos.color = Color.green; }
            Gizmos.DrawSphere(transform.position + Vector3.down * 0.5f, 0.15f);
        }
    }

    #endregion
}