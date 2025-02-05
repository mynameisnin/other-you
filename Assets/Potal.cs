using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Potal : MonoBehaviour
{
    public List<Collider2D> npcColliders; // ��� NPC�� �ݶ��̴� ����Ʈ (Inspector���� NPC �߰�)
    public string targetScene; // �̵��� �� �̸�
    private bool isPlayerNear = false; // �÷��̾ ��Ż ���� ���� �ִ��� Ȯ��
    private FakeLoadingScreen fakeLoadingScreen; // FakeLoadingScreen ����

    private bool allNpcTalked = false; // ��� ��ȭ �Ϸ� ���� üũ 
    public GameObject warningPanel; // ���޽��� ����ϴ� ĵ���� 
    private CanvasGroup warningCanvasGroup; // CanvasGroup ����
    private bool isWarningActive = false; // ��� ĵ���� Ȱ��ȭ ���� (��Ÿ ����)

    void Start()
    {
        fakeLoadingScreen = FindObjectOfType<FakeLoadingScreen>(); // FakeLoadingScreen ã��

        if (warningPanel != null)
        {
            warningCanvasGroup = warningPanel.GetComponent<CanvasGroup>();

            if (warningCanvasGroup == null)
            {
                warningCanvasGroup = warningPanel.AddComponent<CanvasGroup>(); // ������ �߰�
            }

            warningCanvasGroup.alpha = 0f; // ó���� ������ �ʰ� ����
            warningPanel.SetActive(false);
        }
    }

    void Update()
    {
        CheckAllNpcTalked(); // NPC ��ȭ �Ϸ� ���� üũ

        // �÷��̾ ��Ż ���� �ȿ� �ְ�, ���� ����Ű�� ������ ��
        if (isPlayerNear && Input.GetKeyDown(KeyCode.UpArrow))
        {
            if (allNpcTalked)
            {
                if (fakeLoadingScreen != null)
                {
                    fakeLoadingScreen.LoadScene(targetScene); // ��¥ �ε� ����
                    Debug.Log("��¥ �ε� ���� ��...");
                }
                else
                {
                    SceneManager.LoadScene(targetScene); // ��¥ �ε��� ������ �ٷ� �� ��ȯ
                    Debug.Log("�ٷ� �� ��ȯ!");
                }
            }
            else
            {
                if (!isWarningActive) // ��� ĵ������ �̹� Ȱ��ȭ�� ��� �ߺ� ȣ�� ����
                {
                    ShowWarningCanvas();
                }
            }
        }
    }

    void CheckAllNpcTalked()
    {
        foreach (Collider2D npc in npcColliders)
        {
            if (npc.enabled) // �ϳ��� Ȱ��ȭ�� �ݶ��̴��� ������ ���� ��ȭ �� �� NPC�� ����
            {
                allNpcTalked = false;
                return;
            }
        }

        // ��� NPC�� �ݶ��̴��� ��Ȱ��ȭ�Ǿ��� ���� true
        allNpcTalked = true;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("PlayerAttack"))
        {
            isPlayerNear = true; // �÷��̾ ��Ż ���� ���� ������ üũ
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("PlayerAttack"))
        {
            isPlayerNear = false; //  ��Ż ������ ����� false
        }
    }

    void ShowWarningCanvas()
    {
        if (warningPanel != null)
        {
            isWarningActive = true; // ��� ĵ���� Ȱ��ȭ (��Ÿ ����)
            warningPanel.SetActive(true);
            StartCoroutine(FadeCanvasGroup(warningCanvasGroup, 0f, 1f, 0.5f)); // 0.5�� ���� ��Ÿ����
            Debug.Log("��� ĵ���� ǥ��");
            StartCoroutine(HideWarningCanvas()); // 2�� �� �ڵ����� ����
        }
    }

    IEnumerator HideWarningCanvas()
    {
        yield return new WaitForSeconds(2f);
        StartCoroutine(FadeCanvasGroup(warningCanvasGroup, 1f, 0f, 0.5f)); // 0.5�� ���� �������
        yield return new WaitForSeconds(0.5f);
        warningPanel.SetActive(false); // ������ ����� �� ��Ȱ��ȭ
        isWarningActive = false; // ��� ĵ���� ���� �� �ٽ� ȣ�� ����
    }

    IEnumerator FadeCanvasGroup(CanvasGroup canvasGroup, float startAlpha, float endAlpha, float duration)
    {
        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(startAlpha, endAlpha, elapsed / duration);
            yield return null;
        }
        canvasGroup.alpha = endAlpha;
    }
}
