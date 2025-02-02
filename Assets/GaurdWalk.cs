using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GaurdWalk : MonoBehaviour
{
    public float moveSpeed = 2f;  // �̵� �ӵ�
    public Transform player; // �÷��̾� ������Ʈ ���� �Ҵ�
    public Transform[] ignoreObjects; // �߰������� �浹�� ������ ������Ʈ��
    public Collider2D obstacleCollider; // �� �浹 ������ �ݶ��̴�
    public float rayLength = 0.5f; // Raycast ����

    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;
    private Vector2 moveDirection = Vector2.right; // �ʱ� �̵� ���� (������)

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        // **�÷��̾�� �浹 ����**
        if (player != null)
        {
            IgnoreCollisionWith(player.GetComponent<Collider2D>());
        }

        // **�߰����� ������Ʈ��� �浹 ����**
        foreach (Transform obj in ignoreObjects)
        {
            if (obj != null)
            {
                Collider2D objCollider = obj.GetComponent<Collider2D>(); // Collider2D ��������
                IgnoreCollisionWith(objCollider);
            }
        }
    }

    void FixedUpdate()
    {
        // **Ray�� �� ���ʿ��� �߻��ϵ��� ��ġ ����**
        Vector2 rayOrigin = (Vector2)transform.position + new Vector2(0, 0.5f); // �߽ɺ��� ����

        // ��ֹ� ���� (���� ����)
        RaycastHit2D hitLeft = Physics2D.Raycast(rayOrigin, Vector2.left, rayLength, 1 << obstacleCollider.gameObject.layer);
        RaycastHit2D hitRight = Physics2D.Raycast(rayOrigin, Vector2.right, rayLength, 1 << obstacleCollider.gameObject.layer);

        // **Ray �ð�ȭ - ���ʿ��� �� �� ���̵��� ����**
        Debug.DrawRay(rayOrigin, Vector2.left * rayLength, hitLeft ? Color.green : Color.red, 0.1f);
        Debug.DrawRay(rayOrigin, Vector2.right * rayLength, hitRight ? Color.green : Color.blue, 0.1f);

        // ��ֹ� ���� �� ���� ����
        if ((moveDirection.x > 0 && hitRight) || (moveDirection.x < 0 && hitLeft))
        {
            FlipDirection();
        }

        // �̵� ����
        rb.velocity = new Vector2(moveDirection.x * moveSpeed, rb.velocity.y);
    }

    void FlipDirection()
    {
        moveDirection.x *= -1; // �̵� ���� ����
        spriteRenderer.flipX = !spriteRenderer.flipX; // ��������Ʈ ����
    }

    void IgnoreCollisionWith(Collider2D objCollider)
    {
        if (objCollider != null)
        {
            Collider2D enemyCollider = GetComponent<Collider2D>();
            if (enemyCollider != null)
            {
                Physics2D.IgnoreCollision(enemyCollider, objCollider, true);
            }
        }
    }
}
