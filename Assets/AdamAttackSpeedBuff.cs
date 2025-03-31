using System.Collections;
using UnityEngine;

public class AdamAttackSpeedBuff : MonoBehaviour
{
    [Header("Buff Settings")]
    public float attackSpeedMultiplier = 1.5f; // 1.5�� ������
    public float buffDuration = 5f;
    public int manaCost = 20;

    [Header("Effect")]
    public GameObject buffEffect;

    private bool isBuffActive = false;
    private Coroutine buffCoroutine;

    private AdamMovement adamMovement;
    private Animator adamAnimator;
    public SkillCooldownUI skillCooldownUI; // �ν����� ����

    // ������ ����
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
            Debug.Log($"[��Ÿ�� ���� ��] ���� �ð�: {remaining:F2}��");

        if (Input.GetKeyDown(KeyCode.Q))
        {
            TryActivateBuff();
        }
    }

    public void TryActivateBuff()
    {
        if (isBuffActive)
        {
            Debug.Log("���� �̹� Ȱ��ȭ ��");
            return;
        }

        // ? ��Ÿ�� Ȯ�� (Time.time ����)
        if (Time.time < cooldownEndTime)
        {
            Debug.Log("��Ÿ�� ���Դϴ�!");
            return;
        }

        if (!PlayerStats.Instance.HasEnoughMana(manaCost))
        {
            Debug.Log("���� ����");
            ManaBarUI.Instance?.FlashBorder();
            return;
        }

        PlayerStats.Instance.ReduceMana(manaCost);

        // ? ��Ÿ�� ���� �ð� ����
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

        // ���� �� ����
        originalAttackDelay = adamMovement.attackInputCooldown;
        originalAnimatorSpeed = adamAnimator.speed;

        // ���� ����
        adamMovement.attackInputCooldown *= 1f / attackSpeedMultiplier;
        adamAnimator.speed = attackSpeedMultiplier;

        if (buffEffect != null)
            buffEffect.SetActive(true);

        Debug.Log("?? ���� �ӵ� + �ִϸ��̼� �ӵ� ���� ����!");

        yield return new WaitForSeconds(buffDuration);

        ResetSkillState();
    }

    /// <summary>
    /// ����Ī �� �ܺο��� ȣ���Ͽ� ��ų ���¸� �ʱ�ȭ
    /// </summary>
    public void ResetSkillState()
    {
        isBuffActive = false;

        // ���� �ӵ��� ����
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

        Debug.Log(" Adam ���ݼӵ� ���� ���� �ʱ�ȭ��");
    }
}
