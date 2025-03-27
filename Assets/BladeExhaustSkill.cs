using System.Collections;
using UnityEngine;

public class BladeExhaustSkill : MonoBehaviour
{
    [Header("스킬 연결")]
    public AdamMovement movement; // 메인 캐릭터 연결
    public Animator animator;     // 캐릭터 애니메이터 연결
    public Rigidbody2D rb;        // 캐릭터 리지드바디 연결
    public SpriteRenderer sprite; // 캐릭터 방향 판단용

    [Header("데미지 관련")]
    public int damage = 35;
    public string targetTag = "Enemy";
    public bool fromAdam = true;
    public bool fromDeba = false;

    public bool isSlashing = false;

    private void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log("Triggered with: " + other.name); // 이 로그가 찍히는지 테스트
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
        StartCoroutine(SlashCoroutine());
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
}