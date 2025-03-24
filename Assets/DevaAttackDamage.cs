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
        // 1초 후 자동 삭제
        Destroy(gameObject, 1f);
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        // enemyTest와 충돌 시 즉시 삭제
        if (other.CompareTag("Enemy")) // Enemy 태그가 붙어 있어야 함!
        {
            Destroy(gameObject);
        }
    }
}
