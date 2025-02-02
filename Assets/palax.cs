using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class palax : MonoBehaviour
{
    [SerializeField]
    private Transform CameraTransform;
    [SerializeField]
    private float ParallaxEffect = 0.5f;

    private Vector3 lastCameraPosition;
    // Start is called before the first frame update
    void Start()
    {
        lastCameraPosition = CameraTransform.position;
    }

    // Update is called once per frame
    void Update()
    {
        // 카메라와 라스트 카메라 포지선을 빼서 deltaMovement에 저장 
        Vector3 deltaMovement = CameraTransform.position - lastCameraPosition;

        // 배경을 반대 방향으로 움직이게 설정
        transform.position -= new Vector3(deltaMovement.x * ParallaxEffect, deltaMovement.y * ParallaxEffect);
        // 배경을 반대 방향으로 움직이게 설정
        lastCameraPosition = CameraTransform.position;
    }
}

