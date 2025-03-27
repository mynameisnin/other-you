using UnityEngine;

public class SkillFlipHandler : MonoBehaviour
{
    public SpriteRenderer adamSprite;       // �ƴ��� SpriteRenderer ����
    public Transform skillSpawnPoint;       // ���� ��ġ (�ʿ��ϸ� null ����)

    [Header("Spawn Offset Settings")]
    public Vector2 rightOffset = new Vector2(1f, 0f); // �ƴ��� �������� �� �� ������
    public Vector2 leftOffset = new Vector2(-1f, 0f); // �ƴ��� ������ �� �� ������

    void Start()
    {
        FlipPositionAndCollider();
    }

    void FlipPositionAndCollider()
    {
        // ��ġ ����
        if (adamSprite != null)
        {
            Vector2 offset = adamSprite.flipX ? leftOffset : rightOffset;

            Vector3 basePos = adamSprite.transform.position;
            basePos += new Vector3(offset.x, offset.y, 0f); // �� ���� ���� ���
            transform.position = basePos;
        }

        // �ݶ��̴� ����
        BoxCollider2D box = GetComponent<BoxCollider2D>();
        if (box != null)
        {
            Vector2 originalOffset = box.offset;
            originalOffset.x = adamSprite.flipX ? -Mathf.Abs(originalOffset.x) : Mathf.Abs(originalOffset.x);
            box.offset = originalOffset;
        }

        // ��������Ʈ Flip
        SpriteRenderer skillSprite = GetComponent<SpriteRenderer>();
        if (skillSprite != null)
        {
            skillSprite.flipX = adamSprite != null && adamSprite.flipX;
        }
    }

}
