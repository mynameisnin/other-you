using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseManager : MonoBehaviour
{
    public GameObject pausePanel; // Inspectorø°º≠ ø¨∞·
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
        Time.timeScale = 0f; // ∞‘¿” ∏ÿ√„
        pausePanel.SetActive(true); // ∆«≥⁄ ∫∏¿Ã±‚
        isPaused = true;
    }

    public void ResumeGame()
    {
        Time.timeScale = 1f; // ∞‘¿” ¿Á∞≥
        pausePanel.SetActive(false); // ∆«≥⁄ º˚±‚±‚
        isPaused = false;
    }
}

