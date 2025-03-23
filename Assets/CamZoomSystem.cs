using System.Collections;
using UnityEngine;

public class CamZoomSystem : MonoBehaviour
{
    public Transform target;           // �÷��̾� (ī�޶� ���� ���)
    public float zoomSize = 5f;        // Ȯ�� �� ī�޶� ũ��
    public float zoomSpeed = 2f;       // Ȯ�� �ӵ�
    private Camera cam;                // ī�޶� ����
    private float defaultSize;         // �⺻ ī�޶� ũ��
    private bool isZooming = false;    // ���� ���� ������ Ȯ��

    public CameraSmoothSystem smoothCamera; // ī�޶� ������ �̵� ��ũ��Ʈ ����

    void Start()
    {
        cam = GetComponent<Camera>();
        defaultSize = cam.orthographicSize; // �⺻ ī�޶� ũ�� ����
        FindTarget();
    }
    void Update()
    {
        if (target == null)
        {
            FindTarget(); //  �� ���� �� �÷��̾ �ٽ� ã��
        }
    }

    //  ���� ����Ǿ �÷��̾ �ٽ� ã�� �Լ� �߰�
    void FindTarget()
    {
        GameObject playerObj = GameObject.FindGameObjectWithTag("AdamCamPosition");
        if (playerObj != null)
        {
            target = playerObj.transform;
        }
        else
        {
            Debug.LogWarning("�÷��̾ ã�� �� ����! ������ 'Player' �±װ� �ִ� ������Ʈ�� Ȯ���ϼ���.");
        }
    }
    public void ZoomIn()
    {
        if (isZooming) return;  // �̹� �� ���� ���̸� �ߺ� ���� ����
        isZooming = true;

        if (smoothCamera != null)
        {
            smoothCamera.enabled = false; //������ �̵� ��� ��Ȱ��ȭ
        }

        StopAllCoroutines();
        StartCoroutine(ZoomToTarget());
    }

    public void ZoomOut()
    {
        if (!isZooming) return;  // ���� ���� ���� �ƴ� �� ���� ����
        isZooming = false;

        StopAllCoroutines();
        StartCoroutine(ResetCamera());
    }

    private IEnumerator ZoomToTarget()
    {
        Debug.Log("Zoom In ����");

        Vector3 targetPosition = new Vector3(target.position.x, target.position.y, transform.position.z);

        while (Mathf.Abs(cam.orthographicSize - zoomSize) > 0.01f)
        {
            cam.orthographicSize = Mathf.Lerp(cam.orthographicSize, zoomSize, Time.deltaTime * zoomSpeed * 2f);
            transform.position = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime * zoomSpeed * 2f);
            yield return null;
        }

        cam.orthographicSize = zoomSize;
        transform.position = targetPosition;

        Debug.Log("Zoom In �Ϸ�");
    }

    private IEnumerator ResetCamera()
    {
        Debug.Log("Zoom Out ����");

        while (Mathf.Abs(cam.orthographicSize - defaultSize) > 0.01f)
        {
            cam.orthographicSize = Mathf.Lerp(cam.orthographicSize, defaultSize, Time.deltaTime * zoomSpeed * 3f);
            yield return null;
        }

        cam.orthographicSize = defaultSize;

        Debug.Log("Zoom Out �Ϸ�");

        if (smoothCamera != null)
        {
            smoothCamera.enabled = true; // ���� ���� �� ��� Ȱ��ȭ
            StartCoroutine(ForceCameraUpdate()); //  ��� FixedUpdate() ����
        }
    }

    private IEnumerator ForceCameraUpdate()
    {
        yield return new WaitForFixedUpdate(); //  FixedUpdate �� ������ ��ٸ�
        if (smoothCamera != null)
        {
            smoothCamera.enabled = true; //  �ٽ� �� �� Ȱ��ȭ
        }
    }
}
