using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraSmoothSystem : MonoBehaviour
{
    public Transform player;  // 따라갈 대상 (플레이어)
    public Vector3 offset;    // 카메라의 오프셋 위치
    public float speed = 5f;  // 카메라 이동 속도 (기본값 추가)

    [SerializeField]
    private Transform leftBoundary;  // 왼쪽 경계
    [SerializeField]
    private Transform rightBoundary; // 오른쪽 경계

    void Start()
    {
        //  플레이어 찾기 (Start에서 한 번 실행)
        FindPlayer();
    }

    void OnEnable()
    {
        //  씬이 다시 활성화될 때마다 플레이어 찾기
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
        // 플레이어가 삭제되었으면 업데이트 중지
        if (player == null) return;

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
