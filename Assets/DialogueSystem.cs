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
    public CameraSystem CameraZoom;          // CameraZoom ��ũ��Ʈ�� ����

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
                StartCoroutine(Typing());
            }
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
