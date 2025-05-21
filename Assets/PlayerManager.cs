using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerManager : MonoBehaviour
{
    public static PlayerManager Instance;

    [SerializeField]
    private List<string> scenesToDestroyIn = new List<string> { "Chapter2 TimeLine", "end" };

    private bool markedForDestruction = false;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            SceneManager.sceneLoaded += OnSceneLoaded;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // 이미 파괴 예약이 된 경우 아무것도 하지 않음
        if (markedForDestruction) return;

        if (scenesToDestroyIn.Contains(scene.name))
        {
            markedForDestruction = true;
            SceneManager.sceneLoaded -= OnSceneLoaded;
            StartCoroutine(DestroyNextFrameSafely());
        }
    }

    private IEnumerator DestroyNextFrameSafely()
    {
        yield return null; // 한 프레임 대기 (SceneManager 이벤트 완전히 끝난 후)
        if (Instance == this)
        {
            Instance = null;
            Destroy(gameObject);
        }
    }

    private void OnDestroy()
    {
        // 혹시라도 씬 전환 중에 강제로 파괴될 경우 이벤트 해제
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
}
