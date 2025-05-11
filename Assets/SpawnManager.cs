using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SpawnManager : MonoBehaviour
{
    public static SpawnManager Instance;
    public Vector2 spawnPosition;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            SceneManager.sceneLoaded += OnSceneLoaded; // �� �ε� �� �̺�Ʈ ����
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded; // �̺�Ʈ ����
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Ư�� �� �̸��� ���� �ڱ� �ڽ� ����
        if (scene.name == "startscenes")
        {
            Destroy(gameObject);
        }
    }
}
