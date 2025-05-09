using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossAttackHitbox : MonoBehaviour
{
    public int damage = 10; // ������ ���ݷ�
    public bool fromAdam = false;
    public bool fromDeba = false;

    private void OnTriggerEnter2D(Collider2D other)
    {
        // �ƴ�
        if (other.CompareTag("Player"))
        {
            HurtPlayer hurtPlayer = other.GetComponent<HurtPlayer>();
            if (hurtPlayer != null)
            {
                hurtPlayer.TakeDamage(damage);
                Debug.Log("������ Player���� ������!");
            }
        }

        // ����
        else if (other.CompareTag("DevaPlayer"))
        {
            HurtDeva hurtDeva = other.GetComponent<HurtDeva>();
            if (hurtDeva != null)
            {
                hurtDeva.TakeDamage(damage);
                Debug.Log("������ DevaPlayer���� ������!");
            }
        }
    }
}

