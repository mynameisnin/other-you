using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(AngryGodAiCore))]
public class AngryGodUltimateSkill : MonoBehaviour
{
    public bool IsUltimateActive { get; private set; }
    private float lastUsedTime = -999f;

    [Header("Ultimate Skill Settings")]
    [Tooltip("�ñر� ��� �� ���� �������� �ּ� ��Ÿ��")]
    [SerializeField] public float cooldown = 20f;
    [Tooltip("�ñر� �ִϸ��̼��� �� ���� �ð� (�������� + ���� �ߵ� + �ĵ����� ����)")] // ���� ����
    [SerializeField] private float totalAnimationTime = 4.0f; // ��: ����, �ߵ�, �ĵ��� ��� ������ �ִϸ��̼� �� ����

    // ������Ʈ ����
    private Animator animator;
    private Rigidbody2D rb;
    private AngryGodAiCore aiCore;
    private float originalGravityScale; // ���� �߷°� �����

    void Awake()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        aiCore = GetComponent<AngryGodAiCore>();

        if (animator == null) Debug.LogError("Animator component missing on " + gameObject.name);
        if (rb == null) Debug.LogError("Rigidbody2D component missing on " + gameObject.name);
        if (aiCore == null) Debug.LogError("AngryGodAiCore component missing on " + gameObject.name);
    }

    void Start()
    {
        if (rb != null)
        {
            originalGravityScale = rb.gravityScale;
        }
        lastUsedTime = -cooldown; // ���� ���� �� �ٷ� ����� �� �ֵ���
    }

    /// <summary>
    /// �ñر� �õ��� �����ϴ� �ڷ�ƾ.
    /// AngryGodAiCore �� �ܺο��� ȣ��˴ϴ�.
    /// </summary>
    public IEnumerator TryStartUltimate()
    {
        // 1. �̹� ��� ���̰ų� ��Ÿ���� �� ������ �ߴ�
        if (IsUltimateActive || Time.time < lastUsedTime + cooldown)
        {
            Debug.Log($"[UltimateSkill] TryStartUltimate ��� �ߴ�. IsActive: {IsUltimateActive}, CooldownLeft: {(lastUsedTime + cooldown) - Time.time}");
            yield break;
        }

        Debug.Log("[UltimateSkill] TryStartUltimate ���� ���, ������ ���� �غ�.");
        IsUltimateActive = true;
        aiCore.NotifyActionStart(); // AI Core���� �ൿ ���� �˸�
        Debug.Log("[UltimateSkill] IsUltimateActive=true, NotifyActionStart() ȣ���.");

        // 2. ��� �̵� ���� �� ���� ����
        if (rb != null)
        {
            rb.velocity = Vector2.zero;
            rb.gravityScale = 0f; // �ñر� �� �߷� ���� ����
            Debug.Log("[UltimateSkill] Rigidbody �ӵ�/�߷� ������.");
        }
        if (aiCore.GetPlayer() != null) // aiCore�� GetPlayer()�� �־�� ��
        {
            aiCore.ForceFlipTowardsTarget(); // �÷��̾� �������� ��� ��ȯ
            Debug.Log("[UltimateSkill] �÷��̾� �������� ��ȯ��.");
        }

        // 3. "Ultimate" Trigger �ϳ��� ��ü �ñر� �ִϸ��̼� ����
        animator.SetTrigger("Ultimate"); // �ڡڡ� "Ultimate" Trigger ��� �ڡڡ�
        Debug.Log($"[UltimateSkill] 'Ultimate' Ʈ���� �ߵ�. �� �ִϸ��̼� �ð� ({totalAnimationTime}��) ����.");

        // --- ���� �ñر� ����/ȿ�� �ߵ� Ÿ�̹� ���� ---
        // ��� A: �ִϸ��̼� �̺�Ʈ ��� (����)
        //   - "Ultimate" �ִϸ��̼��� Ư�� ������(��: ���� Ÿ��/ȿ�� �߻��ϴ� ����)��
        //     �ִϸ��̼� �̺�Ʈ�� �߰��ϰ�, �� �̺�Ʈ�� �Ʒ��� ���� �Լ��� ȣ���ϵ��� �����մϴ�.
        //     public void AnimEvent_ExecuteUltimateEffect() { /* ���� ���� ���� */ }

        // ��� B: �ð� ������� �ڷ�ƾ ������ ȿ�� �ߵ� (�� ��Ȯ�� �� ����)
        //   - ���� �ִϸ��̼� �̺�Ʈ ����� ��ƴٸ�, ���⼭ Ư�� �ð�(��: �������� �ð�) ��
        //     ���� ������ �����ϴ� �ڷ�ƾ�� �� ������ �� �ֽ��ϴ�.
        //     StartCoroutine(PerformUltimateAttackLogicWithDelay(�������̽ð�));

        // TODO: ���� ����� ��� �� �ϳ��� �����Ͽ� ���� �ñر� ���� ������ �����ϼ���.
        // ���� (�ð� ���, �����δ� AnimEvent_ExecuteUltimateEffect()�� �� ����):
        // yield return new WaitForSeconds(�������̽ð�_�ִϸ��̼ǿ�_���缭);
        // if (IsUltimateActive) // ���� ��ų�� ��ȿ�ϴٸ�
        // {
        //     Debug.Log("[UltimateSkill] ���� ����/ȿ�� �ߵ�!");
        //     // ���⿡ ���� ������ ����, ����Ʈ ���� �� ����
        // }


        // 4. �ñر� �ִϸ��̼��� �� �ð���ŭ ���
        //    �� �ð� ���� �ִϸ��̼��� ��������, ���� �ߵ�, �ĵ����� ���� ��� �����Ͽ� ����ȴٰ� �����մϴ�.
        yield return new WaitForSeconds(totalAnimationTime);
        Debug.Log($"[UltimateSkill] Total animation time ({totalAnimationTime}��) �����.");

        // 5. �ñرⰡ �߰��� ��ҵ��� �ʾҴ��� Ȯ�� (��: ���� ���)
        //    totalAnimationTime ��� �߿� � �����ε� IsUltimateActive�� false�� �Ǿ��ٸ� (AbortUltimate ȣ�� ��)
        if (!IsUltimateActive)
        {
            Debug.Log("[UltimateSkill] Ultimate �� ��ҵ� (IsUltimateActive�� false). �߰� ���� ó�� ���ʿ� (�̹� AbortUltimate���� ó����).");
            // AbortUltimate���� �̹� NotifyActionEnd ���� ȣ������ ���̹Ƿ� ���⼭�� �߰� �۾��� ���� �� �ֽ��ϴ�.
            // ��, AbortUltimate�� ȣ����� �ʾҴµ� IsUltimateActive�� false�� �� �������� ��Ȳ�� ����� ���� �ֽ��ϴ�.
            if (rb != null && rb.gravityScale == 0f) rb.gravityScale = originalGravityScale; // �߷� ���� Ȯ��
            if (aiCore != null && !IsUltimateActive) aiCore.NotifyActionEnd(); // ������ ���� �ѹ� ��
            yield break;
        }

        // 6. �������� �ñر� ���� ó��
        yield return FinishUltimate();
    }

    /// <summary>
    /// �ִϸ��̼� �̺�Ʈ �Ǵ� �ð� ������� ���� �ñر� ȿ���� �ߵ��� �� ȣ��� �� �ִ� �Լ� (����)
    /// </summary>
    public void ExecuteUltimateEffect() // �ִϸ��̼� �̺�Ʈ���� ȣ��ǵ��� �̸� ���� ����
    {
        if (!IsUltimateActive) return; // �̹� ��ҵǾ��ų� �������� ���� ����
        Debug.Log("[UltimateSkill] >>> ���� �ñر� ȿ�� �ߵ�! <<<");
        // ���⿡ ������ ����, ����Ʈ ���� �� ���� �ñر� ������ �����մϴ�.
    }


    private IEnumerator FinishUltimate()
    {
        Debug.Log("[UltimateSkill] Finishing ultimate (���� ����).");
        if (rb != null)
        {
            rb.gravityScale = originalGravityScale; // ���� �߷����� ����
        }
        IsUltimateActive = false;
        lastUsedTime = Time.time; // ���������� �������Ƿ� ��Ÿ�� ����
        aiCore.NotifyActionEnd(); // AI Core���� �ൿ �������� �˸�
        yield break;
    }

    /// <summary>
    /// � �����ε� �ñرⰡ �߰��� �ߴܵǾ�� �� �� ȣ�� (��: ���� ���)
    /// </summary>
    public IEnumerator AbortUltimate()
    {
        Debug.Log("[UltimateSkill] Aborting ultimate (���� �ߴ�).");
        // ���� ���̴� ��� ���� �ڷ�ƾ ���� (�� ��ũ��Ʈ ������ �߰��� ������ �ڷ�ƾ�� �ִٸ�)
        // StopAllCoroutines(); // ���� ���������� TryStartUltimate �ϳ��� ����ǹǷ�, �� ������ �ݵ�� �ʿ����� ����.
        // ���� TryStartUltimate ������ �ٸ� �ڷ�ƾ�� �����Ѵٸ� ���.

        if (rb != null)
        {
            rb.gravityScale = originalGravityScale; // �߷� ���� �õ�
        }

        // �̹� false�� �� ������, Ȯ���ϰ� �ϱ� ����.
        // �׸��� �ٸ� ������ IsUltimateActive ���¸� ���� ��� �ߴ��� �� �ֵ���.
        IsUltimateActive = false;

        // lastUsedTime�� ������Ʈ���� ���� (���������� ���� ���� �ƴϹǷ� ��Ÿ�� ���� �� �� �Ǵ� �ٸ� ��å)
        // �Ǵ�, �ߴܵǾ ��Ÿ���� �����ϰ� �ʹٸ� lastUsedTime = Time.time; �߰�

        aiCore.NotifyActionEnd(); // AI Core���� �ൿ �������� �˸�
        yield break;
    }

    public float GetLastUseTime() => lastUsedTime;
}