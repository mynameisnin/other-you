using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class retry : MonoBehaviour
{

    public void Retry()
    {
        Time.timeScale = 1f; // 혹시 죽을 때 일시정지 했으면 다시 정상으로
        SceneManager.LoadScene(SceneManager.GetActiveScene().name); // 현재 씬 다시 로드
    }
}
