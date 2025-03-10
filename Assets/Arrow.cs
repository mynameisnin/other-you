using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Arrow : MonoBehaviour
{
    public float straightSpeed = 10f; // ���� �ӵ�
    public float arcSpeed = 5f; // ������ �ӵ�
    public float arcHeight = 2f; // ������ ����


    private Rigidbody2D rb;
    private bool isHighAngle;
    private Vector2 targetDirection;
    private bool isFacingRight;
    public int damage = 10;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();

        if (rb == null)
        {
            Debug.LogError(" Rigidbody2D�� ����! Rigidbody2D�� �߰��ϼ���.", this);
        }
    }
    


    void StraightShot()
    {
        if (rb == null)
        {
            Debug.LogError("Rigidbody2D�� �������� ����! Arrow �������� Ȯ���ϼ���.");
            return;
        }

        rb.velocity = targetDirection * straightSpeed;

        //  ȭ�� ���⿡ �°� ȸ��
        float angle = Mathf.Atan2(rb.velocity.y, rb.velocity.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, angle);
    }
    public void SetDirection(bool facingRight, bool highAngle, Vector2 targetPosition)
    {
        isFacingRight = facingRight;
        isHighAngle = highAngle;

        targetDirection = facingRight ? Vector2.right : Vector2.left;

        if (isHighAngle)
        {
            StartCoroutine(ArcShot(targetPosition)); // ?? ��ǥ ��ġ ����
        }
        else
        {
            StraightShot();
        }
    }

    IEnumerator ArcShot(Vector2 targetPosition)
    {
        if (rb == null)
        {
            Debug.LogError("Rigidbody2D�� �������� ����! Arrow �������� Ȯ���ϼ���.");
            yield break;
        }

        Vector2 startPos = transform.position;
        Vector2 direction = targetPosition - startPos; // ��ǥ ��ġ������ ����
        float distanceX = Mathf.Abs(direction.x); // ���� �Ÿ�
        float distanceY = direction.y; // ���� �Ÿ�

        float gravity = Mathf.Abs(Physics2D.gravity.y); // �߷� ��

        // ?? ��ǥ���� �����ϴ� �ð��� `arcSpeed`�� ���� (arcSpeed�� Ŭ���� ����)
        float timeToTarget = distanceX / arcSpeed; // �ӵ��� ������� ���� �ð� ����

        // ?? �ʱ� ���� �ӵ� ��� (arcSpeed �ݿ�)
        float initialVelocityY = (distanceY + (0.5f * gravity * timeToTarget * timeToTarget)) / timeToTarget;

        // ?? ȭ�� �ӵ� ���� ����
        rb.velocity = new Vector2(targetDirection.x * arcSpeed, initialVelocityY);

        while (true)
        {
            // ?? ���� �ӵ� �������� ȸ�� ����
            float angle = Mathf.Atan2(rb.velocity.y, rb.velocity.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0, 0, angle);

            yield return null;
        }
    }





    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player")) // �÷��̾� �浹
        {
            // �÷��̾�� ����� �ֱ�
            HurtPlayer player = collision.GetComponent<HurtPlayer>();
            if (player != null)
            {
                int damage = 10; // ȭ�� �⺻ ����� (���ϴ� ������ ���� ����)
                player.TakeDamage(damage);
            }

            // ȭ�� ����
            Destroy(gameObject);
        }
        else if (collision.CompareTag("isGround")) // ���� �浹�ϸ� ����
        {
            Destroy(gameObject);
        }
    }

}
