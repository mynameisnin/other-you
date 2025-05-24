using UnityEngine;
using UnityEngine.Playables;

public class TimeLineSkip : MonoBehaviour
{
    [SerializeField] private PlayableDirector timeline;
    private bool hasSkipped = false;

    private void Update()
    {
        if (timeline == null || hasSkipped) return;

        if (Input.GetKeyDown(KeyCode.Return))
        {
            Debug.Log("[TimelineSkip] Enter 키 입력됨 → 타임라인 스킵 실행");
            SkipTimeline();
        }
    }

    private void SkipTimeline()
    {
        hasSkipped = true;

        timeline.time = timeline.duration;
        timeline.Evaluate(); // 현재 시점으로 모든 상태 강제 반영
        timeline.Play();     // 재생 시작
    }
}
