using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Playables; // Ÿ�Ӷ��� ���

public class TimelineStarter : MonoBehaviour
{
    public PlayableDirector timeline; // Ÿ�Ӷ��� ����
    public GameObject back;    // �г� ����
    public GameObject loadingPanel;   // �ε� UI �г�
    public Slider progressBar;        // ���α׷��� ��
    public float fakeLoadingTime = 2f; // ��¥ �ε� �ð�

    public string nextSceneName = "Startmunescenes"; // �ε� �� �̵��� ��

    public void PlayTimeline()
    {
        if (timeline != null)
        {
            timeline.Play();
            back.SetActive(false); // �г� ��Ȱ��ȭ (UI ����)
            timeline.stopped += OnTimelineFinished; // Ÿ�Ӷ��� ���� �̺�Ʈ ���
        }
        else
        {
            StartCoroutine(LoadSceneFake(nextSceneName)); // Ÿ�Ӷ��� ������ �ٷ� �ε�
        }
    }

    void OnTimelineFinished(PlayableDirector director)
    {
        timeline.stopped -= OnTimelineFinished; // �̺�Ʈ ����
        StartCoroutine(LoadSceneFake(nextSceneName)); // Ÿ�Ӷ��� ������ �ε� ����
    }

    IEnumerator LoadSceneFake(string sceneName)
    {
        loadingPanel.SetActive(true); // �ε� UI Ȱ��ȭ
        float timer = 0f;

        while (timer < fakeLoadingTime)
        {
            timer += Time.deltaTime;
            progressBar.value = timer / fakeLoadingTime; // ���α׷��� �� ������Ʈ
            yield return null;
        }

        SceneManager.LoadScene(sceneName); // �� ��ȯ
    }
}
