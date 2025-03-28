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

    void Start()
    {
        adamMovement = GetComponent<AdamMovement>();
        adamAnimator = GetComponent<Animator>();
    }

    void Update()
    {
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

        if (!PlayerStats.Instance.HasEnoughMana(manaCost))
        {
            Debug.Log("���� ����");
            ManaBarUI.Instance?.FlashBorder();
            return;
        }

        PlayerStats.Instance.ReduceMana(manaCost);

        if (buffCoroutine != null)
            StopCoroutine(buffCoroutine);

        buffCoroutine = StartCoroutine(ApplyBuff());
    }

    private IEnumerator ApplyBuff()
    {
        isBuffActive = true;

        // ���� �� ����
        float originalAttackDelay = adamMovement.attackInputCooldown;
        float originalAnimatorSpeed = adamAnimator.speed;

        // ���� ����
        adamMovement.attackInputCooldown *= 1f / attackSpeedMultiplier; // �Է� ���� ���̱�
        adamAnimator.speed = attackSpeedMultiplier; // �ִϸ��̼� �ӵ� ����

        if (buffEffect != null)
            buffEffect.SetActive(true);

        Debug.Log("?? ���� �ӵ� + �ִϸ��̼� �ӵ� ���� ����!");

        yield return new WaitForSeconds(buffDuration);

        // ����
        adamMovement.attackInputCooldown = originalAttackDelay;
        adamAnimator.speed = originalAnimatorSpeed;

        if (buffEffect != null)
            buffEffect.SetActive(false);

        isBuffActive = false;
        Debug.Log("? ���� �ӵ� ���� ����");
    }
}
