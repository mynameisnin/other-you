using UnityEngine;

public class DevaSkillHitbox : MonoBehaviour
{
    public Transform attackerTransform; // 공격자 위치 (Deba의 transform)
    public int damage = 2;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Enemy"))
        {
            enemyTest enemy = other.GetComponent<enemyTest>();
            if (enemy != null)
            {
                enemy.TakeDamage(damage, false, true, attackerTransform); // ← 수정된 부분
                Debug.Log("지속 스킬 히트: " + other.name);
            }
        }
    }
}
