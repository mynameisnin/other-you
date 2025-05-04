using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ������ ���� ��ȯ �ɷ��� �����ϴ� ��ũ��Ʈ.
/// AngryGodAiCore�� �����Ͽ� �۵��մϴ�.
/// </summary>
[RequireComponent(typeof(AngryGodAiCore))] // �� ��ũ��Ʈ�� �۵��Ϸ��� AngryGodAiCore�� �ʿ���
[RequireComponent(typeof(Animator))]      // �ִϸ��̼� ������ ���� Animator �ʿ�
public class BossSummoner : MonoBehaviour
{
    #region ���� ����

    // --- ��ȯ ���� ---
    [Header("��ȯ ����")]
    [Tooltip("��ȯ�� ���� ������")]
    [SerializeField] private GameObject monsterPrefab;
    [Tooltip("���Ͱ� ��ȯ�� ��ġ Transform �迭")]
    [SerializeField] private Transform[] summonPoints;
    [Tooltip("��ȯ �ִϸ��̼� ���� �� ���� ��ȯ���� ��� �ð� (��)")]
    [SerializeField] private float summonTiming = 1.0f;
    [Tooltip("��ȯ ��ų ��� �� ���� ��ȯ������ ��� �ð� (��)")]
    [SerializeField] private float summonCooldown = 15f;
    [Tooltip("�� ���� ��ȯ�� ���� ��")]
    [SerializeField] private int monstersPerSummon = 3;

    // --- ���� ���� ---
    private float lastSummonTime = -15f; // ������ ��ȯ �ð� (��ٿ� ����)
    private bool isSummoning = false;    // ���� ��ȯ �ڷ�ƾ ���� ������ ����

    // --- ���� ---
    private AngryGodAiCore aiCore; // ���� AI ��ũ��Ʈ ����
    private Animator animator;     // �ִϸ����� ����

    #endregion

    #region �ʱ�ȭ �� ��ȿ�� �˻�

    private void Awake()
    {
        // �ʿ��� ������Ʈ ���� ��������
        aiCore = GetComponent<AngryGodAiCore>();
        animator = GetComponent<Animator>();

        // ���� ��ȿ�� �˻�
        if (aiCore == null) Debug.LogError("AngryGodAiCore ������ ã�� �� �����ϴ�!", this);
        if (animator == null) Debug.LogError("Animator ������ ã�� �� �����ϴ�!", this);
    }

    private void Start()
    {
        // ��ȯ ���� ���� ��ȿ�� �˻�
        if (monsterPrefab == null) Debug.LogWarning("Monster Prefab�� �Ҵ���� �ʾҽ��ϴ�. ��ȯ�� �Ұ����մϴ�.", this);
        if (summonPoints == null || summonPoints.Length == 0) Debug.LogWarning("Summon Points�� �������� �ʾҽ��ϴ�. ��ȯ ��ġ�� �������ּ���.", this);

        // ��ٿ� �ʱ�ȭ (���� ���� �� �ٷ� ��� �����ϵ���)
        lastSummonTime = -summonCooldown;
    }

    #endregion

    #region ��ȯ ���� �� ����

    /// <summary>
    /// �ܺ�(��: AngryGodAiCore)���� ��ȯ�� �õ��� �� ȣ���ϴ� �Լ�.
    /// ��ٿ� �� ���� ���¸� Ȯ���Ͽ� ��ȯ �ڷ�ƾ�� �����մϴ�.
    /// </summary>
    /// <returns>��ȯ�� ���������� true, �ƴϸ� false</returns>
    public bool TryStartSummon()
    {
        // ��ٿ� Ȯ�� �� ���� ��ȯ ������ Ȯ��
        if (Time.time >= lastSummonTime + summonCooldown && !isSummoning && monsterPrefab != null && summonPoints.Length > 0)
        {
            StartCoroutine(SummonRoutine()); // ��ȯ �ڷ�ƾ ����
            return true; // ��ȯ ���� ����
        }
        return false; // ��ȯ ���� ���� (��ٿ� ���̰ų� �ٸ� ����)
    }

    /// <summary>
    /// ���� ��ȯ ��ü ������ ó���ϴ� �ڷ�ƾ.
    /// </summary>
    private IEnumerator SummonRoutine()
    {
        isSummoning = true; // ��ȯ ���� ����
        // aiCore.isActing = true; // �� �߿�: ���� AI�� �ൿ ���µ� ��� (������������ ����)
        // aiCore.StopMovement(); // �� �߿�: ���� AI�� �̵� ���� ȣ�� (������)
        // aiCore.FlipTowardsTarget(true); // �� �߿�: ���� AI�� Ÿ�� ������ �� (������)
        Debug.Log("���� ��ȯ ����!");

        // ��ȯ �ִϸ��̼� ����
        animator.SetTrigger("Summon"); // "Summon" Ʈ���� �ߵ�

        // �ִϸ��̼� ��� �� ��ȯ Ÿ�ֱ̹��� ���
        yield return new WaitForSeconds(summonTiming);

        // --- ���� ���� ���� ---
        SpawnMonsters();

        // ��ȯ �ð� ��� (��ٿ� ����)
        lastSummonTime = Time.time;

        // ��ȯ �� ª�� ������ (�ִϸ��̼� �ĵ� �� ���)
        yield return new WaitForSeconds(0.5f);

        // aiCore.isActing = false; // �� �߿�: ���� AI�� �ൿ ���� ��� ���� (������)
        isSummoning = false; // ��ȯ ���� ����
        Debug.Log("���� ��ȯ ����.");
        yield break;
    }

    /// <summary>
    /// ���� ���͸� �����ϴ� �Լ�.
    /// </summary>
    private void SpawnMonsters()
    {
        if (monsterPrefab == null || summonPoints.Length == 0) return; // ���� üũ

        Debug.Log($"{monstersPerSummon} ���� ���� ��ȯ ����!");
        for (int i = 0; i < monstersPerSummon; i++)
        {
            Transform spawnPoint = summonPoints[Random.Range(0, summonPoints.Length)];
            if (spawnPoint != null)
            {
                Instantiate(monsterPrefab, spawnPoint.position, spawnPoint.rotation);
                // TODO: ��ȯ ����Ʈ(VFX, SFX) �߰�
            }
            else Debug.LogWarning($"SummonPoints �迭�� �Ϻ� ��Ұ� null�Դϴ�.");
            // �ణ�� �ð��� ��ȯ �ʿ� �� yield return new WaitForSeconds(0.1f); �߰� (���⼭�� ��� ��ȯ)
        }
    }

    #endregion

    #region Gizmos (��ȯ ��ġ �ð�ȭ)

    private void OnDrawGizmosSelected() // �������� ���� Gizmos ǥ��
    {
        if (summonPoints != null && summonPoints.Length > 0)
        {
            Gizmos.color = Color.magenta; // ����� �迭
            foreach (Transform point in summonPoints)
            {
                if (point != null)
                {
                    Gizmos.DrawSphere(point.position, 0.2f); // ��ȯ ��ġ�� ���� �� ǥ��
                    Gizmos.DrawLine(transform.position, point.position); // ������ ���ἱ (������)
                }
            }
        }
    }

    #endregion

    #region �ܺ� ���ٿ� ������Ƽ (������)

    /// <summary>
    /// ���� ��ȯ �ڷ�ƾ�� ���� ������ ���θ� ��ȯ�մϴ�.
    /// </summary>
    public bool IsSummoning => isSummoning;

    #endregion
}
