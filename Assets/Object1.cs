using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Object1 : MonoBehaviour
{
    private bool playerIsClose = false;
    private Animator anim;

    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.UpArrow) && playerIsClose)
        {
            anim.SetTrigger("Trigger");
            //���⿡ ���� ���� ----------------------------------------------------------------------------
            Debug.Log("�ִϸ��̼� �۵�");
        }
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerIsClose = true;

        }
        if (other.CompareTag("DevaPlayer"))
        { playerIsClose = true; }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerIsClose = false;
        }
        if (other.CompareTag("DevaPlayer"))
        { playerIsClose = false; }
            
           
        

    }
}
