
using System.Collections;
using UnityEngine;

public class SkillObject : MonoBehaviour
{
    public int damage = 30;
    public bool fromDeba = true;
    public bool fromAdam = false;
    public string targetTag = "Enemy";

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag(targetTag))
        {
            enemyTest enemy = other.GetComponent<enemyTest>();
            if (enemy != null)
            {
                // ������ ������ ���޸�! �������� enemyTest ���ο��� ó����
                enemy.TakeDamage(damage, fromAdam, fromDeba);
            }
        }
    }

    private void Start()
    {
        Destroy(gameObject, 0.8f); // 1�� �� �ڵ� ���� (���Ѵٸ�)
    }
}
