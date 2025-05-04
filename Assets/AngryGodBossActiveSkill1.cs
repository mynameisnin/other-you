using UnityEngine;
using System.Collections;
using System.Collections.Generic; // Gizmos 디버깅용

/// <summary>
/// 보스의 특정 액티브 스킬 (상승 후 공격)을 관리하는 스크립트.
/// AngryGodAiCore와 협력하며, 애니메이션 이벤트로 상승/하강 및 액션 시점을 제어합니다.
/// </summary>
[RequireComponent(typeof(AngryGodAiCore))]
[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(Rigidbody2D))]
public class AngryGodActiveSkill1 : MonoBehaviour
{
    #region 변수 선언

    [Header("스킬 설정")]
    [Tooltip("스킬 사용 후 다음 사용까지의 대기 시간 (초)")]
    [SerializeField] private float skillCooldown = 20f;
    [Tooltip("상승/하강 시 사용할 속도")]
    [SerializeField] private float ascendDescendSpeed = 5f;
    [Tooltip("최대 상승 높이 (현재 위치 기준)")]
    [SerializeField] private float ascendHeight = 4f;
    [Tooltip("플레이어가 스킬 시전 전에 이 거리보다 가까우면 백대쉬 유도")]
    [SerializeField] private float requiredBackdashDistance = 3.5f; // AttackRange보다 약간 크게

    // --- Prefabs and Effects (Optional) ---
    [Header("Effects (Optional)")]
    [Tooltip("공중 액션 시 생성할 마법 효과/발사체 프리팹")]
    [SerializeField] private GameObject magicEffectPrefab;
    [Tooltip("마법 효과가 생성될 위치 Transform")]
    [SerializeField] private Transform magicSpawnPoint;
    [Tooltip("스킬 시작 시 재생할 VFX")]
    [SerializeField] private GameObject startSkillVFX;
    [Tooltip("공중 액션 중 재생할 VFX")]
    [SerializeField] private GameObject airActionVFX;
    private GameObject currentAirVFXInstance = null; // 현재 재생 중인 공중 VFX 인스턴스


    // --- 내부 상태 ---
    private float lastSkillUseTime = -99f; // 마지막 스킬 사용 시간
    private bool isSkillActive = false;   // 현재 이 스킬 코루틴 실행 중인지 여부
    private Vector2 groundPosition;       // 상승 전 원래 지면 위치 저장용
    private Coroutine moveCoroutine = null; // 현재 실행중인 이동 코루틴 참조
    private float originalGravityScale;   // 원래 중력 스케일 값

    // --- 참조 ---
    private AngryGodAiCore aiCore; // 메인 AI 스크립트 참조
    private Animator animator;     // 애니메이터 참조
    private Rigidbody2D rb;        // Rigidbody2D 참조

    #endregion

    #region 초기화

    private void Awake()
    {
        aiCore = GetComponent<AngryGodAiCore>();
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();

        if (aiCore == null || animator == null || rb == null)
        {
            Debug.LogError("필수 참조 컴포넌트 없음!", this);
            this.enabled = false;
        }
    }

    private void Start()
    {
        lastSkillUseTime = -skillCooldown; // 쿨다운 초기화
        if (rb != null) originalGravityScale = rb.gravityScale; // 원래 중력 저장
        else this.enabled = false;
    }

    #endregion

    #region 스킬 실행 및 제어

    /// <summary>
    /// 외부(AngryGodAiCore)에서 스킬 사용을 시도. 쿨다운, 거리, 상태 확인 후 실행 또는 백대쉬 요청.
    /// </summary>
    public bool TryStartSkill(float distanceToTarget)
    {
        bool cooldownReady = Time.time >= lastSkillUseTime + skillCooldown;
        bool skillNotActive = !isSkillActive;
        bool aiCoreNotActing = (aiCore != null && !aiCore.IsCurrentlyActing());

        Debug.Log($"[Skill1] TryStartSkill Check: Cooldown Ready={cooldownReady}, Skill Not Active={skillNotActive}, AI Core Not Acting={aiCoreNotActing}");
        Debug.Log($"[Skill1] Time: {Time.time}, LastUse: {lastSkillUseTime}, CD: {skillCooldown}");
        if (cooldownReady && skillNotActive && aiCoreNotActing)
        {
            //  수정된 로직: 거리와 상관없이 무조건 백대쉬 → 이후 스킬 시전
            Debug.Log("[Skill1] Forcing backdash before skill.");
            aiCore.InitiateBackdash();

            //  백대쉬 후 자동으로 스킬 시전할 수 있도록 플래그를 주거나,
            // 또는 AngryGodAiCore 쪽에서 백대쉬 종료 후 SkillRoutine 호출 필요
            // (직접 코루틴 호출 X ? InitiateBackdash 내부에서 트리거 필요)

            return true;
        }
        else
        {
            Debug.LogWarning($"[Skill1] Cannot start skill. CooldownReady:{cooldownReady}, SkillNotActive:{skillNotActive}, AICoreNotActing:{aiCoreNotActing}");
            return false;
        }
    }


    /// <summary>
    /// 스킬 전체 시퀀스 코루틴. 애니메이션 이벤트에 의해 주요 동작이 트리거됨.
    /// </summary>
    private IEnumerator SkillRoutine()
    {
        isSkillActive = true;
        aiCore.NotifyActionStart(); // AI 코어 행동 시작 알림
        Debug.Log("[Skill1] 스킬 시퀀스 시작.");

        // 준비 단계
        aiCore.StopMovement(); // 이동 정지
        aiCore.ForceFlipTowardsTarget(); // 타겟 방향 보기
        rb.velocity = Vector2.zero; // 만약을 대비해 속도 0
        rb.gravityScale = 0; // 중력 비활성화
        groundPosition = transform.position; // 현재 위치 저장

        // 시작 VFX (선택적)
        if (startSkillVFX != null) Instantiate(startSkillVFX, transform.position, Quaternion.identity);

        // "ActiveSkill1" 애니메이션 트리거
        animator.SetTrigger("ActiveSkill1");

        // 이 코루틴은 스킬이 완전히 끝날 때까지 대기하는 역할만 함
        // 실제 상승/하강/액션/종료는 애니메이션 이벤트가 담당
        while (isSkillActive)
        {
            // 안전 장치: 타겟이 중간에 사라지면 스킬 중단
             //  수정: aiCore null 체크 추가
            if (aiCore == null || !aiCore.IsPlayerValid())
            {
                Debug.Log("[Skill1] 스킬 중 타겟 소실, 중단 시작.");
                yield return StartCoroutine(AbortSkill()); // 중단 처리 코루틴 실행
                yield break; // 현재 코루틴 종료
            }
            yield return null; // 다음 프레임까지 대기
        }

        Debug.Log("[Skill1] 스킬 시퀀스 코루틴 종료.");
        Debug.Log($"[Skill1] isSkillActive={isSkillActive}, lastSkillUseTime={lastSkillUseTime}, cooldown={skillCooldown}");
        yield break;
    }
    public IEnumerator TryStartSkillAfterBackdash()
    {
        yield return new WaitForSeconds(0.1f); // 딜레이 (안정성)
        StartCoroutine(SkillRoutine()); // 강제 실행
    }
    /// <summary>
    /// [Animation Event] 상승 이동 시작.
    /// </summary>
    public void AnimEvent_StartAscend()
    {
        if (!isSkillActive) return;
        Debug.Log("[Skill1] AnimEvent: 상승 시작");

        if (moveCoroutine != null) StopCoroutine(moveCoroutine);
        Vector2 targetPos = groundPosition + Vector2.up * ascendHeight;

        //  잔상 시작
        if (afterImageCoroutine != null) StopCoroutine(afterImageCoroutine);
        afterImageCoroutine = StartCoroutine(LeaveAfterImage());

        moveCoroutine = StartCoroutine(MoveToPositionRoutine(targetPos, ascendDescendSpeed));
    }

    /// <summary>
    /// [Animation Event] 공중 액션 (마법 시전 등) 시작.
    /// </summary>
    public void AnimEvent_PerformAirAction()
    {
        if (!isSkillActive) return;
        Debug.Log("[Skill1] AnimEvent: 공중 액션 시작");

        // 상승/하강 중이었다면 멈춤
        if (moveCoroutine != null) StopCoroutine(moveCoroutine);
        if (rb != null) rb.velocity = Vector2.zero; // null 체크 추가

        // 공중 액션 실행
        StartCoroutine(AirActionRoutine());
    }

    /// <summary>
    /// 공중 액션 코루틴 (예: 마법 시전).
    /// </summary>
    private IEnumerator AirActionRoutine()
    {
        Debug.Log("[Skill1] 공중 액션 수행 중...");

        // 공중 VFX 시작 (선택적)
        if (airActionVFX != null) currentAirVFXInstance = Instantiate(airActionVFX, transform.position, Quaternion.identity, transform);

        // 마법 효과 생성
        if (magicEffectPrefab != null && magicSpawnPoint != null)
        {
            Instantiate(magicEffectPrefab, magicSpawnPoint.position, magicSpawnPoint.rotation);
        }
        else Debug.LogWarning("[Skill1] 마법 효과 프리팹 또는 생성 위치 미설정.");

        // 애니메이션이 자연스럽게 하강 준비 동작으로 넘어가도록 함
        yield break;
    }


    /// <summary>
    /// [Animation Event] 하강 이동 시작.
    /// </summary>
    public void AnimEvent_StartDescend()
    {
        if (!isSkillActive) return;
        Debug.Log("[Skill1] AnimEvent: 하강 시작");

        if (currentAirVFXInstance != null) Destroy(currentAirVFXInstance);
        if (moveCoroutine != null) StopCoroutine(moveCoroutine);
        //  수정: ascendDescendSpeed 변수 사용 확인 (이미 올바르게 되어 있음)
        moveCoroutine = StartCoroutine(MoveToPositionRoutine(groundPosition, ascendDescendSpeed));
    }
    /// <summary>
    /// [Animation Event] 스킬 애니메이션 완전 종료 시 호출.
    /// </summary>
    public void AnimEvent_SkillEnd()
    {
        if (!isSkillActive) return;
        Debug.Log("[Skill1] AnimEvent: 스킬 종료");
        EndSkill(); // 스킬 마무리 처리
    }

    /// <summary>
    /// 지정된 위치까지 이동하는 코루틴.
    /// </summary>
    private IEnumerator MoveToPositionRoutine(Vector2 targetPosition, float speed)
    {
        //  수정: rb null 체크 추가
        while (rb != null && Vector2.Distance(rb.position, targetPosition) > 0.1f)
        {
            if (!isSkillActive) { if (rb != null) rb.velocity = Vector2.zero; yield break; }
            Vector2 direction = (targetPosition - rb.position).normalized;
             if (rb != null) rb.velocity = direction * speed;
            yield return null;
        }
        if (rb != null)
        {
            rb.velocity = Vector2.zero; // 목표 도달 시 정지
            rb.position = targetPosition; // 정확한 위치로 보정
        }
        moveCoroutine = null;
    }

    /// <summary>
    /// 스킬 종료 처리: 중력 복구, 상태 해제, AI 코어 알림, 쿨다운 시작.
    /// </summary>
    private void EndSkill()
    {
        if (!isSkillActive) return;
        Debug.Log("[Skill1] 스킬 종료 처리.");

        if (moveCoroutine != null) StopCoroutine(moveCoroutine);
        if (rb != null) // null 체크 추가
        {
            rb.velocity = Vector2.zero;
            rb.gravityScale = originalGravityScale; // 원래 중력으로 복구
        }
        if (currentAirVFXInstance != null) Destroy(currentAirVFXInstance);

        isSkillActive = false;
        //  수정: aiCore null 체크 추가
        if (aiCore != null) aiCore.NotifyActionEnd();
        lastSkillUseTime = Time.time;
    }

    /// <summary>
    /// 스킬 중단 처리 코루틴 (예: 타겟 소실 시).
    /// </summary>
    private IEnumerator AbortSkill()
    {
        if (moveCoroutine != null) StopCoroutine(moveCoroutine);

        //  수정: rb null 체크 추가
        if (rb != null && Vector2.Distance(rb.position, groundPosition) > 0.1f)
        {
            Debug.Log("[Skill1] 스킬 중단, 빠른 하강 시도.");
            rb.gravityScale = originalGravityScale;
        }

        EndSkill(); // 최종 정리
        yield break;
    }
    private Coroutine afterImageCoroutine = null;

    private IEnumerator LeaveAfterImage()
    {
        while (isSkillActive)
        {
            CreateAfterImage();
            yield return new WaitForSeconds(0.05f); // 간격 조정 가능
        }
    }

    private void CreateAfterImage()
    {
        GameObject afterImage = new GameObject("AfterImage_Skill1");
        SpriteRenderer sr = afterImage.AddComponent<SpriteRenderer>();
        SpriteRenderer original = GetComponent<SpriteRenderer>();
        sr.sprite = original.sprite;
        sr.color = new Color(1f, 0.2f, 0.2f, 0.7f); // 반투명 붉은 잔상
        sr.flipX = original.flipX;
        sr.sortingLayerName = original.sortingLayerName;
        sr.sortingOrder = original.sortingOrder - 1;
        afterImage.transform.position = transform.position;
        afterImage.transform.localScale = transform.localScale;
        StartCoroutine(FadeOutAndDestroy(sr));
    }

    private IEnumerator FadeOutAndDestroy(SpriteRenderer sr)
    {
        float duration = 0.5f;
        float elapsed = 0f;
        Color startColor = sr.color;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float alpha = Mathf.Lerp(startColor.a, 0, elapsed / duration);
            sr.color = new Color(startColor.r, startColor.g, startColor.b, alpha);
            yield return null;
        }

        if (sr != null && sr.gameObject != null)
            Destroy(sr.gameObject);
    }
    public float GetLastSkillUseTime()
    {
        return lastSkillUseTime;
    }
    #endregion

    #region 외부 접근용 프로퍼티
    /// <summary> 현재 이 스킬이 활성화 상태인지 여부 </summary>
    public bool IsSkillActive => isSkillActive;
    #endregion
}