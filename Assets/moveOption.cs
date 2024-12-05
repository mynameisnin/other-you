using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveOption : MonoBehaviour
{
    public RectTransform[] panels; // ��� �г��� �迭�� ����
    public float slideDuration = 0.5f; // �����̵� ���� �ð�
    private int currentPanelIndex = 0; // ���� Ȱ��ȭ�� �г� �ε���

    public void SlideToNextPanel()
    {
        int nextPanelIndex = (currentPanelIndex + 1) % panels.Length; // ���� �г� �ε���
        SlidePanels(currentPanelIndex, nextPanelIndex, true); // �����ʿ��� ����
    }

    public void SlideToPreviousPanel()
    {
        int prevPanelIndex = (currentPanelIndex - 1 + panels.Length) % panels.Length; // ���� �г� �ε���
        SlidePanels(currentPanelIndex, prevPanelIndex, false); // ���ʿ��� ����
    }

    private void SlidePanels(int current, int target, bool toRight)
    {
        // �����̵� ���� ����
        Vector3 offScreenExit = toRight ? new Vector3(-Screen.width, 0, 0) : new Vector3(Screen.width, 0, 0);
        Vector3 offScreenEnter = toRight ? new Vector3(Screen.width, 0, 0) : new Vector3(-Screen.width, 0, 0);

        RectTransform currentPanel = panels[current]; // ���� �г�
        RectTransform targetPanel = panels[target];   // ��ǥ �г�

        // ���ο� �г��� ���� ��ġ ����
        targetPanel.localPosition = offScreenEnter;
        targetPanel.gameObject.SetActive(true); // Ÿ�� �г� Ȱ��ȭ

        // �ִϸ��̼� ����
        StartCoroutine(SlidePanel(currentPanel, currentPanel.localPosition, offScreenExit, false));
        StartCoroutine(SlidePanel(targetPanel, offScreenEnter, Vector3.zero, true));

        // ���� �г� �ε��� ������Ʈ
        currentPanelIndex = target;
    }

    private IEnumerator SlidePanel(RectTransform panel, Vector3 start, Vector3 end, bool activateAtEnd)
    {
        float elapsedTime = 0;

        while (elapsedTime < slideDuration)
        {
            // �ε巯�� ��ȯ�� ���� SmoothStep ���
            float t = Mathf.SmoothStep(0, 1, elapsedTime / slideDuration);
            panel.localPosition = Vector3.Lerp(start, end, t);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        panel.localPosition = end; // �ִϸ��̼� ���� �� ��ġ ����

        if (!activateAtEnd)
        {
            panel.gameObject.SetActive(false); // ����� �г� ��Ȱ��ȭ
        }
    }

    // ������� ���� �׽�Ʈ�� �޼���
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            SlideToNextPanel();
        }
        else if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            SlideToPreviousPanel();
        }
    }
}
