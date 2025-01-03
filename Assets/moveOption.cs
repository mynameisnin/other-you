using System.Collections;
using UnityEngine;

public class MoveOption : MonoBehaviour
{
    public RectTransform mainPanel;   // ���� �г�
    public RectTransform optionPanel; // �ɼ� �г�
    public float slideDuration = 0.5f; // �����̵� �ִϸ��̼� ���� �ð�

    private void Start()
    {
        // �ʱ� ��ġ ����
        mainPanel.localPosition = Vector3.zero; // ȭ�� �߾�
        optionPanel.localPosition = new Vector3(-Screen.width, 0, 0); // ȭ�� ����
    }

    public void ShowOptionPanel()
    {
        StartCoroutine(SlidePanels(mainPanel, optionPanel, false)); // ���ʿ��� ���������� �����̵�
    }

    public void ShowMainPanel()
    {
        StartCoroutine(SlidePanels(optionPanel, mainPanel, true)); // �����ʿ��� �������� �����̵�
    }

    private IEnumerator SlidePanels(RectTransform currentPanel, RectTransform targetPanel, bool toRight)
    {
        // �����̵� ���� ����
        Vector3 currentEndPos = toRight ? new Vector3(Screen.width, 0, 0) : new Vector3(-Screen.width, 0, 0); // ���� �г��� ���� ����
        Vector3 targetStartPos = toRight ? new Vector3(-Screen.width, 0, 0) : new Vector3(Screen.width, 0, 0); // Ÿ�� �г� ���� ��ġ
        Vector3 targetEndPos = Vector3.zero; // Ÿ�� �г��� �߾����� �̵�

        // Ÿ�� �г� �ʱ� ��ġ ����
        targetPanel.localPosition = targetStartPos;
        targetPanel.gameObject.SetActive(true);

        float elapsedTime = 0f;

        while (elapsedTime < slideDuration)
        {
            float t = elapsedTime / slideDuration;
            currentPanel.localPosition = Vector3.Lerp(Vector3.zero, currentEndPos, t); // ���� �г� �����̵� �ƿ�
            targetPanel.localPosition = Vector3.Lerp(targetStartPos, targetEndPos, t); // Ÿ�� �г� �����̵� ��
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // �ִϸ��̼� ���� �� ��ġ ����
        currentPanel.localPosition = currentEndPos;
        targetPanel.localPosition = targetEndPos;

        currentPanel.gameObject.SetActive(false); // ���� �г� ��Ȱ��ȭ
    }
}
