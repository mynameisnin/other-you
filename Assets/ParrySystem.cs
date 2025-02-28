using System.Collections;
using UnityEngine;

public class ParrySystem : MonoBehaviour
{
    public GameObject parryEffectPrefab; // 패링 이펙트 프리팹
    public GameObject parryParticlePrefab; // 패링 파티클 (별도 관리)
    void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log($"충돌 감지됨: {other.gameObject.name} (태그: {other.tag})");

        // 플레이어의 공격과 충돌했는지 확인
        if (other.CompareTag("PlayerAttack") && gameObject.CompareTag("EnemyAttack"))
        {
            Debug.Log("  패링 성공! 플레이어와 적의 공격이 동시에 감지됨");

            // 패링 이펙트 생성 후 이동
            GameObject parryEffect = InstantiateParryEffect(other.transform.position);
            CallParryParticle(other.transform.position);
            // 특정 오브젝트로 패링 이펙트 이동 (예: 플레이어 위치)
            if (parryEffect != null)
            {
                StartCoroutine(MoveParryEffect(parryEffect, other.transform.position));
            }
            ApplyParryProtection();
            // 적의 공격 취소
            if (transform.parent != null && transform.parent.TryGetComponent(out EnemyMovement enemy))
            {
                enemy.CancelAttack();
            }
        }
    }
    private void ApplyParryProtection()
    {
        // 플레이어의 패링 무적 적용
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null && player.TryGetComponent(out HurtPlayer hurtPlayer))
        {
            hurtPlayer.StartParry();
        }

        // 적의 패링 무적 적용
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

    // 패링 이펙트를 특정 위치로 이동하는 코루틴
    private IEnumerator MoveParryEffect(GameObject effect, Vector3 targetPosition)
    {
        float duration = 0.3f; // 이동 시간
        float elapsed = 0f;
        Vector3 startPosition = effect.transform.position;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            effect.transform.position = Vector3.Lerp(startPosition, targetPosition, elapsed / duration);
            yield return null;
        }

        effect.transform.position = targetPosition; // 최종 위치 고정
        Destroy(effect, 0.1f); // 0.4초 후 삭제
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

            Destroy(particleObject, 5f); // 5초 후 자동 삭제
        }
    }
}
