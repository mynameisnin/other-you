
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
                // 적에게 데미지 전달만! 나머지는 enemyTest 내부에서 처리됨
                enemy.TakeDamage(damage, fromAdam, fromDeba);
            }
        }
    }

    private void Start()
    {
        Destroy(gameObject, 0.8f); // 1초 후 자동 제거 (원한다면)
    }
}
