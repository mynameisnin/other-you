using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AngryGodAiCore))]
[RequireComponent(typeof(Animator))]
public class BossSummoner : MonoBehaviour
{
    [Header("��ȯ ����")]
    [SerializeField] private GameObject monsterPrefab;
    [SerializeField] private Transform[] leftSummonPoints;
    [SerializeField] private Transform[] rightSummonPoints;
    [SerializeField] private float summonCooldown = 15f;
    [SerializeField] private int monstersPerSummon = 3;
    [SerializeField] private GameObject exclamationPrefab; // ����ǥ ������

    private float lastSummonTime = -15f;
    private bool isSummoning = false;

    private AngryGodAiCore aiCore;
    private Animator animator;

    private void Awake()
    {
        aiCore = GetComponent<AngryGodAiCore>();
        animator = GetComponent<Animator>();
    }

    private void Start()
    {
        lastSummonTime = -summonCooldown;
    }

    public bool TryStartSummon()
    {
        if (Time.time < lastSummonTime + summonCooldown || isSummoning || monsterPrefab == null)
            return false;

        if (GameObject.FindGameObjectsWithTag("SummonerEnemy").Length > 0)
            return false;

        Debug.Log("TryStartSummon �����");
        aiCore.InitiateBackdash();
        return true;
    }

    public IEnumerator TryStartSummonAfterBackdash()
    {
        if (Time.time < aiCore.GetGlobalCooldownTime())
            yield break;

        aiCore.SetGlobalCooldownTime(Time.time + 6f);
        yield return new WaitForSeconds(0.1f);
        StartCoroutine(SummonRoutine());
    }

    private IEnumerator SummonRoutine()
    {
        Debug.Log("SummonRoutine ����");
        isSummoning = true;
        aiCore.NotifyActionStart();

        aiCore.StopMovement();
        GetComponent<Rigidbody2D>().velocity = Vector2.zero;
        aiCore.ForceFlipTowardsTarget();

        animator.SetTrigger("Summon");

        while (isSummoning)
        {
            yield return null;
        }

        GetComponent<Rigidbody2D>().gravityScale = 1f;
    }

    public void AnimEvent_PerformSummon()
    {
        Debug.Log("AnimEvent_PerformSummon ȣ���");

        float exclamationDuration = 2f;

        bool isFacingRight = aiCore != null && aiCore.IsFacingRight;
        Transform[] selectedPoints = isFacingRight ? rightSummonPoints : leftSummonPoints;
        string direction = isFacingRight ? "������" : "����";

        foreach (Transform spawnPoint in selectedPoints)
        {
            if (spawnPoint != null && exclamationPrefab != null)
            {
                Vector3 iconPos = spawnPoint.position + new Vector3(0, 0.8f, 0);
                GameObject icon = Instantiate(exclamationPrefab, iconPos, Quaternion.identity);

                // ���� ����
                Vector3 scale = icon.transform.localScale;
                scale.x *= isFacingRight ? 1 : -1;
                icon.transform.localScale = scale;

                Destroy(icon, exclamationDuration); //  ��Ȯ�� 2�� �� �����
            }
        }

        //  ����ǥ�� ������� �� ����, ���� ��ȯ ����
        StartCoroutine(DelayedSummonRoutine(exclamationDuration));
    }


    private IEnumerator DelayedSummonRoutine(float delay)
    {
        yield return new WaitForSeconds(delay); //  exclamationDuration�� �����ϰ� ��ٸ�
        SpawnMonsters();                        //  ��� ��ȯ
        Debug.Log("��ȯ �Ϸ�");
    }



    public void AnimEvent_SummonEnd()
    {
        lastSummonTime = Time.time;
        isSummoning = false;
        aiCore.NotifyActionEnd();
    }

    public void AnimEvent_AllowMovementAfterSummon()
    {
        aiCore.NotifyActionEnd();
    }

    private void SpawnMonsters()
    {
        // ������ ������ ������� ��ȯ ���� ����
        // ������ ������ ������� ��ȯ ���� ����
        bool isFacingRight = aiCore != null && aiCore.IsFacingRight;
        string direction = isFacingRight ? "������" : "����";
        Transform[] selectedPoints = isFacingRight ? rightSummonPoints : leftSummonPoints;

        Debug.Log($"[���� ��ȯ] ��ȯ ����: {direction}");

        if (selectedPoints.Length == 0) return;

        // ��ȯ ����Ʈ�� ����Ʈ�� ��ȯ �� ����
        List<Transform> spawnList = new List<Transform>(selectedPoints);
        Shuffle(spawnList); // ����ȭ

        // �ߺ� ���� ��ȯ (�ִ� monstersPerSummon �� ��ŭ)
        for (int i = 0; i < monstersPerSummon && i < spawnList.Count; i++)
        {
            Transform spawnPoint = spawnList[i];
            if (spawnPoint != null)
            {
                Instantiate(monsterPrefab, spawnPoint.position, spawnPoint.rotation);
                Debug.Log($"[���� ��ȯ] ��ġ: {spawnPoint.position} / ����: {direction}");
            }
        }
    }

    // Fisher-Yates ���� �˰���
    private void Shuffle(List<Transform> list)
    {
        for (int i = list.Count - 1; i > 0; i--)
        {
            int j = Random.Range(0, i + 1);
            (list[i], list[j]) = (list[j], list[i]);
        }
    }

    public bool IsSummoning => isSummoning;

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        if (leftSummonPoints != null)
        {
            foreach (var point in leftSummonPoints)
            {
                if (point != null)
                {
                    Gizmos.DrawSphere(point.position, 0.2f);
                    Gizmos.DrawLine(transform.position, point.position);
                }
            }
        }

        Gizmos.color = Color.cyan;
        if (rightSummonPoints != null)
        {
            foreach (var point in rightSummonPoints)
            {
                if (point != null)
                {
                    Gizmos.DrawSphere(point.position, 0.2f);
                    Gizmos.DrawLine(transform.position, point.position);
                }
            }
        }
    }
}
