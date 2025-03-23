using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraSmoothSystem : MonoBehaviour
{
    public Transform player;  // ���� ��� (�÷��̾�)
    public Vector3 offset;    // ī�޶��� ������ ��ġ
    public float speed = 5f;  // ī�޶� �̵� �ӵ� (�⺻�� �߰�)

    [SerializeField]
    private Transform leftBoundary;  // ���� ���
    [SerializeField]
    private Transform rightBoundary; // ������ ���

    void Start()
    {
        //  �÷��̾� ã�� (Start���� �� �� ����)
        FindPlayer();
    }

    void OnEnable()
    {
        //  ���� �ٽ� Ȱ��ȭ�� ������ �÷��̾� ã��
        FindPlayer();
    }

    void Update()
    {
        if (player == null || !player.gameObject.activeInHierarchy)
        {
            FindPlayer();
        }
    }

    void FindPlayer()
    {
        GameObject foundPlayer = GameObject.FindWithTag("AdamCamPosition");

        if (foundPlayer == null || !foundPlayer.activeInHierarchy)
        {
            foundPlayer = GameObject.FindWithTag("DevaCamPosition");
        }

        if (foundPlayer != null && foundPlayer.activeInHierarchy)
        {
            player = foundPlayer.transform;
        }
    }

    void FixedUpdate()
    {
        // �÷��̾ �����Ǿ����� ������Ʈ ����
        if (player == null) return;

        Vector3 desiredPos = player.position + offset;

        // ī�޶� ���� ��踦 ���� �ʵ��� ����
        if (leftBoundary != null && desiredPos.x < leftBoundary.position.x)
        {
            desiredPos.x = leftBoundary.position.x;
        }

        // ī�޶� ������ ��踦 ���� �ʵ��� ����
        if (rightBoundary != null && desiredPos.x > rightBoundary.position.x)
        {
            desiredPos.x = rightBoundary.position.x;
        }

        // �ε巯�� ī�޶� �̵� ����
        transform.position = Vector3.Lerp(transform.position, desiredPos, speed * Time.fixedDeltaTime);
    }

}
