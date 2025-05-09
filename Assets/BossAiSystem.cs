using UnityEngine;
using System.Collections;

/// <summary>
/// ���� AI �ý��� Ŭ����.
/// �÷��̾� ����, �Ÿ� ��� ���� ����, ������ ���� ���� ����,
/// �̵� �� ���� ��ȯ �ִϸ��̼� ��� ���
/// ���� �̵��� FixedUpdate�� ���
/// </summary>
public class BossAiSystem : MonoBehaviour
{
    #region ���� ����

    // --- ������Ʈ ���� ---
    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;
    private Animator animator;

    // --- �÷��̾� ���� ---
    [Header("�÷��̾� ����")]
    [Tooltip("�÷��̾��� Transform. �±׷� �ڵ� �˻�!! �Ҵ� ����!!!!!")]
    [SerializeField] private Transform player;
    [Tooltip("�÷��̾� �ڵ� �˻� �� ����� �±� ��� (������� �˻�)")]
    [SerializeField] private string[] playerTags = { "AdamCamPosition", "DevaCamPosition", "Player" };

    // --- ���� ���� ---
    [Header("���� ����")]
    [Tooltip("�÷��̾ �� �Ÿ� �ȿ� ������ ������ �����մϴ�.")]
    [SerializeField] private float attackRange = 4f;
    [SerializeField] private Transform attackRangePosition;
    [Tooltip("���� �� ���� ���ݱ����� ��� �ð� (��)")]
    [SerializeField] private float attackCooldown = 2f;
    [Tooltip("���� �ִϸ��̼� ���� �� ���� ������ ���������� �ð� (��). �ִϸ��̼ǰ� ��Ȯ�� ����ȭ �ʿ�.")]
    [SerializeField] private float attackTiming = 0.2f;

    // --- ���� ���� ---
    [Header("���� ����")]
    [Tooltip("�� ����(1, 2, 3)�� ����� ���� ���� ��ġ Transform �迭. �ݵ�� 3���� ������� �Ҵ�.")]
    [SerializeField] private Transform[] attackBoxes = new Transform[3];

    [Tooltip("���� ������� �ν��� ���̾� ����ũ.")]
    [SerializeField] private LayerMask playerLayer;


    // --- �̵� ���� ---
    [Header("�̵� ����")]
    [Tooltip("�÷��̾� ���� �� �̵� �ӵ�")]
    [SerializeField] private float moveSpeed = 2f;

    // --- ���� ���� ���� ---
    private bool isAttacking = false;      // ���� ���� �ڷ�ƾ ���� �� ���� �÷���
    private bool isMoving = false;         // ���� �̵� �ִϸ��̼� ��� �� ���� �÷���
    private bool facingRight = true;       // ���� ������ �������� �ٶ󺸰� �ִ��� ����
    private Vector2 moveDirection = Vector2.zero; // FixedUpdate���� ����� �̵� ���� ����
    private bool shouldMove = false;       // FixedUpdate���� �̵��� �������� ���� �÷���

    #endregion

    #region ����Ƽ �����ֱ� �޼���

    private void Start()
    {
        // �ʼ� ������Ʈ �������� �� null üũ
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();

        // NOTE: Animator�� Rigidbody2D�� �� AI�� �ٽ� ��ɿ� �ʼ����̹Ƿ� ������ ��Ȱ��ȭ.
        if (animator == null || rb == null)
        {
            Debug.LogError($"�ʼ� ������Ʈ ���� (Animator: {animator == null}, Rigidbody2D: {rb == null})", this);
            this.enabled = false;
            return;
        }
        // NOTE: SpriteRenderer�� Flip ��ɿ� �ʿ��մϴ�. ������ ��� ����ϰ� ����� ����.
        if (spriteRenderer == null)
        {
            Debug.LogWarning("SpriteRenderer ������Ʈ�� ���� Flip ����� ���ѵ� �� �ֽ��ϴ�.", this);
        }
        ValidateAttackBoxes(); // �迭 ��ȿ�� �˻� �Լ� ȣ�� 


        // Rigidbody ����: 2D ���̵�� ���ӿ��� �Ϲ����� ����
        rb.constraints = RigidbodyConstraints2D.FreezeRotation;

        // �÷��̾� �ʱ� �˻�
        if (player == null)
        {
            FindPlayerByTags();
        }

    
        // facingRight = !spriteRenderer.flipX; // ��������Ʈ�� �ʱ� flipX ���¸� �������� ����
    }

    // Update: �� ������ ȣ��, �ַ� ���� Ȯ�� �� �ൿ ���� (Decision Making)
    private void Update()
    {
        // �÷��̾� ���� ��ȿ�� �˻� (�� ������ �߿�)
        if (!IsPlayerValid())
        {
            // �÷��̾ �Ҿ��� ��� ó��
            StopMovement(); // �̵� �ߴ�
            if (isAttacking)
            {
                // NOTE: ���� ���� ���� �ڷ�ƾ�� �ߴܽ�ŵ�ϴ�. ���� �����ο� ���� �ٸ� �� ����.
                StopAllCoroutines(); // ��� �ڷ�ƾ �ߴ� (AttackRoutine ����)
                isAttacking = false; // ���� ���� ����
            }
            FindPlayerByTags(); // �÷��̾� �ٽ� ã�� �õ�
            return; // ��ȿ�� �÷��̾ ������ �� �̻� �������� ����
        }

        // �÷��̾���� �Ÿ� ���
        float distance = Vector2.Distance(transform.position, player.position);
        if (attackRangePosition != null)
        {
            attackRangePosition.position = new Vector3(transform.position.x, transform.position.y, transform.position.z);
        }
        // ���� ���� �ƴ� ���� �̵� �Ǵ� ���� ����
        if (!isAttacking)
        {
            // ���� ���� ��: �̵� �غ�
            if (distance > attackRange)
            {
                // �÷��̾ ���ϴ� ���� ���� ��� �� �̵� �غ� �Լ� ȣ��
                PrepareMove((player.position - transform.position).normalized);
            }
            // ���� ���� ��: �̵� ���߰� ���� ����
            else
            {
                StopMovement(); // ���� �� �̵� ���� (�ʼ�)
                StartCoroutine(AttackRoutine()); // ���� �ڷ�ƾ ����
            }
        }
    }

    // FixedUpdate: ������ �ð� �������� ȣ��, �ַ� ���� ���� ó�� (Movement Application)
    private void FixedUpdate()
    {
        // Update���� �̵��ϱ�� �����ߴٸ� (shouldMove == true) ���� �ӵ� ����
        if (shouldMove)
        {
            // NOTE: Y�� �ӵ��� ���� ���� �����Ͽ� �߷� ���� ������ �޵��� �մϴ�.
            rb.velocity = new Vector2(moveDirection.x * moveSpeed, rb.velocity.y);
        }

    }

    #endregion

    #region �̵� �� ���� ��ȯ

    // �̵� �غ� �Լ�: �̵� ���� ����, �̵� ���� �� �ִϸ��̼� �÷��� ����, ���� ��ȯ üũ
    private void PrepareMove(Vector2 direction)
    {
        // FixedUpdate���� ����� �̵� ���� ������Ʈ
        moveDirection = direction;
        // FixedUpdate���� �̵��� �����ϵ��� �÷��� ����
        shouldMove = true;

        // �̵� �ִϸ��̼� ���� ����
        if (!isMoving)
        {
            isMoving = true;
            animator.SetBool("Move", true); // "Move" �Ķ���͸� true�� ���� (Animator Controller �ʿ�)
        }

        // ���� ��ȯ �ʿ� ���� üũ �� ó�� �Լ� ȣ��
        HandleFlip(direction.x);
    }

    // �̵� �ߴ� �Լ�: �̵� ���� �� �ִϸ��̼� �÷��� ����, ���� �ӵ� ����
    private void StopMovement()
    {
        // �̵� ���̾��� ���� ó��
        if (isMoving || shouldMove) // shouldMove�� false�� ����� FixedUpdate���� �̵� �� �ϵ��� ��
        {
            isMoving = false;
            shouldMove = false;
            animator.SetBool("Move", false); // "Move" �Ķ���͸� false�� ����
            rb.velocity = new Vector2(0, rb.velocity.y); // ���� �ӵ� ��� 0���� ����
        }
    }

    // ���� ��ȯ �ʿ� ���� ���� �� Flip �Լ� ȣ��
    private void HandleFlip(float horizontalDirection)
    {
        // NOTE: 0.01f �� ���� ���� �Ӱ谪�� ����Ͽ� ���� ��ȯ�� �ʹ� �ΰ��ϰ� �������� �ʵ��� �ؾ���.
        // ���������� �����ϴµ� ���� ���� ���� ��
        if (horizontalDirection > 0.01f && !facingRight)
            Flip();
        // �������� �����ϴµ� ������ ���� ���� ��
        else if (horizontalDirection < -0.01f && facingRight)
            Flip();
    }

    // ���� ���� ��ȯ ����: ���� ������Ʈ, ��������Ʈ �� ��� ���� �ڽ� ��ġ ����
    private void Flip()
    {
        // ���� ���� ���� ������Ʈ
        facingRight = !facingRight;

        // ��������Ʈ ������ FlipX ����
        if (spriteRenderer != null)
        {
            // NOTE: �� ������ ���� ��������Ʈ�� �������� ������ ������
            // facingRight = true (������ ��) => flipX = false
            // facingRight = false (���� ��) => flipX = true
            spriteRenderer.flipX = !facingRight;
        }

        // ��� ���� �ڽ�(Attack Boxes)�� ���� X ��ġ ����
        if (attackBoxes != null)
        {
            foreach (Transform box in attackBoxes)
            {
                // WARNING: �� ���� �ڽ��� �ݵ�� �� ���� ������Ʈ�� '�ڽ�'�̾�� localPosition ������ �ùٸ��� ����.
                if (box != null && box.parent == transform)
                {
                    // �����ϰ� ���� ���� X ��ġ�� ��ȣ�� ����
                    box.localPosition = new Vector3(-box.localPosition.x, box.localPosition.y, box.localPosition.z);
                }
                // else: �ڽ��� �ƴ� ��� ��� ����ϰų� �ٸ� ó�� �ʿ�
            }
        }

        // CONSIDER: ���� ��ȯ �ִϸ��̼� �߰�
        // animator.SetTrigger("Flip");
    }

    #endregion

    #region ���� ����

    // ���� ��ü �帧�� �����ϴ� �ڷ�ƾ
    private IEnumerator AttackRoutine()
    {
        // ���� ���� ���� ���� �� �̵� �ߴ�
        isAttacking = true;
        StopMovement(); // ���� �߿��� �������� ����

        // ������ ���� ���� ���� (1, 2, 3 �� �ϳ�)
        int randomAttackIndex = Random.Range(1, 4);
        string attackTrigger = $"Attack{randomAttackIndex}"; // ����� �ִϸ����� Ʈ���� �̸�
        animator.SetTrigger(attackTrigger); // �ش� ���� �ִϸ��̼� ����

        // ���� ���� Ÿ�ֱ̹��� ���
        yield return new WaitForSeconds(attackTiming);

        // ���� ������ ���� ����
        TryHitPlayer(randomAttackIndex); // ���õ� ���� �ε����� �´� ���� �õ�

        // ���� �ִϸ��̼� ���� ���
        yield return new WaitUntil(() => !animator.GetCurrentAnimatorStateInfo(0).IsTag("Attack"));

        // ���� ��ٿ� ���
        yield return new WaitForSeconds(attackCooldown);

        // ���� ���� ���·� ����
        isAttacking = false;
    }

    // ���� ���� ���� ó�� �Լ�
    private void TryHitPlayer(int attackIndex)
    {
        // ����� ���� �ڽ� �ε��� ��� (1���� �����ϴ� attackIndex�� 0���� �����ϴ� �迭 �ε����� ��ȯ)
        int boxArrayIndex = attackIndex - 1;

        // �迭 �� �ش� �ε����� ��ȿ�� �˻� �� Ȯ��!!!
        if (attackBoxes == null || boxArrayIndex < 0 || boxArrayIndex >= attackBoxes.Length || attackBoxes[boxArrayIndex] == null)
        {
            Debug.LogError($"���� ���� ����: AttackBoxes[{boxArrayIndex}] (Attack {attackIndex})�� ��ȿ���� �ʽ��ϴ�.", this);
            return; // ��ȿ���� ������ ���� �ߴ�
        }

        // ���� ���ݿ� ����� Attack Box Transform ��������
        Transform currentAttackBox = attackBoxes[boxArrayIndex];


    }

    #endregion

    #region ���� ���

    // �÷��̾� ������ ��ȿ���� (null �ƴϰ� Ȱ��ȭ ��������) Ȯ��
    private bool IsPlayerValid()
    {
        // NOTE: player?. �� null ���Ǻ� �������Դϴ�. player�� null�̸� �ٷ� false ��ȯ.
        return player != null && player.gameObject.activeInHierarchy;
    }

    // ������ �±� ������� Ȱ��ȭ�� �÷��̾� ������Ʈ ã��
    private void FindPlayerByTags()
    {
        GameObject foundPlayer = null;
        foreach (string tag in playerTags)
        {
            GameObject obj = GameObject.FindGameObjectWithTag(tag);
            if (obj != null && obj.activeInHierarchy)
            {
                foundPlayer = obj;
                break; // ã���� �� �̻� �˻����� ����
            }
        }

        // ã�� �÷��̾� ���� �Ǵ� ���� �÷��̾� ���� ����
        if (foundPlayer != null)
        {
            player = foundPlayer.transform;
            // Debug.Log($"�÷��̾� ã��: {player.name}", this); // �ʿ� �� �α� Ȱ��ȭ
        }
        else if (player != null) // ������ �÷��̾ �־��µ� ���� �� ã�� ���
        {
            player = null; // ���� ����
                           // Debug.LogWarning("�÷��̾� �̾�. ���� Update���� �ٽ� �˻��մϴ�.", this); // �ʿ� �� �α� Ȱ��ȭ
        }
    }

    // AttackBoxes �迭 ��ȿ�� �˻� �Լ�
    private void ValidateAttackBoxes()
    {
        if (attackBoxes == null || attackBoxes.Length != 3)
        {
            Debug.LogError($"AttackBoxes �迭 ���� ����: Null �Ǵ� ũ�Ⱑ 3�� �ƴ� (���� ũ��: {attackBoxes?.Length ?? 0})", this);
            // �ʿ� �� this.enabled = false; ó��
            return;
        }
        for (int i = 0; i < attackBoxes.Length; i++)
        {
            if (attackBoxes[i] == null)
            {
                Debug.LogError($"AttackBoxes[{i}]�� ����ֽ��ϴ�. �ν����Ϳ��� �Ҵ��ؾ� �մϴ�.", this);
            }
            else if (attackBoxes[i].parent != transform)
            {
                // WARNING: �ڽ��� �ƴϸ� Flip ������ ������ �۵� ����.
                Debug.LogWarning($"AttackBoxes[{i}] ({attackBoxes[i].name})�� �� ������Ʈ�� �ڽ��� �ƴմϴ�. Flip �� ��ġ ������ �߻��� �� �ֽ��ϴ�.", attackBoxes[i]);
            }
        }
    }


    #endregion

    #region ����� �ð�ȭ

    // ��(Scene) �信�� �������� �� Gizmos �׸���
    private void OnDrawGizmosSelected()
    {
        // ���� ���� ���� (������ ��)
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(attackRangePosition.position, attackRange);


    }

    #endregion
    // ==============================================================
    // ������ ���� ����
    // ==============================================================
    /*
     * 1. ���� ���� ���:
     *    - ���, �� ���� �ִϸ��̼�(Attack1, Attack2, Attack3)�� Ư�� �����ӿ�
     *      �ִϸ��̼� �̺�Ʈ(Animation Event)�� �����Ͽ�,
     *      �ش� ���ݿ� �´� AttackDamageBox �ڽ� ������Ʈ�� Collider (Is Trigger)�� Ȱ��ȭ/��Ȱ��ȭ.
     *    - ���� �÷��̾���� �浹 ���� �� ������ ó���� AttackDamageBox ������Ʈ��
     *      ������ �߰��� ��ũ��Ʈ (��: AttackHitbox.cs - �̸��� ����) ���� OnTriggerEnter2D ��� ����
     *
     * 2. ������ �� ����:
     *    - �߿�: �� BossAiSystem ��ũ��Ʈ�� �������� ������ ������ �߰��ϰų� �Ҵ����� ��!!
     *    - �� ���� ����(Attack1, Attack2, Attack3)�� ���� �ٸ� ������ ���� ���� �� �־���ϴϱ�
     *    - ������ ���� �� AttackDamageBox�� ����� ��ũ��Ʈ �Ǵ� ���� �����Ϳ��� �����Ǿ����.
     *
     * 3. AttackBoxes �迭:
     *    - �ν������� attackBoxes �迭���� �ݵ�� 3���� Transform(AttackDamageBox1, 2, 3 ����)��
     *      �Ҵ��ؾ� �ϸ�, �̵��� ��� �� ���� ������Ʈ�� �ڽ��̾�� Flip ����� ����� ������.
     *      
     *                              �ڵ� ������ ������ ������� ���� ��Ź
     */
}