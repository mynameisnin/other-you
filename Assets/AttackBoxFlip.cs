using UnityEngine;

public class SlashColliderManual : MonoBehaviour
{
    public BoxCollider2D attackCollider;   // ���� �ݶ��̴�
    public SpriteRenderer characterSprite; // ĳ������ SpriteRenderer

    // ������ �ݶ��̴� ���� (�ø� X == false)
    public Vector2 rightOffset = new Vector2(0.5f, 0.0f); // ������ ��ġ
    public Vector2 rightSize = new Vector2(1.2f, 0.4f);   // ������ ũ��
    public float rightRotation = 15f;                    // ������ ȸ��
    public Vector3 rightPosition = new Vector3(0.5f, 0.0f, 0.0f); // ������ ������

    // ���� �ݶ��̴� ���� (�ø� X == true)
    public Vector2 leftOffset = new Vector2(-0.5f, 0.0f); // ���� ��ġ
    public Vector2 leftSize = new Vector2(1.2f, 0.4f);    // ���� ũ��
    public float leftRotation = -15f;                    // ���� ȸ��
    public Vector3 leftPosition = new Vector3(-0.5f, 0.0f, 0.0f); // ���� ������

    void Start()
    {
        if (attackCollider == null || characterSprite == null)
        {
            Debug.LogError("SlashColliderManual: �ʿ��� ������Ʈ�� ������� �ʾҽ��ϴ�!");
        }
    }

    void Update()
    {
        // ĳ������ �ø� ���¿� ���� �ݶ��̴� ����
        UpdateCollider();
    }

    void UpdateCollider()
    {
        if (characterSprite.flipX)
        {
            // ���� �������� �ø�
            attackCollider.offset = leftOffset;                                  // ���� Offset ����
            attackCollider.size = leftSize;                                     // ���� Size ����
            attackCollider.transform.localRotation = Quaternion.Euler(0, 0, leftRotation); // ���� Rotation ����
            attackCollider.transform.localPosition = leftPosition;              // ���� Position ����
        }
        else
        {
            // ������ �������� �ø�
            attackCollider.offset = rightOffset;                                // ������ Offset ����
            attackCollider.size = rightSize;                                    // ������ Size ����
            attackCollider.transform.localRotation = Quaternion.Euler(0, 0, rightRotation); // ������ Rotation ����
            attackCollider.transform.localPosition = rightPosition;             // ������ Position ����
        }
    }

    // �� �信�� �ݶ��̴��� �ð������� ǥ��
    void OnDrawGizmos()
    {
        if (attackCollider == null) return;

        // Gizmos ���� ����
        Gizmos.color = Color.red;

        // ���� �ݶ��̴��� ũ��� ��ġ�� �ð������� ǥ��
        Vector3 position = attackCollider.transform.position + (Vector3)attackCollider.offset;
        Vector3 size = new Vector3(attackCollider.size.x, attackCollider.size.y, 0);
        Gizmos.matrix = Matrix4x4.TRS(position, attackCollider.transform.rotation, Vector3.one);
        Gizmos.DrawWireCube(Vector3.zero, size); // �ݶ��̴��� ũ��� ��ġ ǥ��
    }
}
