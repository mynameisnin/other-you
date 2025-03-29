using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DevaBigLaserSkill : MonoBehaviour
{
    [Header("�ڿ� �� ��Ÿ��")]
    public int manaCost = 40;
    public float cooldown = 6f;
    private float cooldownEndTime = 0f;

    [Header("�ִϸ��̼�")]
    public Animator animator;
    public string laserAnimName = "Cast2";  // �� �ִϸ��̼� �̸� ������

    [Header("UI")]
    public SkillCooldownUI cooldownUI;
    [Header("����")]
    public DebaraMovement devaMovement; // �� �ν����Ϳ��� ������ ��

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.C))
        {
            TryCastLaser();
        }
    }

    public void TryCastLaser()
    {
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

        // ���� ����
        DevaStats.Instance.ReduceMana(manaCost);

        // ��Ÿ�� ����
        cooldownEndTime = Time.time + cooldown;

        // ��Ÿ�� UI ����
        cooldownUI?.StartCooldown();

        //  �̵� �� �ൿ ����
        if (devaMovement != null)
        {
            devaMovement.StopMovement();           // �ӵ� ����
            devaMovement.isAttacking = true;       // �̵� �� ���� ����
        }

        // �ִϸ��̼� ����
        animator.SetTrigger("Trigger_Cast12");

        Debug.Log(" [Deva] ���� ������ Cast2 ����!");
    }

    public void EndBigLaser()
    {
        if (devaMovement != null)
        {
            devaMovement.EndAttack();  // �̵� �� ���� �����ϰ� �ٽ� Ǯ����
        }
    }

    // UI���� ��Ÿ�� ���ٿ� ������Ƽ
    public float CooldownRemaining => Mathf.Max(0, cooldownEndTime - Time.time);
    public float CooldownDuration => cooldown;
}