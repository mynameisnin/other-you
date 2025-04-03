using System.Collections;
using UnityEngine;

public class BladeExhaustSkill : MonoBehaviour
{
    [Header("��ų ����")]
    public AdamMovement movement;
    public Animator animator;
    public Rigidbody2D rb;
    public SpriteRenderer sprite;

    [Header("������ ����")]
    public int damage = 35;
    public string targetTag = "Enemy";
    public bool fromAdam = true;
    public bool fromDeba = false;

    [Header("���� �Ҹ� ����")]
    public int manaCost = 20;

    [Header("��Ÿ�� ����")]
    public float cooldown = 4f; // ��Ÿ�� ���� �ð�
    private bool isOnCooldown = false;

    [Header("UI ����")]
    public SkillCooldownUI skillCooldownUI; // �� SkillCooldownUI ����

    public bool isSlashing = false;

    private void OnTriggerEnter2D(Collider2D other)
    {
        enemyTest enemy = other.GetComponent<enemyTest>();
        if (enemy != null)
        {
            Debug.Log("Hit enemy with Blade Slash!");
            enemy.TakeDamage(damage, fromAdam, fromDeba, transform.root);
        }
    }

    public void StartBladeSlash()
    {
        if (movement == null || animator == null) return;

        if (isOnCooldown)
        {
            Debug.Log("Blade Slash is on cooldown!");
            return;
        }

        if (!PlayerStats.Instance.HasEnoughMana(manaCost))
        {
            Debug.Log("Not enough mana to use Blade Slash!");
            ManaBarUI.Instance?.FlashBorder(); // ���� ���� �� ���
            return;
        }

        PlayerStats.Instance.ReduceMana(manaCost);

        StartCoroutine(SlashCoroutine());
        StartCoroutine(CooldownRoutine());

        //  UI�� ��Ÿ�� �˸�
        if (skillCooldownUI != null)
            skillCooldownUI.StartCooldown();
    }

    private IEnumerator SlashCoroutine()
    {
        isSlashing = true;
        movement.StopMovement();
        movement.isInvincible = true;

        animator.Play("AdamSlash", 0, 0);

        rb.velocity = Vector2.zero;

        // ��Ȯ�� ���� �ð� ����
        yield return new WaitForSeconds(0.7f); // �� �� �ð� ������ ������ ������ ���� ����

        isSlashing = false;
        movement.isInvincible = false;
    }

    private IEnumerator CooldownRoutine()
    {
        isOnCooldown = true;
        yield return new WaitForSeconds(cooldown);
        isOnCooldown = false;
    }
    public Collider2D bladeCollider; // �� �ν����Ϳ��� ����

    public void ResetSkillState()
    {
        isOnCooldown = false;
        isSlashing = false;

        StopAllCoroutines();

        if (movement != null)
            movement.isInvincible = false;

        if (bladeCollider != null)
            bladeCollider.enabled = false; //  �ݶ��̴� ���� ����

        Debug.Log("BladeExhaustSkill ���� + �ݶ��̴� �ʱ�ȭ �Ϸ�");
    }

}
