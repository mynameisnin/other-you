using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerManager : MonoBehaviour
{
    public static PlayerManager Instance;

    [SerializeField]
    private List<string> scenesToDestroyIn = new List<string> { "Chapter2 TimeLine" };

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);

            // 씬이 로드될 때마다 체크
            SceneManager.sceneLoaded += OnSceneLoaded;
        }
        else
        {
            Destroy(gameObject); // 중복 생성 방지
        }
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scenesToDestroyIn.Contains(scene.name))
        {
            SceneManager.sceneLoaded -= OnSceneLoaded; // 이벤트 제거
            Destroy(gameObject); // 해당 씬에서 삭제
        }
    }
}
