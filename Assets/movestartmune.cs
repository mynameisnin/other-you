using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MoveStartMenu : MonoBehaviour
{
    public void LoadNextScene()
    {
        Invoke(nameof(DelayedSceneLoad), 2f); // 3�� �� ����
    }

    void DelayedSceneLoad()
    {
        SceneManager.LoadScene("Stroyscenes"); // �� ����
    }
}
