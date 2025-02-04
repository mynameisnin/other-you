using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraSmoothSystem : MonoBehaviour
{
    public Transform player;  // ���� ��� (�÷��̾�)
    public Vector3 offset;    // ī�޶��� ������ ��ġ
    public float speed;       // ī�޶� �̵� �ӵ�

    [SerializeField]
    private Transform leftBoundary;  // ���� ���
    [SerializeField]
    private Transform rightBoundary; // ������ ���

    void FixedUpdate()
    {
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
