using System.Collections;
using UnityEngine;
using DG.Tweening; // DOTween 사용

public class TipTrigger : MonoBehaviour
{
    public GameObject[] tipPanels; // 여러 개의 팁 패널을 저장
    private int currentPanelIndex = 0; // 현재 표시 중인 패널 인덱스
    private CanvasGroup currentCanvasGroup; // 현재 패널의 CanvasGroup
    private Vector3 originalPosition; // 초기 위치 저장
    public float fadeDuration = 0.5f; // 페이드 지속 시간
    public float moveDistance = 100f; // 이동 거리 (패널 간 이동)

    private bool isTipActive = false; // 팁이 활성화되었는지 체크

    private void Start()
    {
        foreach (GameObject panel in tipPanels)
        {
            if (panel != null)
            {
                CanvasGroup canvas = panel.GetComponent<CanvasGroup>();
                if (canvas == null)
                {
                    canvas = panel.AddComponent<CanvasGroup>();
                }
                canvas.alpha = 0; // 모든 패널을 투명하게 설정
                panel.SetActive(false); // 비활성화
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && !isTipActive)
        {
            ShowTip(0); // 첫 번째 패널을 표시
        }
    }

    private void Update()
    {
        if (isTipActive)
        {
            // Enter, ESC, ↑ 키를 누르면 팁 닫기
            if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.UpArrow))
            {
                HideTip();
            }

            // → 키를 누르면 다음 패널로 이동
            if (Input.GetKeyDown(KeyCode.RightArrow))
            {
                ShowNextTip();
            }

            // ← 키를 누르면 이전 패널로 이동
            if (Input.GetKeyDown(KeyCode.LeftArrow))
            {
                ShowPreviousTip();
            }
        }
    }

    void ShowTip(int index)
    {
        if (index < 0 || index >= tipPanels.Length) return; // 유효한 인덱스인지 확인

        isTipActive = true;
        currentPanelIndex = index;
        GameObject panel = tipPanels[index];
        currentCanvasGroup = panel.GetComponent<CanvasGroup>();

        panel.SetActive(true);
        panel.transform.localPosition = new Vector3(moveDistance, panel.transform.localPosition.y, 0); // 오른쪽에서 시작

        // 오른쪽에서 등장하며 페이드 인
        panel.transform.DOLocalMoveX(0, fadeDuration).SetEase(Ease.OutQuad).SetUpdate(true);
        currentCanvasGroup.DOFade(1, fadeDuration).SetUpdate(true);

        // 게임 일시 정지
        Time.timeScale = 0;
    }

    void ShowNextTip()
    {
        if (currentPanelIndex >= tipPanels.Length - 1) return; // 마지막 패널이면 진행하지 않음

        GameObject currentPanel = tipPanels[currentPanelIndex];
        CanvasGroup currentCanvas = currentPanel.GetComponent<CanvasGroup>();

        GameObject nextPanel = tipPanels[currentPanelIndex + 1];
        CanvasGroup nextCanvas = nextPanel.GetComponent<CanvasGroup>();

        float exitDistance = moveDistance * 1.5f; 
        float enterDistance = moveDistance * 1.5f; 

        // 현재 패널을 왼쪽으로 이동하며 페이드 아웃
        currentPanel.transform.DOLocalMoveX(-exitDistance, fadeDuration).SetEase(Ease.InQuad).SetUpdate(true);
        currentCanvas.DOFade(0, fadeDuration).SetUpdate(true).OnComplete(() =>
        {
            currentPanel.SetActive(false); // 현재 패널 숨김
        });

        // 다음 패널을 오른쪽에서 등장하며 페이드 인
        nextPanel.SetActive(true);
        nextPanel.transform.localPosition = new Vector3(enterDistance, nextPanel.transform.localPosition.y, 0);
        nextPanel.transform.DOLocalMoveX(0, fadeDuration).SetEase(Ease.OutQuad).SetUpdate(true);
        nextCanvas.DOFade(1, fadeDuration).SetUpdate(true);

        // 현재 패널 인덱스를 업데이트
        currentPanelIndex++;
    }

    void ShowPreviousTip()
    {
        if (currentPanelIndex <= 0) return; // 첫 번째 패널이면 진행하지 않음

        GameObject currentPanel = tipPanels[currentPanelIndex];
        CanvasGroup currentCanvas = currentPanel.GetComponent<CanvasGroup>();

        GameObject previousPanel = tipPanels[currentPanelIndex - 1];
        CanvasGroup previousCanvas = previousPanel.GetComponent<CanvasGroup>();

        float exitDistance = moveDistance * 1.5f;
        float enterDistance = moveDistance * 1.5f;

        // 현재 패널을 오른쪽으로 이동하며 페이드 아웃
        currentPanel.transform.DOLocalMoveX(exitDistance, fadeDuration).SetEase(Ease.InQuad).SetUpdate(true);
        currentCanvas.DOFade(0, fadeDuration).SetUpdate(true).OnComplete(() =>
        {
            currentPanel.SetActive(false); // 현재 패널 숨김
        });

        // 이전 패널을 왼쪽에서 등장하며 페이드 인
        previousPanel.SetActive(true);
        previousPanel.transform.localPosition = new Vector3(-enterDistance, previousPanel.transform.localPosition.y, 0);
        previousPanel.transform.DOLocalMoveX(0, fadeDuration).SetEase(Ease.OutQuad).SetUpdate(true);
        previousCanvas.DOFade(1, fadeDuration).SetUpdate(true);

        // 현재 패널 인덱스를 업데이트
        currentPanelIndex--;
    }
    public void HideTip()
    {
        if (!isTipActive) return;
        isTipActive = false;

        GameObject currentPanel = tipPanels[currentPanelIndex];
        CanvasGroup currentCanvas = currentPanel.GetComponent<CanvasGroup>();

        // 현재 패널을 아래로 이동하며 페이드 아웃
        currentPanel.transform.DOLocalMoveY(-moveDistance, fadeDuration).SetUpdate(true);
        currentCanvas.DOFade(0, fadeDuration).SetUpdate(true).OnComplete(() =>
        {
            currentPanel.SetActive(false); // 완전히 투명해지면 비활성화
            Time.timeScale = 1; // 게임 다시 진행
            Destroy(gameObject); // 트리거 오브젝트 삭제
        });
    }
}
