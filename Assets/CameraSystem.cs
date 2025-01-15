using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraSystem : MonoBehaviour
{
    public Transform target;           // 확대할 캐릭터의 Transform
    public float zoomSize = 5f;        // 확대 시 카메라 크기
    public float zoomSpeed = 2f;       // 확대 속도
    private Camera cam;                // 카메라 참조
    private float defaultSize;         // 기본 카메라 크기
    private Vector3 defaultPosition;   // 기본 카메라 위치

    void Start()
    {
        cam = GetComponent<Camera>();
        defaultSize = cam.orthographicSize;        // 기본 카메라 크기 저장
        defaultPosition = transform.position;     // 기본 카메라 위치 저장
    }

    public void ZoomIn()
    {
        StopAllCoroutines();
        StartCoroutine(ZoomToTarget());
    }

    public void ZoomOut()
    {
        StopAllCoroutines();
        StartCoroutine(ResetCamera());
    }

    private IEnumerator ZoomToTarget()
    {
        Debug.Log("Zoom In 시작");

        // 확대될 목표 위치 설정
        Vector3 targetPosition = new Vector3(target.position.x, target.position.y, transform.position.z);

        while (cam.orthographicSize > zoomSize || transform.position != targetPosition)
        {
            // 카메라 크기 점진적으로 줄임
            cam.orthographicSize = Mathf.Lerp(cam.orthographicSize, zoomSize, Time.deltaTime * zoomSpeed);
            // 카메라 위치 점진적으로 이동
            transform.position = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime * zoomSpeed);

            yield return null;
        }

        cam.orthographicSize = zoomSize;
        transform.position = targetPosition;

        Debug.Log("Zoom In 완료");
    }

    private IEnumerator ResetCamera()
    {
        Debug.Log("Zoom Out 시작");

        while (cam.orthographicSize < defaultSize || transform.position != defaultPosition)
        {
            // 카메라 크기 점진적으로 복구
            cam.orthographicSize = Mathf.Lerp(cam.orthographicSize, defaultSize, Time.deltaTime * zoomSpeed);
            // 카메라 위치 점진적으로 복구
            transform.position = Vector3.Lerp(transform.position, defaultPosition, Time.deltaTime * zoomSpeed);

            yield return null;
        }

        cam.orthographicSize = defaultSize;
        transform.position = defaultPosition;

        Debug.Log("Zoom Out 완료");
    }
}
