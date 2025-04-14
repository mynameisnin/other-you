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
            Destroy(gameObject); //  중복 생성 방지
        }
    }
    void Start()
    {
        Scene currentScene = SceneManager.GetActiveScene();
        
        // 특정 씬 이름에 들어가면 자기 자신 삭제
        if (currentScene.name == "startscens") 
        {
            Destroy(gameObject);
        }
    }
}