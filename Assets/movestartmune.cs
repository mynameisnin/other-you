using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MoveStartMenu : MonoBehaviour
{
    public void LoadNextScene()
    {
        Invoke(nameof(DelayedSceneLoad), 2f); // 3초 후 실행
    }

    void DelayedSceneLoad()
    {
        SceneManager.LoadScene("Stroyscenes"); // 씬 변경
    }
}
