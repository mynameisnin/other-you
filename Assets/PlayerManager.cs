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
        // �̹� �ı� ������ �� ��� �ƹ��͵� ���� ����
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
        yield return null; // �� ������ ��� (SceneManager �̺�Ʈ ������ ���� ��)
        if (Instance == this)
        {
            Instance = null;
            Destroy(gameObject);
        }
    }

    private void OnDestroy()
    {
        // Ȥ�ö� �� ��ȯ �߿� ������ �ı��� ��� �̺�Ʈ ����
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
}
