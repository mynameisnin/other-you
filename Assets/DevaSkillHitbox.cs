using UnityEngine;

public class DevaSkillHitbox : MonoBehaviour
{
    public int damage = 2;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Enemy"))
        {
            enemyTest enemy = other.GetComponent<enemyTest>();
            if (enemy != null)
            {
                enemy.TakeDamage(damage, false, true, transform.root);
                Debug.Log(" 지속 스킬 히트: " + other.name);
            }
        }
    }
}
