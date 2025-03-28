using System.Collections;
using UnityEngine;

public class AdamAttackSpeedBuff : MonoBehaviour
{
    [Header("Buff Settings")]
    public float attackSpeedMultiplier = 1.5f;
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

        // ? Animator �Ķ���� �ʱ�ȭ (�߿�!)
        if (adamAnimator != null)
        {
            adamAnimator.SetFloat("attackSpeedMultiplier", 1f);
        }
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

        // ����
        adamMovement.attackInputCooldown *= 1f / attackSpeedMultiplier;
        adamAnimator.SetFloat("attackSpeedMultiplier", attackSpeedMultiplier); // ? �ִϸ����� �Ķ���ͷ� ����

        if (buffEffect != null)
            buffEffect.SetActive(true);

        Debug.Log("?? ���� �ӵ� + �ִϸ��̼� �ӵ� ���� ����!");

        yield return new WaitForSeconds(buffDuration);

        // ����
        adamMovement.attackInputCooldown = originalAttackDelay;
        adamAnimator.SetFloat("attackSpeedMultiplier", 1f);

        if (buffEffect != null)
            buffEffect.SetActive(false);

        isBuffActive = false;
        Debug.Log("? ���� �ӵ� ���� ����");
    }
}
