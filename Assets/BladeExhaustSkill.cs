using System.Collections;
using UnityEngine;

public class BladeExhaustSkill : MonoBehaviour
{
    [Header("스킬 연결")]
    public AdamMovement movement;
    public Animator animator;
    public Rigidbody2D rb;
    public SpriteRenderer sprite;

    [Header("데미지 관련")]
    public int damage = 35;
    public string targetTag = "Enemy";
    public bool fromAdam = true;
    public bool fromDeba = false;

    [Header("마나 소모 설정")]
    public int manaCost = 20;

    [Header("쿨타임 설정")]
    public float cooldown = 4f; // 쿨타임 지속 시간
    private bool isOnCooldown = false;

    [Header("UI 연결")]
    public SkillCooldownUI skillCooldownUI; // ← SkillCooldownUI 연결

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
            ManaBarUI.Instance?.FlashBorder(); // 마나 부족 시 경고
            return;
        }

        PlayerStats.Instance.ReduceMana(manaCost);

        StartCoroutine(SlashCoroutine());
        StartCoroutine(CooldownRoutine());

        //  UI에 쿨타임 알림
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

        // 명확한 유지 시간 설정
        yield return new WaitForSeconds(0.7f); // ← 이 시간 동안은 무조건 슬래시 상태 유지

        isSlashing = false;
        movement.isInvincible = false;
    }

    private IEnumerator CooldownRoutine()
    {
        isOnCooldown = true;
        yield return new WaitForSeconds(cooldown);
        isOnCooldown = false;
    }
    public Collider2D bladeCollider; // ← 인스펙터에서 연결

    public void ResetSkillState()
    {
        isOnCooldown = false;
        isSlashing = false;

        StopAllCoroutines();

        if (movement != null)
            movement.isInvincible = false;

        if (bladeCollider != null)
            bladeCollider.enabled = false; //  콜라이더 강제 종료

        Debug.Log("BladeExhaustSkill 상태 + 콜라이더 초기화 완료");
    }

}
