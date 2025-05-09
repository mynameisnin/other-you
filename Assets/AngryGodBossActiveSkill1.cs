using UnityEngine;
using System.Collections;

/// <summary>
/// ������ Ư�� ��Ƽ�� ��ų (��� �� ����) + MeteorSpawner ���� ����.
/// ������ �ٶ󺸴� �������� ���������� ���׿� 7���� ���Ͻ�Ŵ.
/// </summary>
[RequireComponent(typeof(AngryGodAiCore))]
[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(Rigidbody2D))]
public class AngryGodActiveSkill1 : MonoBehaviour
{
    [Header("Skill Settings")]
    [SerializeField] private float skillCooldown = 8f;
    [SerializeField] private float ascendDescendSpeed = 14.9f;
    [SerializeField] private float ascendHeight = 28.9f;

    [Header("Effects (Optional)")]
    [SerializeField] private GameObject magicEffectPrefab;
    [SerializeField] private Transform magicSpawnPoint;
    [SerializeField] private GameObject startSkillVFX;
    [SerializeField] private GameObject airActionVFX;

    [Header("Meteor Settings")]
    [SerializeField] private GameObject telegraphPrefab;
    [SerializeField] private GameObject meteorPrefab;
    [SerializeField] private float warningDuration = 1.0f;
    [SerializeField] private float meteorFallSpeed = 10f;
    [SerializeField] private int meteorCount = 7;
    [SerializeField] private float meteorSpacing = 1.5f;

    private AngryGodAiCore aiCore;
    private Animator animator;
    private Rigidbody2D rb;

    private bool isSkillActive = false;
    private float lastSkillUseTime = -99f;
    private float originalGravityScale;
    private Coroutine moveCoroutine;
    private Vector2 groundPosition;
    private GameObject currentAirVFXInstance = null;
    private Coroutine afterImageCoroutine = null;

    private void Awake()
    {
        aiCore = GetComponent<AngryGodAiCore>();
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
    }

    private void Start()
    {
        lastSkillUseTime = -skillCooldown;
        originalGravityScale = rb.gravityScale;
    }

    public bool TryStartSkill(float distanceToTarget)
    {
        if (Time.time >= lastSkillUseTime + skillCooldown && !isSkillActive && !aiCore.IsCurrentlyActing())
        {
            aiCore.InitiateBackdash();
            return true;
        }
        return false;
    }

    public IEnumerator TryStartSkillAfterBackdash()
    {
        //  ���� ��Ÿ���� ���������� �������� ����
        if (Time.time < aiCore.GetGlobalCooldownTime())
            yield break;

        //  ���� ��Ÿ�� ���� (��: 6��)
        aiCore.SetGlobalCooldownTime(Time.time + 6f);

        yield return new WaitForSeconds(0.1f);
        StartCoroutine(SkillRoutine());
    }

    private IEnumerator SkillRoutine()
    {
        isSkillActive = true;
        aiCore.NotifyActionStart();

        aiCore.StopMovement();
        aiCore.ForceFlipTowardsTarget();
        rb.velocity = Vector2.zero;
        rb.gravityScale = 0;
        groundPosition = transform.position;

        if (startSkillVFX != null)
            Instantiate(startSkillVFX, transform.position, Quaternion.identity);

        animator.SetTrigger("ActiveSkill1");

        while (isSkillActive)
        {
            if (!aiCore.IsPlayerValid())
            {
                yield return AbortSkill();
                yield break;
            }
            yield return null;
        }
    }

    public void AnimEvent_StartAscend()
    {
        if (!isSkillActive) return;
        if (moveCoroutine != null) StopCoroutine(moveCoroutine);
        if (afterImageCoroutine != null) StopCoroutine(afterImageCoroutine);

        Vector2 targetPos = groundPosition + Vector2.up * ascendHeight;
        afterImageCoroutine = StartCoroutine(LeaveAfterImage());
        moveCoroutine = StartCoroutine(MoveToPositionRoutine(targetPos, ascendDescendSpeed));
    }

    public void AnimEvent_PerformAirAction()
    {
        if (!isSkillActive) return;
        if (moveCoroutine != null) StopCoroutine(moveCoroutine);
        rb.velocity = Vector2.zero;
        StartCoroutine(AirActionRoutine());
    }

    private IEnumerator AirActionRoutine()
    {
        Transform player = aiCore.GetPlayer(); // �ݵ�� AngryGodAiCore�� GetPlayer()�� �־�� ��
        Vector2 playerDir = ((Vector2)player.position - (Vector2)transform.position).normalized;
        Vector2 basePos = (Vector2)transform.position + playerDir * 2f + Vector2.down;

        Vector2[] meteorTargets = new Vector2[meteorCount];
        for (int i = 0; i < meteorCount; i++)
        {
            float offset = i * meteorSpacing * 2.5f;               //  �� �а� ����
            float randomY = Random.Range(-1f, 1f);                  //  Y�� �� ��鸮��
            Vector2 offsetDir = (playerDir + new Vector2(Random.Range(-0.3f, 0.3f), Random.Range(-0.2f, 0.2f))).normalized;
            meteorTargets[i] = basePos + offsetDir * (offset * 0.7f) + Vector2.up * randomY;

        }

        // ���� ����
        for (int i = 0; i < meteorTargets.Length; i++)
        {
            int rand = Random.Range(i, meteorTargets.Length);
            (meteorTargets[i], meteorTargets[rand]) = (meteorTargets[rand], meteorTargets[i]);
        }

        // ���׿� ����
        for (int i = 0; i < meteorTargets.Length; i++)
        {
            Vector2 targetPos = meteorTargets[i];
            // �� �缱���� ��������: �� �ָ���, �� ������
            Vector2 horizontalDir = playerDir.x >= 0 ? Vector2.right : Vector2.left;
            Vector2 spawnOffset = horizontalDir * 14f + Vector2.up * 10f;

            Vector2 spawnPos = targetPos + spawnOffset;

            // ������ �Ʒ��� �������� �ʵ��� ����
            spawnPos.y = Mathf.Max(spawnPos.y, groundPosition.y + 1f);
            StartCoroutine(SpawnMeteorWithWarning(spawnPos, targetPos));
            yield return new WaitForSeconds(Random.Range(0.05f, 0.15f));
        }
    }

    public void AnimEvent_StartDescend()
    {
        if (!isSkillActive) return;
        if (currentAirVFXInstance != null) Destroy(currentAirVFXInstance);
        if (moveCoroutine != null) StopCoroutine(moveCoroutine);
        moveCoroutine = StartCoroutine(MoveToPositionRoutine(groundPosition, ascendDescendSpeed));
    }

    public void AnimEvent_SkillEnd()
    {
        if (!isSkillActive) return;
        EndSkill();
    }

    private IEnumerator MoveToPositionRoutine(Vector2 targetPosition, float speed)
    {
        while (rb != null && Vector2.Distance(rb.position, targetPosition) > 0.1f)
        {
            if (!isSkillActive) { rb.velocity = Vector2.zero; yield break; }
            rb.velocity = (targetPosition - rb.position).normalized * speed;
            yield return null;
        }
        rb.velocity = Vector2.zero;
        rb.position = targetPosition;
        moveCoroutine = null;
    }

    private void EndSkill()
    {
        if (moveCoroutine != null) StopCoroutine(moveCoroutine);
        if (afterImageCoroutine != null) StopCoroutine(afterImageCoroutine);
        if (currentAirVFXInstance != null) Destroy(currentAirVFXInstance);

        rb.velocity = Vector2.zero;
        rb.gravityScale = originalGravityScale;

        isSkillActive = false;
        aiCore.NotifyActionEnd();
        lastSkillUseTime = Time.time;
    }

    private IEnumerator AbortSkill()
    {
        if (moveCoroutine != null) StopCoroutine(moveCoroutine);
        rb.gravityScale = originalGravityScale;
        EndSkill();
        yield break;
    }

    private IEnumerator LeaveAfterImage()
    {
        while (isSkillActive)
        {
            CreateAfterImage();
            yield return new WaitForSeconds(0.05f);
        }
    }

    private void CreateAfterImage()
    {
        GameObject afterImage = new GameObject("AfterImage_Skill1");
        SpriteRenderer sr = afterImage.AddComponent<SpriteRenderer>();
        SpriteRenderer original = GetComponent<SpriteRenderer>();

        sr.sprite = original.sprite;
        sr.color = new Color(1f, 0.2f, 0.2f, 0.7f);
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

        Destroy(sr.gameObject);
    }

    private IEnumerator SpawnMeteorWithWarning(Vector2 spawnPos, Vector2 targetPos)
    {
        GameObject warning = Instantiate(telegraphPrefab, targetPos, Quaternion.identity);

        // ȸ�� ���� ����
        float angle = Mathf.Atan2((targetPos - spawnPos).y, (targetPos - spawnPos).x) * Mathf.Rad2Deg;
        warning.transform.rotation = Quaternion.Euler(0, 0, angle);

        yield return new WaitForSeconds(warningDuration);
        Destroy(warning);

        GameObject meteor = Instantiate(meteorPrefab, spawnPos, Quaternion.identity);
        Rigidbody2D rbMeteor = meteor.GetComponent<Rigidbody2D>();
        rbMeteor.velocity = (targetPos - spawnPos).normalized * meteorFallSpeed;

        // ?? ���� �������� flipX ����
        SpriteRenderer sr = meteor.GetComponent<SpriteRenderer>();
        if (sr != null)
        {
            sr.flipX = (targetPos.x > spawnPos.x);  // �������̸� flipX = true
        }
    }


    public float GetLastSkillUseTime() => lastSkillUseTime;
    public bool IsSkillActive => isSkillActive;
}
