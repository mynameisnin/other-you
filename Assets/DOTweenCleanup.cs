using UnityEngine;
using UnityEngine.SceneManagement;
using DG.Tweening;

public class DOTweenCleanup : MonoBehaviour
{
    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Debug.Log($"씬 변경됨: {scene.name}, DOTween 애니메이션 정리 실행");
        DOTween.KillAll(); // 씬 변경 시 모든 DOTween 애니메이션 정리
    }

    void OnDestroy()
    {
        Debug.Log("DOTweenCleanup 오브젝트 삭제됨, DOKill 실행");
        DOTween.KillAll(); // 오브젝트가 삭제될 때 모든 DOTween 정리
    }


    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
}