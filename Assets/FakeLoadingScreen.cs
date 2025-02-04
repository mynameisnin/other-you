using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class FakeLoadingScreen : MonoBehaviour
{
    public GameObject loadingPanel; // 로딩 UI 패널 (검은 화면)
    public Slider progressBar;      // 프로그레스 바 (가짜 로딩)

    public float fakeLoadingTime = 2f; // 2초 동안 가짜 로딩

    public void LoadScene(string sceneName)
    {
        StartCoroutine(LoadSceneFake(sceneName));
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
