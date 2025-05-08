
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
        enemyTest enemy = other.GetComponent<enemyTest>();
        if (enemy != null)
        {
            enemy.TakeDamage(damage, fromAdam, fromDeba, transform.root);
            return;
        }

        BossHurt boss = other.GetComponent<BossHurt>();
        if (boss != null)
        {
            boss.TakeDamage(damage, fromAdam, fromDeba); // 3개 인자
            return;
        }
    }

    private void Start()
    {
        Destroy(gameObject, 0.8f); // 1초 후 자동 제거 (원한다면)
    }
}
