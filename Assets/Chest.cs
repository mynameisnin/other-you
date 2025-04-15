using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chest : MonoBehaviour
{
    public Animator chestAnim;
    public BoxCollider2D chestCollider;

    private bool opne;

    void Start()
    {
        opne = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (chestAnim != null && opne == true)
        {
            chestAnim.SetTrigger("Open");
        }
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") || other.CompareTag("PlayerAttack"))
        {
            if (Input.GetKeyDown(KeyCode.UpArrow))
            {
                opne = true;
            }
            
        }
    }


}
