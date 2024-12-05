using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class moveOption : MonoBehaviour
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

        // �ִϸ��̼� ����
        StartCoroutine(SlidePanel(currentPanel, currentPanel.localPosition, offScreenExit));
        StartCoroutine(SlidePanel(targetPanel, offScreenEnter, Vector3.zero));

        // ���� �г� �ε��� ������Ʈ
        currentPanelIndex = target;
    }

    private IEnumerator SlidePanel(RectTransform panel, Vector3 start, Vector3 end)
    {
        float elapsedTime = 0;

        while (elapsedTime < slideDuration)
        {
            panel.localPosition = Vector3.Lerp(start, end, elapsedTime / slideDuration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        panel.localPosition = end; // �ִϸ��̼� ���� �� ��ġ ����
    }
}