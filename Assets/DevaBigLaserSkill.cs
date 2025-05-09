using System.Collections;
using UnityEngine;

public class DevaBigLaserSkill : MonoBehaviour
{
    [Header("�ڿ� �� ��Ÿ��")]
    public int manaCost = 40;
    public float cooldown = 6f;
    private float cooldownEndTime = 0f;

    [Header("�ִϸ��̼�")]
    public Animator animator;
    public string laserAnimName = "Cast2";  // �� �ִϸ��̼� �̸�

    [Header("UI")]
    public SkillCooldownUI cooldownUI;

    [Header("����")]
    public DebaraMovement devaMovement; // �� ������ ���� ó��

    [Header("������ ����")]
    public int damage = 50;
    public string targetTag = "Enemy";
    public bool fromAdam = false;
    public bool fromDeba = true;

    [Header("�ݶ��̴� ����")]
    public Collider2D laserCollider; // �� �ν����Ϳ� ������ ��

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
        cooldownUI?.StartCooldown();

        // �̵� ����
        if (devaMovement != null)
        {
            devaMovement.StopMovement();
            devaMovement.isAttacking = true;
        }

        // �ݶ��̴� ��Ȱ��ȭ (�ִϸ��̼ǿ��� Enable�� ����)
        if (laserCollider != null)
            laserCollider.enabled = false;

        // �ִϸ��̼� ����
        animator.SetTrigger("Trigger_Cast12");

        Debug.Log(" Deva ������ ��ų Cast2 ����!");
    }

    //  �ִϸ��̼� �̺�Ʈ���� ȣ���
    public void EnableLaserCollider()
    {
        if (laserCollider != null)
            laserCollider.enabled = true;
    }

    public void DisableLaserCollider()
    {
        if (laserCollider != null)
            laserCollider.enabled = false;
    }

    public void EndBigLaser()
    {
        if (devaMovement != null)
        {
            devaMovement.EndAttack();
        }

        if (laserCollider != null)
        {
            laserCollider.enabled = false;
        }

        Debug.Log(" ������ ��ų �����");
    }

    // ������ ó��
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (laserCollider == null || !laserCollider.enabled) return;

        // �Ϲ� ��
        enemyTest enemy = other.GetComponent<enemyTest>();
        if (enemy != null)
        {
            Debug.Log("Deva ������ ����! (��)");
            enemy.TakeDamage(damage, fromAdam, fromDeba, transform.root);
            return;
        }

        // ����
        BossHurt boss = other.GetComponent<BossHurt>();
        if (boss != null)
        {
            Debug.Log("Deva ������ ����! (����)");
            boss.TakeDamage(damage, fromAdam, fromDeba); // 3�� ����
            return;
        }
    }

    public float CooldownRemaining => Mathf.Max(0, cooldownEndTime - Time.time);
    public float CooldownDuration => cooldown;
}
