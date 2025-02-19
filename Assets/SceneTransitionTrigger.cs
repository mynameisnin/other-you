using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using DG.Tweening;

public class SceneTransitionTrigger : MonoBehaviour
{
    public string nextSceneName; // 전환할 씬 이름
    public RectTransform blackPanel; // 검은 패널 UI (DOTween으로 이동할 오브젝트)
    public float transitionDuration = 1.2f; // 씬 전환 애니메이션 시간
    public Vector2 spawnPosition; // 이동할 씬의 스폰 위치

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("PlayerAttack")) // 충돌 감지
        {
            SpawnManager.Instance.spawnPosition = spawnPosition;
            StartCoroutine(TransitionScene()); //  씬 전환을 코루틴에서 실행 (즉시 변경 방지)
        }
    }

    IEnumerator TransitionScene()
    {
        //  기존 DOTween 애니메이션이 실행 중이라면 중지
        blackPanel.DOComplete();
        blackPanel.DOKill();

        // 검은 패널이 부드럽게 이동하면서 씬 전환
        blackPanel.DOAnchorPos(Vector2.zero, transitionDuration).SetEase(Ease.InOutQuad);

        yield return new WaitForSeconds(transitionDuration); // 애니메이션 끝날 때까지 대기

        //  씬을 변경하기 전에 DOTween 애니메이션 중지
        DOTween.KillAll();

        SceneManager.LoadScene(nextSceneName); // 씬 전환
    }
}
