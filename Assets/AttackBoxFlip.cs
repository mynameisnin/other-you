using UnityEngine;

public class SlashColliderManual : MonoBehaviour
{
    public BoxCollider2D attackCollider;   // 공격 콜라이더
    public SpriteRenderer characterSprite; // 캐릭터의 SpriteRenderer

    // 오른쪽 콜라이더 설정 (플립 X == false)
    public Vector2 rightOffset = new Vector2(0.5f, 0.0f); // 오른쪽 위치
    public Vector2 rightSize = new Vector2(1.2f, 0.4f);   // 오른쪽 크기
    public float rightRotation = 15f;                    // 오른쪽 회전
    public Vector3 rightPosition = new Vector3(0.5f, 0.0f, 0.0f); // 오른쪽 포지션

    // 왼쪽 콜라이더 설정 (플립 X == true)
    public Vector2 leftOffset = new Vector2(-0.5f, 0.0f); // 왼쪽 위치
    public Vector2 leftSize = new Vector2(1.2f, 0.4f);    // 왼쪽 크기
    public float leftRotation = -15f;                    // 왼쪽 회전
    public Vector3 leftPosition = new Vector3(-0.5f, 0.0f, 0.0f); // 왼쪽 포지션

    void Start()
    {
        if (attackCollider == null || characterSprite == null)
        {
            Debug.LogError("SlashColliderManual: 필요한 컴포넌트가 연결되지 않았습니다!");
        }
    }

    void Update()
    {
        // 캐릭터의 플립 상태에 따라 콜라이더 설정
        UpdateCollider();
    }

    void UpdateCollider()
    {
        if (characterSprite.flipX)
        {
            // 왼쪽 방향으로 플립
            attackCollider.offset = leftOffset;                                  // 왼쪽 Offset 설정
            attackCollider.size = leftSize;                                     // 왼쪽 Size 설정
            attackCollider.transform.localRotation = Quaternion.Euler(0, 0, leftRotation); // 왼쪽 Rotation 설정
            attackCollider.transform.localPosition = leftPosition;              // 왼쪽 Position 설정
        }
        else
        {
            // 오른쪽 방향으로 플립
            attackCollider.offset = rightOffset;                                // 오른쪽 Offset 설정
            attackCollider.size = rightSize;                                    // 오른쪽 Size 설정
            attackCollider.transform.localRotation = Quaternion.Euler(0, 0, rightRotation); // 오른쪽 Rotation 설정
            attackCollider.transform.localPosition = rightPosition;             // 오른쪽 Position 설정
        }
    }

    // 씬 뷰에서 콜라이더를 시각적으로 표시
    void OnDrawGizmos()
    {
        if (attackCollider == null) return;

        // Gizmos 색상 설정
        Gizmos.color = Color.red;

        // 현재 콜라이더의 크기와 위치를 시각적으로 표시
        Vector3 position = attackCollider.transform.position + (Vector3)attackCollider.offset;
        Vector3 size = new Vector3(attackCollider.size.x, attackCollider.size.y, 0);
        Gizmos.matrix = Matrix4x4.TRS(position, attackCollider.transform.rotation, Vector3.one);
        Gizmos.DrawWireCube(Vector3.zero, size); // 콜라이더의 크기와 위치 표시
    }
}
