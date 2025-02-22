using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DownJump : MonoBehaviour
{
    private Collider2D platformCollider;
    private AdamMovement player; // �÷��̾� ��ũ��Ʈ ����

    void Start()
    {
        platformCollider = GetComponent<Collider2D>();
        player = FindObjectOfType<AdamMovement>(); // �÷��̾� ã��
    }

    void Update()
    {
        if (IsPlayerOnPlatform() && Input.GetKey(KeyCode.DownArrow) && Input.GetKeyDown(KeyCode.Space))
        {
            StartCoroutine(DisableCollision());
        }
    }

    bool IsPlayerOnPlatform()
    {
        return player != null && player.isGround; // �޼��� ȣ�� X, �����̹Ƿ� ��ȣ ����
    }

    System.Collections.IEnumerator DisableCollision()
    {
        player.isGround = false; // �ٴ� üũ ���� (�߿�!)
        platformCollider.enabled = false; // �浹 ���� (�ٴ� ���)

        yield return new WaitForSeconds(0.7f); // 0.3�� �� �ٽ� �浹 Ȱ��ȭ

        platformCollider.enabled = true;
    }
}