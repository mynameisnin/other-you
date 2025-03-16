using System.Collections;
using UnityEngine;
using UnityEngine.Playables;

public class TimelineTrigger : MonoBehaviour
{
    public PlayableDirector timeline;
    private bool hasTriggered = false; // �� ���� ����ǵ��� ����

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && !hasTriggered)
        {
            Debug.Log("�÷��̾ ������! 2�� �� Ÿ�Ӷ��� ����");
            hasTriggered = true; // ���� ���� üũ
            StartCoroutine(StartTimelineWithDelay(1.5f));
        }
    }

    IEnumerator StartTimelineWithDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        timeline.Play();
    }
}
