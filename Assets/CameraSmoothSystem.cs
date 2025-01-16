using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraSmoothSystem : MonoBehaviour
{
    public Transform player;  // ���� ��� (�÷��̾�)
    public Vector3 offset;    // ī�޶��� ������ ��ġ
    public float speed;       // ī�޶� �̵� �ӵ�

    void FixedUpdate()
    {

        Vector3 desiredPos = player.position + offset;
        transform.position = Vector3.Lerp(transform.position, desiredPos, speed * Time.fixedDeltaTime);
    }

}