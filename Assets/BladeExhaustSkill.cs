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
    public float cooldown = 4f;   // ��Ÿ�� ���� �ð�
    private bool isOnCooldown = false;

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

            //  ���� ���� �� �׵θ� ȿ��
            if (ManaBarUI.Instance != null)
                ManaBarUI.Instance.FlashBorder();

            return;
        }

        PlayerStats.Instance.ReduceMana(manaCost);

        StartCoroutine(SlashCoroutine());
        StartCoroutine(CooldownRoutine()); // ��Ÿ�� ����
    }


    private IEnumerator SlashCoroutine()
    {
        isSlashing = true;
        movement.StopMovement();
        movement.isInvincible = true;

        animator.Play("AdamSlash", 0, 0);

        float duration = 3f;
        while (duration > 0f && animator.GetCurrentAnimatorStateInfo(0).IsName("AdamSlash"))
        {
            rb.velocity = Vector2.zero;
            duration -= Time.deltaTime;
            yield return null;
        }

        isSlashing = false;
        movement.isInvincible = false;
    }

    private IEnumerator CooldownRoutine()
    {
        isOnCooldown = true;
        yield return new WaitForSeconds(cooldown);
        isOnCooldown = false;
    }
}
