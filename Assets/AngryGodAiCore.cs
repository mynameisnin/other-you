using UnityEngine;
using System.Collections;
using System.Collections.Generic; // Gizmos ������

/// <summary>
/// ���� AI �ý��� Ŭ����.
/// �÷��̾� ����, �Ÿ� ��� ���� ����(��뽬 ���� ����), ������ ���� ���� ����,
/// ���� �� �߰� �뽬, �뽬/��뽬(��� ����) �� ���� ��ȯ �ִϸ��̼� ��� ����մϴ�.
/// ��뽬 �̵��� �ִϸ��̼� �̺�Ʈ�� ���۵˴ϴ�. Flip �� ���� �ݶ��̴� Offset�� �����˴ϴ�.
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
    private SpriteRenderer targetSpriteRenderer; // Ÿ���� ��������Ʈ ������ ����

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
    [SerializeField] private float moveSpeed = 2f; // �߰� �뽬 ����

    // --- �뽬 ���� ---
    [Header("�뽬 ����")]
    [Tooltip("����/�Ĺ� �뽬 �� �̵��� �⺻ �Ÿ�")]
    public float dashDistance = 3f;
    [Tooltip("����/�Ĺ� �뽬�� ���ӵǴ� �ð�")]
    public float dashDuration = 0.3f;
    [Tooltip("��뽬 �� �������� �̵��ϴ� ���� (0: ����, 1: 45�� ��)")]
    [Range(0f, 1f)]
    [SerializeField] private float backdashUpwardFactor = 0.3f;
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
    [Tooltip("���� ������ ����� BoxCollider2D�� �ִ� �ڽ� ������Ʈ")]
    [SerializeField] private Transform attackBoxObject; // ���� �ڽ� ������Ʈ ����
    private BoxCollider2D attackCollider; // ���� �ڽ� �ݶ��̴� ����

    // --- ���� ���� ���� ---
    private bool isActing = false;
    private bool facingRight = true;
    private bool isDashing = false;
    private bool isChaseDashing = false;
    private Coroutine stopAttackMovementCoroutine = null; // �� �߰�: ���� ���� ���� �ڷ�ƾ ����
    #endregion

    #region ����Ƽ �����ֱ� �޼���

    void Start()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        if (animator == null || rb == null || spriteRenderer == null) { Debug.LogError("�ʼ� ������Ʈ ����!", this); this.enabled = false; return; }
        if (dashTrail != null) dashTrail.emitting = false; else Debug.LogWarning("Dash Trail ����", this);

        // ���� �ݶ��̴� �ʱ�ȭ �� Ȯ��
        if (attackBoxObject != null)
        {
            attackCollider = attackBoxObject.GetComponent<BoxCollider2D>();
            if (attackCollider == null) Debug.LogError("Attack Box Object�� BoxCollider2D�� �����ϴ�!", attackBoxObject);
        }
        else Debug.LogError("Attack Box Object�� �Ҵ���� �ʾҽ��ϴ�!", this);

        rb.constraints = RigidbodyConstraints2D.FreezeRotation;
        // facingRight = !spriteRenderer.flipX; // �ʱ� ���� ����
    }

    void Update()
    {
        if (isActing || isChaseDashing) return; // ���� �ൿ ���̸� �ߴ�

        FindClosestTarget(); // Ÿ�� ã��
        if (target == null) return; // Ÿ�� ������ �ߴ�

        FlipTowardsTarget(); // Ÿ�� ���� ���� (Collider Offset ���� ����)
        float distance = Vector2.Distance(transform.position, target.position); // �Ÿ� ���

        // --- �ൿ ���� ���� ---
        // 1. ��뽬 ���� Ȯ�� �� �ִϸ��̼� ����
        if (distance < backdashRange && IsPlayerFacingBoss() && CanPredictPlayerAttack())
        {
            isActing = true;
            rb.velocity = Vector2.zero;
            FlipTowardsTarget(true); // ���� ����
            animator.SetTrigger("Backdash"); // �ִϸ��̼� ���� (�̵��� �̺�Ʈ��)
        }
        // 2. ���� ����
        else if (distance < attackRange)
        {
            if (distance >= backdashRange) StartCoroutine(AttackRoutine()); // ���� �Ÿ��� ����
            // else: �ſ� �������� ��뽬 ���� �� ������ ���
        }
        // 3. ���� �뽬 ����
        else if (distance < detectRange)
        {
            StartCoroutine(DashRoutine(1)); // ���� �뽬 (��� �̵�)
        }
        // 4. ���� ��
        // else { /* Idle ���� ó�� */ }
    }

    #endregion

    #region Ÿ�� ã�� �� ���� ��ȯ
    // ���� ����� Ȱ��ȭ�� �÷��̾� Ÿ�ٰ� �� SpriteRenderer ã��
    void FindClosestTarget()
    {
        GameObject adam = GameObject.FindWithTag("Player");
        GameObject deva = GameObject.FindWithTag("DevaPlayer");
        Transform closestTarget = null; SpriteRenderer closestSprite = null; float minDist = Mathf.Infinity;
        if (adam != null && adam.activeInHierarchy) { float dist = Vector2.Distance(transform.position, adam.transform.position); if (dist < minDist) { minDist = dist; closestTarget = adam.transform; closestSprite = adam.GetComponent<SpriteRenderer>(); } }
        if (deva != null && deva.activeInHierarchy) { float dist = Vector2.Distance(transform.position, deva.transform.position); if (dist < minDist) { closestTarget = deva.transform; closestSprite = deva.GetComponent<SpriteRenderer>(); } }
        target = closestTarget; targetSpriteRenderer = closestSprite;
    }

    // Ÿ�� �������� ��������Ʈ �� ���� �ݶ��̴� Offset ������
    void FlipTowardsTarget(bool forceFlip = false)
    {
        if (target == null) return;
        bool shouldFaceRight = (target.position.x > transform.position.x);

        if (facingRight != shouldFaceRight || forceFlip)
        {
            facingRight = shouldFaceRight;

            // 1. ��������Ʈ ����
            if (spriteRenderer != null)
                spriteRenderer.flipX = !facingRight;

            // 2. BoxCollider2D Offset ����
            if (attackCollider != null)
            {
                Vector2 offset = attackCollider.offset;
                offset.x = Mathf.Abs(offset.x) * (facingRight ? 1 : -1); // ���밪�� ���� ���ϱ�
                attackCollider.offset = offset;
            }
        }
    }
    #endregion

    #region �ൿ �ڷ�ƾ (�뽬, ����, �߰� �뽬)

    /// <summary>
    /// �Ϲ� ���� �뽬 �Ǵ� ��뽬(��� ����)�� �����ϴ� �ڷ�ƾ.
    /// </summary>
    IEnumerator DashRoutine(int direction) // 1 = ������, -1 = �ڷ�
    {
        isActing = true;
        isDashing = true;
        animator.SetTrigger(direction == 1 ? "Dash" : "Backdash");

        // ����Ʈ ����
        if (dashTrail != null) dashTrail.emitting = true;
        StartCoroutine(LeaveAfterImage());

        // --- �ڡڡ� �뽬 ���� �� �Ÿ� ��� ���� �ڡڡ� ---
        Vector2 dashDir;
        // �ڡڡ� ����: ����/�Ĺ� ��� �⺻������ dashDistance ��� �ڡڡ�
        float distanceToMove = dashDistance; // �⺻ �̵� �Ÿ��� ���� ����

        if (direction == 1) // ���� �뽬
        {
            if (target != null)
            {
                // �̵� ������ Ÿ�� ����
                dashDir = ((Vector2)target.position - rb.position).normalized;
                FlipTowardsTarget(true); // Ÿ�� ���� ����
                // �� ����: ��ǥ ���� ��� �� �Ÿ� ���� ���� ����
                // Vector2 targetStopPosition = (Vector2)target.position - dirToPlayer * attackRange;
                // distanceToMove = Vector2.Distance(rb.position, targetStopPosition);
                // float dot = Vector2.Dot(dirToPlayer, targetStopPosition - rb.position);
                // if (dot <= 0 || distanceToMove < 0.1f) distanceToMove = 0.1f;
            }
            else // Ÿ�� ������ ���� �������� �⺻ �Ÿ���ŭ
            {
                dashDir = facingRight ? Vector2.right : Vector2.left;
                // distanceToMove = dashDistance; // �̹� ������ ������
            }
        }
        else // ��뽬 (direction == -1)
        {
            Vector2 backwardDir = facingRight ? Vector2.left : Vector2.right;
            Vector2 upwardDir = Vector2.up * backdashUpwardFactor;
            dashDir = (backwardDir + upwardDir).normalized;
            // distanceToMove = dashDistance; // ��뽬�� �׻� ���� �Ÿ� (������ ������)
        }
        // --- ��� ���� �� ---

        // --- �̵� ���� ---
        // �ӵ� ��� (������ �Ÿ��� �ð� ���)
        float currentDashSpeed = (dashDuration > 0.01f) ? distanceToMove / dashDuration : 0;
        rb.velocity = dashDir * currentDashSpeed;

        yield return new WaitForSeconds(dashDuration);

        // --- ���� ó�� ---
        rb.velocity = Vector2.zero;
        isDashing = false;
        if (dashTrail != null) dashTrail.emitting = false;
        yield return new WaitForSeconds(0.3f); // ������
        isActing = false;
        yield break;
    }


    /// <summary>
    /// �ִϸ��̼� �̺�Ʈ���� ȣ��� �Լ�. ���� ��뽬 �̵��� �����մϴ�.
    /// </summary>
    public void TriggerBackdashMovementFromAnim()
    {
        if (!isActing || isDashing || isChaseDashing) return; // �ߺ�/���� ���� ����
        StartCoroutine(ExecuteDashMovement(-1)); // ��뽬 �̵� ����
    }

    /// <summary>
    /// ���� �뽬/��뽬 �̵� �� ���� ó���� ����ϴ� �ڷ�ƾ.
    /// </summary>
    private IEnumerator ExecuteDashMovement(int direction) // -1: ��뽬
    {
        isDashing = true; // ����Ʈ ���ۿ� �÷���

        if (dashTrail != null) dashTrail.emitting = true;
        StartCoroutine(LeaveAfterImage());

        // ���� �� �Ÿ� ���
        Vector2 dashDir;
        float distanceToMove = dashDistance; // ��뽬�� ���� �Ÿ�
        if (direction == -1)
        {
            Vector2 backwardDir = facingRight ? Vector2.left : Vector2.right;
            Vector2 upwardDir = Vector2.up * backdashUpwardFactor;
            dashDir = (backwardDir + upwardDir).normalized;
        }
        else { yield break; } // �� �Լ��� ��뽬 �������� ����


        float currentDashSpeed = (dashDuration > 0.01f) ? distanceToMove / dashDuration : 0;
        rb.velocity = dashDir * currentDashSpeed;
        yield return new WaitForSeconds(dashDuration);
        rb.velocity = Vector2.zero;
        isDashing = false; // ����Ʈ ����� �÷���

        if (dashTrail != null) dashTrail.emitting = false;

        // TODO: ��뽬 �� ��ȯ ���� �ʿ� �� ���⿡ �߰� (BossSummoner ��� ��)
        // bool startedSummoning = false;
        // if (direction == -1 && summoner != null) { startedSummoning = summoner.TryStartSummon(); }
        // if (!startedSummoning) { ... }

        yield return new WaitForSeconds(0.3f); // �ൿ �� ������
        isActing = false; // �� �߿�: ��� �ൿ ����
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

        float timeElapsed = 0f;
        while (timeElapsed < attackHitTiming && isActing)
        {
            if (target != null && !isChaseDashing) { float currentDistance = Vector2.Distance(transform.position, target.position); if (currentDistance > chaseDashTriggerRange) { StartCoroutine(ChaseDashDuringAttack()); } }
            yield return new WaitForSeconds(0.1f);
            timeElapsed += 0.1f;
            if (!IsPlayerValid()) { isActing = false; if (isChaseDashing) { /* �߰� �ߴ� */ } yield break; }
        }

        float remainingTime = attackHitTiming - timeElapsed;
        if (remainingTime > 0) { yield return new WaitForSeconds(remainingTime); }
        while (isChaseDashing) { yield return null; }

        if (isActing) PerformAttackHit();

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
        if (dashTrail != null) dashTrail.emitting = true;
        StartCoroutine(LeaveAfterImage());
        Vector2 chaseDir = Vector2.zero;
        if (IsPlayerValid()) { chaseDir = ((Vector2)target.position - rb.position).normalized; FlipTowardsTarget(true); }
        else { isChaseDashing = false; isDashing = false; if (dashTrail != null) dashTrail.emitting = false; yield break; }
        float chaseSpeed = moveSpeed * chaseDashSpeedMultiplier;
        rb.velocity = chaseDir * chaseSpeed;
        yield return new WaitForSeconds(chaseDashDuration);
        if (isActing) rb.velocity = Vector2.zero; // ���� ���� ���� ����
        isDashing = false;
        isChaseDashing = false;
        if (dashTrail != null) dashTrail.emitting = false;
        yield break;
    }

    // ���� ���� ���� ���� �Լ�
    void PerformAttackHit()
    {
        Debug.Log("���� ���� ����!");
        // TODO: ���� ���� ���� ���� �ʿ�.
        // ����: �ִϸ��̼� �̺�Ʈ�� attackCollider.enabled = true/false ���� ��,
        //       attackCollider�� ���� ��ũ��Ʈ���� OnTriggerEnter2D�� ó��
    }

    #endregion

    #region ���� �� ���� �Լ�
    // �÷��̾ ������ ���ϰ� �ִ��� Ȯ��
    bool IsPlayerFacingBoss()
    {
        if (target == null || targetSpriteRenderer == null) return false;
        bool playerIsFacingRight = !targetSpriteRenderer.flipX;
        bool playerIsRightOfBoss = target.position.x > transform.position.x;
        return (!playerIsRightOfBoss && !playerIsFacingRight) || (playerIsRightOfBoss && playerIsFacingRight);
    }
    // �÷��̾� ���� ���� (����� �׻� true)
    bool CanPredictPlayerAttack() { return true; }
    // �÷��̾� ���� ��ȿ�� Ȯ��
    bool IsPlayerValid() { return target != null && target.gameObject.activeInHierarchy && targetSpriteRenderer != null; }
    // ��� �� ��
    void ValidateAttackBoxes() { }
    #endregion

    #region �뽬 ����Ʈ ����
    // �뽬 �� �ܻ� ���� �ڷ�ƾ
    private IEnumerator LeaveAfterImage()
    {
        while (isDashing) { CreateAfterImage(); yield return new WaitForSeconds(afterImageInterval); }
        yield break;
    }
    // �ܻ� ���� ������Ʈ ���� �� ����
    void CreateAfterImage()
    {
        GameObject afterImage = new GameObject("AfterImage_Boss");
        SpriteRenderer sr = afterImage.AddComponent<SpriteRenderer>();
        sr.sprite = spriteRenderer.sprite;
        sr.color = new Color(1f, 0.2f, 0.2f, 0.7f);
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
        float fadeDuration = afterImageLifetime; Color originalColor = sr.color; float elapsed = 0f;
        while (elapsed < fadeDuration) { if (sr == null) yield break; elapsed += Time.deltaTime; float alpha = Mathf.Lerp(originalColor.a, 0, elapsed / fadeDuration); sr.color = new Color(originalColor.r, originalColor.g, originalColor.b, alpha); yield return null; }
        if (sr != null && sr.gameObject != null) Destroy(sr.gameObject);
        yield break;
    }
    #endregion



    /// <summary>
    /// ���� �ִϸ��̼� ���� �̺�Ʈ�� ȣ���. ª�� ���� �̵�.
    /// </summary>
    public void TriggerAttackLunge()
    {
        if (!isActing || isDashing || isChaseDashing) return;

        Vector2 moveDir = facingRight ? Vector2.right : Vector2.left;
        float lungeDistance = 1.5f;
        float lungeDuration = 0.15f;
        float lungeSpeed = lungeDistance / lungeDuration;

        StartCoroutine(AttackLungeRoutine(moveDir, lungeSpeed, lungeDuration));
    }
    private IEnumerator AttackLungeRoutine(Vector2 moveDir, float speed, float duration)
    {
        isDashing = true;
        if (dashTrail != null)
            dashTrail.emitting = true;

        StartCoroutine(LeaveAfterImage());

        rb.velocity = moveDir * speed;
        yield return new WaitForSeconds(duration);
        rb.velocity = Vector2.zero;

        if (dashTrail != null)
            dashTrail.emitting = false;

        isDashing = false;
    }

    #region Gizmos
    // �� �信�� AI ���� �� ���� �ð�ȭ
    void OnDrawGizmos()
    {
        // ���� �ð�ȭ
        Gizmos.color = Color.red; Gizmos.DrawWireSphere(transform.position, attackRange);
        Gizmos.color = Color.yellow; Gizmos.DrawWireSphere(transform.position, backdashRange);
        Gizmos.color = Color.Lerp(Color.red, Color.yellow, 0.5f); Gizmos.DrawWireSphere(transform.position, chaseDashTriggerRange);
        Gizmos.color = Color.cyan; Gizmos.DrawWireSphere(transform.position, detectRange);

        // ���� �ڽ� �ð�ȭ (BoxCollider2D ����)
        if (attackCollider != null)
        {
            Gizmos.color = Color.magenta;
            // BoxCollider2D�� ���� ��ǥ �ٿ�� ���
            Bounds bounds = attackCollider.bounds;
            Gizmos.DrawWireCube(bounds.center, bounds.size);
        }

        // ���� �ð�ȭ (���� ���� ��)
        if (Application.isPlaying)
        {
            if (target != null) { Gizmos.color = Color.white; Gizmos.DrawLine(transform.position, target.position); Gizmos.color = IsPlayerFacingBoss() ? Color.green : Color.gray; Gizmos.DrawCube(transform.position + Vector3.up * 1.5f, Vector3.one * 0.2f); }
            // bool isSummoningNow = (summoner != null && summoner.IsSummoning); // ��ȯ ��� ��� ��
            if (isActing /*|| isSummoningNow*/) { if (isChaseDashing) Gizmos.color = Color.magenta; else if (isDashing) Gizmos.color = Color.blue; /* else if(isSummoningNow) Gizmos.color = Color.white; */ else Gizmos.color = Color.Lerp(Color.red, Color.black, 0.5f); } else { Gizmos.color = Color.green; }
            Gizmos.DrawSphere(transform.position + Vector3.down * 0.5f, 0.15f);
        }
    }
    #endregion
}