using System.Collections;
using UnityEngine;
using UnityEngine.Playables;

public class TimelineTrigger : MonoBehaviour
{
    public PlayableDirector timeline;
    private bool hasTriggered = false; // 한 번만 실행되도록 제어

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && !hasTriggered)
        {
            Debug.Log("플레이어가 감지됨! 2초 후 타임라인 실행");
            hasTriggered = true; // 실행 여부 체크
            StartCoroutine(StartTimelineWithDelay(1.5f));
        }
    }

    IEnumerator StartTimelineWithDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        timeline.Play();
    }
}
