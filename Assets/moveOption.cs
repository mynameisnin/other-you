using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveOption : MonoBehaviour
{
    public RectTransform[] panels; // 모든 패널을 배열로 관리
    public float slideDuration = 0.5f; // 슬라이드 지속 시간
    private int currentPanelIndex = 0; // 현재 활성화된 패널 인덱스

    public void SlideToNextPanel()
    {
        int nextPanelIndex = (currentPanelIndex + 1) % panels.Length; // 다음 패널 인덱스
        SlidePanels(currentPanelIndex, nextPanelIndex, true); // 오른쪽에서 등장
    }

    public void SlideToPreviousPanel()
    {
        int prevPanelIndex = (currentPanelIndex - 1 + panels.Length) % panels.Length; // 이전 패널 인덱스
        SlidePanels(currentPanelIndex, prevPanelIndex, false); // 왼쪽에서 등장
    }

    private void SlidePanels(int current, int target, bool toRight)
    {
        // 슬라이드 방향 설정
        Vector3 offScreenExit = toRight ? new Vector3(-Screen.width, 0, 0) : new Vector3(Screen.width, 0, 0);
        Vector3 offScreenEnter = toRight ? new Vector3(Screen.width, 0, 0) : new Vector3(-Screen.width, 0, 0);

        RectTransform currentPanel = panels[current]; // 현재 패널
        RectTransform targetPanel = panels[target];   // 목표 패널

        // 새로운 패널의 시작 위치 설정
        targetPanel.localPosition = offScreenEnter;
        targetPanel.gameObject.SetActive(true); // 타겟 패널 활성화

        // 애니메이션 실행
        StartCoroutine(SlidePanel(currentPanel, currentPanel.localPosition, offScreenExit, false));
        StartCoroutine(SlidePanel(targetPanel, offScreenEnter, Vector3.zero, true));

        // 현재 패널 인덱스 업데이트
        currentPanelIndex = target;
    }

    private IEnumerator SlidePanel(RectTransform panel, Vector3 start, Vector3 end, bool activateAtEnd)
    {
        float elapsedTime = 0;

        while (elapsedTime < slideDuration)
        {
            // 부드러운 전환을 위해 SmoothStep 사용
            float t = Mathf.SmoothStep(0, 1, elapsedTime / slideDuration);
            panel.localPosition = Vector3.Lerp(start, end, t);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        panel.localPosition = end; // 애니메이션 종료 후 위치 고정

        if (!activateAtEnd)
        {
            panel.gameObject.SetActive(false); // 종료된 패널 비활성화
        }
    }

    // 디버깅을 위한 테스트용 메서드
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
