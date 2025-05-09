using UnityEngine;

public class DevaSkillHitbox : MonoBehaviour
{
    public Transform attackerTransform; // 공격자 위치 (Deba의 transform)
    public int damage = 2;

    private void OnTriggerEnter2D(Collider2D other)
    {
        // 일반 적 처리
        enemyTest enemy = other.GetComponent<enemyTest>();
        if (enemy != null)
        {
            enemy.TakeDamage(damage, false, true, attackerTransform);
            Debug.Log("지속 스킬 히트 (적): " + other.name);
            return;
        }

        // 보스 처리
        BossHurt boss = other.GetComponent<BossHurt>();
        if (boss != null)
        {
            boss.TakeDamage(damage, false, true); // 3개 인자
            Debug.Log("지속 스킬 히트 (보스): " + other.name);
            return;
        }
    }
}
