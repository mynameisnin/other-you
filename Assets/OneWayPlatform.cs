using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OneWayPlatform : MonoBehaviour
{
    private Collider2D platformCollider;

    void Start()
    {
        platformCollider = GetComponent<Collider2D>();
    }

    public void DisableCollision(float duration)
    {
        StartCoroutine(TemporarilyDisableCollision(duration));
    }

    private IEnumerator TemporarilyDisableCollision(float duration)
    {
        platformCollider.enabled = false; // 충돌 비활성화
        yield return new WaitForSeconds(duration);
        platformCollider.enabled = true; // 다시 충돌 활성화
    }
}