using UnityEngine;

public class DevaSkillHitbox : MonoBehaviour
{
    public Transform attackerTransform; // ������ ��ġ (Deba�� transform)
    public int damage = 2;

    private void OnTriggerEnter2D(Collider2D other)
    {
        // �Ϲ� �� ó��
        enemyTest enemy = other.GetComponent<enemyTest>();
        if (enemy != null)
        {
            enemy.TakeDamage(damage, false, true, attackerTransform);
            Debug.Log("���� ��ų ��Ʈ (��): " + other.name);
            return;
        }

        // ���� ó��
        BossHurt boss = other.GetComponent<BossHurt>();
        if (boss != null)
        {
            boss.TakeDamage(damage, false, true); // 3�� ����
            Debug.Log("���� ��ų ��Ʈ (����): " + other.name);
            return;
        }
    }
}
