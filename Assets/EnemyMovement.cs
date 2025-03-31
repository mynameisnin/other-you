using System.Collections; 
using System.Collections.Generic; 
using UnityEngine;

public class EnemyMovement : MonoBehaviour // ���� �̵� �� �ൿ�� �����ϴ� ��ũ��Ʈ
{
    private Animator enemyAnimator; // �ִϸ����� ������Ʈ ����
    private Rigidbody2D rb; // Rigidbody2D ������Ʈ ����

    public Transform player; // �÷��̾� ��ġ ����
    public Transform groundCheck; // ���� üũ ����Ʈ
    public LayerMask groundLayer; // ���� ���̾� ����
    public LayerMask playerLayer; // �÷��̾� ���̾� ����
    public LayerMask obstacleLayer; // ��ֹ� ���̾� ����
    public Transform attackBox; // ���� ���� �ڽ� ��ġ
    public float attackBoxSize = 1.5f; // ���� ���� ũ��

    [Header("Detection Settings")] // �ν����� ���п� ���
    public float detectionRange = 5f; // �÷��̾� Ž�� �Ÿ�
    public float attackRange = 1.5f; // ���� ���� �Ÿ�
    public float speed = 3f; // �̵� �ӵ�
    public bool isFacingRight = true; // ĳ���Ͱ� �������� ���� �ִ��� ����
    private bool isChasing = false; // �÷��̾� ���� ������ ����
    private bool isAttacking = false; // ���� ������ ����
    private bool isTurning = false; // ���� ��ȯ ������ ����
    private bool isStunned = false; // ���� ���� ����

    private bool isPatrolling = true; // ���� ������ ����
    private float patrolTime = 2f; // ���� �ð� ����
    private float patrolTimer = 0f; // ���� Ÿ�̸�
    private float patrolDirection = 1f; // ���� ���� (1: ������, -1: ����)
    public int attackDamage = 100; // ���� ������ ��

    private bool isBlocked = false; // �տ� ���� �־� ���� �ִ��� ����

    public Collider2D frontCollider; // �տ� �ִ� �ݶ��̴� (�±� ������)

    public List<string> ignoreEnemyNames = new List<string>(); // ������ �� �̸� ����Ʈ
    void Start()
    {
        enemyAnimator = GetComponent<Animator>(); // �ִϸ����� �ʱ�ȭ
        rb = GetComponent<Rigidbody2D>(); // ������ٵ� �ʱ�ȭ
        patrolTimer = patrolTime; // ���� Ÿ�̸� �ʱ� ����
        FindPlayer(); // �÷��̾� ��ü ã��
    }

    void Update()
    {
        if (isStunned || isBlocked) return; // ���� ���³� ���� ������ ���� �ߴ�
        if (!isAttacking && !isTurning)
        {
            DetectPlayer(); // �÷��̾� ���� �õ�
        }

        if (isChasing && !isAttacking && !isTurning)
        {
            ChasePlayer(); // ���� �ൿ
        }
        else if (isPatrolling && !isAttacking && !isTurning)
        {
            Patrol(); // ���� �ൿ
        }
        if (player == null || !player.gameObject.activeInHierarchy)
        {
            FindPlayer(); // �÷��̾ �ٲ���ų� ��Ȱ��ȭ�� ��� �ٽ� ã��
        }
    }

    void FindPlayer()
    {
        GameObject adam = GameObject.FindGameObjectWithTag("AdamCamPosition"); // �ƴ� �±� ã��
        GameObject deba = GameObject.FindGameObjectWithTag("DevaCamPosition"); // ���� �±� ã��

        if (adam != null && adam.activeInHierarchy)
        {
            player = adam.transform; // �ƴ��� Ÿ������ ����
        }
        else if (deba != null && deba.activeInHierarchy)
        {
            player = deba.transform; // ���ٸ� Ÿ������ ����
        }
    }

    void DetectPlayer()
    {
        if (isStunned) return; // ���� �����̸� ���� �ߴ�

        RaycastHit2D hitRight = Physics2D.Raycast(transform.position, Vector2.right, detectionRange, playerLayer); // ������ ����
        RaycastHit2D hitLeft = Physics2D.Raycast(transform.position, Vector2.left, detectionRange, playerLayer); // ���� ����

        if (hitRight.collider != null)
        {
            isChasing = true; // ���� ����
            isPatrolling = false; // ���� ����
            if (!isFacingRight)
            {
                StartCoroutine(FlipAndTurn()); // ���� ��ȯ
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
        if (player == null || isBlocked) return; // Ÿ�� ���� �Ǵ� �̵� ���� �� �ߴ�

        float distance = Vector2.Distance(transform.position, player.position); // �÷��̾�� �Ÿ� ����

        if (distance <= attackRange)
        {
            StartCoroutine(Attack()); // ���� ����
        }
        else
        {
            enemyAnimator.SetBool("isWalking", true); // �ȱ� �ִϸ��̼� ����
            Vector2 targetPosition = new Vector2(player.position.x, transform.position.y); // ���� ��ġ�� �̵�
            transform.position = Vector2.MoveTowards(transform.position, targetPosition, speed * Time.deltaTime); // �̵�

            if ((player.position.x > transform.position.x && !isFacingRight) ||
                (player.position.x < transform.position.x && isFacingRight))
            {
                StartCoroutine(FlipAndTurn()); // ���� ��ȯ �ʿ� �� ����
            }
        }
    }

    IEnumerator Attack()
    {
        isAttacking = true; // ���� ���� ����
        enemyAnimator.SetBool("isWalking", false); // �ȱ� ����
        enemyAnimator.SetTrigger("Attack"); // ���� Ʈ���� ����

        yield return new WaitForSeconds(0.1f); // �ִϸ��̼� ���

        while (enemyAnimator.GetCurrentAnimatorStateInfo(0).IsName("Attack"))
        {
            yield return null; // ���� �ִϸ��̼� ���� ������ ���
        }

        if (!CheckPlayerInAttackRange())
        {
            Debug.Log("�÷��̾ ������ ������Ƿ� �̵� �簳");
            isAttacking = false; // ���� ����
        }
        else
        {
            Debug.Log("�÷��̾ ���� ���� ����, �ٽ� ���� ����");
            StartCoroutine(Attack()); // ���� ����
        }
    }

    bool CheckPlayerInAttackRange()
    {
        if (isStunned) return false; // ���� �� ���� �Ұ�

        Collider2D hit = Physics2D.OverlapCircle(attackBox.position, attackBoxSize, playerLayer); // ���� ���� Ȯ��
        return hit != null; // ���� ���� �÷��̾� ������ true
    }

    void Patrol()
    {
        patrolTimer -= Time.deltaTime; // ���� Ÿ�̸� ����

        if (patrolTimer <= 0)
        {
            patrolDirection *= -1f; // ���� ����
            StartCoroutine(FlipAndTurn()); // ȸ�� �ִϸ��̼� ����
            patrolTimer = patrolTime;
            patrolTimer = GetRandomPatrolTime(); // ���� �ð� ����
        }

        Vector2 patrolTarget = new Vector2(transform.position.x + patrolDirection * speed * Time.deltaTime, transform.position.y); // �̵��� ��ġ ���

        if (ObstacleInFront() || !GroundAhead())
        {
            patrolDirection *= -1f; // ����
            StartCoroutine(FlipAndTurn()); // ���� ��ȯ
            patrolTimer = GetRandomPatrolTime(); // Ÿ�̸� �缳��
        }

        transform.position = patrolTarget; // �̵�
        enemyAnimator.SetBool("isWalking", true); // �ȱ� �ִϸ��̼�
    }

    float GetRandomPatrolTime()
    {
        return Random.Range(1.5f, 6f); // ���� ���� �ð� ��ȯ
    }

    public int GetDamage()
    {
        return attackDamage; // ���� ������ ��ȯ
    }

    IEnumerator FlipAndTurn()
    {
        isTurning = true; // ���� ��ȯ ��
        Flip(); // ���� ���� ��ȯ
        enemyAnimator.SetTrigger("Turn"); // ȸ�� �ִϸ��̼�

        yield return new WaitForSeconds(0.5f); // �ִϸ��̼� ���

        isTurning = false; // ��ȯ �Ϸ�
    }

    bool ObstacleInFront()
    {
        Vector2 direction = isFacingRight ? Vector2.right : Vector2.left; // �ٶ󺸴� ���� ����
        RaycastHit2D hit = Physics2D.Raycast(transform.position, direction, 4f, obstacleLayer); // ��ֹ� Ž��

        if (hit.collider != null)
        {
            Gizmos.color = Color.red; // ������
        }
        else
        {
            Gizmos.color = Color.green; // ����
        }

        return hit.collider != null; // ��ֹ� ���� ��ȯ
    }

    bool GroundAhead()
    {
        RaycastHit2D hit = Physics2D.Raycast(groundCheck.position, Vector2.down, 1f, groundLayer); // ���� ����
        if (hit.collider != null)
        {
            Debug.Log("���� ������: " + hit.collider.gameObject.name);
        }
        else
        {
            Debug.Log("���� ����");
        }
        return hit.collider != null; // ���� ���� ���� ��ȯ
    }

    public void CancelAttack()
    {
        isAttacking = false; // ���� ���
        enemyAnimator.ResetTrigger("Attack"); // ���� Ʈ���� ����
        enemyAnimator.SetTrigger("Parry"); // �и� �ִϸ��̼� ����
        Debug.Log(" �� ������ �и���! �и� �ִϸ��̼� ���� �� ����");

        rb.velocity = Vector2.zero; // �̵� ����

        StartCoroutine(StunCoroutine(3f)); // 3�� ����
    }

    IEnumerator StunCoroutine(float stunDuration)
    {
        isStunned = true; // ���� ����
        isChasing = false;
        isAttacking = false;
        isPatrolling = false;

        enemyAnimator.SetBool("isWalking", false); // �ȱ� ����
        rb.velocity = Vector2.zero; // �̵� ����
        rb.constraints = RigidbodyConstraints2D.FreezePositionX | RigidbodyConstraints2D.FreezeRotation; // �̵� ����

        yield return new WaitForSeconds(stunDuration); // ���� ���

        isStunned = false; // ���� ����
        isChasing = true;
        isPatrolling = true;

        rb.constraints = RigidbodyConstraints2D.FreezeRotation; // �̵� ���, ȸ�� ����

        Debug.Log(" �� ���� ����!");
    }

    void Flip()
    {
        isFacingRight = !isFacingRight; // ���� ���
        transform.Rotate(0f, 180f, 0f); // �ð��� ȸ��
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (other.CompareTag("Enemy")) // �ٸ� �� ���� ��
        {
            if (ignoreEnemyNames.Contains(other.gameObject.name)) // ���� ����̸� ���
            {
                Debug.Log($"���õ� �� �߰�: {other.gameObject.name} -> �̵� ����");
                isBlocked = false;
                return;
            }

            Debug.Log("�տ� �� ������!");

            float myDistance = Vector2.Distance(transform.position, player.position); // �� �Ÿ�
            float otherDistance = Vector2.Distance(other.transform.position, player.position); // ��� �Ÿ�

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

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Enemy")) // ���� ������� ��
        {
            Debug.Log("�տ� �ִ� ���� �����! �̵� ����");
            isBlocked = false;
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(attackBox.position, attackBoxSize); // ���� ���� �ð�ȭ
        Gizmos.DrawRay(transform.position, Vector2.right * detectionRange); // ������ Ž�� �ð�ȭ
        Gizmos.DrawRay(transform.position, Vector2.left * detectionRange); // ���� Ž�� �ð�ȭ
        Gizmos.color = Color.blue;
        Gizmos.DrawRay(groundCheck.position, Vector2.down * 1f); // ���� ���� �ð�ȭ
        Gizmos.color = Color.yellow;
        Gizmos.DrawRay(transform.position, isFacingRight ? Vector2.right * 1f : Vector2.left * 1f); // ��ֹ� ���� �ð�ȭ
    }
}
