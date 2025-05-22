using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using DG.Tweening;

public class GuardSuicideTrigger : MonoBehaviour
{
    [Header("타임라인 연관 요소")]
    public PlayableDirector timeline;            // (옵션) 타임라인
    public Collider2D triggerCollider;           // 플레이어가 진입할 트리거 콜라이더
    public Transform fallingObject;              // 위에서 떨어질 오브젝트
    public float fallDistance = 5f;              // 떨어지는 거리
    public float fallDuration = 1.5f;            // 떨어지는 시간

    [Header("몬스터 리스트")]
    public List<GameObject> fieldMonsters = new List<GameObject>();

    private bool triggerReady = false;           // 트리거를 사용할 수 있는 상태인지
    private bool hasTriggered = false;           // 트리거 한 번만 실행

    void Start()
    {
        if (triggerCollider != null)
            triggerCollider.enabled = false;

        // 초기 몬스터 리스트 정리 (죽은 오브젝트 제거)
        fieldMonsters.RemoveAll(m => m == null);
    }

    void Update()
    {
        // 몬스터 리스트에서 죽은 몬스터 제거
        fieldMonsters.RemoveAll(m => m == null);

        // 몬스터가 모두 죽었고, 아직 트리거를 준비하지 않았다면
        if (fieldMonsters.Count == 0 && !triggerReady)
        {
            triggerReady = true;
            Debug.Log("[GuardSuicideTrigger] 모든 몬스터가 제거됨, 트리거 활성화");

            if (triggerCollider != null)
                triggerCollider.enabled = true;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!triggerReady || hasTriggered) return;

        if (other.CompareTag("Player"))
        {
            hasTriggered = true;
            Debug.Log("[GuardSuicideTrigger] 플레이어가 트리거에 진입함, 오브젝트 낙하 시작");

            if (fallingObject != null)
            {
                float targetY = fallingObject.position.y - fallDistance;
                fallingObject.DOMoveY(targetY, fallDuration)
                    .SetEase(Ease.InQuad)
                    .OnComplete(() => Debug.Log("[GuardSuicideTrigger] 오브젝트 낙하 완료"));
            }
            else
            {
                Debug.LogError("[GuardSuicideTrigger] 낙하 오브젝트가 없습니다.");
            }
        }
    }
}
