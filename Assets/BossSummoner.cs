using System.Collections;
using UnityEngine;

[RequireComponent(typeof(AngryGodAiCore))]
[RequireComponent(typeof(Animator))]
public class BossSummoner : MonoBehaviour
{
    [Header("소환 설정")]
    [SerializeField] private GameObject monsterPrefab;
    [SerializeField] private Transform[] leftSummonPoints;
    [SerializeField] private Transform[] rightSummonPoints;
    [SerializeField] private float summonTiming = 1.0f;
    [SerializeField] private float summonCooldown = 15f;
    [SerializeField] private int monstersPerSummon = 3;
    [SerializeField] private GameObject exclamationPrefab; // 느낌표 프리팹

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

        Debug.Log("TryStartSummon 실행됨");
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
        Debug.Log("SummonRoutine 시작");
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
        Debug.Log("AnimEvent_PerformSummon 호출됨");

        float exclamationDuration = 2.1f; // 느낌표 애니메이션 시간에 맞춰 설정

        // 소환 지점 방향 선택
        bool isFacingRight = transform.localScale.x > 0f;
        Transform[] selectedPoints = isFacingRight ? rightSummonPoints : leftSummonPoints;

        // 각 지점 위에 느낌표 생성
        foreach (Transform spawnPoint in selectedPoints)
        {
            if (spawnPoint != null && exclamationPrefab != null)
            {
                Vector3 iconPos = spawnPoint.position + new Vector3(0, 0.8f, 0); // 아래 위치
                GameObject icon = Instantiate(exclamationPrefab, iconPos, Quaternion.identity);

                
                Vector3 scale = icon.transform.localScale;
                scale.x *= isFacingRight ? 1 : -1; // 오른쪽이면 그대로, 왼쪽이면 반전
                icon.transform.localScale = scale;

                Destroy(icon, exclamationDuration);
            }
        }

        // 애니메이션 끝날 때까지 대기 후 소환 시작
        StartCoroutine(DelayedSummonRoutine(exclamationDuration));
    }

    private IEnumerator DelayedSummonRoutine(float delay)
    {
        yield return new WaitForSeconds(delay);
        StartCoroutine(SummonMonstersRoutine());
    }

    private IEnumerator SummonMonstersRoutine()
    {
        yield return new WaitForSeconds(summonTiming);
        SpawnMonsters();
        Debug.Log("소환 완료");
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
        // ← AngryGodAiCore로부터 방향을 가져와 정확하게 판별
        bool isFacingRight = aiCore != null && aiCore.IsFacingRight;
        Transform[] selectedPoints = isFacingRight ? rightSummonPoints : leftSummonPoints;

        for (int i = 0; i < monstersPerSummon; i++)
        {
            if (selectedPoints.Length == 0) return;

            Transform spawnPoint = selectedPoints[Random.Range(0, selectedPoints.Length)];
            if (spawnPoint != null)
            {
                Instantiate(monsterPrefab, spawnPoint.position, spawnPoint.rotation);
            }
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
