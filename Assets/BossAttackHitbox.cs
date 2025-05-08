using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossAttackHitbox : MonoBehaviour
{
    public int damage = 10; // 보스의 공격력
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
                Debug.Log("보스가 Player에게 데미지!");
            }
        }

        // 데바
        else if (other.CompareTag("DevaPlayer"))
        {
            HurtDeva hurtDeva = other.GetComponent<HurtDeva>();
            if (hurtDeva != null)
            {
                hurtDeva.TakeDamage(damage);
                Debug.Log("보스가 DevaPlayer에게 데미지!");
            }
        }
    }
}

