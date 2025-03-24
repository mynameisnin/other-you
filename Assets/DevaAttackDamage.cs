using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DevaAttackDamage : MonoBehaviour
{
    // Start is called before the first frame update
    public int magicDamage = 40;

    public int GetMagicDamage()
    {
        return magicDamage;
    }
    private void Start()
    {
        // 1�� �� �ڵ� ����
        Destroy(gameObject, 1f);
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        // enemyTest�� �浹 �� ��� ����
        if (other.CompareTag("Enemy")) // Enemy �±װ� �پ� �־�� ��!
        {
            Destroy(gameObject);
        }
    }
}
