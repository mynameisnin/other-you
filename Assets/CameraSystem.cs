using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraSystem : MonoBehaviour
{
    public Transform target;           // Ȯ���� ĳ������ Transform
    public float zoomSize = 5f;        // Ȯ�� �� ī�޶� ũ��
    public float zoomSpeed = 2f;       // Ȯ�� �ӵ�
    private Camera cam;                // ī�޶� ����
    private float defaultSize;         // �⺻ ī�޶� ũ��
    private Vector3 defaultPosition;   // �⺻ ī�޶� ��ġ

    void Start()
    {
        cam = GetComponent<Camera>();
        defaultSize = cam.orthographicSize;        // �⺻ ī�޶� ũ�� ����
        defaultPosition = transform.position;     // �⺻ ī�޶� ��ġ ����
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
        Debug.Log("Zoom In ����");

        // Ȯ��� ��ǥ ��ġ ����
        Vector3 targetPosition = new Vector3(target.position.x, target.position.y, transform.position.z);

        while (cam.orthographicSize > zoomSize || transform.position != targetPosition)
        {
            // ī�޶� ũ�� ���������� ����
            cam.orthographicSize = Mathf.Lerp(cam.orthographicSize, zoomSize, Time.deltaTime * zoomSpeed);
            // ī�޶� ��ġ ���������� �̵�
            transform.position = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime * zoomSpeed);

            yield return null;
        }

        cam.orthographicSize = zoomSize;
        transform.position = targetPosition;

        Debug.Log("Zoom In �Ϸ�");
    }

    private IEnumerator ResetCamera()
    {
        Debug.Log("Zoom Out ����");

        while (cam.orthographicSize < defaultSize || transform.position != defaultPosition)
        {
            // ī�޶� ũ�� ���������� ����
            cam.orthographicSize = Mathf.Lerp(cam.orthographicSize, defaultSize, Time.deltaTime * zoomSpeed);
            // ī�޶� ��ġ ���������� ����
            transform.position = Vector3.Lerp(transform.position, defaultPosition, Time.deltaTime * zoomSpeed);

            yield return null;
        }

        cam.orthographicSize = defaultSize;
        transform.position = defaultPosition;

        Debug.Log("Zoom Out �Ϸ�");
    }
}
