using UnityEngine;

public class DevaSkillHitbox : MonoBehaviour
{
    public Transform attackerTransform; // ������ ��ġ (Deba�� transform)
    public int damage = 2;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Enemy"))
        {
            enemyTest enemy = other.GetComponent<enemyTest>();
            if (enemy != null)
            {
                enemy.TakeDamage(damage, false, true, attackerTransform); // �� ������ �κ�
                Debug.Log("���� ��ų ��Ʈ: " + other.name);
            }
        }
    }
}
