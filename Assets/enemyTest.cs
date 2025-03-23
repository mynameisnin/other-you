using System.Collections;
using UnityEngine;

public class enemyTest : MonoBehaviour
{
    private Animator TestAnime;
    public GameObject[] bloodEffectPrefabs;
    public GameObject parringEffects;
    public ParticleSystem bloodEffectParticle;

    private CameraShakeSystem cameraShake;
    private Rigidbody2D rb;
    private Collider2D col;

    public int MaxHealth = 100;
    public int currentHealth;

    public float knockbackForce = 5f;
    private bool isDying = false;

    [Header("Hit Effect Position")]
    public Transform pos;
    private bool isParrying = false;
    public int xpReward = 50; // �� ���͸� óġ�ϸ� �ִ� ����ġ

    private PlayerExperience playerXP;
    void Start()
    {
        TestAnime = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        col = GetComponent<Collider2D>();

        cameraShake = Camera.main != null ? Camera.main.GetComponent<CameraShakeSystem>() : null;
        currentHealth = MaxHealth;
        PlayerExperience playerXP = FindObjectOfType<PlayerExperience>();
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
        if (isParrying || isDying) return;

        // ? ���� ���ݵ� ����
        if (other.CompareTag("PlayerAttack"))
        {
            DevaAttackDamage debaDamage = other.GetComponentInParent<DevaAttackDamage>();
            PlayerAttackDamage adamDamage = other.GetComponentInParent<PlayerAttackDamage>();

            int damage = 0;

            if (adamDamage != null)
                damage = adamDamage.GetNomalAttackDamage();
            else if (debaDamage != null)
                damage = debaDamage.GetMagicDamage();

            // ����� ������ �˻�
            enemyTest closestEnemy = FindClosestEnemy(other.transform);
            if (closestEnemy == this && damage > 0)
            {
                if (currentHealth > 0 && !isDying)
                {
                    TestAnime.Play("Hurt", 0, 0f);
                    TakeDamage(damage);
                    ShowBloodEffect();
                    Knockback(other.transform);

                    if (cameraShake != null)
                    {
                        StartCoroutine(cameraShake.Shake(0.1f, 0.1f));
                    }
                }
            }
        }
    }


    //  �÷��̾�� ���� ����� ���� ã�� �Լ�
    enemyTest FindClosestEnemy(Transform playerAttack)
    {
        enemyTest[] enemies = FindObjectsOfType<enemyTest>(); // ��� �� ��������
        enemyTest closestEnemy = null;
        float closestDistance = Mathf.Infinity;

        foreach (enemyTest enemy in enemies)
        {
            float distance = Vector2.Distance(playerAttack.position, enemy.transform.position);

            if (distance < closestDistance)
            {
                closestDistance = distance;
                closestEnemy = enemy;
            }
        }

        return closestEnemy;
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
    public void TakeDamage(int Nomaldamage)
    {
        if (isDying) return;

        currentHealth -= Nomaldamage;
        currentHealth = Mathf.Clamp(currentHealth, 0, MaxHealth);

        if (currentHealth <= 0)
        {
            StartCoroutine(Die());
        }
    }

    private void Knockback(Transform playerTransform)
    {
        if (rb == null) return;

        float direction = transform.position.x - playerTransform.position.x > 0 ? 1f : -1f;
        rb.velocity = new Vector2(knockbackForce * direction, rb.velocity.y + 1f);
    }

    private IEnumerator Die()
    {
        if (isDying) yield break;
        isDying = true;

        

        PlayerExperience playerXP = FindObjectOfType<PlayerExperience>();

        if (playerXP != null)
        {
           
            playerXP.GainXP(xpReward);
        }
        else
        {
            Debug.LogError(" PlayerExperience�� ã�� �� ����!");
        }

        if (col != null) col.enabled = false;
        if (rb != null) rb.simulated = false;

        TestAnime.SetTrigger("Die");
        StartCoroutine(DestroyAfterDelay(0.6f));
    }

    private IEnumerator DestroyAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        DestroyEnemy();
    }

    private void DestroyEnemy()
    {
        Debug.Log($"{gameObject.name} ������ ���ŵ�!");
        Destroy(gameObject);
    }
}
