using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Object3 : MonoBehaviour
{
    private bool playerIsClose = false;

    public GameObject oj3;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.UpArrow) && playerIsClose)
        {
            //여기에 보스 입구 구현 -------------------------------------------------------------------
            oj3.SetActive(false);
        }
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerIsClose = true;

        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerIsClose = false;
        }
    }
}
