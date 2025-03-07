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
    public int xpReward = 50; // 이 몬스터를 처치하면 주는 경험치

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
        if (isParrying) return;

        if (other.CompareTag("PlayerAttack"))
        {
            PlayerAttackDamage NomalDamage = other.GetComponentInParent<PlayerAttackDamage>();

            if (NomalDamage == null) return;

            //  플레이어와 가장 가까운 적 찾기
            enemyTest closestEnemy = FindClosestEnemy(other.transform);

            if (closestEnemy == this) // 내가 가장 가까운 적이라면 피격 처리
            {
                if (currentHealth > 0 && !isDying)
                {
                    TestAnime.Play("Hurt", 0, 0f);
                    int Nomaldamage = NomalDamage.GetNomalAttackDamage();
                    TakeDamage(Nomaldamage);
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

    //  플레이어와 가장 가까운 적을 찾는 함수
    enemyTest FindClosestEnemy(Transform playerAttack)
    {
        enemyTest[] enemies = FindObjectsOfType<enemyTest>(); // 모든 적 가져오기
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

        Debug.LogError($"{gameObject.name}이(가) 사망 이벤트 실행됨!"); //  로그 추가

        PlayerExperience playerXP = FindObjectOfType<PlayerExperience>();

        if (playerXP != null)
        {
            Debug.LogError($"플레이어 경험치 시스템 찾음! {playerXP.name}");
            playerXP.GainXP(xpReward);
        }
        else
        {
            Debug.LogError(" PlayerExperience를 찾을 수 없음!");
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
        Debug.Log($"{gameObject.name} 완전히 제거됨!");
        Destroy(gameObject);
    }
}
