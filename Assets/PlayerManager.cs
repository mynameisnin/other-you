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

            // ���� �ε�� ������ üũ
            SceneManager.sceneLoaded += OnSceneLoaded;
        }
        else
        {
            Destroy(gameObject); // �ߺ� ���� ����
        }
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scenesToDestroyIn.Contains(scene.name))
        {
            SceneManager.sceneLoaded -= OnSceneLoaded; // �̺�Ʈ ����
            Destroy(gameObject); // �ش� ������ ����
        }
    }
}
