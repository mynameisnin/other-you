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
    private Rigidbody2D rb; // Rigidbody2D �߰�

    public int MaxHealth = 100;
    public int currentHealth;

    public float knockbackForce = 5f; // �ǰ� �� �и��� ��

    void Start()
    {
        TestAnime = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>(); // Rigidbody2D �Ҵ�

        cameraShake = Camera.main != null ? Camera.main.GetComponent<CameraShakeSystem>() : null;

        if (cameraShake == null)
        {
            Debug.LogWarning("ī�޶󿡼� CameraShakeSystem ��ũ��Ʈ�� ã�� �� �����ϴ�.");
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
            Debug.LogWarning("bloodEffectPrefabs �迭�� ��� �ֽ��ϴ�!");
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
            Knockback(other.transform); // �ǰ� �� �˹� ����

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

        // �÷��̾� ��ġ�� �������� ���� �ݴ� �������� �и��� ����
        float direction = transform.position.x - playerTransform.position.x > 0 ? 1f : -1f;

        // �и��� ���� ���� (X�� �������� �и���, �ణ �������� Ƣ�� ����)
        rb.velocity = new Vector2(knockbackForce * direction, rb.velocity.y + 1f);
    }

    private void Die()
    {
        Debug.Log($"{gameObject.name} ���!");

        // �� ��� ó�� (���⼱ �����ϴ� �κ� �ּ� ó��)
        /*
        Destroy(gameObject, 1f);
        */
    }
}
