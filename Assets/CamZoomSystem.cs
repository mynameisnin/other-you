using System.Collections;
using UnityEngine;

public class CamZoomSystem : MonoBehaviour
{
    public Transform target;           // 플레이어 (카메라가 따라갈 대상)
    public float zoomSize = 5f;        // 확대 시 카메라 크기
    public float zoomSpeed = 2f;       // 확대 속도
    private Camera cam;                // 카메라 참조
    private float defaultSize;         // 기본 카메라 크기
    private bool isZooming = false;    // 줌이 진행 중인지 확인

    public CameraSmoothSystem smoothCamera; // 카메라 스무스 이동 스크립트 참조

    void Start()
    {
        cam = GetComponent<Camera>();
        defaultSize = cam.orthographicSize; // 기본 카메라 크기 저장
        FindTarget();
    }
    void Update()
    {
        if (target == null)
        {
            FindTarget(); //  씬 변경 시 플레이어를 다시 찾음
        }
    }

    //  씬이 변경되어도 플레이어를 다시 찾는 함수 추가
    void FindTarget()
    {
        GameObject playerObj = GameObject.FindGameObjectWithTag("AdamCamPosition");
        if (playerObj != null)
        {
            target = playerObj.transform;
        }
        else
        {
            Debug.LogWarning("플레이어를 찾을 수 없음! 씬에서 'Player' 태그가 있는 오브젝트를 확인하세요.");
        }
    }
    public void ZoomIn()
    {
        if (isZooming) return;  // 이미 줌 실행 중이면 중복 실행 방지
        isZooming = true;

        if (smoothCamera != null)
        {
            smoothCamera.enabled = false; //스무스 이동 기능 비활성화
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

        while (Mathf.Abs(cam.orthographicSize - zoomSize) > 0.01f)
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

        while (Mathf.Abs(cam.orthographicSize - defaultSize) > 0.01f)
        {
            cam.orthographicSize = Mathf.Lerp(cam.orthographicSize, defaultSize, Time.deltaTime * zoomSpeed * 3f);
            yield return null;
        }

        cam.orthographicSize = defaultSize;

        Debug.Log("Zoom Out 완료");

        if (smoothCamera != null)
        {
            smoothCamera.enabled = true; // 줌이 끝난 후 즉시 활성화
            StartCoroutine(ForceCameraUpdate()); //  즉시 FixedUpdate() 실행
        }
    }

    private IEnumerator ForceCameraUpdate()
    {
        yield return new WaitForFixedUpdate(); //  FixedUpdate 한 프레임 기다림
        if (smoothCamera != null)
        {
            smoothCamera.enabled = true; //  다시 한 번 활성화
        }
    }
}
