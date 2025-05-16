using UnityEngine;
using System.Collections;

public class BossHurt : MonoBehaviour
{
    [Header("Phase Object")]
    public GameObject phase2Object; // ���� 2������ ������Ʈ
    public GameObject phase2ObjectAnime;
    private bool phase2Triggered = false; // �ߺ� ������

    private Rigidbody2D rb;
    private Collider2D col;

    public GameObject[] bloodEffectPrefabs;
    public GameObject parringEffects; // ������� ������ �����ص� ����
    public ParticleSystem bloodEffectParticle;

    private CameraShakeSystem cameraShake;

    [Header("Stats")]
    public int MaxHealth = 100;
    public int currentHealth;
    public int xpReward = 50;

    [Header("Flags")]
    private bool isDying = false;

    [Header("Hit Effect Position")]
    public Transform pos;

    private AngryGodAiCore aiCore; // AI Core ����
    private void Awake() // Awake�� �����Ͽ� Start���� ���� ���� ����
    {
        rb = GetComponent<Rigidbody2D>();
        col = GetComponent<Collider2D>();
        cameraShake = Camera.main != null ? Camera.main.GetComponent<CameraShakeSystem>() : null;
        aiCore = GetComponent<AngryGodAiCore>(); // AI Core ���� ��������

        if (aiCore == null) Debug.LogError("AngryGodAiCore component missing on " + gameObject.name);

        currentHealth = MaxHealth;
    }


    public void ShowBloodEffect()
    {
        if (bloodEffectPrefabs != null && bloodEffectPrefabs.Length > 0)
        {
            int randomIndex = Random.Range(0, bloodEffectPrefabs.Length);
            GameObject selectedEffect = bloodEffectPrefabs[randomIndex];

            GameObject bloodEffect = Instantiate(selectedEffect, pos.position, Quaternion.identity);
            Destroy(bloodEffect, 0.3f);

            if (bloodEffectParticle != null)
            {
                ParticleSystem bloodParticle = Instantiate(bloodEffectParticle, pos.position, Quaternion.identity);
                bloodParticle.Play();
                Destroy(bloodParticle.gameObject, bloodParticle.main.duration + 0.5f);
            }
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (isDying) return;

        if (other.CompareTag("PlayerAttack"))
        {
            DevaAttackDamage debaDamage = other.GetComponentInParent<DevaAttackDamage>();
            PlayerAttackDamage adamDamage = other.GetComponentInParent<PlayerAttackDamage>();

            int damage = 0;
            bool isFromAdam = false;
            bool isFromDeba = false;

            if (adamDamage != null)
            {
                damage = adamDamage.GetNomalAttackDamage();
                isFromAdam = true;
            }
            else if (debaDamage != null)
            {
                damage = debaDamage.GetMagicDamage();
                isFromDeba = true;
            }

            BossHurt closest = FindClosestEnemy(other.transform);
            if (closest == this && damage > 0)
            {
                if (currentHealth > 0 && !isDying)
                {
                    TakeDamage(damage, isFromAdam, isFromDeba);
                }
            }
        }
    }

    BossHurt FindClosestEnemy(Transform attacker)
    {
        BossHurt[] enemies = FindObjectsOfType<BossHurt>();
        BossHurt closest = null;
        float closestDistance = Mathf.Infinity;

        foreach (BossHurt enemy in enemies)
        {
            float dist = Vector2.Distance(attacker.position, enemy.transform.position);
            if (dist < closestDistance)
            {
                closestDistance = dist;
                closest = enemy;
            }
        }

        return closest;
    }

    public void TakeDamage(int damage, bool fromAdam, bool fromDeba)
    {
        if (isDying || (aiCore != null && aiCore.IsCurrentlyActingOrSkillActive())) // �ڡڡ� ������ �߿��� �ൿ ���� ���� ������ ó�� �� ���� ��û�� ��� ������ �� ���� (������)
        {
            // �Ǵ� �������� �޵�, ���� ��û ���� �˻縦 �Ʒ� if�����θ� ����
            // ����� isDying�� üũ�ϵ��� ����
        }
        if (isDying) return;


        currentHealth -= damage;
        currentHealth = Mathf.Clamp(currentHealth, 0, MaxHealth);

        // 50% ���� ���� Ʈ���� ó�� (�� �� ����)
        // phase2Triggered�� BossHurt �������� "���� ��û�� ���´°�"�� ����
        if (!phase2Triggered && currentHealth <= MaxHealth / 2)
        {
            phase2Triggered = true; // ���� ��û�� ���� ������ ǥ�� (�ߺ� ����)
            Debug.Log("�� HP 50% ����! ���� ������ ��û �غ�.");

            if (aiCore != null)
            {
                aiCore.RequestAwakeningSequence(); // AI Core�� ���� ������ ���� ��û
                                                   // phase2Object �� phase2ObjectAnime�� Ȱ��ȭ�� AI Core�� ���� ������ ������ ó��
            }
            else
            {
                Debug.LogWarning("AI Core ������ ���� ���� �������� ��û�� �� �����ϴ�!");
                // AI Core�� ���ٸ� ����ó�� ���⼭ ���� phase2Object ���� Ȱ��ȭ�� �� ������,
                // "������ ��뽬 ��" ��� ������ AI Core�� ����ؾ� ��.
                if (phase2Object != null) phase2Object.SetActive(true);
                if (phase2ObjectAnime != null) phase2ObjectAnime.SetActive(true);
            }
        }

        if (currentHealth <= 0)
        {
            StartCoroutine(Die(fromAdam, fromDeba));
        }
        else
        {
            ShowBloodEffect();
            if (cameraShake != null) // null üũ �߰�
                cameraShake.StartCoroutine(cameraShake.Shake(0.1f, 0.1f));
        }
    }



    private IEnumerator Die(bool fromAdam, bool fromDeba)
    {
        if (isDying) yield break;
        isDying = true;

        ShowBloodEffect();

        if (fromAdam && PlayerExperience.Instance != null)
        {
            PlayerExperience.Instance.GainXP(xpReward);
        }
        else if (fromDeba && DevaExperience.Instance != null)
        {
            DevaExperience.Instance.GainXP(xpReward);
        }

        if (rb != null)
        {
            rb.velocity = Vector2.zero;
            rb.bodyType = RigidbodyType2D.Kinematic;
            rb.simulated = false;
        }

        if (col != null) col.enabled = false;

        yield return new WaitForSeconds(0.6f);
        Destroy(gameObject);
    }
}
