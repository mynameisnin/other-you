using UnityEngine;
using System.Collections;

/// <summary>
/// ���� AI �ý��� Ŭ����.
/// �÷��̾� ����, �Ÿ� ��� ���� ����(��뽬 ���� ����), ������ ���� ���� ����,
/// ���� �� �߰� �뽬, �뽬/��뽬(��� ����) �� ���� ��ȯ �ִϸ��̼� ��� ����մϴ�.
/// </summary>
public class AngryGodAiCore : MonoBehaviour
{
    #region ���� ����

    // --- ������Ʈ ���� ---
    private Animator animator;
    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;

    // --- �÷��̾� ���� ---
    [Header("�÷��̾� ����")]
    private Transform target; // ���� ����� Ÿ��
    private SpriteRenderer targetSpriteRenderer; // �� �߰�: Ÿ���� ��������Ʈ ������ ����

    // --- AI �ൿ ���� ---
    [Header("AI �ൿ ����")]
    [Tooltip("�÷��̾ Ž���ϱ� �����ϴ� �ִ� �Ÿ�")]
    public float detectRange = 10f;
    [Tooltip("�� �Ÿ� �ȿ� �÷��̾ ������ ������ ����")]
    public float attackRange = 2.5f;
    [Tooltip("�÷��̾ �� �Ÿ����� ��������� Ư�� ������ �����ϸ� ��뽬 ����")]
    public float backdashRange = 1.5f;
    [Tooltip("���� �� �÷��̾ �� �Ÿ����� �־����� �߰� �뽬 ����")]
    public float chaseDashTriggerRange = 3.0f;

    // --- �̵� ���� ---
    [Header("�̵� ����")]
    [Tooltip("�÷��̾� ���� �� �⺻ �̵� �ӵ�")]
    [SerializeField] private float moveSpeed = 2f; // �Ϲ� �̵� �ӵ� (�߰� �뽬 ����)

    // --- �뽬 ���� ---
    [Header("�뽬 ����")]
    [Tooltip("����/�Ĺ� �뽬 �� �̵��� �� �Ÿ�")]
    public float dashDistance = 3f;
    [Tooltip("����/�Ĺ� �뽬�� ���ӵǴ� �ð�")]
    public float dashDuration = 0.3f;
    [Tooltip("��뽬 �� �������� �̵��ϴ� ���� (0: ����, 1: 45�� ��)")]
    [Range(0f, 1f)]
    [SerializeField] private float backdashUpwardFactor = 0.3f; // ��뽬 ��� ���� ����
    [Tooltip("���� �� �߰� �뽬 �� �̵� �ӵ� ���� (moveSpeed ����)")]
    public float chaseDashSpeedMultiplier = 1.5f;
    [Tooltip("���� �� �߰� �뽬 ���� �ð� (��)")]
    public float chaseDashDuration = 0.2f;

    // --- �뽬 ����Ʈ ---
    [Header("�뽬 ����Ʈ")]
    [Tooltip("�뽬 �� ����� Trail Renderer ������Ʈ")]
    [SerializeField] private TrailRenderer dashTrail;
    [Tooltip("�ܻ��� �����Ǵ� ���� (��)")]
    [SerializeField] private float afterImageInterval = 0.05f;
    [Tooltip("�ܻ��� ���������� �ɸ��� �ð� (��)")]
    [SerializeField] private float afterImageLifetime = 0.5f;

    // --- ���� ���� ---
    [Header("���� ����")]
    [Tooltip("���� ���� �� ���� �ൿ������ ��� �ð�")]
    public float attackCooldown = 0.8f;
    [Tooltip("���� �ִϸ��̼� ���� �� ���� ������ ������ �߻��ϴ� ���� (��)")]
    [SerializeField] private float attackHitTiming = 0.5f;

    // --- ���� ���� ���� ---
    private bool isActing = false;        // ���� �ൿ(���� �Ǵ� �Ϲ�/�� �뽬) ������ ����
    private bool facingRight = true;      // ���� �ٶ󺸴� ���� (true: ������)
    private bool isDashing = false;       // ���� ��� ������ �뽬(�Ϲ�, ��, �߰�) ������ (����Ʈ �����)
    private bool isChaseDashing = false;    // ���� ���� �� �߰� �뽬 ������ ����

    #endregion

    #region ����Ƽ �����ֱ� �޼���

    void Start()
    {
        // �ʼ� ������Ʈ �������� �� Ȯ��
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        if (animator == null || rb == null || spriteRenderer == null)
        {
            Debug.LogError($"�ʼ� ������Ʈ ����! (Animator: {animator == null}, Rigidbody2D: {rb == null}, SpriteRenderer: {spriteRenderer == null})", this);
            this.enabled = false;
            return;
        }
        if (dashTrail != null) dashTrail.emitting = false;
        else Debug.LogWarning("Dash Trail Renderer�� �Ҵ���� �ʾҽ��ϴ�.", this);

        rb.constraints = RigidbodyConstraints2D.FreezeRotation;
        // facingRight = !spriteRenderer.flipX; // �ʱ� ���� ���� �ʿ� ��
    }

    void Update()
    {
        // ���� �ൿ ���̸� �ߴ�
        if (isActing || isChaseDashing) return;

        // Ÿ�� ã�� (�� ��������Ʈ ������ ����)
        FindClosestTarget();
        if (target == null) return; // Ÿ�� ������ �ߴ�

        // Ÿ�� �������� �� ������
        FlipTowardsTarget();

        // �Ÿ� ���
        float distance = Vector2.Distance(transform.position, target.position);

        // --- �ൿ ���� ���� ---
        // 1. ��뽬 ����
        if (distance < backdashRange && IsPlayerFacingBoss() && CanPredictPlayerAttack())
        {
            StartCoroutine(DashRoutine(-1)); // ��뽬
        }
        // 2. ���� ���� (��뽬 ���� ������ ��)
        else if (distance < attackRange)
        {
            // ��뽬 �Ÿ� �ȿ� ������ �ü�/���� �Ҹ��� �� ó��
            if (distance < backdashRange)
            {
                // TODO: ���� ��� �ٸ� �ൿ (���, ��� ��) �߰� ����
                // ����: �ƹ��͵� �� �ϰ� ���� �����ӿ� �ٽ� �Ǵ�
                // Debug.Log("��뽬 ���� ������, ���� ����");
            }
            else // ������ ���� �Ÿ�
            {
                StartCoroutine(AttackRoutine()); // ����
            }
        }
        // 3. ���� �뽬 ����
        else if (distance < detectRange)
        {
            StartCoroutine(DashRoutine(1)); // ���� �뽬
        }
        // 4. ���� �� (����� �ƹ� �ൿ �� ��)
        // else { /* Idle ���� ó�� */ }
    }

    #endregion

    #region Ÿ�� ã�� �� ���� ��ȯ

    // ���� ����� Ȱ��ȭ�� �÷��̾� Ÿ�ٰ� �� SpriteRenderer ã��
    void FindClosestTarget()
    {
        GameObject adam = GameObject.FindWithTag("Player");
        GameObject deva = GameObject.FindWithTag("DevaPlayer");
        Transform closestTarget = null;
        SpriteRenderer closestSprite = null; // �� �߰�
        float minDist = Mathf.Infinity;

        // Adam üũ
        if (adam != null && adam.activeInHierarchy)
        {
            float dist = Vector2.Distance(transform.position, adam.transform.position);
            if (dist < minDist)
            {
                minDist = dist;
                closestTarget = adam.transform;
                closestSprite = adam.GetComponent<SpriteRenderer>(); // �� �߰�
            }
        }
        // Deva üũ
        if (deva != null && deva.activeInHierarchy)
        {
            float dist = Vector2.Distance(transform.position, deva.transform.position);
            if (dist < minDist)
            {
                closestTarget = deva.transform;
                closestSprite = deva.GetComponent<SpriteRenderer>(); // �� �߰�
            }
        }
        target = closestTarget;
        targetSpriteRenderer = closestSprite; // �� �߰�: ã�� ��������Ʈ ����
    }

    // Ÿ�� �������� ��������Ʈ ������
    void FlipTowardsTarget(bool forceFlip = false)
    {
        if (target == null) return;
        bool shouldFaceRight = (target.position.x > transform.position.x);
        if (facingRight != shouldFaceRight || forceFlip)
        {
            facingRight = shouldFaceRight;
            if (spriteRenderer != null) spriteRenderer.flipX = !facingRight;
        }
    }
    #endregion

    #region �ൿ �ڷ�ƾ (�뽬, ����, �߰� �뽬)

    /// <summary>
    /// �Ϲ� ���� �뽬 �Ǵ� ��뽬(��� ����)�� �����ϴ� �ڷ�ƾ.
    /// </summary>
    IEnumerator DashRoutine(int direction)
    {
        isActing = true;
        isDashing = true;
        animator.SetTrigger(direction == 1 ? "Dash" : "Backdash");

        // ����Ʈ ����
        if (dashTrail != null) dashTrail.emitting = true;
        StartCoroutine(LeaveAfterImage());

        // ���� ���
        Vector2 dashDir;
        if (direction == 1) // ����
        {
            if (target != null) { dashDir = ((Vector2)target.position - rb.position).normalized; FlipTowardsTarget(true); }
            else { dashDir = facingRight ? Vector2.right : Vector2.left; }
        }
        else // �Ĺ� (��뽬)
        {
            Vector2 backwardDir = facingRight ? Vector2.left : Vector2.right;
            Vector2 upwardDir = Vector2.up * backdashUpwardFactor;
            dashDir = (backwardDir + upwardDir).normalized;
        }

        // �̵� ����
        float currentDashSpeed = dashDistance / dashDuration;
        rb.velocity = dashDir * currentDashSpeed;

        yield return new WaitForSeconds(dashDuration);

        // ���� ó��
        rb.velocity = Vector2.zero;
        isDashing = false;
        if (dashTrail != null) dashTrail.emitting = false;
        yield return new WaitForSeconds(0.3f); // ������
        isActing = false;
        yield break;
    }

    /// <summary>
    /// �⺻ ���� �ڷ�ƾ (�߰� �뽬 ����).
    /// </summary>
    IEnumerator AttackRoutine()
    {
        isActing = true;
        rb.velocity = Vector2.zero;
        FlipTowardsTarget(true);
        animator.SetTrigger("NomalAttack");

        // ���� �� �Ÿ� üũ �� �߰� �뽬
        float timeElapsed = 0f;
        while (timeElapsed < attackHitTiming && isActing)
        {
            if (target != null && !isChaseDashing)
            {
                float currentDistance = Vector2.Distance(transform.position, target.position);
                if (currentDistance > chaseDashTriggerRange)
                {
                    StartCoroutine(ChaseDashDuringAttack());
                }
            }
            yield return new WaitForSeconds(0.1f);
            timeElapsed += 0.1f;
            if (!IsPlayerValid()) // Ÿ�� ��ȿ�� ��Ȯ��
            {
                isActing = false;
                if (isChaseDashing) { isDashing = false; isChaseDashing = false; if (dashTrail != null) dashTrail.emitting = false; rb.velocity = Vector2.zero; }
                yield break;
            }
        }

        // ���� ���� �������� ���
        float remainingTime = attackHitTiming - timeElapsed;
        if (remainingTime > 0) { yield return new WaitForSeconds(remainingTime); }
        while (isChaseDashing) { yield return null; } // �߰� �뽬 �Ϸ� ���

        // ���� ����
        if (isActing) PerformAttackHit();

        // ��ٿ� �� ����
        yield return new WaitForSeconds(attackCooldown);
        isActing = false;
        yield break;
    }

    /// <summary>
    /// ���� �� ª�� �߰� �뽬 �ڷ�ƾ.
    /// </summary>
    private IEnumerator ChaseDashDuringAttack()
    {
        isChaseDashing = true;
        isDashing = true;

        // ����Ʈ ����
        if (dashTrail != null) dashTrail.emitting = true;
        StartCoroutine(LeaveAfterImage());

        // �̵� ����
        Vector2 chaseDir = Vector2.zero;
        if (IsPlayerValid()) // Ÿ�� ��ȿ�� Ȯ��
        {
            chaseDir = ((Vector2)target.position - rb.position).normalized;
            FlipTowardsTarget(true);
        }
        else { isChaseDashing = false; isDashing = false; if (dashTrail != null) dashTrail.emitting = false; yield break; }

        float chaseSpeed = moveSpeed * chaseDashSpeedMultiplier;
        rb.velocity = chaseDir * chaseSpeed;

        yield return new WaitForSeconds(chaseDashDuration);

        // ���� ó��
        if (isActing) rb.velocity = Vector2.zero; // ������ ��� ���� ���� ����
        isDashing = false;
        isChaseDashing = false;
        if (dashTrail != null) dashTrail.emitting = false;
        yield break;
    }

    // ���� ���� ���� ���� �Լ�
    void PerformAttackHit()
    {
        Debug.Log("���� ���� ����!");
        // TODO: ���� ���� ���� ����
    }

    #endregion

    #region ���� �� ���� �Լ�

    // �÷��̾ ������ ���ϰ� �ִ��� Ȯ��
    bool IsPlayerFacingBoss()
    {
        // �� ����: ����� targetSpriteRenderer ���
        if (target == null || targetSpriteRenderer == null) return false;

        bool playerIsFacingRight = !targetSpriteRenderer.flipX;
        bool playerIsRightOfBoss = target.position.x > transform.position.x;
        return (!playerIsRightOfBoss && !playerIsFacingRight) || (playerIsRightOfBoss && playerIsFacingRight);
    }

    // �÷��̾� ���� ���� (����� �׻� true)
    bool CanPredictPlayerAttack()
    {
        // TODO: ���� ���� ��ȭ
        return true;
    }

    // �÷��̾� ���� ��ȿ�� Ȯ��
    bool IsPlayerValid()
    {
        // targetSpriteRenderer�� �Բ� Ȯ�� (FindClosestTarget���� ���� �����ǹǷ�)
        return target != null && target.gameObject.activeInHierarchy && targetSpriteRenderer != null;
    }

    // AttackBoxes �迭 ��ȿ�� �˻� (���� ��� �� ��)
    void ValidateAttackBoxes() { }

    #endregion

    #region �뽬 ����Ʈ ����

    // �뽬 �� �ܻ� ���� �ڷ�ƾ
    private IEnumerator LeaveAfterImage()
    {
        while (isDashing)
        {
            CreateAfterImage();
            yield return new WaitForSeconds(afterImageInterval);
        }
        yield break;
    }

    // �ܻ� ���� ������Ʈ ���� �� ����
    void CreateAfterImage()
    {
        GameObject afterImage = new GameObject("AfterImage_Boss");
        SpriteRenderer sr = afterImage.AddComponent<SpriteRenderer>();
        sr.sprite = spriteRenderer.sprite;
        sr.color = new Color(1f, 0.2f, 0.2f, 0.7f); // ������ �ܻ�
        sr.flipX = spriteRenderer.flipX;
        sr.sortingLayerName = spriteRenderer.sortingLayerName;
        sr.sortingOrder = spriteRenderer.sortingOrder - 1;
        afterImage.transform.position = transform.position;
        afterImage.transform.localScale = transform.localScale;
        StartCoroutine(FadeOutAndDestroy(sr));
    }

    // �ܻ� ���̵� �ƿ� �� �ڵ� �ı� �ڷ�ƾ
    private IEnumerator FadeOutAndDestroy(SpriteRenderer sr)
    {
        if (sr == null) yield break;
        float fadeDuration = afterImageLifetime;
        Color originalColor = sr.color;
        float elapsed = 0f;
        while (elapsed < fadeDuration)
        {
            if (sr == null) yield break;
            elapsed += Time.deltaTime;
            float alpha = Mathf.Lerp(originalColor.a, 0, elapsed / fadeDuration);
            sr.color = new Color(originalColor.r, originalColor.g, originalColor.b, alpha);
            yield return null;
        }
        if (sr != null && sr.gameObject != null) Destroy(sr.gameObject);
        yield break;
    }
    #endregion

    #region Gizmos

    // �� �信�� AI ���� �� ���� �ð�ȭ (�׻� ǥ��)
    void OnDrawGizmos()
    {
        // ���� �ð�ȭ
        Gizmos.color = Color.red; Gizmos.DrawWireSphere(transform.position, attackRange);
        Gizmos.color = Color.yellow; Gizmos.DrawWireSphere(transform.position, backdashRange);
        Gizmos.color = Color.Lerp(Color.red, Color.yellow, 0.5f); Gizmos.DrawWireSphere(transform.position, chaseDashTriggerRange);
        Gizmos.color = Color.cyan; Gizmos.DrawWireSphere(transform.position, detectRange);

        // ���� �ð�ȭ (���� ���� ��)
        if (Application.isPlaying)
        {
            // Ÿ�� ����
            if (target != null)
            {
                Gizmos.color = Color.white;
                Gizmos.DrawLine(transform.position, target.position);
                // �÷��̾� �ü� ǥ��
                Gizmos.color = IsPlayerFacingBoss() ? Color.green : Color.gray;
                Gizmos.DrawCube(transform.position + Vector3.up * 1.5f, Vector3.one * 0.2f);
            }
            // ���� �ൿ ���� ǥ��
            if (isActing) { if (isChaseDashing) Gizmos.color = Color.magenta; else if (isDashing) Gizmos.color = Color.blue; else Gizmos.color = Color.Lerp(Color.red, Color.black, 0.5f); }
            else { Gizmos.color = Color.green; }
            Gizmos.DrawSphere(transform.position + Vector3.down * 0.5f, 0.15f);
        }
    }

    #endregion
}