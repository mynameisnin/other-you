using System.Collections;
using UnityEngine;

public class DevaContinuousSkill : MonoBehaviour
{
    [Header("Skill Settings")]
    public float skillDuration = 5f;
    public int manaCost = 20;

    [Header("Cooldown")]
    public float cooldownDuration = 10f;
    private float cooldownEndTime = 0f;

    [Header("Effect & Animation")]
    public GameObject skillEffect;
    public Animator animator;
    public string skillAnimTrigger = "Trigger_SkillLoop";  // 시작 트리거
    public string endAnimTrigger = "Trigger_SkillEnd";      // 종료 트리거

    [Header("UI")]
    public SkillCooldownUI cooldownUI;

    private Coroutine skillCoroutine;
    private bool isUsingSkill = false;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Z))
        {
            TryUseSkill();
        }
    }

    public void TryUseSkill()
    {
        if (isUsingSkill)
        {
            Debug.Log(" 이미 스킬 사용 중");
            return;
        }

        if (Time.time < cooldownEndTime)
        {
            Debug.Log(" 쿨타임 중");
            return;
        }

        if (!DevaStats.Instance.HasEnoughMana(manaCost))
        {
            Debug.Log(" 마나 부족");
            DevaManaBarUI.Instance?.FlashBorder();
            return;
        }

        DevaStats.Instance.ReduceMana(manaCost);
        cooldownEndTime = Time.time + cooldownDuration;

        if (cooldownUI != null)
            cooldownUI.StartCooldown();

        skillCoroutine = StartCoroutine(SkillRoutine());
    }

    private IEnumerator SkillRoutine()
    {
        isUsingSkill = true;

        // 이펙트 활성화 (이 안에 콜라이더와 데미지 처리 있음)
        if (skillEffect != null)
            skillEffect.SetActive(true);

        if (animator != null && animator.runtimeAnimatorController != null)
            animator.SetTrigger(skillAnimTrigger);

        yield return new WaitForSeconds(skillDuration);

        if (animator != null && animator.runtimeAnimatorController != null)
            animator.SetTrigger(endAnimTrigger);

        if (skillEffect != null)
            skillEffect.SetActive(false);

        isUsingSkill = false;
    }

    public void ResetSkillState()
    {
        if (skillCoroutine != null)
        {
            StopCoroutine(skillCoroutine);
            skillCoroutine = null;
        }

        isUsingSkill = false;

        if (skillEffect != null)
            skillEffect.SetActive(false);

        Debug.Log(" 지속 스킬 상태 초기화 완료");
    }
}
