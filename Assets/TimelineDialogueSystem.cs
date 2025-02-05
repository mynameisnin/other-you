using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.Playables; // Ÿ�Ӷ����� ���� ���ӽ����̽�

[System.Serializable]
public class TimelineDialogue
{
    [TextArea] public string dialogueText; // ��� ����
    public Sprite characterImage; // ĳ���� �ʻ�ȭ
    public string characterName; // ĳ���� �̸�
}

public class TimelineDialogueSystem : MonoBehaviour
{
    public GameObject dialoguePanel; // ���̾�α� �г�
    public TMP_Text dialogueText; // ��� �ؽ�Ʈ
    public Image characterImage; // ĳ���� �ʻ�ȭ
    public TMP_Text characterNameText; // ĳ���� �̸� �ؽ�Ʈ
    public TimelineDialogue[] dialogues; // ��� �迭

    private int index = 0;
    private bool isTyping = false;//dsad

    public float typingSpeed = 0.05f; // Ÿ���� �ӵ�

    private void Start()
    {
        dialoguePanel.SetActive(false); // �ʱ� ���¿��� ��Ȱ��ȭ
    }

    public void StartDialogue()
    {
        if (dialogues.Length == 0) return;

        index = 0;
        dialoguePanel.SetActive(true);
        StartCoroutine(Typing());
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.UpArrow)) // ���� ȭ��ǥ Ű �Է�
        {
            if (isTyping)
            {
                CompleteTyping();
            }
            else
            {
                NextDialogue();
            }
        }
    }

    private IEnumerator Typing()
    {
        isTyping = true;
        dialogueText.text = "";

        // ĳ���� �ʻ�ȭ & �̸� ������Ʈ
        if (dialogues[index].characterImage != null)
        {
            characterImage.sprite = dialogues[index].characterImage;
        }
        characterNameText.text = dialogues[index].characterName;

        // �� ���ھ� ���
        foreach (char letter in dialogues[index].dialogueText.ToCharArray())
        {
            dialogueText.text += letter;
            yield return new WaitForSeconds(typingSpeed);
        }

        isTyping = false;
    }

    private void CompleteTyping()
    {
        StopAllCoroutines();
        dialogueText.text = dialogues[index].dialogueText;
        isTyping = false;
    }

    private void NextDialogue()
    {
        if (index < dialogues.Length - 1)
        {
            index++;
            StartCoroutine(Typing());
        }
        else
        {
            CloseDialogue();
        }
    }

    private void CloseDialogue()
    {
        dialoguePanel.SetActive(false);
        index = 0;
    }
}
