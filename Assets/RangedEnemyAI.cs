using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RangedEnemyAI : MonoBehaviour
{
    public Transform player; // �÷��̾� ��ġ
    public Transform firePoint; // ȭ�� �߻� ��ġ
    public GameObject arrowPrefab; // ȭ�� ������

    public float detectRange = 7f; // ���� ����
    public float straightShotRange = 3f; // ���� �߻� ����
    public float attackCooldown = 2f; // ���� ��Ÿ��

    private Animator animator;
    private bool canAttack = true;
    private bool isFacingRight = true;
    private bool lastAttackWasHighAngle; // ������ ���� ���� ����

    void Start()
    {
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        DetectPlayer();
        FlipDirection();
    }

    void DetectPlayer()
    {
        Collider2D playerInRange = Physics2D.OverlapCircle(transform.position, detectRange, LayerMask.GetMask("Player"));

        if (playerInRange != null && canAttack)
        {
            float distanceToPlayer = Vector2.Distance(transform.position, player.position);

            if (distanceToPlayer <= straightShotRange)
            {
                StartCoroutine(AttackRoutine(false)); // ���� ����
            }
            else
            {
                StartCoroutine(AttackRoutine(true)); // ������ ����
            }
        }
    }

    void FlipDirection()
    {
        if (player == null) return;

        bool playerOnRight = player.position.x > transform.position.x;
        if (playerOnRight && !isFacingRight)
        {
            Flip();
        }
        else if (!playerOnRight && isFacingRight)
        {
            Flip();
        }
    }

    void Flip()
    {
        isFacingRight = !isFacingRight;
        transform.Rotate(0f, 180f, 0f);

        animator.SetTrigger("Flip");
    }


    IEnumerator AttackRoutine(bool isHighAngle)
    {
        canAttack = false;
        lastAttackWasHighAngle = isHighAngle; // ?? ���� ��� ����

        if (isHighAngle)
        {
            animator.SetTrigger("AttackHigh"); // �� ���� �ִϸ��̼�
        }
        else
        {
            animator.SetTrigger("AttackStraight"); // ���� ���� �ִϸ��̼�
        }

        yield return new WaitForSeconds(0.5f); // �ִϸ��̼� ��� �ð�

        //  �ִϸ��̼� �̺�Ʈ���� ShootArrowEvent()�� �����
        yield return new WaitForSeconds(attackCooldown);
        canAttack = true;
    }

    public void ShootArrowEvent()
    {
        ShootArrow(lastAttackWasHighAngle); //  ������ ���� ��� ����
    }


    void ShootArrow(bool isHighAngle)
    {
        if (arrowPrefab == null || firePoint == null || player == null)
        {
            Debug.LogError(" ȭ�� ������, FirePoint �Ǵ� Player�� �������� ����!");
            return;
        }

        GameObject arrow = Instantiate(arrowPrefab, firePoint.position, Quaternion.identity);

        if (arrow == null)
        {
            Debug.LogError(" ȭ���� �������� �ʾ���! �������� Ȯ���ϼ���.");
            return;
        }

        Arrow arrowScript = arrow.GetComponent<Arrow>();

        if (arrowScript == null)
        {
            Debug.LogError(" Arrow ��ũ��Ʈ�� �������� ����! �������� �ùٸ��� Ȯ���ϼ���.", arrow);
            return;
        }

        Rigidbody2D arrowRb = arrow.GetComponent<Rigidbody2D>();

        if (arrowRb == null)
        {
            Debug.LogError(" Rigidbody2D�� ȭ�� ������Ʈ���� �������� ����! ȭ�� �������� Ȯ���ϼ���.", arrow);
            return;
        }

        // ?? �÷��̾� ��ġ�� ��ǥ�� ����
        arrowScript.SetDirection(isFacingRight, isHighAngle, player.position);
    }



    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectRange);
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, straightShotRange);
    }
}