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

    public int MaxHealth = 100;
    public int currentHealth;


    void Start()
    {
        TestAnime = GetComponent<Animator>();
        cameraShake = Camera.main != null ? Camera.main.GetComponent<CameraShakeSystem>() : null;

        if (cameraShake == null)
        {
            Debug.LogWarning("카메라에서 cameShake 스크립트를 찾을 수 없습니다.");
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

    private void Die()
    {
        Debug.Log($"{gameObject.name} 사망!");

        /*  if (statusGUI != null)
          {
              statusGUI.gameObject.SetActive(false);
          }

          Destroy(gameObject, 1f);
        */
    }

}
