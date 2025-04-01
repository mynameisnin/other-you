using System.Collections.Generic;
using UnityEngine;

public class UltimateHitbox : MonoBehaviour
{
    public int damageAmount = 50;
    private HashSet<GameObject> hitEnemies = new HashSet<GameObject>();

    public void DealUltimateDamage()
    {
        hitEnemies.Clear(); // �� Ÿ�ݸ��� �ʱ�ȭ�ص� �ǰ�, �ñر� ��ü �߿��� �� �� �ϰ� �ص� OK

        Collider2D[] hitList = Physics2D.OverlapBoxAll(transform.position, GetComponent<Collider2D>().bounds.size, 0f);
        foreach (var hit in hitList)
        {
            if (hit.CompareTag("Enemy") && !hitEnemies.Contains(hit.gameObject))
            {
                enemyTest enemy = hit.GetComponent<enemyTest>();
                if (enemy != null)
                {
                    enemy.TakeDamage(damageAmount, true, false, transform.root);
                    hitEnemies.Add(hit.gameObject);
                    Debug.Log("�ñر� Ÿ�� ����!");
                }
            }
        }
    }
}
