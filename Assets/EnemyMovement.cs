using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMovement : MonoBehaviour
{
    private Animator enemyAnimator;
    private Rigidbody2D rb;

    public Transform player;
    public Transform groundCheck;
    public LayerMask groundLayer;
    public LayerMask playerLayer;
    public LayerMask obstacleLayer;
    public Transform attackBox; // ���� �ڽ� (Trigger Collider)
    public float attackBoxSize = 1.5f;

    [Header("Detection Settings")]
    public float detectionRange = 5f;
    public float attackRange = 1.5f;
    public float speed = 3f;
    public bool isFacingRight = true;
    private bool isChasing = false;
    private bool isAttacking = false;
    private bool isTurning = false;
    private bool isStunned = false; //  ���� ���� ���� �߰�

    private bool isPatrolling = true;
    private float patrolTime = 2f;
    private float patrolTimer = 0f;
    private float patrolDirection = 1f; // -1�̸� ����, 1�̸� ������
    public int attackDamage = 100; // �⺻ ���� ������

    private bool isBlocked = false; //  ���� �տ� �����ִ��� üũ

    public Collider2D frontCollider; //  �±� ������ Ʈ���� �ݶ��̴�


    public List<string> ignoreEnemyNames = new List<string>(); // ������ �� �̸� ����Ʈ
    void Start()
    {
        enemyAnimator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        patrolTimer = patrolTime;
        FindPlayer();
    }

    void Update()
    {
      
        if (isStunned || isBlocked) return; //  ���� ���������� �̵� X
        if (!isAttacking && !isTurning)
        {
            DetectPlayer();
        }

        if (isChasing && !isAttacking && !isTurning)
        {
            ChasePlayer();
        }
        else if (isPatrolling && !isAttacking && !isTurning)
        {
            Patrol();
        }
        if (player == null || !player.gameObject.activeInHierarchy)
        {
            FindPlayer(); // �÷��̾ ����ġ�Ǿ��ų� ��������� �ٽ� ã��
        }
    }
    void FindPlayer()
    {
        GameObject adam = GameObject.FindGameObjectWithTag("AdamCamPosition");
        GameObject deba = GameObject.FindGameObjectWithTag("DevaCamPosition");

        if (adam != null && adam.activeInHierarchy)
        {
            player = adam.transform;
        }
        else if (deba != null && deba.activeInHierarchy)
        {
            player = deba.transform;
        }

    }


    void DetectPlayer()
    {
        // ���� ���¿����� �÷��̾� ���� �ߴ�
        if (isStunned) return;

        RaycastHit2D hitRight = Physics2D.Raycast(transform.position, Vector2.right, detectionRange, playerLayer);
        RaycastHit2D hitLeft = Physics2D.Raycast(transform.position, Vector2.left, detectionRange, playerLayer);

        if (hitRight.collider != null)
        {
            isChasing = true;
            isPatrolling = false;
            if (!isFacingRight)
            {
                StartCoroutine(FlipAndTurn());
            }
        }
        else if (hitLeft.collider != null)
        {
            isChasing = true;
            isPatrolling = false;
            if (isFacingRight)
            {
                StartCoroutine(FlipAndTurn());
            }
        }
    }
    void ChasePlayer()
    {
        if (player == null || isBlocked) return; //  �տ� ���� ������ �̵� X

        float distance = Vector2.Distance(transform.position, player.position);

        if (distance <= attackRange)
        {
            StartCoroutine(Attack());
        }
        else
        {
            enemyAnimator.SetBool("isWalking", true);
            Vector2 targetPosition = new Vector2(player.position.x, transform.position.y);
            transform.position = Vector2.MoveTowards(transform.position, targetPosition, speed * Time.deltaTime);

            if ((player.position.x > transform.position.x && !isFacingRight) ||
                (player.position.x < transform.position.x && isFacingRight))
            {
                StartCoroutine(FlipAndTurn());
            }
        }
    }


    IEnumerator Attack()
    {
        isAttacking = true;
        enemyAnimator.SetBool("isWalking", false);
        enemyAnimator.SetTrigger("Attack");

        yield return new WaitForSeconds(0.1f); // �ִϸ��̼� ���� ���

        // ���� �ִϸ��̼��� ���� ������ ���
        while (enemyAnimator.GetCurrentAnimatorStateInfo(0).IsName("Attack"))
        {
            yield return null; // �� ������ ���
        }

        // ���� �ִϸ��̼��� ���� ��, �÷��̾ ���� ���� ���� ���� �ִ��� Ȯ��
        if (!CheckPlayerInAttackRange())
        {
            Debug.Log("�÷��̾ ������ ������Ƿ� �̵� �簳");
            isAttacking = false;
        }
        else
        {
            Debug.Log("�÷��̾ ���� ���� ����, �ٽ� ���� ����");
            StartCoroutine(Attack()); // ���� ���� ����
        }
    }

    // �÷��̾ ���� ���� ���� ���� �ִ��� Ȯ���ϴ� �Լ�
    bool CheckPlayerInAttackRange()
    {
        // ���� ���¿����� ���� ���� �ߴ�
        if (isStunned) return false;

        Collider2D hit = Physics2D.OverlapCircle(attackBox.position, attackBoxSize, playerLayer);
        return hit != null;
    }


    void Patrol()
    {
        patrolTimer -= Time.deltaTime;

        if (patrolTimer <= 0)
        {
            patrolDirection *= -1f;
            StartCoroutine(FlipAndTurn());
            patrolTimer = patrolTime;
            patrolTimer = GetRandomPatrolTime(); // ������ Ÿ�̸� ����
        }

        Vector2 patrolTarget = new Vector2(transform.position.x + patrolDirection * speed * Time.deltaTime, transform.position.y);

        if (ObstacleInFront() || !GroundAhead())
        {
            patrolDirection *= -1f;
            StartCoroutine(FlipAndTurn());
            patrolTimer = GetRandomPatrolTime(); // ������ Ÿ�̸� �缳��
        }

        transform.position = patrolTarget;
        enemyAnimator.SetBool("isWalking", true);
    }
    float GetRandomPatrolTime()
    {
        return Random.Range(1.5f, 6f); // 1.5�� ~ 3.5�� ������ ������ �� ��ȯ
    }
    public int GetDamage()
    {
        return attackDamage;
    }
    IEnumerator FlipAndTurn()
    {
        isTurning = true;
        Flip(); // ���� ��� ��ȯ
        enemyAnimator.SetTrigger("Turn");

        yield return new WaitForSeconds(0.5f); // Turn �ִϸ��̼� ���� �ð�

        isTurning = false;
    }

    bool ObstacleInFront()
    {
        Vector2 direction = isFacingRight ? Vector2.right : Vector2.left;
        RaycastHit2D hit = Physics2D.Raycast(transform.position, direction, 4f, obstacleLayer);

        if (hit.collider != null)
        {
          
            Gizmos.color = Color.red;
        }
        else
        {
      
            Gizmos.color = Color.green;
        }

        return hit.collider != null;
    }
    bool GroundAhead()
    {
        RaycastHit2D hit = Physics2D.Raycast(groundCheck.position, Vector2.down, 1f, groundLayer);
        if (hit.collider != null)
        {
            Debug.Log("���� ������: " + hit.collider.gameObject.name);
        }
        else
        {
            Debug.Log("���� ����");
        }
        return hit.collider != null;
    }
    public void CancelAttack()
    {
        isAttacking = false;
        enemyAnimator.ResetTrigger("Attack");
        enemyAnimator.SetTrigger("Parry"); // �и� �ִϸ��̼� ����
        Debug.Log(" �� ������ �и���! �и� �ִϸ��̼� ���� �� ����");

        rb.velocity = Vector2.zero;

        StartCoroutine(StunCoroutine(3f)); // 3�ʰ� ����
    }

    IEnumerator StunCoroutine(float stunDuration)
    {
        isStunned = true; //  ���� ���� Ȱ��ȭ
        isChasing = false;
        isAttacking = false;
        isPatrolling = false;

        enemyAnimator.SetBool("isWalking", false);
        rb.velocity = Vector2.zero;
        // X�� �̵��� ����, Z�� ȸ���� ����
        rb.constraints = RigidbodyConstraints2D.FreezePositionX | RigidbodyConstraints2D.FreezeRotation;

        yield return new WaitForSeconds(stunDuration);

        isStunned = false; //  ���� ����
        isChasing = true;
        isPatrolling = true;

        // Z�� ȸ���� ��� �����ϰ� X�� �̵��� Ǯ����
        rb.constraints = RigidbodyConstraints2D.FreezeRotation;

        Debug.Log(" �� ���� ����!");
    }

    void Flip()
    {
        isFacingRight = !isFacingRight;
        transform.Rotate(0f, 180f, 0f);
    }
    //  ���� �����Ǹ� �÷��̾�� ����� ���� �̵��� �� �ֵ��� ����
    private void OnTriggerStay2D(Collider2D other)
    {
        if (other.CompareTag("Enemy"))
        {
            // Ư�� �̸��� ���̸� �����ϰ� �̵� ����
            if (ignoreEnemyNames.Contains(other.gameObject.name))
            {
                Debug.Log($"���õ� �� �߰�: {other.gameObject.name} -> �̵� ����");
                isBlocked = false;
                return;
            }

            Debug.Log("�տ� �� ������!");

            float myDistance = Vector2.Distance(transform.position, player.position);
            float otherDistance = Vector2.Distance(other.transform.position, player.position);

            if (myDistance < otherDistance)
            {
                Debug.Log("���� �÷��̾�� �� ����� -> �̵� ����");
                isBlocked = false;
            }
            else
            {
                Debug.Log("���� �÷��̾�� ���� -> �̵� ����");
                isBlocked = true;
                rb.velocity = Vector2.zero;
                enemyAnimator.SetBool("isWalking", false);
            }
        }
    }

    //  ���� ������� �ٽ� �̵� ����
    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Enemy"))
        {
            Debug.Log("�տ� �ִ� ���� �����! �̵� ����");
            isBlocked = false;
        }
    }


    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(attackBox.position, attackBoxSize); // ���� ���� ǥ��
        Gizmos.DrawRay(transform.position, Vector2.right * detectionRange);
        Gizmos.DrawRay(transform.position, Vector2.left * detectionRange);
        Gizmos.color = Color.blue;
        Gizmos.DrawRay(groundCheck.position, Vector2.down * 1f); // ���� ���� Ray Ȯ��
        Gizmos.color = Color.yellow;
        Gizmos.DrawRay(transform.position, isFacingRight ? Vector2.right * 1f : Vector2.left * 1f); // ��ֹ� ���� �����
    }
}
