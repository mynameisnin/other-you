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
            damageCollider.enabled = false; //  �ݶ��̴� ��Ȱ��ȭ
            yield return new WaitForSeconds(duration);
            damageCollider.enabled = true;  //  �ٽ� Ȱ��ȭ
        }
    }

    public void TriggerDamageCooldown(float duration)
    {
        StartCoroutine(DisableColliderForSeconds(duration));
    }
}