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
    private Rigidbody2D rb;

    public int MaxHealth = 100;
    public int currentHealth;

    public float knockbackForce = 5f;

    [Header("Hit Effect Position")]
    public Transform pos; //  수동으로 위치 조정 가능한 피격 이펙트 위치

    void Start()
    {
        TestAnime = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();

        cameraShake = Camera.main != null ? Camera.main.GetComponent<CameraShakeSystem>() : null;

        if (cameraShake == null)
        {
            Debug.LogWarning("카메라에서 CameraShakeSystem 스크립트를 찾을 수 없습니다.");
        }

        currentHealth = MaxHealth;
    }

    public void ShowBloodEffect()
    {
        if (bloodEffectPrefabs != null && bloodEffectPrefabs.Length > 0)
        {
            int randomIndex = Random.Range(0, bloodEffectPrefabs.Length);
            GameObject selectedEffect = bloodEffectPrefabs[randomIndex];

            //  pos 위치에서 이펙트 생성
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
            Debug.LogWarning("bloodEffectPrefabs 배열이 비어 있습니다!");
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other != null && other.CompareTag("PlayerAttack"))
        {
            //  애니메이션 즉시 다시 실행
            TestAnime.Play("Hurt", 0, 0f);

            TakeDamage(20);
            ShowBloodEffect(); //  pos 위치에서 이펙트 생성
            Knockback(other.transform);

            if (cameraShake != null)
            {
                StartCoroutine(cameraShake.Shake(0.1f, 0.1f));
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

        float direction = transform.position.x - playerTransform.position.x > 0 ? 1f : -1f;
        rb.velocity = new Vector2(knockbackForce * direction, rb.velocity.y + 1f);
    }

    private void Die()
    {
        Debug.Log($"{gameObject.name} 사망!");
    }
}
