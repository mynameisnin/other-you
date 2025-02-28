using System.Collections;
using UnityEngine;

public class ParrySystem : MonoBehaviour
{
    public GameObject parryEffectPrefab; // �и� ����Ʈ ������
    public GameObject parryParticlePrefab; // �и� ��ƼŬ (���� ����)
    void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log($"�浹 ������: {other.gameObject.name} (�±�: {other.tag})");

        // �÷��̾��� ���ݰ� �浹�ߴ��� Ȯ��
        if (other.CompareTag("PlayerAttack") && gameObject.CompareTag("EnemyAttack"))
        {
            Debug.Log("  �и� ����! �÷��̾�� ���� ������ ���ÿ� ������");

            // �и� ����Ʈ ���� �� �̵�
            GameObject parryEffect = InstantiateParryEffect(other.transform.position);
            CallParryParticle(other.transform.position);
            // Ư�� ������Ʈ�� �и� ����Ʈ �̵� (��: �÷��̾� ��ġ)
            if (parryEffect != null)
            {
                StartCoroutine(MoveParryEffect(parryEffect, other.transform.position));
            }
            ApplyParryProtection();
            // ���� ���� ���
            if (transform.parent != null && transform.parent.TryGetComponent(out EnemyMovement enemy))
            {
                enemy.CancelAttack();
            }
        }
    }
    private void ApplyParryProtection()
    {
        // �÷��̾��� �и� ���� ����
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null && player.TryGetComponent(out HurtPlayer hurtPlayer))
        {
            hurtPlayer.StartParry();
        }

        // ���� �и� ���� ����
        if (transform.parent != null && transform.parent.TryGetComponent(out enemyTest enemy))
        {
            enemy.StartParry();
        }
    }
    private GameObject InstantiateParryEffect(Vector3 position)
    {
        if (parryEffectPrefab != null)
        {
            GameObject effect = Instantiate(parryEffectPrefab, position, Quaternion.identity);
            return effect;
        }
        return null;
    }

    // �и� ����Ʈ�� Ư�� ��ġ�� �̵��ϴ� �ڷ�ƾ
    private IEnumerator MoveParryEffect(GameObject effect, Vector3 targetPosition)
    {
        float duration = 0.3f; // �̵� �ð�
        float elapsed = 0f;
        Vector3 startPosition = effect.transform.position;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            effect.transform.position = Vector3.Lerp(startPosition, targetPosition, elapsed / duration);
            yield return null;
        }

        effect.transform.position = targetPosition; // ���� ��ġ ����
        Destroy(effect, 0.1f); // 0.4�� �� ����
    }

    private void CallParryParticle(Vector3 position)
    {
        if (parryParticlePrefab != null)
        {
            GameObject particleObject = Instantiate(parryParticlePrefab, position, Quaternion.identity);
            ParticleSystem particleSystem = particleObject.GetComponent<ParticleSystem>();

            if (particleSystem != null)
            {
                particleSystem.Play(); 
            }

            Destroy(particleObject, 5f); // 5�� �� �ڵ� ����
        }
    }
}
