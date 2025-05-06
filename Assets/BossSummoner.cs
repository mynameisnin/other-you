using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AngryGodAiCore))]
[RequireComponent(typeof(Animator))]
public class BossSummoner : MonoBehaviour
{
    [Header("소환 설정")]
    [SerializeField] private GameObject monsterPrefab;
    [SerializeField] private Transform[] leftSummonPoints;
    [SerializeField] private Transform[] rightSummonPoints;
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

        float exclamationDuration = 2f;

        bool isFacingRight = aiCore != null && aiCore.IsFacingRight;
        Transform[] selectedPoints = isFacingRight ? rightSummonPoints : leftSummonPoints;
        string direction = isFacingRight ? "오른쪽" : "왼쪽";

        foreach (Transform spawnPoint in selectedPoints)
        {
            if (spawnPoint != null && exclamationPrefab != null)
            {
                Vector3 iconPos = spawnPoint.position + new Vector3(0, 0.8f, 0);
                GameObject icon = Instantiate(exclamationPrefab, iconPos, Quaternion.identity);

                // 방향 반전
                Vector3 scale = icon.transform.localScale;
                scale.x *= isFacingRight ? 1 : -1;
                icon.transform.localScale = scale;

                Destroy(icon, exclamationDuration); //  정확히 2초 후 사라짐
            }
        }

        //  느낌표가 사라지는 그 순간, 몬스터 소환 시작
        StartCoroutine(DelayedSummonRoutine(exclamationDuration));
    }


    private IEnumerator DelayedSummonRoutine(float delay)
    {
        yield return new WaitForSeconds(delay); //  exclamationDuration과 동일하게 기다림
        SpawnMonsters();                        //  즉시 소환
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
        // 보스의 방향을 기반으로 소환 지점 선택
        // 보스의 방향을 기반으로 소환 지점 선택
        bool isFacingRight = aiCore != null && aiCore.IsFacingRight;
        string direction = isFacingRight ? "오른쪽" : "왼쪽";
        Transform[] selectedPoints = isFacingRight ? rightSummonPoints : leftSummonPoints;

        Debug.Log($"[몬스터 소환] 소환 방향: {direction}");

        if (selectedPoints.Length == 0) return;

        // 소환 포인트를 리스트로 변환 후 셔플
        List<Transform> spawnList = new List<Transform>(selectedPoints);
        Shuffle(spawnList); // 랜덤화

        // 중복 없이 소환 (최대 monstersPerSummon 수 만큼)
        for (int i = 0; i < monstersPerSummon && i < spawnList.Count; i++)
        {
            Transform spawnPoint = spawnList[i];
            if (spawnPoint != null)
            {
                Instantiate(monsterPrefab, spawnPoint.position, spawnPoint.rotation);
                Debug.Log($"[몬스터 소환] 위치: {spawnPoint.position} / 방향: {direction}");
            }
        }
    }

    // Fisher-Yates 셔플 알고리즘
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
