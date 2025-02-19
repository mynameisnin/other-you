using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using DG.Tweening;

public class SceneTransitionTrigger : MonoBehaviour
{
    public string nextSceneName; // ��ȯ�� �� �̸�
    public RectTransform blackPanel; // ���� �г� UI (DOTween���� �̵��� ������Ʈ)
    public float transitionDuration = 1.2f; // �� ��ȯ �ִϸ��̼� �ð�
    public Vector2 spawnPosition; // �̵��� ���� ���� ��ġ

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("PlayerAttack")) // �浹 ����
        {
            SpawnManager.Instance.spawnPosition = spawnPosition;
            StartCoroutine(TransitionScene()); //  �� ��ȯ�� �ڷ�ƾ���� ���� (��� ���� ����)
        }
    }

    IEnumerator TransitionScene()
    {
        //  ���� DOTween �ִϸ��̼��� ���� ���̶�� ����
        blackPanel.DOComplete();
        blackPanel.DOKill();

        // ���� �г��� �ε巴�� �̵��ϸ鼭 �� ��ȯ
        blackPanel.DOAnchorPos(Vector2.zero, transitionDuration).SetEase(Ease.InOutQuad);

        yield return new WaitForSeconds(transitionDuration); // �ִϸ��̼� ���� ������ ���

        //  ���� �����ϱ� ���� DOTween �ִϸ��̼� ����
        DOTween.KillAll();

        SceneManager.LoadScene(nextSceneName); // �� ��ȯ
    }
}
