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
    public string skillAnimTrigger = "Trigger_SkillLoop";  // ���� Ʈ����
    public string endAnimTrigger = "Trigger_SkillEnd";      // ���� Ʈ����

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
            Debug.Log(" �̹� ��ų ��� ��");
            return;
        }

        if (Time.time < cooldownEndTime)
        {
            Debug.Log(" ��Ÿ�� ��");
            return;
        }

        if (!DevaStats.Instance.HasEnoughMana(manaCost))
        {
            Debug.Log(" ���� ����");
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

        // ����Ʈ Ȱ��ȭ (�� �ȿ� �ݶ��̴��� ������ ó�� ����)
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

        Debug.Log(" ���� ��ų ���� �ʱ�ȭ �Ϸ�");
    }
}
