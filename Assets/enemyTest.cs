using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class enemyTest : MonoBehaviour
{
    private Animator TestAnime;
    public GameObject[] bloodEffectPrefabs;
    public GameObject parringEffects;
    public ParticleSystem bloodEffectParticle;

    private CameraShakeSystem cameraShake;
    private Rigidbody2D rb; // Rigidbody2D 추가

    public int MaxHealth = 100;
    public int currentHealth;

    public float knockbackForce = 5f; // 피격 시 밀리는 힘

    void Start()
    {
        TestAnime = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>(); // Rigidbody2D 할당

        cameraShake = Camera.main != null ? Camera.main.GetComponent<CameraShakeSystem>() : null;

        if (cameraShake == null)
        {
            Debug.LogWarning("카메라에서 CameraShakeSystem 스크립트를 찾을 수 없습니다.");
        }

        currentHealth = MaxHealth;
    }

    public void ShowBloodEffect(Vector3 hitPosition)
    {
        if (bloodEffectPrefabs != null && bloodEffectPrefabs.Length > 0)
        {
            int randomIndex = Random.Range(0, bloodEffectPrefabs.Length);
            GameObject selectedEffect = bloodEffectPrefabs[randomIndex];

            GameObject bloodEffect = Instantiate(selectedEffect, hitPosition, Quaternion.identity);
            bloodEffect.transform.position += new Vector3(0f, 1f, -1);
            Destroy(bloodEffect, 0.3f);

            if (bloodEffectParticle != null)
            {
                ParticleSystem bloodParticle = Instantiate(bloodEffectParticle, hitPosition, Quaternion.identity);
                bloodParticle.transform.position += new Vector3(0f, 1f, 0f);
                bloodParticle.Play();
                Destroy(bloodParticle.gameObject, bloodParticle.main.duration + 0.5f);
            }
        }
        else
        {
            Debug.LogWarning("bloodEffectPrefabs 배열이 비어 있습니다!");
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other != null && other.CompareTag("PlayerAttack"))
        {
            Vector3 hitPosition = transform.position;

            TestAnime.SetTrigger("hurt");

            TakeDamage(20);
            ShowBloodEffect(hitPosition);
            Knockback(other.transform); // 피격 시 넉백 실행

            if (cameraShake != null)
            {
                StartCoroutine(cameraShake.Shake(0.2f, 0.1f));
            }
        }
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        currentHealth = Mathf.Clamp(currentHealth, 0, MaxHealth);

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void Knockback(Transform playerTransform)
    {
        if (rb == null) return;

        // 플레이어 위치를 기준으로 적이 반대 방향으로 밀리게 설정
        float direction = transform.position.x - playerTransform.position.x > 0 ? 1f : -1f;

        // 밀리는 힘을 적용 (X축 방향으로 밀리고, 약간 위쪽으로 튀게 설정)
        rb.velocity = new Vector2(knockbackForce * direction, rb.velocity.y + 1f);
    }

    private void Die()
    {
        Debug.Log($"{gameObject.name} 사망!");

        // 적 사망 처리 (여기선 삭제하는 부분 주석 처리)
        /*
        Destroy(gameObject, 1f);
        */
    }
}
