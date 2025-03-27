using UnityEngine;

public class BladeHitbox : MonoBehaviour
{
    private int damage;
    private bool fromAdam;
    private bool fromDeba;

    public string targetTag = "Enemy";
    private bool active = false;

    public void ActivateSkill(int dmg, bool adam, bool deba)
    {
        damage = dmg;
        fromAdam = adam;
        fromDeba = deba;
        active = true;

        Invoke("Deactivate", 0.3f); // 타격 활성화 시간
    }

    private void Deactivate()
    {
        active = false;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!active) return;
        if (!other.CompareTag(targetTag)) return;

        enemyTest enemy = other.GetComponent<enemyTest>();
        if (enemy != null)
        {
            enemy.TakeDamage(damage, fromAdam, fromDeba, transform);
        }
    }
}
