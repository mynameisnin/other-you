using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.Playables;

[System.Serializable]
public class TimelineDialogue
{
    [TextArea] public string dialogueText;
    public Sprite characterImage;
    public string characterName;
}

public class TimelineDialogueSystem : MonoBehaviour
{
    public GameObject dialoguePanel;
    public TMP_Text dialogueText;
    public Image characterImage;
    public TMP_Text characterNameText;
    public TimelineDialogue[] dialogues;

    public PlayableDirector timeline; // Ÿ�Ӷ��� ��Ʈ��
    private int index = 0;
    private bool isTyping = false;
    public float typingSpeed = 0.05f;
    private bool isDialogueActive = false;  // ���� �߰�
    private void Start()
    {
        dialoguePanel.SetActive(false);
    }

    public void StartDialogue()
    {
        if (dialogues.Length == 0) return;

        index = 0;
        dialoguePanel.SetActive(true);
        timeline.Pause(); // Ÿ�Ӷ��� �Ͻ� ����
        StartCoroutine(Typing());
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.UpArrow))
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

        if (dialogues[index].characterImage != null)
        {
            characterImage.sprite = dialogues[index].characterImage;
        }
        characterNameText.text = dialogues[index].characterName;

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
            EndDialogue();
        }
    }

    public void EndDialogue()
    {
        dialoguePanel.SetActive(false);
        index = 0;
        isDialogueActive = false;

        // ���� Ÿ�Ӷ��� ��ġ�� �����ϸ鼭 �ٽ� ����
        timeline.time = timeline.time;
        timeline.Play();
    }

}
