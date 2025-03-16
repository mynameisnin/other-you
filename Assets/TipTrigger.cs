using System.Collections;
using UnityEngine;
using DG.Tweening; // DOTween ���

public class TipTrigger : MonoBehaviour
{
    public GameObject[] tipPanels; // ���� ���� �� �г��� ����
    private int currentPanelIndex = 0; // ���� ǥ�� ���� �г� �ε���
    private CanvasGroup currentCanvasGroup; // ���� �г��� CanvasGroup
    private Vector3 originalPosition; // �ʱ� ��ġ ����
    public float fadeDuration = 0.5f; // ���̵� ���� �ð�
    public float moveDistance = 100f; // �̵� �Ÿ� (�г� �� �̵�)

    private bool isTipActive = false; // ���� Ȱ��ȭ�Ǿ����� üũ

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
                canvas.alpha = 0; // ��� �г��� �����ϰ� ����
                panel.SetActive(false); // ��Ȱ��ȭ
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && !isTipActive)
        {
            ShowTip(0); // ù ��° �г��� ǥ��
        }
    }

    private void Update()
    {
        if (isTipActive)
        {
            // Enter, ESC, �� Ű�� ������ �� �ݱ�
            if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.UpArrow))
            {
                HideTip();
            }

            // �� Ű�� ������ ���� �гη� �̵�
            if (Input.GetKeyDown(KeyCode.RightArrow))
            {
                ShowNextTip();
            }

            // �� Ű�� ������ ���� �гη� �̵�
            if (Input.GetKeyDown(KeyCode.LeftArrow))
            {
                ShowPreviousTip();
            }
        }
    }

    void ShowTip(int index)
    {
        if (index < 0 || index >= tipPanels.Length) return; // ��ȿ�� �ε������� Ȯ��

        isTipActive = true;
        currentPanelIndex = index;
        GameObject panel = tipPanels[index];
        currentCanvasGroup = panel.GetComponent<CanvasGroup>();

        panel.SetActive(true);
        panel.transform.localPosition = new Vector3(moveDistance, panel.transform.localPosition.y, 0); // �����ʿ��� ����

        // �����ʿ��� �����ϸ� ���̵� ��
        panel.transform.DOLocalMoveX(0, fadeDuration).SetEase(Ease.OutQuad).SetUpdate(true);
        currentCanvasGroup.DOFade(1, fadeDuration).SetUpdate(true);

        // ���� �Ͻ� ����
        Time.timeScale = 0;
    }

    void ShowNextTip()
    {
        if (currentPanelIndex >= tipPanels.Length - 1) return; // ������ �г��̸� �������� ����

        GameObject currentPanel = tipPanels[currentPanelIndex];
        CanvasGroup currentCanvas = currentPanel.GetComponent<CanvasGroup>();

        GameObject nextPanel = tipPanels[currentPanelIndex + 1];
        CanvasGroup nextCanvas = nextPanel.GetComponent<CanvasGroup>();

        float exitDistance = moveDistance * 1.5f; 
        float enterDistance = moveDistance * 1.5f; 

        // ���� �г��� �������� �̵��ϸ� ���̵� �ƿ�
        currentPanel.transform.DOLocalMoveX(-exitDistance, fadeDuration).SetEase(Ease.InQuad).SetUpdate(true);
        currentCanvas.DOFade(0, fadeDuration).SetUpdate(true).OnComplete(() =>
        {
            currentPanel.SetActive(false); // ���� �г� ����
        });

        // ���� �г��� �����ʿ��� �����ϸ� ���̵� ��
        nextPanel.SetActive(true);
        nextPanel.transform.localPosition = new Vector3(enterDistance, nextPanel.transform.localPosition.y, 0);
        nextPanel.transform.DOLocalMoveX(0, fadeDuration).SetEase(Ease.OutQuad).SetUpdate(true);
        nextCanvas.DOFade(1, fadeDuration).SetUpdate(true);

        // ���� �г� �ε����� ������Ʈ
        currentPanelIndex++;
    }

    void ShowPreviousTip()
    {
        if (currentPanelIndex <= 0) return; // ù ��° �г��̸� �������� ����

        GameObject currentPanel = tipPanels[currentPanelIndex];
        CanvasGroup currentCanvas = currentPanel.GetComponent<CanvasGroup>();

        GameObject previousPanel = tipPanels[currentPanelIndex - 1];
        CanvasGroup previousCanvas = previousPanel.GetComponent<CanvasGroup>();

        float exitDistance = moveDistance * 1.5f;
        float enterDistance = moveDistance * 1.5f;

        // ���� �г��� ���������� �̵��ϸ� ���̵� �ƿ�
        currentPanel.transform.DOLocalMoveX(exitDistance, fadeDuration).SetEase(Ease.InQuad).SetUpdate(true);
        currentCanvas.DOFade(0, fadeDuration).SetUpdate(true).OnComplete(() =>
        {
            currentPanel.SetActive(false); // ���� �г� ����
        });

        // ���� �г��� ���ʿ��� �����ϸ� ���̵� ��
        previousPanel.SetActive(true);
        previousPanel.transform.localPosition = new Vector3(-enterDistance, previousPanel.transform.localPosition.y, 0);
        previousPanel.transform.DOLocalMoveX(0, fadeDuration).SetEase(Ease.OutQuad).SetUpdate(true);
        previousCanvas.DOFade(1, fadeDuration).SetUpdate(true);

        // ���� �г� �ε����� ������Ʈ
        currentPanelIndex--;
    }
    public void HideTip()
    {
        if (!isTipActive) return;
        isTipActive = false;

        GameObject currentPanel = tipPanels[currentPanelIndex];
        CanvasGroup currentCanvas = currentPanel.GetComponent<CanvasGroup>();

        // ���� �г��� �Ʒ��� �̵��ϸ� ���̵� �ƿ�
        currentPanel.transform.DOLocalMoveY(-moveDistance, fadeDuration).SetUpdate(true);
        currentCanvas.DOFade(0, fadeDuration).SetUpdate(true).OnComplete(() =>
        {
            currentPanel.SetActive(false); // ������ ���������� ��Ȱ��ȭ
            Time.timeScale = 1; // ���� �ٽ� ����
            Destroy(gameObject); // Ʈ���� ������Ʈ ����
        });
    }
}
