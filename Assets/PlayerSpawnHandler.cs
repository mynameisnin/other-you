using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerSpawnHandler : MonoBehaviour
{
    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (SpawnManager.Instance != null)
        {
            Vector2 targetPosition = SpawnManager.Instance.spawnPosition;
            Debug.Log("Applying Spawn Position After Scene Load: " + targetPosition);
            transform.position = targetPosition;
        }
    }
}

