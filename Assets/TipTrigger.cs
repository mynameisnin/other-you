using System.Collections;
using UnityEngine;
using DG.Tweening; // DOTween ���

public class TipTrigger : MonoBehaviour
{
    public GameObject tipPanel; // �� UI �г�
    private CanvasGroup canvasGroup; // ���̵� ȿ���� ���� CanvasGroup
    private Vector3 originalPosition; // �ʱ� ��ġ ����
    public float fadeDuration = 0.5f; // ���̵� ���� �ð�
    public float moveDistance = 50f; // �ö󰡴� �Ÿ�
    private bool isTipActive = false; // ���� ���� Ȱ��ȭ�Ǿ����� üũ

    private void Start()
    {
        if (tipPanel != null)
        {
            canvasGroup = tipPanel.GetComponent<CanvasGroup>();
            if (canvasGroup == null)
            {
                canvasGroup = tipPanel.AddComponent<CanvasGroup>();
            }

            originalPosition = tipPanel.transform.localPosition; // �ʱ� ��ġ ����
            tipPanel.SetActive(false); // ó������ ��Ȱ��ȭ
            canvasGroup.alpha = 0; // �����ϰ� ����
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
            // Enter, ESC, �� Ű�� ������ ����
            if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.UpArrow))
            {
                HideTip();
            }
        }
    }

    void ShowTip()
    {
        isTipActive = true;
        tipPanel.SetActive(true); // �� �г� Ȱ��ȭ
        tipPanel.transform.localPosition = originalPosition - new Vector3(0, moveDistance, 0); // �Ʒ����� ����

        // UI �ִϸ��̼�: ���� �̵� + ���̵� �� (SetUpdate(true) �߰�)
        tipPanel.transform.DOLocalMoveY(originalPosition.y, fadeDuration).SetUpdate(true);
        canvasGroup.DOFade(1, fadeDuration).SetUpdate(true);

        // ���� �Ͻ� ����
        Time.timeScale = 0;
    }

    public void HideTip()
    {
        if (!isTipActive) return;
        isTipActive = false;

        // UI �ִϸ��̼�: �Ʒ��� �̵� + ���̵� �ƿ� (SetUpdate(true) �߰�)
        tipPanel.transform.DOLocalMoveY(originalPosition.y - moveDistance, fadeDuration).SetUpdate(true);
        canvasGroup.DOFade(0, fadeDuration).SetUpdate(true).OnComplete(() =>
        {
            tipPanel.SetActive(false); // ������ ���������� ��Ȱ��ȭ
            Time.timeScale = 1; // ���� �ٽ� ����

            // Ʈ���� ������Ʈ ���� (�ٽ� ������� �ʵ���)
            Destroy(gameObject);
        });
    }

}
