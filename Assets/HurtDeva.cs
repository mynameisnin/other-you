using System.Collections;
using UnityEngine;
using DG.Tweening;

public class HurtDeva : MonoBehaviour
{
    private Animator animator;
    public GameObject[] bloodEffectPrefabs;
    public GameObject parringEffects;
    public ParticleSystem bloodEffectParticle;

    public CameraShakeSystem cameraShake;
    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;

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
    private int originalSortingOrder;

    void Awake()
    {
        if (Instance == null)
            Instance = this;
    }

    void Start()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        originalSortingOrder = spriteRenderer != null ? spriteRenderer.sortingOrder : 0;
        FindCameraShake();
        FindDeathBackground();

        DevaStats.Instance.currentHealth = DevaStats.Instance.maxHealth;
        if (healthBarUI != null)
            healthBarUI.Initialize(DevaStats.Instance.maxHealth);

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
            if (movement != null && movement.isInvincible) return;

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

        DevaStats.Instance.currentHealth -= damage;
        DevaStats.Instance.currentHealth = Mathf.Clamp(DevaStats.Instance.currentHealth, 0, DevaStats.Instance.maxHealth);

        DebaraMovement movement = GetComponent<DebaraMovement>();
        if (movement != null) movement.ForceEndAttack();

        if (healthBarUI != null)
            healthBarUI.UpdateHealthBar(DevaStats.Instance.currentHealth, true);

        if (charStateGUIEffect != null)
            charStateGUIEffect.TriggerHitEffect();

        if (DevaStats.Instance.currentHealth <= 0)
            Die();
    }

    public void UpdateHealthUI()
    {
        if (healthBarUI != null)
            healthBarUI.UpdateHealthBar(DevaStats.Instance.currentHealth, true);
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

                //  UI 표시 추가
                ShowDeathPanelUI();
            });
        }
        else
        {
            animator.SetTrigger("Die");
            ChangeLayerOnDeath();

            //  UI 표시 추가
            ShowDeathPanelUI();
        }

   
    }
    public void RespawnDeva()
    {
        if (!isDead) return;

        isDead = false;
        gameObject.SetActive(true);
        if (animator != null)
        {
            animator.ResetTrigger("Die"); // 죽음 트리거 리셋
            animator.Play("DevaIdle");        // 기본 상태로 복귀
        }
        // Rigidbody 초기화
        if (rb != null)
        {
            rb.bodyType = RigidbodyType2D.Dynamic;
            rb.simulated = true;
            rb.velocity = Vector2.zero;
        }

        // 체력 초기화
        if (DevaStats.Instance != null)
        {
            DevaStats.Instance.currentHealth = DevaStats.Instance.maxHealth;
            DevaStats.Instance.SetCurrentEnergy(DevaStats.Instance.maxEnergy);
            DevaStats.Instance.SetCurrentMana(DevaStats.Instance.maxMana);
        }
        if (PlayerStats.Instance != null)
        {
            PlayerStats.Instance.currentHealth = PlayerStats.Instance.maxHealth;
            PlayerStats.Instance.SetCurrentEnergy(PlayerStats.Instance.maxEnergy);
            PlayerStats.Instance.SetCurrentMana(PlayerStats.Instance.maxMana);
            if (HurtPlayer.Instance != null)
                HurtPlayer.Instance.UpdateHealthUI();
        }

        // UI 초기화
        UpdateHealthUI();

        // 애니메이션 초기화
        if (animator != null)
        {
            animator.ResetTrigger("Die");
            animator.Play("Idle");
        }

        // 컨트롤러 재활성화
        DebaraMovement movement = GetComponent<DebaraMovement>();
        if (movement != null) movement.enabled = true;

        MagicAttack magic = GetComponent<MagicAttack>();
        if (magic != null) magic.enabled = true;

        // 레이어 초기화
        if (spriteRenderer != null)
            spriteRenderer.sortingOrder = 0;

        // 검은 배경 투명화
        if (deathBackground != null)
        {
            Color color = deathBackground.color;
            color.a = 0f;
            deathBackground.color = color;
        }

        // 리스폰 위치 이동
        if (SpawnManager.Instance != null)
        {
            transform.position = SpawnManager.Instance.spawnPosition;
        }
        if (spriteRenderer != null)
        {
            spriteRenderer.sortingOrder = originalSortingOrder; //  복원
        }

        Debug.Log("[HurtDeva] 데바 부활 완료!");
    }


    private void DisableControls()
    {
        DebaraMovement movement = GetComponent<DebaraMovement>();
        if (movement != null)
        {
            if (movement.isInvincible) return;
            movement.enabled = false;
            movement.ForceEndAttack();
        }

        MagicAttack attack = GetComponent<MagicAttack>();
        if (attack != null) attack.enabled = false;
    }

    private void ChangeLayerOnDeath()
    {
        if (spriteRenderer != null)
            spriteRenderer.sortingOrder = 11;
    }

    private IEnumerator DisableAfterDeath()
    {
        yield return new WaitForSeconds(5f);
        gameObject.SetActive(false);
    }
    public bool IsDead()
    {
        return isDead;
    }
    private void ShowDeathPanelUI()
    {
        SceneUIManager sceneUIManager = FindObjectOfType<SceneUIManager>();

        if (sceneUIManager != null)
        {
            sceneUIManager.ShowManagedDeathPanel();
            Debug.Log("[HurtDeva] DeathPanel 호출 완료!");
        }
        else
        {
            Debug.LogError("[HurtDeva] SceneUIManager가 존재하지 않아 DeathPanel을 표시할 수 없습니다.");
        }
    }
}