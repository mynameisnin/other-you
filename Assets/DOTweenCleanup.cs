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
        Debug.Log($"�� �����: {scene.name}, DOTween �ִϸ��̼� ���� ����");
        DOTween.KillAll(); // �� ���� �� ��� DOTween �ִϸ��̼� ����
    }

    void OnDestroy()
    {
        Debug.Log("DOTweenCleanup ������Ʈ ������, DOKill ����");
        DOTween.KillAll(); // ������Ʈ�� ������ �� ��� DOTween ����
    }


    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
}