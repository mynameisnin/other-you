using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chest : MonoBehaviour
{
    public Animator chestAnim;
    public Collider2D chestCollider;

    private bool isPlayerNear = false;
    private bool open = false;
    private bool alreadyOpened = false;

    void Update()
    {
        if (chestAnim != null && open && !alreadyOpened)
        {
            chestAnim.SetTrigger("Open");
            alreadyOpened = true;
        }


        if (isPlayerNear && Input.GetKeyDown(KeyCode.UpArrow))
        {
            open = true;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerNear = true;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerNear = false;
        }
    }

}
