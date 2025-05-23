using UnityEngine;
using UnityEngine.SceneManagement;

public class StartButton : MonoBehaviour
{
    public string nextSceneName = "map_village"; // 전환할 씬 이름

    public void OnStartButtonClicked()
    {
        SceneManager.LoadScene(nextSceneName);
    }
}
