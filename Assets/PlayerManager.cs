using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class PlayerManager : MonoBehaviour
{
    public static PlayerManager Instance;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject); //  �ߺ� ���� ����
        }
    }
    void Start()
    {
        Scene currentScene = SceneManager.GetActiveScene();
        
        // Ư�� �� �̸��� ���� �ڱ� �ڽ� ����
        if (currentScene.name == "startscens") 
        {
            Destroy(gameObject);
        }
    }
}