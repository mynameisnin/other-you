using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TimeLineSecenManager : MonoBehaviour

{
    public string nextSceneName; // ������ �� �̸�


    public static Vector3 targetPosition; // �� �̵� �� ��ġ ����

    public void ChangeScene()
    {


        SceneManager.LoadScene(nextSceneName);
    }
}