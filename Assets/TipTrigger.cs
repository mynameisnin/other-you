using System.Collections;
using UnityEngine;
using DG.Tweening; // DOTween 사용

public class TipTrigger : MonoBehaviour
{
    public GameObject tipPanel; // 팁 UI 패널
    private CanvasGroup canvasGroup; // 페이드 효과를 위한 CanvasGroup
    private Vector3 originalPosition; // 초기 위치 저장
    public float fadeDuration = 0.5f; // 페이드 지속 시간
    public float moveDistance = 50f; // 올라가는 거리
    private bool isTipActive = false; // 팁이 현재 활성화되었는지 체크

    private void Start()
    {
        if (tipPanel != null)
        {
            canvasGroup = tipPanel.GetComponent<CanvasGroup>();
            if (canvasGroup == null)
            {
                canvasGroup = tipPanel.AddComponent<CanvasGroup>();
            }

            originalPosition = tipPanel.transform.localPosition; // 초기 위치 저장
            tipPanel.SetActive(false); // 처음에는 비활성화
            canvasGroup.alpha = 0; // 투명하게 시작
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && !isTipActive)
        {
            ShowTip();
        }
    }

    private void Update()
    {
        if (isTipActive)
        {
            // Enter, ESC, ↑ 키를 누르면 닫힘
            if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.UpArrow))
            {
                HideTip();
            }
        }
    }

    void ShowTip()
    {
        isTipActive = true;
        tipPanel.SetActive(true); // 팁 패널 활성화
        tipPanel.transform.localPosition = originalPosition - new Vector3(0, moveDistance, 0); // 아래에서 시작

        // UI 애니메이션: 위로 이동 + 페이드 인 (SetUpdate(true) 추가)
        tipPanel.transform.DOLocalMoveY(originalPosition.y, fadeDuration).SetUpdate(true);
        canvasGroup.DOFade(1, fadeDuration).SetUpdate(true);

        // 게임 일시 정지
        Time.timeScale = 0;
    }

    public void HideTip()
    {
        if (!isTipActive) return;
        isTipActive = false;

        // UI 애니메이션: 아래로 이동 + 페이드 아웃 (SetUpdate(true) 추가)
        tipPanel.transform.DOLocalMoveY(originalPosition.y - moveDistance, fadeDuration).SetUpdate(true);
        canvasGroup.DOFade(0, fadeDuration).SetUpdate(true).OnComplete(() =>
        {
            tipPanel.SetActive(false); // 완전히 투명해지면 비활성화
            Time.timeScale = 1; // 게임 다시 진행

            // 트리거 오브젝트 삭제 (다시 실행되지 않도록)
            Destroy(gameObject);
        });
    }

}
