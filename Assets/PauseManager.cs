using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseManager : MonoBehaviour
{
    public GameObject pausePanel; // Inspector���� ����
    private bool isPaused = false;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPaused)
                ResumeGame();
            else
                PauseGame();
        }
    }

    public void PauseGame()
    {
        Time.timeScale = 0f; // ���� ����
        pausePanel.SetActive(true); // �ǳ� ���̱�
        isPaused = true;
    }

    public void ResumeGame()
    {
        Time.timeScale = 1f; // ���� �簳
        pausePanel.SetActive(false); // �ǳ� �����
        isPaused = false;
    }
}

