using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class HurtDeva : MonoBehaviour
{
    private Animator animator;
    public GameObject[] bloodEffectPrefabs;
    public GameObject parringEffects;
    public ParticleSystem bloodEffectParticle;

    public CameraShakeSystem cameraShake;
    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;
    public int MaxHealth = 100;
    public int currentHealth;

    public float knockbackForce = 5f;
    private bool isParrying = false;

    [Header("Hit Effect Position")]
    public Transform pos;

    public DevaHealthBarUI healthBarUI;
    public CharStateGUIEffect charStateGUIEffect;
    private bool isDead = false;

    [Header("Death Effect Elements")]
    public SpriteRenderer deathBackground;

    public static HurtDeva Instance;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }

    void Start()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        FindCameraShake();
        FindDeathBackground();

        currentHealth = MaxHealth;
        if (healthBarUI != null)
            healthBarUI.Initialize(MaxHealth);

        if (deathBackground != null)
        {
            Color startColor = deathBackground.color;
            startColor.a = 0f;
            deathBackground.color = startColor;
        }
    }

    void Update()
    {
        if (cameraShake == null) FindCameraShake();
        if (deathBackground == null) FindDeathBackground();
    }

    void FindDeathBackground()
    {
        GameObject backgroundObj = GameObject.Find("DeathBackground");
        if (backgroundObj != null)
            deathBackground = backgroundObj.GetComponent<SpriteRenderer>();
    }

    void FindCameraShake()
    {
        cameraShake = Camera.main != null ? Camera.main.GetComponent<CameraShakeSystem>() : null;
    }

    public void ShowBloodEffect()
    {
        if (bloodEffectPrefabs.Length > 0)
        {
            int index = Random.Range(0, bloodEffectPrefabs.Length);
            GameObject effect = Instantiate(bloodEffectPrefabs[index], pos.position, Quaternion.identity);
            Destroy(effect, 0.3f);

            if (bloodEffectParticle != null)
            {
                ParticleSystem particle = Instantiate(bloodEffectParticle, pos.position, Quaternion.identity);
                particle.Play();
                Destroy(particle.gameObject, particle.main.duration + 0.5f);
            }
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (isParrying || isDead) return;

        EnemyMovement enemy = other.GetComponentInParent<EnemyMovement>();
        Arrow arrow = other.GetComponent<Arrow>();

        if (other.CompareTag("EnemyAttack") || other.CompareTag("damageAmount"))
        {
            DebaraMovement movement = GetComponent<DebaraMovement>();
            if (movement != null && movement.isInvincible)
            {
                return;
            }

            EnemyDamageBumpAgainst bump = other.GetComponent<EnemyDamageBumpAgainst>();
            if (bump != null) bump.TriggerDamageCooldown(0.5f);

            int damage = 0;
            if (enemy != null) damage = enemy.GetDamage();
            else if (arrow != null) damage = arrow.damage;

            TakeDamage(damage);
            animator.Play("Hurt", 0, 0f);
            ShowBloodEffect();
            Knockback(other.transform);

            if (cameraShake != null)
                StartCoroutine(cameraShake.Shake(0.15f, 0.15f));
        }
    }

    public void TakeDamage(int damage)
    {
        if (isDead) return;

        currentHealth -= damage;
        currentHealth = Mathf.Clamp(currentHealth, 0, MaxHealth);
        // 공격 상태 해제
        DebaraMovement movement = GetComponent<DebaraMovement>();
        if (movement != null) movement.ForceEndAttack();
        if (healthBarUI != null)
            healthBarUI.UpdateHealthBar(currentHealth, true);

        if (charStateGUIEffect != null)
            charStateGUIEffect.TriggerHitEffect();

        if (currentHealth <= 0)
            Die();
    }

    public void UpdateHealthUI()
    {
        if (healthBarUI != null)
            healthBarUI.UpdateHealthBar(currentHealth, true);
    }

    public void CancelDamage()
    {
        animator.ResetTrigger("Hurt");
    }

    public void StartParry()
    {
        isParrying = true;
        StartCoroutine(ResetParry());
    }

    IEnumerator ResetParry()
    {
        yield return new WaitForSeconds(0.1f);
        isParrying = false;
    }

    private void Knockback(Transform enemyTransform)
    {
        if (rb == null) return;

        float direction = transform.position.x - enemyTransform.position.x > 0 ? 1f : -1f;
        rb.velocity = new Vector2(knockbackForce * direction, rb.velocity.y + 1f);
    }

    private void Die()
    {
        if (isDead) return;
        isDead = true;

        DisableControls();

        if (rb != null)
        {
            rb.velocity = Vector2.zero;
            rb.bodyType = RigidbodyType2D.Kinematic;
            rb.simulated = false;
        }

        if (deathBackground != null)
        {
            deathBackground.DOFade(1f, 0.5f).OnComplete(() =>
            {
                animator.SetTrigger("Die");
                ChangeLayerOnDeath();
            });
        }
        else
        {
            animator.SetTrigger("Die");
            ChangeLayerOnDeath();
        }

        StartCoroutine(DisableAfterDeath());
    }

    private void DisableControls()
    {
        DebaraMovement movement = GetComponent<DebaraMovement>();
        if (movement != null) movement.enabled = false;
        {
            if (movement.isInvincible)
                return;

            movement.ForceEndAttack(); // <- 공격 상태 강제 종료
        }
        MagicAttack attack = GetComponent<MagicAttack>();
        if (attack != null) attack.enabled = false;
    }

    private void ChangeLayerOnDeath()
    {
        if (spriteRenderer != null)
        {
            spriteRenderer.sortingOrder = 11;
        }
    }

    private IEnumerator DisableAfterDeath()
    {
        yield return new WaitForSeconds(5f);
        gameObject.SetActive(false);
    }
}