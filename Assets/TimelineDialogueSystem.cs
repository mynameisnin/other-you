using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.Playables; // 타임라인을 위한 네임스페이스

[System.Serializable]
public class TimelineDialogue
{
    [TextArea] public string dialogueText; // 대사 내용
    public Sprite characterImage; // 캐릭터 초상화
    public string characterName; // 캐릭터 이름
}

public class TimelineDialogueSystem : MonoBehaviour
{
    public GameObject dialoguePanel; // 다이얼로그 패널
    public TMP_Text dialogueText; // 대사 텍스트
    public Image characterImage; // 캐릭터 초상화
    public TMP_Text characterNameText; // 캐릭터 이름 텍스트
    public TimelineDialogue[] dialogues; // 대사 배열

    private int index = 0;
    private bool isTyping = false;//dsad

    public float typingSpeed = 0.05f; // 타이핑 속도

    private void Start()
    {
        dialoguePanel.SetActive(false); // 초기 상태에서 비활성화
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
        if (Input.GetKeyDown(KeyCode.UpArrow)) // 위쪽 화살표 키 입력
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

        // 캐릭터 초상화 & 이름 업데이트
        if (dialogues[index].characterImage != null)
        {
            characterImage.sprite = dialogues[index].characterImage;
        }
        characterNameText.text = dialogues[index].characterName;

        // 한 글자씩 출력
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
