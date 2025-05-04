using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 보스의 몬스터 소환 능력을 관리하는 스크립트.
/// AngryGodAiCore와 협력하여 작동합니다.
/// </summary>
[RequireComponent(typeof(AngryGodAiCore))] // 이 스크립트가 작동하려면 AngryGodAiCore가 필요함
[RequireComponent(typeof(Animator))]      // 애니메이션 실행을 위해 Animator 필요
public class BossSummoner : MonoBehaviour
{
    #region 변수 선언

    // --- 소환 설정 ---
    [Header("소환 설정")]
    [Tooltip("소환할 몬스터 프리팹")]
    [SerializeField] private GameObject monsterPrefab;
    [Tooltip("몬스터가 소환될 위치 Transform 배열")]
    [SerializeField] private Transform[] summonPoints;
    [Tooltip("소환 애니메이션 시작 후 실제 소환까지 대기 시간 (초)")]
    [SerializeField] private float summonTiming = 1.0f;
    [Tooltip("소환 스킬 사용 후 다음 소환까지의 대기 시간 (초)")]
    [SerializeField] private float summonCooldown = 15f;
    [Tooltip("한 번에 소환할 몬스터 수")]
    [SerializeField] private int monstersPerSummon = 3;

    // --- 내부 상태 ---
    private float lastSummonTime = -15f; // 마지막 소환 시간 (쿨다운 계산용)
    private bool isSummoning = false;    // 현재 소환 코루틴 실행 중인지 여부

    // --- 참조 ---
    private AngryGodAiCore aiCore; // 메인 AI 스크립트 참조
    private Animator animator;     // 애니메이터 참조

    #endregion

    #region 초기화 및 유효성 검사

    private void Awake()
    {
        // 필요한 컴포넌트 참조 가져오기
        aiCore = GetComponent<AngryGodAiCore>();
        animator = GetComponent<Animator>();

        // 참조 유효성 검사
        if (aiCore == null) Debug.LogError("AngryGodAiCore 참조를 찾을 수 없습니다!", this);
        if (animator == null) Debug.LogError("Animator 참조를 찾을 수 없습니다!", this);
    }

    private void Start()
    {
        // 소환 관련 설정 유효성 검사
        if (monsterPrefab == null) Debug.LogWarning("Monster Prefab이 할당되지 않았습니다. 소환이 불가능합니다.", this);
        if (summonPoints == null || summonPoints.Length == 0) Debug.LogWarning("Summon Points가 설정되지 않았습니다. 소환 위치를 지정해주세요.", this);

        // 쿨다운 초기화 (게임 시작 시 바로 사용 가능하도록)
        lastSummonTime = -summonCooldown;
    }

    #endregion

    #region 소환 실행 및 제어

    /// <summary>
    /// 외부(예: AngryGodAiCore)에서 소환을 시도할 때 호출하는 함수.
    /// 쿨다운 및 현재 상태를 확인하여 소환 코루틴을 시작합니다.
    /// </summary>
    /// <returns>소환을 시작했으면 true, 아니면 false</returns>
    public bool TryStartSummon()
    {
        // 쿨다운 확인 및 현재 소환 중인지 확인
        if (Time.time >= lastSummonTime + summonCooldown && !isSummoning && monsterPrefab != null && summonPoints.Length > 0)
        {
            StartCoroutine(SummonRoutine()); // 소환 코루틴 시작
            return true; // 소환 시작 성공
        }
        return false; // 소환 시작 실패 (쿨다운 중이거나 다른 이유)
    }

    /// <summary>
    /// 몬스터 소환 전체 과정을 처리하는 코루틴.
    /// </summary>
    private IEnumerator SummonRoutine()
    {
        isSummoning = true; // 소환 상태 시작
        // aiCore.isActing = true; // ★ 중요: 메인 AI의 행동 상태도 잠금 (선택적이지만 권장)
        // aiCore.StopMovement(); // ★ 중요: 메인 AI의 이동 정지 호출 (선택적)
        // aiCore.FlipTowardsTarget(true); // ★ 중요: 메인 AI가 타겟 보도록 함 (선택적)
        Debug.Log("몬스터 소환 시작!");

        // 소환 애니메이션 실행
        animator.SetTrigger("Summon"); // "Summon" 트리거 발동

        // 애니메이션 재생 및 소환 타이밍까지 대기
        yield return new WaitForSeconds(summonTiming);

        // --- 몬스터 생성 로직 ---
        SpawnMonsters();

        // 소환 시간 기록 (쿨다운 시작)
        lastSummonTime = Time.time;

        // 소환 후 짧은 딜레이 (애니메이션 후딜 등 고려)
        yield return new WaitForSeconds(0.5f);

        // aiCore.isActing = false; // ★ 중요: 메인 AI의 행동 상태 잠금 해제 (선택적)
        isSummoning = false; // 소환 상태 종료
        Debug.Log("몬스터 소환 종료.");
        yield break;
    }

    /// <summary>
    /// 실제 몬스터를 생성하는 함수.
    /// </summary>
    private void SpawnMonsters()
    {
        if (monsterPrefab == null || summonPoints.Length == 0) return; // 이중 체크

        Debug.Log($"{monstersPerSummon} 마리 몬스터 소환 실행!");
        for (int i = 0; i < monstersPerSummon; i++)
        {
            Transform spawnPoint = summonPoints[Random.Range(0, summonPoints.Length)];
            if (spawnPoint != null)
            {
                Instantiate(monsterPrefab, spawnPoint.position, spawnPoint.rotation);
                // TODO: 소환 이펙트(VFX, SFX) 추가
            }
            else Debug.LogWarning($"SummonPoints 배열의 일부 요소가 null입니다.");
            // 약간의 시간차 소환 필요 시 yield return new WaitForSeconds(0.1f); 추가 (여기서는 즉시 소환)
        }
    }

    #endregion

    #region Gizmos (소환 위치 시각화)

    private void OnDrawGizmosSelected() // 선택했을 때만 Gizmos 표시
    {
        if (summonPoints != null && summonPoints.Length > 0)
        {
            Gizmos.color = Color.magenta; // 보라색 계열
            foreach (Transform point in summonPoints)
            {
                if (point != null)
                {
                    Gizmos.DrawSphere(point.position, 0.2f); // 소환 위치에 작은 구 표시
                    Gizmos.DrawLine(transform.position, point.position); // 보스와 연결선 (선택적)
                }
            }
        }
    }

    #endregion

    #region 외부 접근용 프로퍼티 (선택적)

    /// <summary>
    /// 현재 소환 코루틴이 실행 중인지 여부를 반환합니다.
    /// </summary>
    public bool IsSummoning => isSummoning;

    #endregion
}
