using System.Collections;
using UnityEngine;

/// <summary>
/// ������ ȭ�� �극�� ��ų. ���� ���� �� �÷��̾ ������ �ߵ�.
/// �̵� ���� �� ������ �ִϸ��̼� �̺�Ʈ�� ����.
/// ȭ�� ���� �ܻ� ���� + �ݶ��̴� ���� �ڵ� ����.
/// </summary>
[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(AngryGodAiCore))]
public class AngryGodFlameSkill : MonoBehaviour
{
    [Header("Flame Skill Settings")]
    [SerializeField] private float flameDetectRange = 4.5f;
    [SerializeField] private float flameCooldown = 15f;
    [SerializeField] private BoxCollider2D flameCollider; // ?? ������ BoxCollider2D

    [Header("AfterImage Settings")]
    [SerializeField] private float afterImageInterval = 0.05f;
    [SerializeField] private float afterImageLifetime = 0.5f;

    private Animator animator;
    private Rigidbody2D rb;
    private AngryGodAiCore aiCore;
    private SpriteRenderer spriteRenderer;

    private float lastFlameTime = -99f;
    private bool isFlaming = false;
    private Coroutine afterImageCoroutine;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        aiCore = GetComponent<AngryGodAiCore>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void Update()
    {
        if (isFlaming || aiCore == null || aiCore.IsCurrentlyActing())
            return;

        Transform target = aiCore.GetPlayer();
        if (target == null) return;

        float distance = Vector2.Distance(transform.position, target.position);
        if (distance <= flameDetectRange && Time.time >= lastFlameTime + flameCooldown)
        {
            StartCoroutine(FlameRoutine());
        }
    }

    private IEnumerator FlameRoutine()
    {
        isFlaming = true;
        aiCore.NotifyActionStart();
        rb.velocity = Vector2.zero;
        aiCore.ForceFlipTowardsTarget();

        UpdateFlameColliderDirection(); // �� ���⿡ ���� �ݶ��̴� ��ġ ����

        animator.SetTrigger("Flame");
        lastFlameTime = Time.time;

        afterImageCoroutine = StartCoroutine(LeaveAfterImage());
        yield break;
    }

    public void AnimEvent_FlameStart()
    {
        rb.velocity = Vector2.zero;

        UpdateFlameColliderDirection(); // �ٽ� �� �� ����
    }

    public void AnimEvent_FlameEnd()
    {
        aiCore.NotifyActionEnd();
        isFlaming = false;

        if (afterImageCoroutine != null)
            StopCoroutine(afterImageCoroutine);
    }

    private void UpdateFlameColliderDirection()
    {
        if (flameCollider == null || spriteRenderer == null) return;

        Vector2 offset = flameCollider.offset;
        float absX = Mathf.Abs(offset.x);

        // SpriteRenderer�� flipX�� true�� ��������, �ƴϸ� ����������
        offset.x = spriteRenderer.flipX ? -absX : absX;
        flameCollider.offset = offset;
    }


    private IEnumerator LeaveAfterImage()
    {
        while (isFlaming)
        {
            CreateAfterImage();
            yield return new WaitForSeconds(afterImageInterval);
        }
    }

    private void CreateAfterImage()
    {
        GameObject afterImage = new GameObject("FlameAfterImage");
        SpriteRenderer sr = afterImage.AddComponent<SpriteRenderer>();

        sr.sprite = spriteRenderer.sprite;
        sr.flipX = spriteRenderer.flipX;
        sr.sortingLayerName = spriteRenderer.sortingLayerName;
        sr.sortingOrder = spriteRenderer.sortingOrder - 1;
        sr.color = new Color(1f, 0.3f, 0.1f, 0.7f);

        afterImage.transform.position = transform.position;
        afterImage.transform.localScale = transform.localScale;

        StartCoroutine(FadeOutAndDestroy(sr));
    }

    private IEnumerator FadeOutAndDestroy(SpriteRenderer sr)
    {
        float elapsed = 0f;
        Color originalColor = sr.color;

        while (elapsed < afterImageLifetime)
        {
            elapsed += Time.deltaTime;
            float alpha = Mathf.Lerp(originalColor.a, 0, elapsed / afterImageLifetime);
            sr.color = new Color(originalColor.r, originalColor.g, originalColor.b, alpha);
            yield return null;
        }

        if (sr != null && sr.gameObject != null)
            Destroy(sr.gameObject);
    }

    public bool IsFlaming => isFlaming;
    public float GetLastFlameTime() => lastFlameTime;
    public IEnumerator TryUseFlame()
    {
        if (isFlaming || aiCore == null || aiCore.IsCurrentlyActing())
            yield break;

        yield return FlameRoutine(); // ���� FlameRoutine �ڷ�ƾ ���
    }
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, flameDetectRange);
    }
}
