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
            Debug.Log("  패링 시도: 플레이어와 적의 공격이 감지됨");

            //  플레이어와 가장 가까운 적 찾기
            enemyTest closestEnemy = FindClosestEnemy(other.transform);

            //  내가 가장 가까운 적이라면 패링 실행
            if (closestEnemy != null && closestEnemy.gameObject == transform.parent.gameObject)
            {
                Debug.Log("  패링 성공! 가장 가까운 적이 패링 처리됨");

                // 패링 이펙트 생성 후 이동
                GameObject parryEffect = InstantiateParryEffect(other.transform.position);
                CallParryParticle(other.transform.position);

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
            else
            {
                Debug.Log("  패링 실패: 가장 가까운 적이 아님");
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
