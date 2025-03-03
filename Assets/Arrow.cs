using System.Collections;
using UnityEngine;

public class Arrow : MonoBehaviour
{
    public float lifetime = 3f;
    private Rigidbody2D rb;
    private bool hasHit = false;

    public float arrowSpeed = 10f; // ȭ�� �ӵ� (���� �߻��)
    public float arcForceX = 6f;   // ������ �߻� - X�� ��
    public float arcForceY = 8f;   // ������ �߻� - Y�� ��

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        if (rb == null)
        {
            Debug.LogError("ȭ�쿡 Rigidbody2D�� ����!");
            return;
        }

        Destroy(gameObject, lifetime); // ���� �ð� �� ����
    }

    // ȭ�� �߻� ������ �����ϴ� �Լ�
    public void SetDirection(bool isRight, bool isHighAngle)
    {
        if (rb == null) return;

        float direction = isRight ? 1f : -1f;

        if (isHighAngle)
        {
            // ���������� �߻� (��� �ӵ� ����)
            rb.velocity = new Vector2(direction * arcForceX, arcForceY);
        }
        else
        {
            // �������� �߻�
            rb.velocity = new Vector2(direction * arrowSpeed, 0f);
        }
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (hasHit) return; // �̹� �浹������ �� �̻� ó�� �� ��

        if (collision.CompareTag("Player"))
        {
            Debug.Log("�÷��̾� ����!");
            hasHit = true;
            Destroy(gameObject); // �÷��̾� ������ ����
        }
        else if (collision.CompareTag("Wall") || collision.CompareTag("Ground"))
        {
            Debug.Log("���̳� �ٴڿ� �浹!");
            Destroy(gameObject); // ���� �ٴڿ� ������ ����
        }
    }
}
