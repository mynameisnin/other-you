using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseMenu : MonoBehaviour
{
    [SerializeField]
    private GameObject CanvasObject;
    private bool isPasused = false;
    // Start is called before the first frame update
   public void Start()
    {
        CanvasObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyUp(KeyCode.Escape))
        {
            if (isPasused)
            {
                ResumeGame();
            }
            else
            {
                PauseGame();
            }

        }
    }
   public void PauseGame()
    {
        CanvasObject.SetActive(true);
        Time.timeScale = 0;
        isPasused = true;
    }

   public void ResumeGame()
    {
        CanvasObject.SetActive(false);
        Time.timeScale = 1f;
        isPasused = false;
    }
}
