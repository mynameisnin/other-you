using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraSmoothSystem : MonoBehaviour
{
    public Transform player;  // 따라갈 대상 (플레이어)
    public Vector3 offset;    // 카메라의 오프셋 위치
    public float speed;       // 카메라 이동 속도

    void FixedUpdate()
    {

        Vector3 desiredPos = player.position + offset;
        transform.position = Vector3.Lerp(transform.position, desiredPos, speed * Time.fixedDeltaTime);
    }

}