using System.Collections;
using UnityEngine;

public class AdamAttackSpeedBuff : MonoBehaviour
{
    [Header("Buff Settings")]
    public float attackSpeedMultiplier = 1.5f; // 1.5배 빠르게
    public float buffDuration = 5f;
    public int manaCost = 20;

    [Header("Effect")]
    public GameObject buffEffect;

    private bool isBuffActive = false;
    private Coroutine buffCoroutine;

    private AdamMovement adamMovement;
    private Animator adamAnimator;
    public SkillCooldownUI skillCooldownUI; // 인스펙터 연결

    // 복원용 저장
    private float originalAttackDelay;
    private float originalAnimatorSpeed;
    [Header("Cooldown Settings")]
    public float cooldownDuration = 5f;

    private float cooldownEndTime = 0f;
    void Start()
    {
        adamMovement = GetComponent<AdamMovement>();
        adamAnimator = GetComponent<Animator>();
        originalAttackDelay = adamMovement.attackInputCooldown;
        originalAnimatorSpeed = adamAnimator.speed;
    }

    void Update()
    {
        float remaining = cooldownEndTime - Time.time;

        if (remaining > 0f)
            Debug.Log($"[쿨타임 진행 중] 남은 시간: {remaining:F2}초");

        if (Input.GetKeyDown(KeyCode.Q))
        {
            TryActivateBuff();
        }
    }

    public void TryActivateBuff()
    {
        if (isBuffActive)
        {
            Debug.Log("버프 이미 활성화 중");
            return;
        }

        // ? 쿨타임 확인 (Time.time 기준)
        if (Time.time < cooldownEndTime)
        {
            Debug.Log("쿨타임 중입니다!");
            return;
        }

        if (!PlayerStats.Instance.HasEnoughMana(manaCost))
        {
            Debug.Log("마나 부족");
            ManaBarUI.Instance?.FlashBorder();
            return;
        }

        PlayerStats.Instance.ReduceMana(manaCost);

        // ? 쿨타임 종료 시간 설정
        cooldownEndTime = Time.time + cooldownDuration;

        if (skillCooldownUI != null)
            skillCooldownUI.StartCooldown();

        if (buffCoroutine != null)
            StopCoroutine(buffCoroutine);

        buffCoroutine = StartCoroutine(ApplyBuff());
    }



    private IEnumerator ApplyBuff()
    {
        isBuffActive = true;

        // 기존 값 저장
        originalAttackDelay = adamMovement.attackInputCooldown;
        originalAnimatorSpeed = adamAnimator.speed;

        // 버프 적용
        adamMovement.attackInputCooldown *= 1f / attackSpeedMultiplier;
        adamAnimator.speed = attackSpeedMultiplier;

        if (buffEffect != null)
            buffEffect.SetActive(true);

        Debug.Log("?? 공격 속도 + 애니메이션 속도 버프 적용!");

        yield return new WaitForSeconds(buffDuration);

        ResetSkillState();
    }

    /// <summary>
    /// 스위칭 시 외부에서 호출하여 스킬 상태를 초기화
    /// </summary>
    public void ResetSkillState()
    {
        isBuffActive = false;

        // 원래 속도로 복원
        if (adamMovement != null)
            adamMovement.attackInputCooldown = originalAttackDelay;

        if (adamAnimator != null)
            adamAnimator.speed = originalAnimatorSpeed;

        if (buffEffect != null)
            buffEffect.SetActive(false);

        if (buffCoroutine != null)
        {
            StopCoroutine(buffCoroutine);
            buffCoroutine = null;
        }

        Debug.Log(" Adam 공격속도 버프 상태 초기화됨");
    }
}
