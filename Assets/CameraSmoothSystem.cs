using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraSmoothSystem : MonoBehaviour
{
    public Transform player;  // 따라갈 대상 (플레이어)
    public Vector3 offset;    // 카메라의 오프셋 위치
    public float speed;       // 카메라 이동 속도

    [SerializeField]
    private Transform leftBoundary;  // 왼쪽 경계
    [SerializeField]
    private Transform rightBoundary; // 오른쪽 경계

    void FixedUpdate()
    {
        Vector3 desiredPos = player.position + offset;

        // 카메라가 왼쪽 경계를 넘지 않도록 제한
        if (leftBoundary != null && desiredPos.x < leftBoundary.position.x)
        {
            desiredPos.x = leftBoundary.position.x;
        }

        // 카메라가 오른쪽 경계를 넘지 않도록 제한
        if (rightBoundary != null && desiredPos.x > rightBoundary.position.x)
        {
            desiredPos.x = rightBoundary.position.x;
        }

        // 부드러운 카메라 이동 적용
        transform.position = Vector3.Lerp(transform.position, desiredPos, speed * Time.fixedDeltaTime);
    }
}
