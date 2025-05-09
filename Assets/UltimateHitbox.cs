using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UltimateHitbox : MonoBehaviour
{
    public int damageAmount = 50;
    public string targetTag = "Enemy";
    public bool fromAdam = true;
    public bool fromDeba = false;
    public Transform attackerTransform; // ���� �������ִ� ������ ��ġ
    [Header("��ġ ���� (���� ����)")]
    public Transform adamSprite;

    public Vector2 offsetRight = new Vector2(1.5f, 0f);
    public Vector2 offsetLeft = new Vector2(-1.5f, 0f);

    private void LateUpdate()
    {
        if (adamSprite != null)
        {
            bool facingLeft = adamSprite.GetComponent<SpriteRenderer>().flipX;
            Vector2 desiredOffset = facingLeft ? offsetLeft : offsetRight;
            transform.localPosition = desiredOffset;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag(targetTag))
        {
            // �Ϲ� ��
            enemyTest enemy = other.GetComponent<enemyTest>();
            if (enemy != null)
            {
                Debug.Log("Hit enemy with Ultimate Skill!");
                enemy.TakeDamage(damageAmount, fromAdam, fromDeba, attackerTransform);
                return;
            }

            // ����
            BossHurt boss = other.GetComponent<BossHurt>();
            if (boss != null)
            {
                Debug.Log("Hit boss with Ultimate Skill!");
                boss.TakeDamage(damageAmount, fromAdam, fromDeba); // <- 3�� ����
                return;
            }
        }
    }
}
