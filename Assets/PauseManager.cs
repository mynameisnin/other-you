using UnityEngine;

public class PauseManager : MonoBehaviour
{
    public GameObject pausePanel;
    private bool isPaused = false;
    private AudioSource currentBgm;

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
        Time.timeScale = 0f;
        pausePanel.SetActive(true);
        isPaused = true;

        currentBgm = Bgmcontrol.Instance?.GetCurrentBgm();
        if (currentBgm != null)
        {
            currentBgm.Pause();
        }
    }

    public void ResumeGame()
    {
        Time.timeScale = 1f;
        pausePanel.SetActive(false);
        isPaused = false;

        if (currentBgm != null)
        {
            currentBgm.UnPause();
        }
    }
}
