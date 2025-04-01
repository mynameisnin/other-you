using System.Collections.Generic;
using UnityEngine;

public class UltimateHitbox : MonoBehaviour
{
    public int damageAmount = 50;
    private HashSet<GameObject> hitEnemies = new HashSet<GameObject>();

    [Header("반전 설정")]
    public Transform adamSprite; // SpriteRenderer 있는 오브젝트 (flipX 참조용)
    public Vector2 originalOffset = new Vector2(1.5f, 0f); // 오른쪽 위치 기준

    void LateUpdate()
    {
        if (adamSprite != null)
        {
            // Sprite가 flipX 상태라면 왼쪽, 아니라면 오른쪽
            bool facingLeft = adamSprite.GetComponent<SpriteRenderer>().flipX;
            Vector2 newOffset = facingLeft ? new Vector2(-originalOffset.x, originalOffset.y) : originalOffset;

            transform.localPosition = newOffset;
        }
    }
    public void DealUltimateDamage()
    {
        hitEnemies.Clear(); // 매 타격마다 초기화해도 되고, 궁극기 전체 중에만 한 번 하게 해도 OK

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
                    Debug.Log("궁극기 타격 성공!");
                }
            }
        }
    }
}
