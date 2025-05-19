using UnityEngine;
using System.Collections;

public class GameStartPause : MonoBehaviour
{
    public float pauseDuration = 2f; // 일시정지 시간 (초)

    void Start()
    {
        StartCoroutine(PauseAtGameStart());
    }

    IEnumerator PauseAtGameStart()
    {
        Time.timeScale = 0f; // 게임 일시정지
        float pauseTime = pauseDuration;

        // Time.unscaledDeltaTime을 사용해 실제 시간 기준으로 기다림
        while (pauseTime > 0)
        {
            pauseTime -= Time.unscaledDeltaTime;
            yield return null;
        }

        Time.timeScale = 1f; // 게임 재시작
    }
}