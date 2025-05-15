using UnityEngine;
using System.Collections;
using System.Collections.Generic; // Gizmos 디버깅용

/// <summary>
/// 보스 AI 시스템 클래스.
/// 플레이어 추적, 거리 기반 공격 결정(백대쉬 예측 포함), 무작위 공격 패턴 실행,
/// 공격 중 추격 대쉬, 대쉬/백대쉬(상승 포함) 및 방향 전환 애니메이션 제어를 담당합니다.
/// 백대쉬 이동은 애니메이션 이벤트로 시작됩니다. Flip 시 공격 콜라이더 Offset도 반전됩니다.
/// </summary>
 [RequireComponent(typeof(AngryGodActiveSkill1))] // 액티브 스킬 스크립트 필수
[RequireComponent(typeof(AngryGodUltimateSkill))]
public class AngryGodAiCore : MonoBehaviour
{
    #region 변수 선언

    // --- 컴포넌트 참조 ---
    private Animator animator;
    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;
    private AngryGodActiveSkill1 activeSkill1; // ★ 추가: 액티브 스킬 스크립트 참조

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
    [Tooltip("액티브 스킬 1을 사용할 조건을 만족하는 거리 (예시)")] // ★ 추가
    public float activeSkill1TriggerRange = 8f; // 예시: 탐지 범위 내 특정 거리
    private BossSummoner bossSummoner;
    private float globalActionCooldownTime = -99f; // 공용 쿨타임 시작
    private bool isAwakening = false; // 각성 애니메이션 중인지 여부
    private bool awakeningRequested = false; // BossHurt로부터 각성 요청을 받았는지 여부
    public float awakeningAnimationDuration = 3.0f; // 각성 애니메이션의 예상 길이 (인스펙터에서 조절 가능하게 public으로)
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
    [Header("AI 행동 확률")] // ★ 추가: 확률 관련 변수 그룹
    [Tooltip("백대쉬 범위 내에서 백대쉬를 시도할 확률 (0.0 ~ 1.0)")]
    [Range(0f, 1f)]
    [SerializeField] private float backdashProbability = 0.6f; // 예: 60% 확률로 백대쉬
    private AngryGodFlameSkill flameSkill;
    private AngryGodUltimateSkill ultimateSkill;
    #endregion

    #region 유니티 생명주기 메서드

    void Start()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        activeSkill1 = GetComponent<AngryGodActiveSkill1>(); // ★ 추가: 액티브 스킬 참조 가져오기
        bossSummoner = GetComponent<BossSummoner>();
        flameSkill = GetComponent<AngryGodFlameSkill>();
        ultimateSkill = GetComponent<AngryGodUltimateSkill>();
        // 필수 컴포넌트 확인
        if (animator == null || rb == null || spriteRenderer == null || activeSkill1 == null) // ★ 수정: activeSkill1 추가
        {
         
            this.enabled = false; return;
        }

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

        // 현재 다른 행동 중이면 중단
        if (isActing || isChaseDashing ||
            (activeSkill1 != null && activeSkill1.IsSkillActive) ||
            (bossSummoner != null && bossSummoner.IsSummoning) ||
            (GetComponent<AngryGodFlameSkill>()?.IsFlaming ?? false)
            || (ultimateSkill != null && ultimateSkill.IsUltimateActive)) // ★★★ 추가 ★★★

        {
            return;
        }


        FindClosestTarget();
        if (target == null) { /* Debug.Log("[AI Core] Target is null."); */ return; }

        FlipTowardsTarget();
        float distance = Vector2.Distance(transform.position, target.position);
        // AngryGodAiCore.cs 내부 Update 함수 상단에 추가


        // --- 행동 결정 로직 ---
        bool decidedAction = false;

        // 0순위: 액티브 스킬 시도 (주석 처리됨)
        // ...

        if (!decidedAction && distance < backdashRange) // 백대쉬 범위 안에 있을 때
        {
            float randomValue = Random.value;
            bool isFacing = IsPlayerFacingBoss();
            bool canPredict = CanPredictPlayerAttack();

            bool forceBackdash = (backdashProbability >= 1.0f);
            bool shouldBackdash = forceBackdash || (randomValue < backdashProbability);

            // 먼저 FlameSkill 발동 가능 여부 확인
            if (flameSkill != null && Time.time >= flameSkill.GetLastFlameTime() + 15f)
            {
                StartCoroutine(flameSkill.TryUseFlame());
                decidedAction = true;
            }
            else if (shouldBackdash)
            {
                PrepareBackdash();
                decidedAction = true;
            }
            else
            {
                StartCoroutine(AttackRoutine());
                decidedAction = true;
            }
        }



        // 2. 일반 공격 시도 (백대쉬/근접공격 안 했고, 공격 범위 안)
        // ★ 디버그: 일반 공격 조건 확인
        bool inAttackRange = !decidedAction && distance < attackRange; // 자동으로 distance >= backdashRange
        

        if (inAttackRange)
        {
            
            StartCoroutine(AttackRoutine());
            decidedAction = true;
        }

        // 3. 접근 대쉬 시도 (위 행동들 안 했고, 탐지 범위 안)
        // ★ 디버그: 접근 대쉬 조건 확인
        bool inDetectRange = !decidedAction && distance < detectRange; // 자동으로 distance >= attackRange
       

        if (inDetectRange)
        {
           
            StartCoroutine(DashRoutine(1));
            decidedAction = true;
        }
        // 각성 요청이 있고, 현재 다른 중요한 행동을 하고 있지 않다면 각성 백대쉬 시도
        if (awakeningRequested && !IsCurrentlyActingOrSkillActive())
        {
            PrepareAndExecuteAwakeningBackdash();
            // awakeningRequested는 ExecuteDashMovement에서 각성 애니메이션 시작 시 false로 변경
        }

        // 현재 다른 행동 중이면 중단 (isAwakening 포함)
        if (IsCurrentlyActingOrSkillActive())
        {
            return;
        }
        // 4. 아무 행동도 결정되지 않음
        if (!decidedAction)
        {
            Debug.Log("[AI Core]   => DECISION: No action decided (Idle or out of range).");
        }
        Debug.Log("[AI Core] ----- Frame End -----");
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

    // 백대쉬 준비 함수 (애니메이션만 트리거)
    private void PrepareBackdash()
    {
        if (isActing || isChaseDashing) return; // 중복 방지 중요
        isActing = true;
        rb.velocity = Vector2.zero;
        FlipTowardsTarget(true);
        animator.SetTrigger("Backdash");
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
        else
        {
            Debug.LogWarning($"[AI Core] ExecuteDashMovement 호출되었으나 direction이 -1이 아님: {direction}");
            isDashing = false;
            if (dashTrail != null) dashTrail.emitting = false;
            // isActing = false; // 여기서 isActing을 false로 해야 할 수도 있음 (PrepareBackdash에서 true로 했다면)
            yield break;
        }

        float currentDashSpeed = (dashDuration > 0.01f) ? distanceToMove / dashDuration : 0;
        rb.velocity = dashDir * currentDashSpeed;
        yield return new WaitForSeconds(dashDuration);
        rb.velocity = Vector2.zero;
        isDashing = false; // 이펙트 종료용 플래그

        if (dashTrail != null) dashTrail.emitting = false;

        // --- 백대쉬 후 행동 결정 ---
        // 가장 먼저 각성 요청이 있었는지 확인하고 처리 (최우선 순위)
        if (awakeningRequested && direction == -1)
        {
            awakeningRequested = false;
            isAwakening = true;
            // isActing은 이미 true 상태일 것임 (PrepareBackdash 등에서)

            Debug.Log("[AI Core] 백대쉬 후 각성 애니메이션 트리거 호출!");
            animator.SetTrigger("Awakening"); // ★★★ 각성 애니메이션 트리거 호출 ★★★
                                              // "Awakening"은 Animator Controller에 정의된 Trigger 이름이어야 합니다.

            // BossHurt의 phase2ObjectAnime은 이제 다른 용도(예: 각성 시 특별 이펙트만 담당)거나
            // 사용하지 않을 수 있습니다. 또는 각성 애니메이션 자체에 이펙트가 포함될 수도 있습니다.
            // 만약 phase2ObjectAnime이 순수하게 이펙트용이라면 여기서 활성화할 수 있습니다.
            BossHurt bossHurt = GetComponent<BossHurt>();
            if (bossHurt != null && bossHurt.phase2ObjectAnime != null)
            {
                // 만약 phase2ObjectAnime이 각성 '이펙트' 전용 오브젝트라면 여기서 활성화
                // bossHurt.phase2ObjectAnime.SetActive(true);
                // 이 부분은 각성 애니메이션과 phase2ObjectAnime의 역할에 따라 결정됩니다.
                // 지금은 애니메이션 트리거로 각성 동작을 제어하므로, 주석 처리하거나 역할을 명확히 합니다.
            }

            yield return new WaitForSeconds(awakeningAnimationDuration); // 각성 애니메이션 지속 시간만큼 대기

            Debug.Log("[AI Core] 각성 애니메이션 종료.");
            isAwakening = false;

            if (bossHurt != null && bossHurt.phase2Object != null && !bossHurt.phase2Object.activeSelf)
            {
                Debug.Log("[AI Core] 각성 후 Phase 2 오브젝트 활성화.");
                bossHurt.phase2Object.SetActive(true);
            }
        }
        else if (direction == -1) // 각성 요청이 없었고, 백대쉬 상황일 때의 다른 행동들
        {
            // 액티브 스킬 1 시도 (각성하지 않았을 경우에만)
            if (activeSkill1 != null && !activeSkill1.IsSkillActive)
            {
                if (Time.time >= activeSkill1.GetLastSkillUseTime() + 8f) // 8f은 예시 쿨타임
                {
                    Debug.Log("[AI Core] 백대쉬 완료 후 액티브 스킬 1 시도 (쿨타임 충족).");
                    yield return StartCoroutine(activeSkill1.TryStartSkillAfterBackdash()); // activeSkill1이 끝날 때까지 대기 (선택적)
                }
                else
                {
                    Debug.Log("[AI Core] 액티브 스킬 1 쿨타임 미충족 - 사용 안함");
                }
            }

            // 소환 시도 (각성하지 않았고, 액티브 스킬 1도 사용하지 않았거나 끝났을 경우 - 순서 조정 가능)
            if (bossSummoner != null && !bossSummoner.IsSummoning)
            {
                // bossSummoner에 쿨타임이 있다면 여기서 체크
                Debug.Log("[AI Core] 백대쉬 완료 후 소환 시도.");
                yield return StartCoroutine(bossSummoner.TryStartSummonAfterBackdash()); // 소환이 끝날 때까지 대기 (선택적)
            }

            // 궁극기 시도 (각성하지 않았고, 위 스킬들도 사용하지 않았거나 끝났을 경우 - 순서 조정 가능)
            if (ultimateSkill != null && !ultimateSkill.IsUltimateActive)
            {
                if (Time.time >= ultimateSkill.GetLastUseTime() + ultimateSkill.cooldown)
                {
                    Debug.Log("[AI Core] 백대쉬 후 궁극기 발동 시도.");
                    yield return StartCoroutine(ultimateSkill.TryStartUltimate()); // 궁극기가 끝날 때까지 대기
                    Debug.Log("[AI Core] 이번 백대쉬 후 궁극기를 사용했습니다 (시도함).");
                }
                else
                {
                    Debug.Log("[AI Core] 궁극기 쿨타임입니다 (백대쉬 후 체크).");
                    Debug.Log("[AI Core] 이번 백대쉬 후 궁극기를 사용하지 않았습니다 (쿨타임).");
                }
            }
            else if (ultimateSkill != null && ultimateSkill.IsUltimateActive)
            {
                Debug.Log("[AI Core] 궁극기를 시도할 수 없음: 이미 궁극기가 활성 상태입니다.");
                Debug.Log("[AI Core] 이번 백대쉬 후 궁극기를 사용하지 않았습니다 (이미 활성).");
            }
            else if (ultimateSkill == null)
            {
                Debug.LogWarning("[AI Core] 궁극기를 시도할 수 없음: ultimateSkill 참조가 null입니다.");
                Debug.Log("[AI Core] 이번 백대쉬 후 궁극기를 사용하지 않았습니다 (참조 없음).");
            }
        }

        // 모든 백대쉬 후속 행동(각성 포함)이 완료된 후 약간의 딜레이
        yield return new WaitForSeconds(0.1f); // 이 딜레이는 각성 애니메이션 후에도 적용됨

        isActing = false; // 이 코루틴(백대쉬 및 연계 행동)의 최종 종료
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
   public bool IsPlayerValid() { return target != null && target.gameObject.activeInHierarchy && targetSpriteRenderer != null; }
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
    public Transform GetPlayer()
    {
        return target;
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
        Gizmos.color = Color.blue; Gizmos.DrawWireSphere(transform.position, activeSkill1TriggerRange); // ★ 추가: 스킬 발동 범위

        // 공격 박스 시각화
        if (attackCollider != null) { Gizmos.color = Color.magenta; Bounds bounds = attackCollider.bounds; Gizmos.DrawWireCube(bounds.center, bounds.size); }

        // 상태 시각화 (게임 실행 중)
        if (Application.isPlaying)
        {
            if (target != null) { Gizmos.color = Color.white; Gizmos.DrawLine(transform.position, target.position); Gizmos.color = IsPlayerFacingBoss() ? Color.green : Color.gray; Gizmos.DrawCube(transform.position + Vector3.up * 1.5f, Vector3.one * 0.2f); }

            bool isSkill1Active = (activeSkill1 != null && activeSkill1.IsSkillActive); // 스킬 활성 상태 확인

            if (isActing || isSkill1Active) // isActing 또는 스킬 활성 상태일 때
            {
                if (isChaseDashing) Gizmos.color = Color.magenta;
                else if (isDashing) Gizmos.color = Color.blue;
                else if (isSkill1Active) Gizmos.color = Color.white; // 스킬 사용 중: 흰색 (예시)
                else Gizmos.color = Color.Lerp(Color.red, Color.black, 0.5f); // 공격 중
            }
            else { Gizmos.color = Color.green; } // 대기 중
            Gizmos.DrawSphere(transform.position + Vector3.down * 0.5f, 0.15f);
        }
    }
    #endregion
    public bool IsCurrentlyActingOrSkillActive()
    {
        return isActing || isChaseDashing ||
               (activeSkill1 != null && activeSkill1.IsSkillActive) ||
               (ultimateSkill != null && ultimateSkill.IsUltimateActive) ||
               (bossSummoner != null && bossSummoner.IsSummoning) ||
               (flameSkill != null && flameSkill.IsFlaming) || // IsFlaming은 flameSkill에 있어야 함
               isAwakening;
    }
    #region 외부 상호작용 함수 (ActiveSkill1 및 필요시 다른 스크립트용)

    /// <summary> 외부 스크립트가 AI의 행동 시작을 알릴 때 호출. isActing = true 설정. </summary>
    public void NotifyActionStart() { this.isActing = true; }
    /// <summary> 외부 스크립트가 AI의 행동 종료를 알릴 때 호출. isActing = false 설정. </summary>
    public void NotifyActionEnd() { this.isActing = false; }
    /// <summary> 외부 스크립트가 AI의 이동을 멈추도록 요청. </summary>
    public void StopMovement() { if (rb != null) rb.velocity = Vector2.zero; }
    /// <summary> 외부 스크립트가 AI의 방향 전환을 강제. </summary>
    public void ForceFlipTowardsTarget() { FlipTowardsTarget(true); }
    /// <summary> 현재 AI가 '주 행동'(공격, 일반/백 대쉬, 스킬) 또는 '추격 대쉬' 중인지 확인. </summary>
    public bool IsCurrentlyActing() { return isActing || isChaseDashing; }
    /// <summary> 외부 스크립트(예: ActiveSkill1)가 백대쉬 시작을 요청할 때 호출합니다. </summary>
    public void InitiateBackdash()
    {
        // 이미 행동 중이 아니어야 함
        if (!isActing && !isChaseDashing) { PrepareBackdash(); }
        else { Debug.LogWarning("다른 행동 중이라 백대쉬 시작 불가."); }
    }
    /// </summary>
    public void RequestAwakeningSequence()
    {
        if (!isAwakening && !awakeningRequested) // 중복 요청 방지
        {
            Debug.Log("[AI Core] 각성 시퀀스 요청 받음.");
            awakeningRequested = true;
            // 현재 행동을 즉시 중단하고 백대쉬를 시도할 수도 있고,
            // 또는 다음 행동 결정 시 우선적으로 백대쉬를 하도록 할 수도 있습니다.
            // 여기서는 즉시 백대쉬를 준비하도록 합니다.
            if (!IsCurrentlyActingOrSkillActive()) // 다른 중요한 행동 중이 아닐 때만 즉시 실행
            {
                PrepareAndExecuteAwakeningBackdash();
            }
            // 만약 IsCurrentlyActingOrSkillActive()가 true라면, Update()에서 awakeningRequested를 체크하여
            // 현재 행동이 끝난 후 백대쉬를 하도록 유도할 수 있습니다. (더 복잡한 로직 필요)
            // 현재는 즉시 시도하는 것으로 단순화합니다.
        }
    }

    /// <summary>
    /// 각성을 위한 백대쉬를 준비하고 실행합니다.
    /// </summary>
    private void PrepareAndExecuteAwakeningBackdash()
    {
        if (isActing || isChaseDashing || isAwakening) return; // 이미 다른 행동 중이거나 각성 중이면 중복 방지

        Debug.Log("[AI Core] 각성 백대쉬 준비.");
        isActing = true; // 백대쉬도 하나의 행동
        rb.velocity = Vector2.zero;
        FlipTowardsTarget(true); // 플레이어가 어느 쪽에 있든 반대 방향으로 백대쉬하기 위함
        animator.SetTrigger("Backdash"); // 백대쉬 애니메이션 시작 (이후 AnimEvent로 ExecuteDashMovement 호출)
                                         // ExecuteDashMovement에서는 awakeningRequested를 확인하여 각성 애니메이션으로 연결
    }

    public float GetGlobalCooldownTime() => globalActionCooldownTime;
    public void SetGlobalCooldownTime(float nextTime) => globalActionCooldownTime = nextTime;
    public bool IsFacingRight => transform.localScale.x > 0f;

    #endregion
}