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
        platformCollider.enabled = false; // �浹 ���� (�ٴ� ���)

        yield return new WaitForSeconds(0.5f); // 0.3�� �� �ٽ� �浹 Ȱ��ȭ

        platformCollider.enabled = true;
    }
}