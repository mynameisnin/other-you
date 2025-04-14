using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss1 : MonoBehaviour
{
    public enemyTest ET;
    public GameObject enemyPrefab;
    public Transform spawnPoint1;
    public Transform spawnPoint2;
    public Transform spawnPoint3;
    private bool[] Spawn = { false, false, false };

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        Debug.Log(ET.currentHealth);
        if (Spawn[0] == false && ET.currentHealth <= 200) // ü���� 50 ������ �� ����
        {
            Instantiate(enemyPrefab, spawnPoint1.position, Quaternion.identity);
            Instantiate(enemyPrefab, spawnPoint2.position, Quaternion.identity);
            Instantiate(enemyPrefab, spawnPoint3.position, Quaternion.identity);
            Spawn[0] = true;
        }
    }
}
