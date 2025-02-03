using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

// ��ȭ �����͸� �����ϴ� Ŭ����
[System.Serializable]
public class Dialogue
{
    [TextArea] public string dialogueText; // ��ȭ ����
    public Sprite characterImage;         // ĳ���� �ʻ�ȭ �̹���
    public string characterName;          // ĳ���� �̸�
}

public class DialogueSystem : MonoBehaviour
{
    public GameObject DialogPanel;           // ��ȭâ ������Ʈ
    public TMP_Text dialogText;              // ��ȭ �ؽ�Ʈ ������Ʈ
    public Image characterImage;             // ĳ���� �ʻ�ȭ �̹��� ������Ʈ
    public TMP_Text characterNameText;       // ĳ���� �̸� �ؽ�Ʈ ������Ʈ
    public Dialogue[] dialog;                // ��ȭ �迭 (�ؽ�Ʈ�� �̹��� ����)

    private int index;                       // ���� ��ȭ �ε���

    public GameObject continueButton;        // "���" ��ư ������Ʈ
    public float wordSpeed;                  // ���� Ÿ���� �ӵ�
    public bool playerIsClose;               // �÷��̾���� �Ÿ� Ȯ�� ����

    private bool isTyping = false;           // ���� Ÿ���� ������ Ȯ��
    private bool hasTalked = false;          // NPC�� �� ���̶� ��ȭ�ߴ��� ����
    public CamZoomSystem CameraZoom;         // CameraZoom ��ũ��Ʈ�� ����

    //�߰�: ��ȭ ���� ������ (����ǥ, ���ϴ� ��)
    public GameObject exclamationIcon;       // "!" ������ (ó�� ��ȭ ��)
    public GameObject talkingIcon;           // "..." ������ (��ȭ ��)
    public Transform iconPosition;           // ������ ��ġ�� ������ Transform (NPC �Ӹ� ��)

    void Start()
    {
        UpdateIcon(); // ó�� ���� ����
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.UpArrow) && playerIsClose)
        {
            if (DialogPanel.activeInHierarchy)
            {
                if (isTyping)
                {
                    Debug.Log("CompleteTyping ȣ��");
                    CompleteTyping();
                }
                else
                {
                    Debug.Log("NextLine ȣ��");
                    NextLine();
                }
            }
            else
            {
                Debug.Log("Camera Zoom In ȣ��");
                DialogPanel.SetActive(true);
                CameraZoom.ZoomIn(); // ī�޶� Ȯ��
                hasTalked = true; // ù ��ȭ �� "!" ������ ����
                UpdateIcon(); // ������ ������Ʈ
                StartCoroutine(Typing());
            }
        }

        UpdateIconPosition(); //������ ��ġ ������Ʈ
    }

    //�߰�: ������ ���� ������Ʈ
    void UpdateIcon()
    {
        if (!hasTalked && !isTyping)
        {
            // ��ȭ�� �� ���� ����, ��ȭ ���� �ƴ� �� "!" ǥ��
            exclamationIcon.SetActive(true);
            talkingIcon.SetActive(false);
        }
        else if (isTyping)
        {
            // ��ȭ ���̸� "..." ǥ��
            exclamationIcon.SetActive(false);
            talkingIcon.SetActive(true);
        }
        else
        {
            // ��ȭ�� �������� ������ ����
            exclamationIcon.SetActive(false);
            talkingIcon.SetActive(false);
        }
    }

    //�߰�: ������ ��ġ�� NPC �Ӹ� ���� ����
    void UpdateIconPosition()
    {
        if (iconPosition != null)
        {
            exclamationIcon.transform.position = iconPosition.position;
            talkingIcon.transform.position = iconPosition.position;
        }
    }

    // ��ȭ �ʱ�ȭ �޼���
    public void zeroText()
    {
        dialogText.text = "";
        characterNameText.text = ""; // �̸� �ؽ�Ʈ �ʱ�ȭ
        index = 0;
        isTyping = false;
        DialogPanel.SetActive(false);

        if (CameraZoom != null)
        {
            Debug.Log("Camera Zoom Out ȣ��");
            CameraZoom.ZoomOut(); // ī�޶� ����
        }
        else
        {
            Debug.LogWarning("CameraSystem�� null�Դϴ�.");
        }

        UpdateIcon(); //�߰�: ������ ������Ʈ
    }

    // Ÿ���� �ڷ�ƾ (�� ���ھ� ���)
    IEnumerator Typing()
    {
        if (index < 0 || index >= dialog.Length)
        {
            Debug.LogWarning("��ȭ �ε����� ��ȿ���� �ʽ��ϴ�.");
            yield break;
        }

        isTyping = true;
        dialogText.text = "";

        // ���� ��� �ؽ�Ʈ�� �̹���, �̸� ����
        if (dialog[index].characterImage != null)
        {
            characterImage.sprite = dialog[index].characterImage;
        }

        if (!string.IsNullOrEmpty(dialog[index].characterName))
        {
            characterNameText.text = dialog[index].characterName;
        }

        UpdateIcon(); //�߰�: ������ ������Ʈ (��ȭ �� ����)

        foreach (char letter in dialog[index].dialogueText.ToCharArray())
        {
            dialogText.text += letter;
            yield return new WaitForSeconds(wordSpeed);
        }

        isTyping = false;
        if (continueButton != null)
        {
            continueButton.SetActive(true); // Ÿ���� �Ϸ� �� ��ư Ȱ��ȭ
        }

        UpdateIcon(); //�߰�: ������ ������Ʈ (��ȭ �Ϸ� ����)
    }

    // ���� ��縦 ��� �Ϸ��ϴ� �޼���
    private void CompleteTyping()
    {
        StopAllCoroutines(); // ��� �ڷ�ƾ ����

        if (index >= 0 && index < dialog.Length)
        {
            dialogText.text = dialog[index].dialogueText;
        }

        isTyping = false;

        if (continueButton != null)
        {
            continueButton.SetActive(true);
        }

        UpdateIcon(); //�߰�: ������ ������Ʈ
    }

    // ���� ���� �Ѿ�� �޼���
    public void NextLine()
    {
        if (continueButton != null)
        {
            continueButton.SetActive(false);
        }

        if (index < dialog.Length - 1)
        {
            index++;
            StopAllCoroutines(); // ���� �ڷ�ƾ ����
            StartCoroutine(Typing());
        }
        else
        {
            zeroText();
        }
    }

    // �÷��̾ Ʈ���� ������ ������ �� ȣ��
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("PlayerAttack"))
        {
            playerIsClose = true;
        }
    }

    // �÷��̾ Ʈ���� ������ ����� �� ȣ��
    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("PlayerAttack"))
        {
            playerIsClose = false;
            zeroText();
        }
    }
}
