using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TimeLineSecenManager : MonoBehaviour

{
    public string nextSceneName; // 변경할 씬 이름


    public Vector2 spawnPosition; // 이동할 씬의 스폰 위치

    public void ChangeScene()
    {
        SceneManager.LoadScene(nextSceneName);
    }
}