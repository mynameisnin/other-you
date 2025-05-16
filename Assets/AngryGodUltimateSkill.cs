using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(AngryGodAiCore))]
public class AngryGodUltimateSkill : MonoBehaviour
{
    public bool IsUltimateActive { get; private set; }
    private float lastUsedTime = -999f;

    [Header("Ultimate Skill Settings")]
    [Tooltip("궁극기 사용 후 다음 사용까지의 최소 쿨타임")]
    [SerializeField] public float cooldown = 20f;
    [Tooltip("궁극기 애니메이션의 총 예상 시간 (선딜레이 + 실제 발동 + 후딜레이 포함)")] // 설명 변경
    [SerializeField] private float totalAnimationTime = 4.0f; // 예: 선딜, 발동, 후딜을 모두 포함한 애니메이션 총 길이

    // 컴포넌트 참조
    private Animator animator;
    private Rigidbody2D rb;
    private AngryGodAiCore aiCore;
    private float originalGravityScale; // 원래 중력값 저장용

    void Awake()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        aiCore = GetComponent<AngryGodAiCore>();

        if (animator == null) Debug.LogError("Animator component missing on " + gameObject.name);
        if (rb == null) Debug.LogError("Rigidbody2D component missing on " + gameObject.name);
        if (aiCore == null) Debug.LogError("AngryGodAiCore component missing on " + gameObject.name);
    }

    void Start()
    {
        if (rb != null)
        {
            originalGravityScale = rb.gravityScale;
        }
        lastUsedTime = -cooldown; // 게임 시작 시 바로 사용할 수 있도록
    }

    /// <summary>
    /// 궁극기 시도를 시작하는 코루틴.
    /// AngryGodAiCore 등 외부에서 호출됩니다.
    /// </summary>
    public IEnumerator TryStartUltimate()
    {
        // 1. 이미 사용 중이거나 쿨타임이 안 됐으면 중단
        if (IsUltimateActive || Time.time < lastUsedTime + cooldown)
        {
            Debug.Log($"[UltimateSkill] TryStartUltimate 즉시 중단. IsActive: {IsUltimateActive}, CooldownLeft: {(lastUsedTime + cooldown) - Time.time}");
            yield break;
        }

        Debug.Log("[UltimateSkill] TryStartUltimate 조건 통과, 시퀀스 시작 준비.");
        IsUltimateActive = true;
        aiCore.NotifyActionStart(); // AI Core에게 행동 시작 알림
        Debug.Log("[UltimateSkill] IsUltimateActive=true, NotifyActionStart() 호출됨.");

        // 2. 즉시 이동 정지 및 방향 고정
        if (rb != null)
        {
            rb.velocity = Vector2.zero;
            rb.gravityScale = 0f; // 궁극기 중 중력 영향 제거
            Debug.Log("[UltimateSkill] Rigidbody 속도/중력 조절됨.");
        }
        if (aiCore.GetPlayer() != null) // aiCore에 GetPlayer()가 있어야 함
        {
            aiCore.ForceFlipTowardsTarget(); // 플레이어 방향으로 즉시 전환
            Debug.Log("[UltimateSkill] 플레이어 방향으로 전환됨.");
        }

        // 3. "Ultimate" Trigger 하나로 전체 궁극기 애니메이션 시작
        animator.SetTrigger("Ultimate"); // ★★★ "Ultimate" Trigger 사용 ★★★
        Debug.Log($"[UltimateSkill] 'Ultimate' 트리거 발동. 총 애니메이션 시간 ({totalAnimationTime}초) 시작.");

        // --- 실제 궁극기 공격/효과 발동 타이밍 제어 ---
        // 방법 A: 애니메이션 이벤트 사용 (권장)
        //   - "Ultimate" 애니메이션의 특정 프레임(예: 실제 타격/효과 발생하는 시점)에
        //     애니메이션 이벤트를 추가하고, 그 이벤트가 아래와 같은 함수를 호출하도록 설정합니다.
        //     public void AnimEvent_ExecuteUltimateEffect() { /* 실제 공격 로직 */ }

        // 방법 B: 시간 기반으로 코루틴 내에서 효과 발동 (덜 정확할 수 있음)
        //   - 만약 애니메이션 이벤트 사용이 어렵다면, 여기서 특정 시간(예: 선딜레이 시간) 후
        //     공격 로직을 실행하는 코루틴을 또 시작할 수 있습니다.
        //     StartCoroutine(PerformUltimateAttackLogicWithDelay(선딜레이시간));

        // TODO: 위에 설명된 방법 중 하나를 선택하여 실제 궁극기 공격 로직을 구현하세요.
        // 예시 (시간 기반, 실제로는 AnimEvent_ExecuteUltimateEffect()가 더 좋음):
        // yield return new WaitForSeconds(선딜레이시간_애니메이션에_맞춰서);
        // if (IsUltimateActive) // 아직 스킬이 유효하다면
        // {
        //     Debug.Log("[UltimateSkill] 실제 공격/효과 발동!");
        //     // 여기에 실제 데미지 판정, 이펙트 생성 등 로직
        // }


        // 4. 궁극기 애니메이션의 총 시간만큼 대기
        //    이 시간 동안 애니메이션이 선딜레이, 실제 발동, 후딜레이 등을 모두 포함하여 재생된다고 가정합니다.
        yield return new WaitForSeconds(totalAnimationTime);
        Debug.Log($"[UltimateSkill] Total animation time ({totalAnimationTime}초) 종료됨.");

        // 5. 궁극기가 중간에 취소되지 않았는지 확인 (예: 보스 사망)
        //    totalAnimationTime 대기 중에 어떤 이유로든 IsUltimateActive가 false가 되었다면 (AbortUltimate 호출 등)
        if (!IsUltimateActive)
        {
            Debug.Log("[UltimateSkill] Ultimate 중 취소됨 (IsUltimateActive가 false). 추가 종료 처리 불필요 (이미 AbortUltimate에서 처리됨).");
            // AbortUltimate에서 이미 NotifyActionEnd 등을 호출했을 것이므로 여기서는 추가 작업이 없을 수 있습니다.
            // 단, AbortUltimate가 호출되지 않았는데 IsUltimateActive가 false가 된 예외적인 상황을 대비할 수는 있습니다.
            if (rb != null && rb.gravityScale == 0f) rb.gravityScale = originalGravityScale; // 중력 복구 확인
            if (aiCore != null && !IsUltimateActive) aiCore.NotifyActionEnd(); // 만약을 위해 한번 더
            yield break;
        }

        // 6. 정상적인 궁극기 종료 처리
        yield return FinishUltimate();
    }

    /// <summary>
    /// 애니메이션 이벤트 또는 시간 기반으로 실제 궁극기 효과를 발동할 때 호출될 수 있는 함수 (예시)
    /// </summary>
    public void ExecuteUltimateEffect() // 애니메이션 이벤트에서 호출되도록 이름 변경 가능
    {
        if (!IsUltimateActive) return; // 이미 취소되었거나 끝났으면 실행 안함
        Debug.Log("[UltimateSkill] >>> 실제 궁극기 효과 발동! <<<");
        // 여기에 데미지 판정, 이펙트 생성 등 실제 궁극기 로직을 구현합니다.
    }


    private IEnumerator FinishUltimate()
    {
        Debug.Log("[UltimateSkill] Finishing ultimate (정상 종료).");
        if (rb != null)
        {
            rb.gravityScale = originalGravityScale; // 원래 중력으로 복원
        }
        IsUltimateActive = false;
        lastUsedTime = Time.time; // 성공적으로 끝났으므로 쿨타임 시작
        aiCore.NotifyActionEnd(); // AI Core에게 행동 끝났음을 알림
        yield break;
    }

    /// <summary>
    /// 어떤 이유로든 궁극기가 중간에 중단되어야 할 때 호출 (예: 보스 사망)
    /// </summary>
    public IEnumerator AbortUltimate()
    {
        Debug.Log("[UltimateSkill] Aborting ultimate (강제 중단).");
        // 진행 중이던 모든 관련 코루틴 중지 (이 스크립트 내에서 추가로 시작한 코루틴이 있다면)
        // StopAllCoroutines(); // 현재 구조에서는 TryStartUltimate 하나만 실행되므로, 이 라인이 반드시 필요하진 않음.
        // 만약 TryStartUltimate 내에서 다른 코루틴을 시작한다면 고려.

        if (rb != null)
        {
            rb.gravityScale = originalGravityScale; // 중력 복원 시도
        }

        // 이미 false일 수 있지만, 확실하게 하기 위해.
        // 그리고 다른 곳에서 IsUltimateActive 상태를 보고 즉시 중단할 수 있도록.
        IsUltimateActive = false;

        // lastUsedTime은 업데이트하지 않음 (성공적으로 끝난 것이 아니므로 쿨타임 적용 안 함 또는 다른 정책)
        // 또는, 중단되어도 쿨타임을 적용하고 싶다면 lastUsedTime = Time.time; 추가

        aiCore.NotifyActionEnd(); // AI Core에게 행동 끝났음을 알림
        yield break;
    }

    public float GetLastUseTime() => lastUsedTime;
}