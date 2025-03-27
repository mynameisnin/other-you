using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Playables; // 타임라인 사용

public class TimelineStarter : MonoBehaviour
{
    public PlayableDirector timeline; // 타임라인 연결
    public GameObject back;    // 패널 정리
    public GameObject loadingPanel;   // 로딩 UI 패널
    public Slider progressBar;        // 프로그레스 바
    public float fakeLoadingTime = 2f; // 가짜 로딩 시간

    public string nextSceneName = "Startmunescenes"; // 로딩 후 이동할 씬

    public void PlayTimeline()
    {
        if (timeline != null)
        {
            timeline.Play();
            back.SetActive(false); // 패널 비활성화 (UI 정리)
            timeline.stopped += OnTimelineFinished; // 타임라인 종료 이벤트 등록
        }
        else
        {
            StartCoroutine(LoadSceneFake(nextSceneName)); // 타임라인 없으면 바로 로딩
        }
    }

    void OnTimelineFinished(PlayableDirector director)
    {
        timeline.stopped -= OnTimelineFinished; // 이벤트 해제
        StartCoroutine(LoadSceneFake(nextSceneName)); // 타임라인 끝나면 로딩 시작
    }

    IEnumerator LoadSceneFake(string sceneName)
    {
        loadingPanel.SetActive(true); // 로딩 UI 활성화
        float timer = 0f;

        while (timer < fakeLoadingTime)
        {
            timer += Time.deltaTime;
            progressBar.value = timer / fakeLoadingTime; // 프로그레스 바 업데이트
            yield return null;
        }

        SceneManager.LoadScene(sceneName); // 씬 전환
    }
}
