using System.Collections;
using UnityEngine;

public class CharacterAttack : MonoBehaviour
{
    private Animator animator;
    private int currentComboStep = 0; // ���� �޺� �ܰ�
    private float comboTimer = 0f; // �޺� Ÿ�̸�
    private float comboMaxTime = 0.03f; // �޺� �Է� ���� �ð�
    private bool isAttacking = false; // ���� ���� ������ ����
    private bool energyConsumed = false;
    public EnergyBarUI energyBarUI; //  ������ UI �߰�
    public float attackEnergyCost = 10f; // ���� �� �Һ�Ǵ� ������ ��

    // ���� ���¸� �ٸ� ��ũ��Ʈ���� �б� ���� ������Ƽ
    public bool IsAttacking => isAttacking;

    void Start()
    {
        animator = GetComponent<Animator>();
        //  EnergyBarUI�� ������� �ʾ��� ��� ���
        if (energyBarUI == null)
        {
            Debug.LogError("[CharacterAttack] EnergyBarUI�� ������� ����!");
        }
    }

    void Update()
    {
        HandleComboInput();
        UpdateComboTimer();
    }

    private void HandleComboInput()
    {
        // ���� ��ư �Է� ����
        if (Input.GetKeyDown(KeyCode.M)) //  KeyDown���� �����Ͽ� ���� �Է� ����
        {
            TriggerAttack(); // ���� Ʈ���� ȣ��
        }
    }

    public void TriggerAttack()
    {
        if (energyBarUI == null)
        {
            Debug.LogError("EnergyBarUI�� ������� ����!");
            return;
        }

        float currentEnergy = energyBarUI.GetCurrentEnergy(); // ���� ������ ��������

        if (currentEnergy <= 0)
        {
            Debug.Log("���� �Ұ�: ENERGY ����!");
            energyBarUI.FlashBorder();
            return;
        }

        if (isAttacking)
        {
            ContinueCombo();
        }
        else
        {
            StartCombo();
        }
       
        //  �ִϸ��̼� ���� �� M Ű�� ������ �������� �����ϵ��� ����
        StartCoroutine(ConsumeEnergyAfterAnimation());
    }

    private void StartCombo()
    {
        isAttacking = true;
        energyConsumed = false; //  ���� ���� �� ������ ���� ���� �ʱ�ȭ
        currentComboStep = 1; // ù ��° ���� �ܰ�
        comboTimer = comboMaxTime; // �޺� Ÿ�̸� �ʱ�ȭ
        animator.SetTrigger("Attack1"); // ù ��° ���� �ִϸ��̼� ����
    }

    private void ContinueCombo()
    {
        if (currentComboStep == 1)
        {
            currentComboStep = 2; // �� ��° ���� �ܰ�
            animator.SetTrigger("Attack2"); // �� ��° ���� �ִϸ��̼� ����
        }

        comboTimer = comboMaxTime; // �޺� Ÿ�̸� ����
    }

    private void UpdateComboTimer()
    {
        if (isAttacking)
        {
            comboTimer -= Time.deltaTime;

            // �޺� Ÿ�̸Ӱ� ����Ǹ� �ʱ�ȭ
            if (comboTimer <= 0)
            {
                ResetCombo();
            }
        }
    }

    private void ResetCombo()
    {
        isAttacking = false;
        currentComboStep = 0;
        comboTimer = 0f;
        animator.ResetTrigger("Attack1");
        animator.ResetTrigger("Attack2");
    }
    private IEnumerator ConsumeEnergyAfterAnimation()
    {
        yield return new WaitUntil(() => animator.GetCurrentAnimatorStateInfo(0).IsName("Attack1") || animator.GetCurrentAnimatorStateInfo(0).IsName("Attack2"));

        if (!energyConsumed && energyBarUI != null) // ? �������� �ߺ� ���� ����
        {
            float currentEnergy = energyBarUI.GetCurrentEnergy();
            float energyToConsume = Mathf.Min(attackEnergyCost, currentEnergy);
            energyBarUI.ReduceEnergy(energyToConsume);
            energyConsumed = true; // ? �� ���� �����ǵ��� ����
        }
    }
    // �ִϸ��̼� �̺�Ʈ: ���� ���� �� ȣ��
    public void EndAttack()
    {
        isAttacking = false; // ���� ����
    }
}
