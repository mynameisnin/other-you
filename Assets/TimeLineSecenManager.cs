using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TimeLineSecenManager : MonoBehaviour

{
    public string nextSceneName; // 변경할 씬 이름


    public static Vector3 targetPosition; // 씬 이동 후 위치 저장

    public void ChangeScene()
    {


        SceneManager.LoadScene(nextSceneName);
    }
}