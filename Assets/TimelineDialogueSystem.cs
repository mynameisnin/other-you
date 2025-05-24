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

    public PlayableDirector timeline; // 타임라인 컨트롤
    private int index = 0;
    private bool isTyping = false;
    public float typingSpeed = 0.05f;
    public bool isDialogueActive = false;

    [Header("사운드 설정")]
    public AudioSource bgmAudioSource;
    public AudioClip specialBgmClip;

    private void Start()
    {
        dialoguePanel.SetActive(false);
    }

    public void StartDialogue()
    {
        if (dialogues.Length == 0) return;

        index = 0;
        dialoguePanel.SetActive(true);
        timeline.Pause(); // 타임라인 일시 정지
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

            // 3번째 대사에서 사운드 재생
            if (index == 2)
            {
                PlaySoundAtThirdDialogue();
            }

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

        // 타임라인 재개
        timeline.time = timeline.time;
        timeline.Play();
    }

    private void PlaySoundAtThirdDialogue()
    {
        if (bgmAudioSource != null && specialBgmClip != null)
        {
            bgmAudioSource.clip = specialBgmClip;
            bgmAudioSource.Play();
            Debug.Log("3번째 대사에서 음악 재생됨");
        }
    }
}
