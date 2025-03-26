using UnityEngine;

public class SkillFlipHandler : MonoBehaviour
{
    public SpriteRenderer adamSprite;       // 아담의 SpriteRenderer 연결
    public Transform skillSpawnPoint;       // 기준 위치 (필요하면 null 가능)

    [Header("Spawn Offset Settings")]
    public Vector2 rightOffset = new Vector2(1f, 0f); // 아담이 오른쪽을 볼 때 오프셋
    public Vector2 leftOffset = new Vector2(-1f, 0f); // 아담이 왼쪽을 볼 때 오프셋

    void Start()
    {
        FlipPositionAndCollider();
    }

    void FlipPositionAndCollider()
    {
        // 위치 조절
        if (adamSprite != null)
        {
            Vector2 offset = adamSprite.flipX ? leftOffset : rightOffset;

            Vector3 basePos = adamSprite.transform.position;
            basePos += new Vector3(offset.x, offset.y, 0f); // ← 에러 없는 방식
            transform.position = basePos;
        }

        // 콜라이더 반전
        BoxCollider2D box = GetComponent<BoxCollider2D>();
        if (box != null)
        {
            Vector2 originalOffset = box.offset;
            originalOffset.x = adamSprite.flipX ? -Mathf.Abs(originalOffset.x) : Mathf.Abs(originalOffset.x);
            box.offset = originalOffset;
        }

        // 스프라이트 Flip
        SpriteRenderer skillSprite = GetComponent<SpriteRenderer>();
        if (skillSprite != null)
        {
            skillSprite.flipX = adamSprite != null && adamSprite.flipX;
        }
    }

}
