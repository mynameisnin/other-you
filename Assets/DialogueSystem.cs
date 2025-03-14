using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

[System.Serializable]
public class Dialogue
{
    [TextArea] public string dialogueText;
    public Sprite characterImage;
    public string characterName;
}

public class DialogueSystem : MonoBehaviour
{
    public GameObject DialogPanel;
    public TMP_Text dialogText;
    public Image characterImage;
    public TMP_Text characterNameText;
    public Dialogue[] dialog;

    private int index;
    public GameObject continueButton;
    public float wordSpeed;
    public bool playerIsClose;

    private bool isTyping = false;
    private bool hasTalked = false;
    public CamZoomSystem CameraZoom;

    public GameObject player;
    public AdamMovement adamMovement;
    public VillageMovement villageMovement;

    // NPC 머리 위에 표시될 아이콘
    public GameObject exclamationIcon;
    public GameObject talkingIcon;
    public Transform iconPosition; // 아이콘 위치 (NPC 머리 위)

    // 플레이어 머리 위에 표시될 아이콘
    public GameObject playerIcon;

    public Collider2D dialogueCollider;

    void Start()
    {
        UpdateIcon();
        if (playerIcon != null)
        {
            playerIcon.SetActive(false);
        }

        // 아이콘이 처음부터 NPC 머리 위에 표시되도록 초기화
        if (exclamationIcon != null)
        {
            exclamationIcon.SetActive(true);
        }

        if (talkingIcon != null)
        {
            talkingIcon.SetActive(false);
        }
    }

    void Update()
    {
        UpdateIconPosition(); // 아이콘 위치 업데이트

        if (Input.GetKeyDown(KeyCode.UpArrow) && playerIsClose)
        {
            if (DialogPanel.activeInHierarchy)
            {
                if (isTyping)
                {
                    CompleteTyping();
                }
                else
                {
                    NextLine();
                }
            }
            else
            {
                DialogPanel.SetActive(true);
                CameraZoom.ZoomIn();
                hasTalked = true; // 대화가 시작되었음을 표시
                UpdateIcon();
                StartCoroutine(Typing());
                DisablePlayerMovement();
            }
        }
    }

    void DisablePlayerMovement()
    {
        if (adamMovement != null)
        {
            adamMovement.enabled = false;
        }

        if (villageMovement != null)
        {
            villageMovement.enabled = false;
        }

        Rigidbody2D rb = player.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.velocity = Vector2.zero; // 모든 이동 정지
        }

        Animator animator = player.GetComponent<Animator>();
        if (animator != null)
        {
            animator.SetBool("run", false);
            animator.ResetTrigger("Jump");
            animator.SetBool("Fall", false);
        }
    }


    void EnablePlayerMovement()
    {
        if (adamMovement != null) // villageMovement와 상관없이 adamMovement를 활성화해야 함
        {
            adamMovement.enabled = true;
        }

        if (villageMovement != null) // villageMovement도 존재하면 활성화
        {
            villageMovement.enabled = true;
        }
    }

    void UpdateIcon()
    {
        // 플레이어 머리 위 아이콘 설정
        if (playerIcon != null)
        {
            playerIcon.SetActive(playerIsClose && !hasTalked); // 대화를 끝낸 후에는 아이콘 표시 안 함
        }

        // NPC 머리 위 아이콘 설정
        if (!hasTalked && !isTyping)
        {
            if (exclamationIcon != null)
            {
                exclamationIcon.SetActive(true);
            }
            if (talkingIcon != null)
            {
                talkingIcon.SetActive(false);
            }
        }
        else if (isTyping)
        {
            if (exclamationIcon != null)
            {
                exclamationIcon.SetActive(false);
            }
            if (talkingIcon != null)
            {
                talkingIcon.SetActive(true);
            }
        }
        else
        {
            if (exclamationIcon != null)
            {
                exclamationIcon.SetActive(false);
            }
            if (talkingIcon != null)
            {
                talkingIcon.SetActive(false);
            }
        }
    }

    void UpdateIconPosition()
    {
        if (iconPosition != null)
        {
            if (exclamationIcon != null)
                exclamationIcon.transform.position = iconPosition.position;

            if (talkingIcon != null)
                talkingIcon.transform.position = iconPosition.position;
        }
    }

    public void zeroText()
    {
        dialogText.text = "";
        characterNameText.text = "";
        index = 0;
        isTyping = false;
        DialogPanel.SetActive(false);

        if (CameraZoom != null)
        {
            CameraZoom.ZoomOut();
        }

        EnablePlayerMovement();

        // 다이얼로그가 끝나면 콜라이더 비활성화 및 아이콘 숨기기
        if (dialogueCollider != null)
        {
            dialogueCollider.enabled = false;
        }

        UpdateIcon(); // 다이얼로그 종료 시 아이콘 업데이트
    }

    private IEnumerator Typing()
    {
        if (index < 0 || index >= dialog.Length)
        {
            yield break;
        }

        isTyping = true;
        dialogText.text = "";

        if (dialog[index].characterImage != null)
        {
            characterImage.sprite = dialog[index].characterImage;
        }

        if (!string.IsNullOrEmpty(dialog[index].characterName))
        {
            characterNameText.text = dialog[index].characterName;
        }

        UpdateIcon();

        foreach (char letter in dialog[index].dialogueText.ToCharArray())
        {
            dialogText.text += letter;
            yield return new WaitForSeconds(wordSpeed);
        }

        isTyping = false;
        if (continueButton != null)
        {
            continueButton.SetActive(true);
        }

        UpdateIcon();
    }

    private void CompleteTyping()
    {
        StopAllCoroutines();

        if (index >= 0 && index < dialog.Length)
        {
            dialogText.text = dialog[index].dialogueText;
        }

        isTyping = false;

        if (continueButton != null)
        {
            continueButton.SetActive(true);
        }

        UpdateIcon();
    }

    public void NextLine()
    {
        if (continueButton != null)
        {
            continueButton.SetActive(false);
        }

        if (index < dialog.Length - 1)
        {
            index++;
            StopAllCoroutines();
            StartCoroutine(Typing());
        }
        else
        {
            zeroText();
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerIsClose = true;
            UpdateIcon(); // 아이콘 표시
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerIsClose = false;
            UpdateIcon(); // 아이콘 숨김
        }
    }
}
