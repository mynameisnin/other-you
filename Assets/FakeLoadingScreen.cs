using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class FakeLoadingScreen : MonoBehaviour
{
    public GameObject loadingPanel; // �ε� UI �г� (���� ȭ��)
 
    public float fakeLoadingTime = 2f; // 2�� ���� ��¥ �ε�

    public void LoadScene(string sceneName)
    {
        StartCoroutine(LoadSceneFake(sceneName));
    }

    IEnumerator LoadSceneFake(string sceneName)
    {
        loadingPanel.SetActive(true); // �ε� UI Ȱ��ȭ

        float timer = 0f;

        while (timer < fakeLoadingTime)
        {
            timer += Time.deltaTime;
            yield return null;
        }

        SceneManager.LoadScene(sceneName); // �� ��ȯ
    }
}
