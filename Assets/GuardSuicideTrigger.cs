using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using DG.Tweening;

public class GuardSuicideTrigger : MonoBehaviour
{
    public PlayableDirector timeline; // 타임라인
    public Collider2D triggerCollider; // 플레이어가 진입할 트리거 콜라이더
    public Transform fallingObject; // 위에서 떨어질 오브젝트
    public float fallDistance = 5f; // 떨어지는 거리
    public float fallDuration = 1.5f; // 떨어지는 시간

    private bool timelineStarted = false; // 타임라인이 실행되었는지 체크

    void Start()
    {
        // 트리거 비활성화 (초기 상태)
        if (triggerCollider != null)
        {
            triggerCollider.enabled = false;
        }
    }

    void Update()
    {
        // 타임라인 상태 로그 출력
        if (timeline != null)
        {
            Debug.Log($"타임라인 상태: {timeline.state}");
        }

        // 타임라인이 실행 중인지 확인 후 트리거 활성화
        if (timeline != null && timeline.state == PlayState.Playing && !timelineStarted)
        {
            timelineStarted = true; // 한 번만 실행되도록 체크
            if (triggerCollider != null)
            {
                triggerCollider.enabled = true; // 트리거 활성화
                Debug.Log(" 트리거 활성화됨.");
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log($"트리거 진입: {other.gameObject.name}");

        if (other.CompareTag("Player")) // 플레이어가 트리거에 닿으면 실행
        {
            Debug.Log(" 플레이어가 트리거에 진입함! 오브젝트가 바로 아래로 떨어집니다.");

            if (fallingObject != null)
            {
                Debug.Log(" 떨어질 오브젝트 확인됨.");

                // 아래로 떨어질 목표 위치 설정 (현재 위치에서 -fallDistance 만큼 아래)
                float targetY = fallingObject.position.y - fallDistance;

                // DOTween 애니메이션 적용 (즉시 떨어지게)
                fallingObject.DOMoveY(targetY, fallDuration)
                    .SetEase(Ease.InQuad) // 부드럽게 떨어지는 효과 (빠르게 떨어지는 느낌)
                    .OnComplete(() => Debug.Log(" 오브젝트 낙하 완료"));
            }
            else
            {
                Debug.LogError(" 떨어질 오브젝트가 존재하지 않습니다!");
            }
        }
    }
}
