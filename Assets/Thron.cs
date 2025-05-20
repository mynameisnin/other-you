using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Thron : MonoBehaviour
{
    public int damage = 10; 
    public bool fromAdam = false;
    public bool fromDeba = false;

    private void OnTriggerEnter2D(Collider2D other)
    {
        // 아담
        if (other.CompareTag("Player"))
        {
            HurtPlayer hurtPlayer = other.GetComponent<HurtPlayer>();
            if (hurtPlayer != null)
            {
                hurtPlayer.TakeDamage(damage);
               
            }
        }

        // 데바
        else if (other.CompareTag("DevaPlayer"))
        {
            HurtDeva hurtDeva = other.GetComponent<HurtDeva>();
            if (hurtDeva != null)
            {
                hurtDeva.TakeDamage(damage);
             
            }
        }
    }
}


