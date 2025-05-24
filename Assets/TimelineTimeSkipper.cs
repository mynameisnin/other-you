using UnityEngine;
using UnityEngine.Playables;

public class TimelineTimeSkipper : MonoBehaviour
{
    [SerializeField] private PlayableDirector timeline;
    [SerializeField] private double skipToTime = 12.5; // 스킵할 타임라인 시간 (초)

    private bool hasSkipped = false;

    void Update()
    {
        if (timeline == null || hasSkipped) return;

        if (Input.GetKeyDown(KeyCode.Return))
        {
            Debug.Log("[Timeline Skipper] Enter 키 입력됨 → 타임라인 " + skipToTime + "초로 점프");
            timeline.time = skipToTime;
            timeline.Evaluate(); // 즉시 반영
            timeline.Play();
            hasSkipped = true;
        }
    }
}
