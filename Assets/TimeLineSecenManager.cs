using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TimeLineSecenManager : MonoBehaviour

{
    public string nextSceneName; // ������ �� �̸�


    public Vector2 spawnPosition; // �̵��� ���� ���� ��ġ

    public void ChangeScene()
    {
        SceneManager.LoadScene(nextSceneName);
    }
}