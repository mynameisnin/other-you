using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using DG.Tweening;

public class SceneTransitionTrigger2 : MonoBehaviour
{
    public string nextSceneName;                  // ��ȯ�� �� �̸�

    [Header("�� ���� ���� �г�")]
    public RectTransform leftPanel;               // ���ʿ��� ���� �г�
    public RectTransform rightPanel;              // �����ʿ��� ���� �г�

    public float transitionDuration = 1.2f;

    private bool isTransitioning = false;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (isTransitioning) return;

        if (other.CompareTag("Player"))
        {
            isTransitioning = true;
            Debug.Log("[��Ż] �÷��̾� ������ �� �� ��ȯ ����");
            StartCoroutine(TransitionScene());
        }
    }

    private IEnumerator TransitionScene()
    {
        // �г��� ��Ȱ��ȭ�Ǿ� �ִٸ� �ѱ�
        if (leftPanel != null) leftPanel.gameObject.SetActive(true);
        if (rightPanel != null) rightPanel.gameObject.SetActive(true);

        // ���� �ִϸ��̼� ����
        leftPanel?.DOComplete(); leftPanel?.DOKill();
        rightPanel?.DOComplete(); rightPanel?.DOKill();

        // ���� ��ġ: ȭ�� �ٱ���
        leftPanel.anchoredPosition = new Vector2(-Screen.width, 0);
        rightPanel.anchoredPosition = new Vector2(Screen.width, 0);

        // �߾����� �����̵� ��
        leftPanel.DOAnchorPos(Vector2.zero, transitionDuration).SetEase(Ease.InOutQuad);
        rightPanel.DOAnchorPos(Vector2.zero, transitionDuration).SetEase(Ease.InOutQuad);

        yield return new WaitForSeconds(transitionDuration);

        DOTween.KillAll();
        SceneManager.LoadScene(nextSceneName);
    }
}
