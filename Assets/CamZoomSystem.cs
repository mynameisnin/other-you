using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CamZoomSystem : MonoBehaviour
{
    public Transform target;           // 확대할 캐릭터의 Transform
    public float zoomSize = 5f;        // 확대 시 카메라 크기
    public float zoomSpeed = 2f;       // 확대 속도
    private Camera cam;                // 카메라 참조
    private float defaultSize;         // 기본 카메라 크기
    private Vector3 defaultPosition;   // 기본 카메라 위치
    private bool isZooming = false;    // 줌이 진행 중인지 확인

    public CameraSmoothSystem smoothCamera; // 카메라 스무스 이동 스크립트 참조

    void Start()
    {
        cam = GetComponent<Camera>();
        defaultSize = cam.orthographicSize;       // 기본 카메라 크기 저장
        defaultPosition = transform.position;     // 기본 카메라 위치 저장
    }

    public void ZoomIn()
    {
        if (isZooming) return;  // 이미 줌 실행 중이면 중복 실행 방지
        isZooming = true;

        if (smoothCamera != null)
        {
            smoothCamera.enabled = false; // 스무스 이동 기능 비활성화
        }

        StopAllCoroutines();
        StartCoroutine(ZoomToTarget());
    }

    public void ZoomOut()
    {
        if (!isZooming) return;  // 줌이 실행 중이 아닐 때 실행 방지
        isZooming = false;

        StopAllCoroutines();
        StartCoroutine(ResetCamera());
    }

    private IEnumerator ZoomToTarget()
    {
        Debug.Log("Zoom In 시작");

        Vector3 targetPosition = new Vector3(target.position.x, target.position.y, transform.position.z);

        while (Mathf.Abs(cam.orthographicSize - zoomSize) > 0.01f || Vector3.Distance(transform.position, targetPosition) > 0.01f)
        {
            cam.orthographicSize = Mathf.Lerp(cam.orthographicSize, zoomSize, Time.deltaTime * zoomSpeed * 2f);
            transform.position = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime * zoomSpeed * 2f);
            yield return null;
        }

        cam.orthographicSize = zoomSize;
        transform.position = targetPosition;

        Debug.Log("Zoom In 완료");
    }

    private IEnumerator ResetCamera()
    {
        Debug.Log("Zoom Out 시작");

        while (Mathf.Abs(cam.orthographicSize - defaultSize) > 0.01f || Vector3.Distance(transform.position, defaultPosition) > 0.01f)
        {
            cam.orthographicSize = Mathf.Lerp(cam.orthographicSize, defaultSize, Time.deltaTime * zoomSpeed * 3f);
            transform.position = Vector3.Lerp(transform.position, defaultPosition, Time.deltaTime * zoomSpeed * 3f);
            yield return null;
        }

        cam.orthographicSize = defaultSize;
        transform.position = defaultPosition;

        Debug.Log("Zoom Out 완료");

        if (smoothCamera != null)
        {
            smoothCamera.enabled = true; // 줌이 끝난 후 다시 스무스 이동 활성화
        }
    }
}
