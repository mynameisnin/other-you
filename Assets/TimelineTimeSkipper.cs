using UnityEngine;
using UnityEngine.Playables;

public class TimelineTimeSkipper : MonoBehaviour
{
    [SerializeField] private PlayableDirector timeline;
    [SerializeField] private double skipToTime = 12.5; // ��ŵ�� Ÿ�Ӷ��� �ð� (��)

    private bool hasSkipped = false;

    void Update()
    {
        if (timeline == null || hasSkipped) return;

        if (Input.GetKeyDown(KeyCode.Return))
        {
            Debug.Log("[Timeline Skipper] Enter Ű �Էµ� �� Ÿ�Ӷ��� " + skipToTime + "�ʷ� ����");
            timeline.time = skipToTime;
            timeline.Evaluate(); // ��� �ݿ�
            timeline.Play();
            hasSkipped = true;
        }
    }
}
