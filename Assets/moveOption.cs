using System.Collections;
using UnityEngine;

public class MoveOption : MonoBehaviour
{
    public RectTransform mainPanel;   // 메인 패널
    public RectTransform optionPanel; // 옵션 패널
    public float slideDuration = 0.5f; // 슬라이드 애니메이션 지속 시간

    private void Start()
    {
        // 초기 위치 설정
        mainPanel.localPosition = Vector3.zero; // 화면 중앙
        optionPanel.localPosition = new Vector3(-Screen.width, 0, 0); // 화면 왼쪽
    }

    public void ShowOptionPanel()
    {
        StartCoroutine(SlidePanels(mainPanel, optionPanel, false)); // 왼쪽에서 오른쪽으로 슬라이드
    }

    public void ShowMainPanel()
    {
        StartCoroutine(SlidePanels(optionPanel, mainPanel, true)); // 오른쪽에서 왼쪽으로 슬라이드
    }

    private IEnumerator SlidePanels(RectTransform currentPanel, RectTransform targetPanel, bool toRight)
    {
        // 슬라이드 방향 설정
        Vector3 currentEndPos = toRight ? new Vector3(Screen.width, 0, 0) : new Vector3(-Screen.width, 0, 0); // 현재 패널이 나갈 방향
        Vector3 targetStartPos = toRight ? new Vector3(-Screen.width, 0, 0) : new Vector3(Screen.width, 0, 0); // 타겟 패널 시작 위치
        Vector3 targetEndPos = Vector3.zero; // 타겟 패널은 중앙으로 이동

        // 타겟 패널 초기 위치 설정
        targetPanel.localPosition = targetStartPos;
        targetPanel.gameObject.SetActive(true);

        float elapsedTime = 0f;

        while (elapsedTime < slideDuration)
        {
            float t = elapsedTime / slideDuration;
            currentPanel.localPosition = Vector3.Lerp(Vector3.zero, currentEndPos, t); // 현재 패널 슬라이드 아웃
            targetPanel.localPosition = Vector3.Lerp(targetStartPos, targetEndPos, t); // 타겟 패널 슬라이드 인
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // 애니메이션 종료 후 위치 고정
        currentPanel.localPosition = currentEndPos;
        targetPanel.localPosition = targetEndPos;

        currentPanel.gameObject.SetActive(false); // 현재 패널 비활성화
    }
}
