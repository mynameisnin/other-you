using UnityEngine;
using System.Collections;
using System.Collections.Generic; // Gizmos ������

/// <summary>
/// ������ Ư�� ��Ƽ�� ��ų (��� �� ����)�� �����ϴ� ��ũ��Ʈ.
/// AngryGodAiCore�� �����ϸ�, �ִϸ��̼� �̺�Ʈ�� ���/�ϰ� �� �׼� ������ �����մϴ�.
/// </summary>
[RequireComponent(typeof(AngryGodAiCore))]
[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(Rigidbody2D))]
public class AngryGodActiveSkill1 : MonoBehaviour
{
    #region ���� ����

    [Header("��ų ����")]
    [Tooltip("��ų ��� �� ���� �������� ��� �ð� (��)")]
    [SerializeField] private float skillCooldown = 20f;
    [Tooltip("���/�ϰ� �� ����� �ӵ�")]
    [SerializeField] private float ascendDescendSpeed = 5f;
    [Tooltip("�ִ� ��� ���� (���� ��ġ ����)")]
    [SerializeField] private float ascendHeight = 4f;
    [Tooltip("�÷��̾ ��ų ���� ���� �� �Ÿ����� ������ ��뽬 ����")]
    [SerializeField] private float requiredBackdashDistance = 3.5f; // AttackRange���� �ణ ũ��

    // --- Prefabs and Effects (Optional) ---
    [Header("Effects (Optional)")]
    [Tooltip("���� �׼� �� ������ ���� ȿ��/�߻�ü ������")]
    [SerializeField] private GameObject magicEffectPrefab;
    [Tooltip("���� ȿ���� ������ ��ġ Transform")]
    [SerializeField] private Transform magicSpawnPoint;
    [Tooltip("��ų ���� �� ����� VFX")]
    [SerializeField] private GameObject startSkillVFX;
    [Tooltip("���� �׼� �� ����� VFX")]
    [SerializeField] private GameObject airActionVFX;
    private GameObject currentAirVFXInstance = null; // ���� ��� ���� ���� VFX �ν��Ͻ�


    // --- ���� ���� ---
    private float lastSkillUseTime = -99f; // ������ ��ų ��� �ð�
    private bool isSkillActive = false;   // ���� �� ��ų �ڷ�ƾ ���� ������ ����
    private Vector2 groundPosition;       // ��� �� ���� ���� ��ġ �����
    private Coroutine moveCoroutine = null; // ���� �������� �̵� �ڷ�ƾ ����
    private float originalGravityScale;   // ���� �߷� ������ ��

    // --- ���� ---
    private AngryGodAiCore aiCore; // ���� AI ��ũ��Ʈ ����
    private Animator animator;     // �ִϸ����� ����
    private Rigidbody2D rb;        // Rigidbody2D ����

    #endregion

    #region �ʱ�ȭ

    private void Awake()
    {
        aiCore = GetComponent<AngryGodAiCore>();
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();

        if (aiCore == null || animator == null || rb == null)
        {
            Debug.LogError("�ʼ� ���� ������Ʈ ����!", this);
            this.enabled = false;
        }
    }

    private void Start()
    {
        lastSkillUseTime = -skillCooldown; // ��ٿ� �ʱ�ȭ
        if (rb != null) originalGravityScale = rb.gravityScale; // ���� �߷� ����
        else this.enabled = false;
    }

    #endregion

    #region ��ų ���� �� ����

    /// <summary>
    /// �ܺ�(AngryGodAiCore)���� ��ų ����� �õ�. ��ٿ�, �Ÿ�, ���� Ȯ�� �� ���� �Ǵ� ��뽬 ��û.
    /// </summary>
    public bool TryStartSkill(float distanceToTarget)
    {
        bool cooldownReady = Time.time >= lastSkillUseTime + skillCooldown;
        bool skillNotActive = !isSkillActive;
        bool aiCoreNotActing = (aiCore != null && !aiCore.IsCurrentlyActing());

        Debug.Log($"[Skill1] TryStartSkill Check: Cooldown Ready={cooldownReady}, Skill Not Active={skillNotActive}, AI Core Not Acting={aiCoreNotActing}");
        Debug.Log($"[Skill1] Time: {Time.time}, LastUse: {lastSkillUseTime}, CD: {skillCooldown}");
        if (cooldownReady && skillNotActive && aiCoreNotActing)
        {
            //  ������ ����: �Ÿ��� ������� ������ ��뽬 �� ���� ��ų ����
            Debug.Log("[Skill1] Forcing backdash before skill.");
            aiCore.InitiateBackdash();

            //  ��뽬 �� �ڵ����� ��ų ������ �� �ֵ��� �÷��׸� �ְų�,
            // �Ǵ� AngryGodAiCore �ʿ��� ��뽬 ���� �� SkillRoutine ȣ�� �ʿ�
            // (���� �ڷ�ƾ ȣ�� X ? InitiateBackdash ���ο��� Ʈ���� �ʿ�)

            return true;
        }
        else
        {
            Debug.LogWarning($"[Skill1] Cannot start skill. CooldownReady:{cooldownReady}, SkillNotActive:{skillNotActive}, AICoreNotActing:{aiCoreNotActing}");
            return false;
        }
    }


    /// <summary>
    /// ��ų ��ü ������ �ڷ�ƾ. �ִϸ��̼� �̺�Ʈ�� ���� �ֿ� ������ Ʈ���ŵ�.
    /// </summary>
    private IEnumerator SkillRoutine()
    {
        isSkillActive = true;
        aiCore.NotifyActionStart(); // AI �ھ� �ൿ ���� �˸�
        Debug.Log("[Skill1] ��ų ������ ����.");

        // �غ� �ܰ�
        aiCore.StopMovement(); // �̵� ����
        aiCore.ForceFlipTowardsTarget(); // Ÿ�� ���� ����
        rb.velocity = Vector2.zero; // ������ ����� �ӵ� 0
        rb.gravityScale = 0; // �߷� ��Ȱ��ȭ
        groundPosition = transform.position; // ���� ��ġ ����

        // ���� VFX (������)
        if (startSkillVFX != null) Instantiate(startSkillVFX, transform.position, Quaternion.identity);

        // "ActiveSkill1" �ִϸ��̼� Ʈ����
        animator.SetTrigger("ActiveSkill1");

        // �� �ڷ�ƾ�� ��ų�� ������ ���� ������ ����ϴ� ���Ҹ� ��
        // ���� ���/�ϰ�/�׼�/����� �ִϸ��̼� �̺�Ʈ�� ���
        while (isSkillActive)
        {
            // ���� ��ġ: Ÿ���� �߰��� ������� ��ų �ߴ�
             //  ����: aiCore null üũ �߰�
            if (aiCore == null || !aiCore.IsPlayerValid())
            {
                Debug.Log("[Skill1] ��ų �� Ÿ�� �ҽ�, �ߴ� ����.");
                yield return StartCoroutine(AbortSkill()); // �ߴ� ó�� �ڷ�ƾ ����
                yield break; // ���� �ڷ�ƾ ����
            }
            yield return null; // ���� �����ӱ��� ���
        }

        Debug.Log("[Skill1] ��ų ������ �ڷ�ƾ ����.");
        Debug.Log($"[Skill1] isSkillActive={isSkillActive}, lastSkillUseTime={lastSkillUseTime}, cooldown={skillCooldown}");
        yield break;
    }
    public IEnumerator TryStartSkillAfterBackdash()
    {
        yield return new WaitForSeconds(0.1f); // ������ (������)
        StartCoroutine(SkillRoutine()); // ���� ����
    }
    /// <summary>
    /// [Animation Event] ��� �̵� ����.
    /// </summary>
    public void AnimEvent_StartAscend()
    {
        if (!isSkillActive) return;
        Debug.Log("[Skill1] AnimEvent: ��� ����");

        if (moveCoroutine != null) StopCoroutine(moveCoroutine);
        Vector2 targetPos = groundPosition + Vector2.up * ascendHeight;

        //  �ܻ� ����
        if (afterImageCoroutine != null) StopCoroutine(afterImageCoroutine);
        afterImageCoroutine = StartCoroutine(LeaveAfterImage());

        moveCoroutine = StartCoroutine(MoveToPositionRoutine(targetPos, ascendDescendSpeed));
    }

    /// <summary>
    /// [Animation Event] ���� �׼� (���� ���� ��) ����.
    /// </summary>
    public void AnimEvent_PerformAirAction()
    {
        if (!isSkillActive) return;
        Debug.Log("[Skill1] AnimEvent: ���� �׼� ����");

        // ���/�ϰ� ���̾��ٸ� ����
        if (moveCoroutine != null) StopCoroutine(moveCoroutine);
        if (rb != null) rb.velocity = Vector2.zero; // null üũ �߰�

        // ���� �׼� ����
        StartCoroutine(AirActionRoutine());
    }

    /// <summary>
    /// ���� �׼� �ڷ�ƾ (��: ���� ����).
    /// </summary>
    private IEnumerator AirActionRoutine()
    {
        Debug.Log("[Skill1] ���� �׼� ���� ��...");

        // ���� VFX ���� (������)
        if (airActionVFX != null) currentAirVFXInstance = Instantiate(airActionVFX, transform.position, Quaternion.identity, transform);

        // ���� ȿ�� ����
        if (magicEffectPrefab != null && magicSpawnPoint != null)
        {
            Instantiate(magicEffectPrefab, magicSpawnPoint.position, magicSpawnPoint.rotation);
        }
        else Debug.LogWarning("[Skill1] ���� ȿ�� ������ �Ǵ� ���� ��ġ �̼���.");

        // �ִϸ��̼��� �ڿ������� �ϰ� �غ� �������� �Ѿ���� ��
        yield break;
    }


    /// <summary>
    /// [Animation Event] �ϰ� �̵� ����.
    /// </summary>
    public void AnimEvent_StartDescend()
    {
        if (!isSkillActive) return;
        Debug.Log("[Skill1] AnimEvent: �ϰ� ����");

        if (currentAirVFXInstance != null) Destroy(currentAirVFXInstance);
        if (moveCoroutine != null) StopCoroutine(moveCoroutine);
        //  ����: ascendDescendSpeed ���� ��� Ȯ�� (�̹� �ùٸ��� �Ǿ� ����)
        moveCoroutine = StartCoroutine(MoveToPositionRoutine(groundPosition, ascendDescendSpeed));
    }
    /// <summary>
    /// [Animation Event] ��ų �ִϸ��̼� ���� ���� �� ȣ��.
    /// </summary>
    public void AnimEvent_SkillEnd()
    {
        if (!isSkillActive) return;
        Debug.Log("[Skill1] AnimEvent: ��ų ����");
        EndSkill(); // ��ų ������ ó��
    }

    /// <summary>
    /// ������ ��ġ���� �̵��ϴ� �ڷ�ƾ.
    /// </summary>
    private IEnumerator MoveToPositionRoutine(Vector2 targetPosition, float speed)
    {
        //  ����: rb null üũ �߰�
        while (rb != null && Vector2.Distance(rb.position, targetPosition) > 0.1f)
        {
            if (!isSkillActive) { if (rb != null) rb.velocity = Vector2.zero; yield break; }
            Vector2 direction = (targetPosition - rb.position).normalized;
             if (rb != null) rb.velocity = direction * speed;
            yield return null;
        }
        if (rb != null)
        {
            rb.velocity = Vector2.zero; // ��ǥ ���� �� ����
            rb.position = targetPosition; // ��Ȯ�� ��ġ�� ����
        }
        moveCoroutine = null;
    }

    /// <summary>
    /// ��ų ���� ó��: �߷� ����, ���� ����, AI �ھ� �˸�, ��ٿ� ����.
    /// </summary>
    private void EndSkill()
    {
        if (!isSkillActive) return;
        Debug.Log("[Skill1] ��ų ���� ó��.");

        if (moveCoroutine != null) StopCoroutine(moveCoroutine);
        if (rb != null) // null üũ �߰�
        {
            rb.velocity = Vector2.zero;
            rb.gravityScale = originalGravityScale; // ���� �߷����� ����
        }
        if (currentAirVFXInstance != null) Destroy(currentAirVFXInstance);

        isSkillActive = false;
        //  ����: aiCore null üũ �߰�
        if (aiCore != null) aiCore.NotifyActionEnd();
        lastSkillUseTime = Time.time;
    }

    /// <summary>
    /// ��ų �ߴ� ó�� �ڷ�ƾ (��: Ÿ�� �ҽ� ��).
    /// </summary>
    private IEnumerator AbortSkill()
    {
        if (moveCoroutine != null) StopCoroutine(moveCoroutine);

        //  ����: rb null üũ �߰�
        if (rb != null && Vector2.Distance(rb.position, groundPosition) > 0.1f)
        {
            Debug.Log("[Skill1] ��ų �ߴ�, ���� �ϰ� �õ�.");
            rb.gravityScale = originalGravityScale;
        }

        EndSkill(); // ���� ����
        yield break;
    }
    private Coroutine afterImageCoroutine = null;

    private IEnumerator LeaveAfterImage()
    {
        while (isSkillActive)
        {
            CreateAfterImage();
            yield return new WaitForSeconds(0.05f); // ���� ���� ����
        }
    }

    private void CreateAfterImage()
    {
        GameObject afterImage = new GameObject("AfterImage_Skill1");
        SpriteRenderer sr = afterImage.AddComponent<SpriteRenderer>();
        SpriteRenderer original = GetComponent<SpriteRenderer>();
        sr.sprite = original.sprite;
        sr.color = new Color(1f, 0.2f, 0.2f, 0.7f); // ������ ���� �ܻ�
        sr.flipX = original.flipX;
        sr.sortingLayerName = original.sortingLayerName;
        sr.sortingOrder = original.sortingOrder - 1;
        afterImage.transform.position = transform.position;
        afterImage.transform.localScale = transform.localScale;
        StartCoroutine(FadeOutAndDestroy(sr));
    }

    private IEnumerator FadeOutAndDestroy(SpriteRenderer sr)
    {
        float duration = 0.5f;
        float elapsed = 0f;
        Color startColor = sr.color;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float alpha = Mathf.Lerp(startColor.a, 0, elapsed / duration);
            sr.color = new Color(startColor.r, startColor.g, startColor.b, alpha);
            yield return null;
        }

        if (sr != null && sr.gameObject != null)
            Destroy(sr.gameObject);
    }
    public float GetLastSkillUseTime()
    {
        return lastSkillUseTime;
    }
    #endregion

    #region �ܺ� ���ٿ� ������Ƽ
    /// <summary> ���� �� ��ų�� Ȱ��ȭ �������� ���� </summary>
    public bool IsSkillActive => isSkillActive;
    #endregion
}