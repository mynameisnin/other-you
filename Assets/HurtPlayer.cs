using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; // UI ���� ���ӽ����̽� (HealthBarUI �� ��� ��)
using DG.Tweening;    // DOTween ��� ��
using UnityEngine.SceneManagement; // SceneManager ����� ���� �߰�

public class HurtPlayer : MonoBehaviour
{
    private Animator TestAnime;
    public GameObject[] bloodEffectPrefabs;
    public GameObject parringEffects; // ��Ÿ ���� ����: parryingEffects
    public ParticleSystem bloodEffectParticle;

    public CameraShakeSystem cameraShake;
    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;

    // PlayerStats �̱��� �ν��Ͻ��� ���� ü�� ���� ����
    public int CurrentHealth => PlayerStats.Instance != null ? PlayerStats.Instance.currentHealth : 0;
    public int MaxHealth => PlayerStats.Instance != null ? PlayerStats.Instance.maxHealth : 100; // �⺻�� 100

    public float knockbackForce = 5f;
    private bool isParrying = false;

    [Header("Hit Effect Position")]
    public Transform pos; // �ǰ� ����Ʈ ���� ��ġ

    [Header("UI References")]
    public HealthBarUI healthBarUI; // ü�¹� UI ���� (�ν����Ϳ��� �Ҵ� �Ǵ� Find)
    public CharStateGUIEffect charStateGUIEffect; // ĳ���� ���� GUI ȿ�� ���� (�ν����Ϳ��� �Ҵ� �Ǵ� Find)

    private bool isDead = false; // �÷��̾� ��� ����

    // --- SceneUIManager ���� ---
    private SceneUIManager currentSceneUIManager; // ���� ���� UI �Ŵ��� ����
    // --- SceneUIManager ���� �� ---

    // --- DeathBackground ���� (HurtPlayer�� ���� ����) ---
    [Header("Death Effect Elements (HurtPlayer Managed)")]
    public SpriteRenderer deathBackground; // ���� ��� SpriteRenderer (�ν����� �Ǵ� Find�� �Ҵ�)
    // --- DeathBackground ���� �� ---

    public static HurtPlayer Instance; // HurtPlayer �̱��� �ν��Ͻ�

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // �÷��̾� ������Ʈ�� �� ��ȯ �� �ı����� �ʵ��� ����
        }
        else if (Instance != this) // �̹� �ٸ� �ν��Ͻ��� �ִٸ� ���� ������Ʈ�� �ı�
        {
            Destroy(gameObject);
            return;
        }

        // �÷��̾��� �ֿ� ������Ʈ�� �ʱ�ȭ
        TestAnime = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void OnEnable()
    {
        // �� �ε� �̺�Ʈ ����
        SceneManager.sceneLoaded += OnSceneLoaded;
        // ���� �� (�Ǵ� ù ��)�� ���� ��ҵ� �ʱ�ȭ
        InitializeSceneDependentElements();
    }

    void OnDisable()
    {
        // �� �ε� �̺�Ʈ ���� ���� (������Ʈ ��Ȱ��ȭ �Ǵ� �ı� ��)
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    // ���� �ε�� ������ ȣ��Ǵ� �޼ҵ�
    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // ���� �ε�� ���� ���� ��ҵ� �ʱ�ȭ
        InitializeSceneDependentElements();
    }

    // �� �������� ��ҵ��� �ʱ�ȭ�ϰ�, �÷��̾� ���¸� �����ϴ� �޼ҵ�
    void InitializeSceneDependentElements()
    {
        // ���� ���� SceneUIManager ã��
        currentSceneUIManager = FindObjectOfType<SceneUIManager>();
        if (currentSceneUIManager == null)
        {
            Debug.LogError("���� ������ SceneUIManager�� ã�� �� �����ϴ�! �� ���� SceneUIManager ������Ʈ�� �ְ� Ȱ��ȭ�Ǿ� �ִ��� Ȯ���ϼ���.");
        }

        // ī�޶� ����ũ �ý��� ã��
        FindCameraShake();
        // DeathBackground ã�� �ʱ�ȭ (HurtPlayer�� ����)
        FindDeathBackground(); // ���⼭ deathBackground�� ���İ��� 0���� �ʱ�ȭ�˴ϴ�.

        // ---!!! �߿�: �÷��̾� ���� �ʱ�ȭ ���� !!!---

        // 1. ü�� �ʱ�ȭ
        if (PlayerStats.Instance != null)
        {
            PlayerStats.Instance.currentHealth = PlayerStats.Instance.maxHealth; // ���� ü���� �ִ� ü������!
            Debug.Log($"[HurtPlayer] �÷��̾� ü�� �ʱ�ȭ: {PlayerStats.Instance.currentHealth}/{PlayerStats.Instance.maxHealth}");
        }

        // 2. ��� ���� �÷��� �ʱ�ȭ
        isDead = false;
        Debug.Log("[HurtPlayer] isDead �÷��� �ʱ�ȭ: false");

        // 3. ��Ȱ��ȭ�ߴ� ������Ʈ�� �ٽ� Ȱ��ȭ
        AdamMovement movement = GetComponent<AdamMovement>(); // AdamMovement ��ũ��Ʈ ����
        if (movement != null)
        {
            movement.enabled = true;
            Debug.Log("[HurtPlayer] AdamMovement Ȱ��ȭ��");
        }

        CharacterAttack attack = GetComponent<CharacterAttack>(); // CharacterAttack ��ũ��Ʈ ����
        if (attack != null)
        {
            attack.enabled = true;
            Debug.Log("[HurtPlayer] CharacterAttack Ȱ��ȭ��");
        }
        // ���� �ٸ� ��ũ��Ʈ(��: ����, �뽬 ��)�� Die()���� ��Ȱ��ȭ�ߴٸ� ���⼭ �ٽ� Ȱ��ȭ�ؾ� �մϴ�.

        // 4. Rigidbody ���� �ʱ�ȭ
        if (rb != null)
        {
            rb.bodyType = RigidbodyType2D.Dynamic; // ���� Ÿ���� Dynamic���� �ǵ���
            rb.simulated = true;                   // ���� �ùķ��̼��� �ٽ� ��
            rb.velocity = Vector2.zero;            // Ȥ�� �� �ӵ� �ܿ��� ����
            Debug.Log("[HurtPlayer] Rigidbody ���� �ʱ�ȭ (Dynamic, Simulated)");
        }

        // 5. �ִϸ����� ���� �ʱ�ȭ
        if (TestAnime != null)
        {
            TestAnime.ResetTrigger("Die"); // "Die" �ִϸ��̼� Ʈ���� ����
            // ���� Bool �Ķ���� (��: "IsDead")�� ���� ���¸� �����Ѵٸ�:
            // TestAnime.SetBool("IsDead", false);
            TestAnime.Play("Idle", 0, 0f); // �⺻ ���� (��: "Idle") �ִϸ��̼����� ��� ��ȯ�Ͽ� ���� �ִϸ��̼ǿ��� ���
            Debug.Log("[HurtPlayer] Animator ���� �ʱ�ȭ (Die Ʈ���� ����, Idle ���·� ��ȯ)");
        }

        // ---!!! �÷��̾� ���� �ʱ�ȭ �� !!!---

        // ü�¹� UI ������Ʈ ����
        if (PlayerStats.Instance != null && healthBarUI != null)
        {
            healthBarUI.Initialize(MaxHealth); // healthBarUI�� �ִ�ġ�� �ٽ� ����
            healthBarUI.UpdateHealthBar(CurrentHealth, false); // �ʱ�ȭ�� ���� ü������ ������Ʈ
        }
        else if (healthBarUI == null)
        {
            // HealthBarUI�� �ʼ������� �ʰų� �ٸ� ������� ������ ��� ��� ������ ���߰ų� ������ �� �ֽ��ϴ�.
            Debug.LogWarning("HealthBarUI ������ �����ϴ�. �ν����Ϳ��� �Ҵ�Ǿ����� �Ǵ� ���� �����ϴ��� Ȯ���ϼ���.");
        }
    }

    // ���� ī�޶󿡼� CameraShakeSystem�� ã�� �޼ҵ�
    void FindCameraShake()
    {
        if (Camera.main != null)
        {
            cameraShake = Camera.main.GetComponent<CameraShakeSystem>();
        }
        if (cameraShake == null)
        {
            // Debug.LogWarning("ī�޶󿡼� CameraShakeSystem ��ũ��Ʈ�� ã�� �� �����ϴ�! �� ��ȯ �� Ȯ���ϼ���.");
        }
    }

    // "DeathBackground"��� �̸��� ������Ʈ�� ã�� SpriteRenderer�� �������� �ʱ�ȭ�ϴ� �޼ҵ�
    void FindDeathBackground()
    {
        GameObject backgroundObj = GameObject.Find("DeathBackground"); // �̸����� ã��
        if (backgroundObj != null)
        {
            deathBackground = backgroundObj.GetComponent<SpriteRenderer>();
            if (deathBackground != null)
            {
                // �� �ε� �� (�׸��� ���� ���� �ʾ��� ��) �����ϰ� �ʱ�ȭ
                deathBackground.gameObject.SetActive(true); // ���� ������Ʈ ��ü�� Ȱ��ȭ ���¿��� ��
                Color startColor = deathBackground.color;
                startColor.a = 0f; // ���İ��� 0���� (���� ����)
                deathBackground.color = startColor;
            }
            else
            {
                Debug.LogWarning("HurtPlayer: 'DeathBackground' ������Ʈ�� SpriteRenderer ������Ʈ�� �����ϴ�.");
            }
        }
        else
        {
            Debug.LogWarning("HurtPlayer: 'DeathBackground' �̸��� ������Ʈ�� ������ ã�� �� �����ϴ�.");
            deathBackground = null; // �� ã������ ������ null�� ����
        }
    }

    // �ǰ� �� ���� ����Ʈ�� �����ִ� �޼ҵ�
    public void ShowBloodEffect()
    {
        if (bloodEffectPrefabs != null && bloodEffectPrefabs.Length > 0)
        {
            int randomIndex = Random.Range(0, bloodEffectPrefabs.Length);
            GameObject selectedEffect = bloodEffectPrefabs[randomIndex];

            // 'pos' ��ġ�� ����Ʈ ����
            GameObject bloodEffectInstance = Instantiate(selectedEffect, pos.position, Quaternion.identity);
            Destroy(bloodEffectInstance, 0.3f); // 0.3�� �� �ڵ� �ı�

            if (bloodEffectParticle != null)
            {
                ParticleSystem bloodParticleInstance = Instantiate(bloodEffectParticle, pos.position, Quaternion.identity);
                bloodParticleInstance.Play();
                // ��ƼŬ �ý����� main ��� duration ���� �ణ�� ������ ���� �ı� �ð� ����
                Destroy(bloodParticleInstance.gameObject, bloodParticleInstance.main.duration + bloodParticleInstance.main.startLifetime.constantMax + 0.5f);
            }
        }
        // else
        // {
        //     Debug.LogWarning("bloodEffectPrefabs �迭�� ��� �ְų� �Ҵ���� �ʾҽ��ϴ�!");
        // }
    }

    // �浹 ���� (�ַ� ���� ����)
    void OnTriggerEnter2D(Collider2D other)
    {
        if (isParrying || isDead) return; // �и� ���̰ų� �̹� �׾����� ����

        // ���� ��ü Ȯ�� (�پ��� ���� ���� ���)
        EnemyMovement enemy = other.GetComponentInParent<EnemyMovement>(); // �ٰŸ� ��
        Arrow enemyArrow = other.GetComponent<Arrow>(); // ���Ÿ� ȭ��
        Thron thron = other.GetComponent<Thron>(); // ���� ����

        // "EnemyAttack" �Ǵ� "damageAmount" �±׸� ���� ������Ʈ�� �浹 ��
        if (other.CompareTag("EnemyAttack") || other.CompareTag("damageAmount"))
        {
            // �÷��̾� ���� ���� Ȯ��
            AdamMovement playerMovement = GetComponent<AdamMovement>();
            AdamUltimateSkill ultimateSkill = GetComponent<AdamUltimateSkill>(); // �ñر� ��� �� ���� ��

            if ((playerMovement != null && playerMovement.isInvincible) ||
                (ultimateSkill != null && ultimateSkill.isCasting)) // isCasting ���� �ñر� ��� �� ���� ����
            {
                Debug.Log("�÷��̾� ���� ����! ����� ����.");
                return; // ����� ó�� �� ��
            }

            // ���� ���� ������ ���� ��ٿ� (���� ��ü ��ũ��Ʈ�� ���� �ٸ� �� ����)
            EnemyDamageBumpAgainst damageTrigger = other.GetComponent<EnemyDamageBumpAgainst>();
            if (damageTrigger != null)
            {
                damageTrigger.TriggerDamageCooldown(0.5f); // ����: 0.5�� ��ٿ�
            }

            int damage = 0;
            if (enemy != null)
            {
                damage = enemy.GetDamage(); // �� ��ũ��Ʈ���� ����� �� ��������
            }
            else if (enemyArrow != null)
            {
                damage = enemyArrow.damage; // ȭ�� ��ũ��Ʈ���� ����� �� ��������
            }
            else if (thron != null)
            {
                damage = thron.damage; // ���� ��ũ��Ʈ���� ����� �� ��������
                // Debug.Log("���ÿ� ��!");
            }

            if (damage > 0) // ��ȿ�� ������� ���� ��쿡�� ó��
            {
                TakeDamage(damage); // ����� ����
                TestAnime.Play("Hurt", 0, 0f); // �ǰ� �ִϸ��̼� ��� ����
                ShowBloodEffect(); // �ǰ� ����Ʈ ����
                Knockback(other.transform); // �˹� ȿ��

                if (cameraShake != null)
                {
                    StartCoroutine(cameraShake.Shake(0.15f, 0.15f)); // ī�޶� ��鸲
                }
            }
        }
    }

    // �÷��̾ ������� �޴� ����
    public void TakeDamage(int damage)
    {
        if (isDead || PlayerStats.Instance == null) return; // �̹� �׾��ų� PlayerStats ������ ����

        PlayerStats.Instance.currentHealth -= damage;
        // ü���� 0 �̸����� �������� �ʵ��� Mathf.Clamp ���
        PlayerStats.Instance.currentHealth = Mathf.Clamp(PlayerStats.Instance.currentHealth, 0, MaxHealth);

        Debug.Log($"[HurtPlayer] ü�� ����: {CurrentHealth} / {MaxHealth}");

        // ü�¹� UI ������Ʈ
        if (healthBarUI != null)
        {
            healthBarUI.UpdateHealthBar(CurrentHealth, true); // true�� �ִϸ��̼� ȿ�� ���� ���� ��
        }

        // ĳ���� ���� GUI ȿ�� �ߵ� (ȭ�� ������ ��)
        if (charStateGUIEffect != null)
        {
            charStateGUIEffect.TriggerHitEffect();
        }

        // ü���� 0 ���ϰ� �Ǹ� ��� ó��
        if (CurrentHealth <= 0)
        {
            Die();
        }
    }

    // �ܺο��� �÷��̾� ü�� UI�� ������ ������Ʈ�ؾ� �� �� ��� (��: ���������� ȸ�� ��)
    public void UpdateHealthUI()
    {
        if (healthBarUI != null && PlayerStats.Instance != null)
        {
            healthBarUI.UpdateHealthBar(CurrentHealth, true);
        }
    }

    // �и� ���� �� ����� ��ȿȭ (�ܺο��� ȣ��� �� ����)
    public void CancelDamage()
    {
        Debug.Log("�и� ����! ����� ��ȿȭ��.");
        TestAnime.ResetTrigger("Hurt"); // �ǰ� �ִϸ��̼� ���� ����
    }

    // �и� ���� (�ܺο��� ȣ��)
    public void StartParry()
    {
        isParrying = true;
        StartCoroutine(ResetParry()); // ���� �ð� �� �и� ���� ����
    }

    // �и� ���¸� ª�� �ð� �� �����ϴ� �ڷ�ƾ
    private IEnumerator ResetParry()
    {
        yield return new WaitForSeconds(0.1f); // ����: 0.1�� ���� �и� ��ȿ
        isParrying = false;
    }

    // �˹� ȿ���� �ִ� �޼ҵ�
    private void Knockback(Transform attackerTransform)
    {
        if (rb == null || rb.bodyType == RigidbodyType2D.Kinematic) return; // Rigidbody ���ų� Kinematic�̸� �˹� �Ұ�

        // ������ ��ġ�� �������� �÷��̾ �з��� ���� ����
        float direction = (transform.position.x - attackerTransform.position.x > 0) ? 1f : -1f;
        // X������ �˹� ���� ���ϰ�, Y�����δ� �ణ ���� ���� ȿ�� �߰� ����
        rb.velocity = new Vector2(knockbackForce * direction, rb.velocity.y + knockbackForce * 0.2f); // Y�� �� ���� ����
    }

    // �÷��̾� ��� ó�� �޼ҵ�
    private void Die()
    {
        if (isDead) return; // �̹� ��� ó�� ���̸� �ߺ� ���� ����
        isDead = true; // ��� ���·� ����

        Debug.Log($"{gameObject.name} ���!");
        DisablePlayerControls(); // �÷��̾� ���� ��Ȱ��ȭ

        // Rigidbody�� Kinematic���� �����Ͽ� ������ ������ ����
        if (rb != null)
        {
            rb.velocity = Vector2.zero; // ���� �ӵ� ����
            rb.bodyType = RigidbodyType2D.Kinematic; // ���� ���� ��Ȱ��ȭ
            rb.simulated = false; // ������ٵ� �ùķ��̼� ����
            Debug.Log("[HurtPlayer] ������ٵ� ��Ȱ��ȭ �Ϸ� (Kinematic, Not Simulated)");
        }

        // DeathBackground ó�� (HurtPlayer�� ����)
        if (deathBackground != null)
        {
            // ����� ������ ��Ӱ� �ϴ� DOTween �ִϸ��̼�
            deathBackground.DOFade(1f, 0.5f) // 0.5�� ���� ���İ��� 1�� (���� ������)
                .OnComplete(() => {
                    // ��� ��ο��� �� ����� ����
                    TestAnime.SetTrigger("Die"); // ��� �ִϸ��̼� ����
                    ChangeLayerOnDeath();       // ĳ���� ���̾� ���� (�ٸ� ������Ʈ ���� �ö������)

                    // SceneUIManager�� ���� DeathPanel Ȱ��ȭ ��û
                    if (currentSceneUIManager != null)
                    {
                        currentSceneUIManager.ShowManagedDeathPanel();
                    }
                    else
                    {
                        Debug.LogError("Die (After Fade): currentSceneUIManager�� �����ϴ�. DeathPanel�� ǥ���� �� �����ϴ�.");
                    }
                });
        }
        else
        {
            // DeathBackground�� ������ �ٷ� ���� ���� ����
            TestAnime.SetTrigger("Die");
            ChangeLayerOnDeath();

            if (currentSceneUIManager != null)
            {
                currentSceneUIManager.ShowManagedDeathPanel();
            }
            else
            {
                Debug.LogError("Die (No Background): currentSceneUIManager�� �����ϴ�. DeathPanel�� ǥ���� �� �����ϴ�.");
            }
        }
        // StartCoroutine(DisableAfterDeath()); // �ʿ��ϴٸ� ���� �ð� �� ĳ���� ������Ʈ ��ü�� ��Ȱ��ȭ
    }

    // �÷��̾� ���� ���� ������Ʈ���� ��Ȱ��ȭ�ϴ� �޼ҵ�
    private void DisablePlayerControls()
    {
        AdamMovement movement = GetComponent<AdamMovement>();
        if (movement != null) movement.enabled = false;

        CharacterAttack attack = GetComponent<CharacterAttack>();
        if (attack != null) attack.enabled = false;

        // �ٸ� ���� ���� ������Ʈ�� �ִٸ� ���⼭ ��Ȱ��ȭ (��: ����, �뽬 ��ũ��Ʈ)
        // Jump jumpScript = GetComponent<Jump>();
        // if (jumpScript != null) jumpScript.enabled = false;

        Debug.Log("�÷��̾� ��Ʈ�ѷ��� ��Ȱ��ȭ��.");
    }

    // ��� �� ĳ������ SpriteRenderer Sorting Order�� �����Ͽ� �ٸ� ������Ʈ ���� ���̵��� ��
    private void ChangeLayerOnDeath()
    {
        if (spriteRenderer != null)
        {
            spriteRenderer.sortingOrder = 11; // ������ ������ ���� (��: UI���ٴ� ����, ��溸�ٴ� ����)
            Debug.Log($"[HurtPlayer] ĳ���� Order in Layer �����: {spriteRenderer.sortingOrder}");
        }
    }

    // ���� �ð� �� �÷��̾� ���� ������Ʈ ��ü�� ��Ȱ��ȭ�ϴ� �ڷ�ƾ (���� ����)
    private IEnumerator DisableAfterDeath()
    {
        yield return new WaitForSeconds(5f); // ��: 5�� ��
        gameObject.SetActive(false);
        Debug.Log("�÷��̾� ������Ʈ ��Ȱ��ȭ��.");
    }
}