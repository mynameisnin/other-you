using UnityEngine;
using UnityEngine.SceneManagement;

public class StartButton : MonoBehaviour
{
    public string nextSceneName = "map_village"; // ��ȯ�� �� �̸�

    public void OnStartButtonClicked()
    {
        SceneManager.LoadScene(nextSceneName);
    }
}
