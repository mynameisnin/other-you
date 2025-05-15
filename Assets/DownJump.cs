using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DownJump : MonoBehaviour
{
    private BoxCollider2D platformCollider;

    void Start()
    {
        platformCollider = GetComponent<BoxCollider2D>();
    }

    void Update()
    {
 

    }

    public void TriggerDownJump()
    {
        StartCoroutine(DisableCollision());
    }

    System.Collections.IEnumerator DisableCollision()
    {
        platformCollider.enabled = false; // 충돌 해제 (바닥 통과)

        yield return new WaitForSeconds(0.5f); // 0.3초 후 다시 충돌 활성화

        platformCollider.enabled = true;
    }
}