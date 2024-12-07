using System.Collections;
using UnityEngine;

public class CharacterAttack : MonoBehaviour
{
    private Animator animator;
    private int currentComboStep = 0; // ���� �޺� �ܰ�
    private float comboTimer = 0f; // �޺� Ÿ�̸�
    private float comboMaxTime = 0.008f; // �޺� �Է� ���� �ð�
    private bool isAttacking = false; // ���� ���� ������ ����

    // ���� ���¸� �ٸ� ��ũ��Ʈ���� �б� ���� ������Ƽ
    public bool IsAttacking => isAttacking;

    void Start()
    {
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        HandleComboInput();
        UpdateComboTimer();
    }

    private void HandleComboInput()
    {
        // ���� ��ư �Է� ����
        if (Input.GetKey(KeyCode.M))
        {
            TriggerAttack(); // ���� Ʈ���� ȣ��
        }
    }

    public void TriggerAttack()
    {
        // �޺� �Է��� ���� Ʈ����
        if (isAttacking)
        {
            ContinueCombo(); // �޺� ����
        }
        else
        {
            StartCombo(); // ù ����
        }
    }

    private void StartCombo()
    {
        isAttacking = true;
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
        else if (currentComboStep == 2)
        {
            currentComboStep = 3; // �� ��° ���� �ܰ�
            animator.SetTrigger("Attack3"); // �� ��° ���� �ִϸ��̼� ����
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
        animator.ResetTrigger("Attack3");
    }

    // �ִϸ��̼� �̺�Ʈ: ���� ���� �� ȣ��
    public void EndAttack()
    {
        isAttacking = false; // ���� ����
    }
}
