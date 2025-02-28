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

    void Start()
    {
        TestAnime = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        col = GetComponent<Collider2D>();

        cameraShake = Camera.main != null ? Camera.main.GetComponent<CameraShakeSystem>() : null;
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
        if (isParrying) return;
        if (other != null && other.CompareTag("PlayerAttack"))
        {
            if (currentHealth > 0 && !isDying)
            {
                TestAnime.Play("Hurt", 0, 0f);
                TakeDamage(20);
                ShowBloodEffect();
                Knockback(other.transform);

                if (cameraShake != null)
                {
                    StartCoroutine(cameraShake.Shake(0.1f, 0.1f));
                }
            }
        }
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
    public void TakeDamage(int damage)
    {
        if (isDying) return;

        currentHealth -= damage;
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

        Debug.Log($"{gameObject.name} 사망!");

        //  콜라이더와 리지드바디 제거
        if (col != null) col.enabled = false;
        if (rb != null) rb.simulated = false;

        //  Die 애니메이션 실행
        TestAnime.SetTrigger("Die");

        // 2초 후 삭제 코루틴 실행
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
