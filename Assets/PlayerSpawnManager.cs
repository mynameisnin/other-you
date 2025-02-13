using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSpawnManager : MonoBehaviour
{
    void Start()
    {
        if (SpawnManager.Instance != null)
        {
            transform.position = SpawnManager.Instance.spawnPosition;
        }
    }
}