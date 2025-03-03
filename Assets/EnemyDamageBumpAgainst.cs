using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyDamageBumpAgainst : MonoBehaviour
{
    private Collider2D damageCollider;

    void Start()
    {
        damageCollider = GetComponent<Collider2D>();
    }

    private IEnumerator DisableColliderForSeconds(float duration)
    {
        if (damageCollider != null)
        {
            damageCollider.enabled = false; //  콜라이더 비활성화
            yield return new WaitForSeconds(duration);
            damageCollider.enabled = true;  //  다시 활성화
        }
    }

    public void TriggerDamageCooldown(float duration)
    {
        StartCoroutine(DisableColliderForSeconds(duration));
    }
}