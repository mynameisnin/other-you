using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using DG.Tweening;

public class SceneTransitionTrigger2 : MonoBehaviour
{
    public string nextSceneName;                  // 전환할 씬 이름

    [Header("두 개의 검은 패널")]
    public RectTransform leftPanel;               // 왼쪽에서 오는 패널
    public RectTransform rightPanel;              // 오른쪽에서 오는 패널

    public float transitionDuration = 1.2f;

    private bool isTransitioning = false;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (isTransitioning) return;

        if (other.CompareTag("Player"))
        {
            isTransitioning = true;
            Debug.Log("[포탈] 플레이어 감지됨 → 씬 전환 시작");
            StartCoroutine(TransitionScene());
        }
    }

    private IEnumerator TransitionScene()
    {
        // 패널이 비활성화되어 있다면 켜기
        if (leftPanel != null) leftPanel.gameObject.SetActive(true);
        if (rightPanel != null) rightPanel.gameObject.SetActive(true);

        // 기존 애니메이션 정리
        leftPanel?.DOComplete(); leftPanel?.DOKill();
        rightPanel?.DOComplete(); rightPanel?.DOKill();

        // 시작 위치: 화면 바깥쪽
        leftPanel.anchoredPosition = new Vector2(-Screen.width, 0);
        rightPanel.anchoredPosition = new Vector2(Screen.width, 0);

        // 중앙으로 슬라이드 인
        leftPanel.DOAnchorPos(Vector2.zero, transitionDuration).SetEase(Ease.InOutQuad);
        rightPanel.DOAnchorPos(Vector2.zero, transitionDuration).SetEase(Ease.InOutQuad);

        yield return new WaitForSeconds(transitionDuration);

        DOTween.KillAll();
        SceneManager.LoadScene(nextSceneName);
    }
}
