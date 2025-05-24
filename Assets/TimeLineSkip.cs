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
            Debug.Log("[TimelineSkip] Enter Ű �Էµ� �� Ÿ�Ӷ��� ��ŵ ����");
            SkipTimeline();
        }
    }

    private void SkipTimeline()
    {
        hasSkipped = true;

        timeline.time = timeline.duration;
        timeline.Evaluate(); // ���� �������� ��� ���� ���� �ݿ�
        timeline.Play();     // ��� ����
    }
}
