using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
public class HurtPlayer : MonoBehaviour
{
    private Animator TestAnime;
    public GameObject[] bloodEffectPrefabs;
    public GameObject parringEffects;
    public ParticleSystem bloodEffectParticle;


    public CameraShakeSystem cameraShake;
    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;
    public int CurrentHealth => PlayerStats.Instance != null ? PlayerStats.Instance.currentHealth : 0;
    public int MaxHealth => PlayerStats.Instance != null ? PlayerStats.Instance.maxHealth : 100;


    public float knockbackForce = 5f;
    private bool isParrying = false;

    [Header("Hit Effect Position")]
    public Transform pos; //  �������� ��ġ ���� ������ �ǰ� ����Ʈ ��ġ

    public HealthBarUI healthBarUI; //  UI ü�¹� ���� �߰�
    public CharStateGUIEffect charStateGUIEffect;
    private bool isDead = false; //  ��� ���� Ȯ��

    [Header("Death Effect Elements")]
    public SpriteRenderer deathBackground; //  ����� ��Ӱ� �� ������Ʈ (SpriteRenderer)
    public static HurtPlayer Instance; // �̱��� �ν��Ͻ� �߰�
    private int originalSortingOrder; // ó�� ���̾� ����
    void Start()
    {
        TestAnime = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        cameraShake = Camera.main != null ? Camera.main.GetComponent<CameraShakeSystem>() : null;

        if (cameraShake == null)
        {
            Debug.LogWarning("ī�޶󿡼� CameraShakeSystem ��ũ��Ʈ�� ã�� �� �����ϴ�.");
        }



        if (healthBarUI != null)
        {
            healthBarUI.Initialize(MaxHealth);
        }
        //  ���� ����� ������ 0���� �ʱ�ȭ (���� ����)
        if (deathBackground != null)
        {
            Color startColor = deathBackground.color;
            startColor.a = 0f;
            deathBackground.color = startColor;
        }
        FindCameraShake();
        FindDeathBackground(); //  �� ���� �� deathBackground ã��
        originalSortingOrder = spriteRenderer != null ? spriteRenderer.sortingOrder : 0;
    }

    void Update()
    {
        if (cameraShake == null)
        {
            FindCameraShake(); //  ���� �ٲ���� ��� �ٽ� ã��
        }
        if (deathBackground == null)
        {
            FindDeathBackground(); //  �� ���� �� �ٽ� deathBackground ã��
        }
    }
    void FindDeathBackground()
    {
        GameObject backgroundObj = GameObject.Find("DeathBackground");
        if (backgroundObj != null)
        {
            deathBackground = backgroundObj.GetComponent<SpriteRenderer>();
        }
        else
        {
            Debug.LogWarning("DeathBackground�� ã�� �� �����ϴ�! �� ��ȯ �� Ȯ���ϼ���.");
        }
    }
    //  ī�޶� ����ũ �ý����� �ٽ� ã�� �Լ� �߰�
    void FindCameraShake()
    {
        cameraShake = Camera.main != null ? Camera.main.GetComponent<CameraShakeSystem>() : null;

        if (cameraShake == null)
        {
            Debug.LogWarning("ī�޶󿡼� CameraShakeSystem ��ũ��Ʈ�� ã�� �� �����ϴ�! �� ��ȯ �� Ȯ���ϼ���.");
        }
    }
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }

    public void ShowBloodEffect()
    {
        if (bloodEffectPrefabs != null && bloodEffectPrefabs.Length > 0)
        {
            int randomIndex = Random.Range(0, bloodEffectPrefabs.Length);
            GameObject selectedEffect = bloodEffectPrefabs[randomIndex];

            //  pos ��ġ���� ����Ʈ ����
            GameObject bloodEffect = Instantiate(selectedEffect, pos.position, Quaternion.identity);
            Destroy(bloodEffect, 0.3f);

            if (bloodEffectParticle != null)
            {
                ParticleSystem bloodParticle = Instantiate(bloodEffectParticle, pos.position, Quaternion.identity);
                bloodParticle.Play();
                Destroy(bloodParticle.gameObject, bloodParticle.main.duration + 0.5f);
            }
        }
        else
        {
            Debug.LogWarning("bloodEffectPrefabs �迭�� ��� �ֽ��ϴ�!");
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (isParrying || isDead) return; // �и� ���°ų� ��������� ����

        // �ٰŸ� �� �������� �˻�
        EnemyMovement enemy = other.GetComponentInParent<EnemyMovement>();
        // ���Ÿ� ����(ȭ��)���� �˻�
        Arrow enemyArrow = other.GetComponent<Arrow>();
        // �������� �˻�
        Thron thron = other.GetComponent<Thron>();

        // ������ "EnemyAttack" �Ǵ� "damageAmount" �±׸� ���� ��� ����
        if (other.CompareTag("EnemyAttack") || other.CompareTag("damageAmount"))
        {
            // �÷��̾ ���� �������� Ȯ��
            AdamMovement playerMovement = GetComponent<AdamMovement>();
            AdamUltimateSkill ultimateSkill = GetComponent<AdamUltimateSkill>();

            if ((playerMovement != null && playerMovement.isInvincible) ||
                (ultimateSkill != null && ultimateSkill.isCasting))
            {
                Debug.Log("���� ����! ����� ����");
                return; // ����� ó�� �� ��
            }
            // 0.5�� ���� �߰� ������� ���� �ʵ��� ���� (���� ���� ����)
            EnemyDamageBumpAgainst damageTrigger = other.GetComponent<EnemyDamageBumpAgainst>();
            if (damageTrigger != null)
            {
                damageTrigger.TriggerDamageCooldown(0.5f);
            }

            //  �ٰŸ� �� �������� Ȯ�� �� ����� ����
            int damage = 0;
            if (enemy != null)
            {
                damage = enemy.GetDamage(); // ���� �ִ� ������� ������
            }
            //  ���Ÿ� ����(ȭ��)���� Ȯ�� �� ����� ����
            else if (enemyArrow != null)
            {
                damage = enemyArrow.damage; // ȭ���� ���� ����� ����
            }
            //  �������� Ȯ�� �� ����� ����
            else if (thron != null)
            {
                damage = thron.damage; // ���� ����� ����
                Debug.Log("����");
            }

            // ����� ���� (�ǰ� ó��)
            TakeDamage(damage);

            //  �ǰ� �ִϸ��̼� ��� ����
            TestAnime.Play("Hurt", 0, 0f);

            // �ǰ� ����Ʈ ����
            ShowBloodEffect();

            //  �˹�(���) ȿ�� ����
            Knockback(other.transform);

            //  ī�޶� ��鸲 (ī�޶� ����ũ)
            if (cameraShake != null)
            {
                StartCoroutine(cameraShake.Shake(0.15f, 0.15f)); // 0.15�� ���� ȭ�� ��鸲
            }
        }
    }


    public void TakeDamage(int damage)
    {
        if (isDead || PlayerStats.Instance == null) return;

        PlayerStats.Instance.currentHealth -= damage;
        PlayerStats.Instance.currentHealth = Mathf.Clamp(PlayerStats.Instance.currentHealth, 0, PlayerStats.Instance.maxHealth);

        Debug.Log($"[HurtPlayer] ü�� ����: {PlayerStats.Instance.currentHealth} / {PlayerStats.Instance.maxHealth}");

        if (healthBarUI != null)
        {
            healthBarUI.UpdateHealthBar(PlayerStats.Instance.currentHealth, true);
        }

        if (charStateGUIEffect != null)
        {
            charStateGUIEffect.TriggerHitEffect();
        }

        if (PlayerStats.Instance.currentHealth <= 0)
        {
            Die();
        }
    }

    public void UpdateHealthUI()
    {
        if (healthBarUI != null)
        {
            healthBarUI.UpdateHealthBar(PlayerStats.Instance.currentHealth, true);
        }
    }


    public void CancelDamage()
    {
        Debug.Log(" �и� ����! ����� ��ȿȭ");
        TestAnime.ResetTrigger("Hurt"); // �ǰ� �ִϸ��̼� ���� ����
    }
    public void StartParry()
    {
        isParrying = true;
        StartCoroutine(ResetParry());
    }

    private IEnumerator ResetParry()
    {
        yield return new WaitForSeconds(0.1f);
        isParrying = false;
    }

    private void Knockback(Transform playerTransform)
    {
        if (rb == null) return;

        float direction = transform.position.x - playerTransform.position.x > 0 ? 1f : -1f;
        rb.velocity = new Vector2(knockbackForce * direction, rb.velocity.y + 1f);
    }

    private void Die()
    {
        if (isDead) return;
        isDead = true;

        Debug.Log($"{gameObject.name} ��� ���!");

        DisablePlayerControls();

        if (rb != null)
        {
            rb.velocity = Vector2.zero;
            rb.bodyType = RigidbodyType2D.Kinematic;
            rb.simulated = false;
            Debug.Log("[HurtPlayer] ������ٵ� ��Ȱ��ȭ �Ϸ�!");
        }

        if (deathBackground != null)
        {
            deathBackground.DOFade(1f, 0.5f).OnComplete(() =>
            {
                TestAnime.SetTrigger("Die");
                ChangeLayerOnDeath();

                //  ��� �� DeathPanel UI ȣ��
                ShowDeathPanelUI();
            });
        }
        else
        {
            TestAnime.SetTrigger("Die");
            ChangeLayerOnDeath();

            //  ��� �� DeathPanel UI ȣ��
            ShowDeathPanelUI();
        }

        StartCoroutine(DisableAfterDeath());
    }

    private void ShowDeathPanelUI()
    {
        SceneUIManager sceneUIManager = FindObjectOfType<SceneUIManager>();

        if (sceneUIManager != null)
        {
            sceneUIManager.ShowManagedDeathPanel();
            Debug.Log("[HurtPlayer] DeathPanel ȣ�� �Ϸ�!");
        }
        else
        {
            Debug.LogError("[HurtPlayer] SceneUIManager�� �������� �ʾ� DeathPanel�� ǥ���� �� �����ϴ�.");
        }
    }

    private void DisablePlayerControls()
    {
        //  �̵�, ����, �뽬, ���� �� ��� �Է� ����
        AdamMovement movement = GetComponent<AdamMovement>();
        if (movement != null) movement.enabled = false;

        CharacterAttack attack = GetComponent<CharacterAttack>();
        if (attack != null) attack.enabled = false;

        Debug.Log("��� ��Ʈ�ѷ� ��Ȱ��ȭ��");
    }

    private void ChangeLayerOnDeath()
    {
        if (spriteRenderer != null)
        {
            spriteRenderer.sortingOrder = 11; //  ĳ���Ͱ� ��� ���� �ö󰡵��� ����
            Debug.Log($"[HurtPlayer] Order in Layer �����: {spriteRenderer.sortingOrder}");
        }
    }
    public void RespawnPlayer()
    {
        if (!isDead) return;

        isDead = false;
        gameObject.SetActive(true);

        // Rigidbody �ʱ�ȭ
        if (rb != null)
        {
            rb.bodyType = RigidbodyType2D.Dynamic;
            rb.simulated = true;
            rb.velocity = Vector2.zero;
        }

        // ü�� �ʱ�ȭ
        if (PlayerStats.Instance != null)
        {
            PlayerStats.Instance.currentHealth = PlayerStats.Instance.maxHealth;
            PlayerStats.Instance.SetCurrentEnergy(PlayerStats.Instance.maxEnergy);
            PlayerStats.Instance.SetCurrentMana(PlayerStats.Instance.maxMana); //  �̰� �� �ʿ�!
        }
        if (DevaStats.Instance != null)
        {
            DevaStats.Instance.currentHealth = DevaStats.Instance.maxHealth;
            DevaStats.Instance.SetCurrentEnergy(DevaStats.Instance.maxEnergy);
            DevaStats.Instance.SetCurrentMana(DevaStats.Instance.maxMana);

            if (HurtDeva.Instance != null)
                HurtDeva.Instance.UpdateHealthUI(); // ���� ü�� UI�� ����
        }
        // UI �ʱ�ȭ
        UpdateHealthUI();

        // �ִϸ��̼� �ʱ�ȭ
        if (TestAnime != null)
        {
            TestAnime.ResetTrigger("Die");
            TestAnime.Play("Idle"); // Idle�� �ǵ�����
        }

        // ��Ʈ�ѷ� ��Ȱ��ȭ
        EnablePlayerControls();

        // ���̾� �ʱ�ȭ
        if (spriteRenderer != null)
        {
            spriteRenderer.sortingOrder = 0;
        }

        // ���� ��� ����ȭ
        if (deathBackground != null)
        {
            Color color = deathBackground.color;
            color.a = 0f;
            deathBackground.color = color;
        }
        if (spriteRenderer != null)
        {
            spriteRenderer.sortingOrder = originalSortingOrder; // ������� ����
        }

        Debug.Log("[HurtPlayer] �÷��̾� ��Ȱ �Ϸ�!");
    }
    private void EnablePlayerControls()
    {
        AdamMovement movement = GetComponent<AdamMovement>();
        if (movement != null) movement.enabled = true;

        CharacterAttack attack = GetComponent<CharacterAttack>();
        if (attack != null) attack.enabled = true;

        Debug.Log("��� ��Ʈ�ѷ� ��Ȱ��ȭ��");
    }
    public bool IsDead()
    {
        return isDead;
    }
    private IEnumerator DisableAfterDeath()
    {
        yield return new WaitForSeconds(5f);

    }
}