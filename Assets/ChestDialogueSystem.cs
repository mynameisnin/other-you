using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class ChestDialogueSystem : MonoBehaviour
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

    public GameObject exclamationIcon;
    public GameObject talkingIcon;
    public Transform iconPosition;
    public GameObject playerIcon;

    public Collider2D dialogueCollider;

    void Start()
    {
        // 자동 할당
        if (player == null)
        {
            player = GameObject.FindWithTag("Player");
            Debug.Log("[DialogueSystem] player 자동 할당됨: " + (player != null));
        }

        if (adamMovement == null && player != null)
        {
            adamMovement = player.GetComponent<AdamMovement>();
            Debug.Log("[DialogueSystem] adamMovement 자동 할당됨: " + (adamMovement != null));
        }

        if (villageMovement == null && player != null)
        {
            villageMovement = player.GetComponent<VillageMovement>();
            Debug.Log("[DialogueSystem] villageMovement 자동 할당됨: " + (villageMovement != null));
        }

        UpdateIcon();

        if (playerIcon != null)
            playerIcon.SetActive(false);

        if (exclamationIcon != null)
            exclamationIcon.SetActive(true);

        if (talkingIcon != null)
            talkingIcon.SetActive(false);

        RandomTest();
    }

    void Update()
    {
        UpdateIconPosition();

        if (Input.GetKeyDown(KeyCode.UpArrow) && playerIsClose)
        {
            if (DialogPanel.activeInHierarchy)
            {
                if (isTyping)
                    CompleteTyping();
                else
                    NextLine();
            }
            else
            {
                DialogPanel.SetActive(true);
                CameraZoom.ZoomIn();
                hasTalked = true;
                UpdateIcon();
                RandomDialogue();
                StartCoroutine(Typing());
                DisablePlayerMovement();
            }
        }
    }

    void DisablePlayerMovement()
    {
        if (adamMovement != null) adamMovement.enabled = false;
        if (villageMovement != null) villageMovement.enabled = false;

        if (player != null)
        {
            Rigidbody2D rb = player.GetComponent<Rigidbody2D>();
            if (rb != null) rb.velocity = Vector2.zero;

            Animator animator = player.GetComponent<Animator>();
            if (animator != null)
            {
                animator.SetBool("run", false);
                animator.ResetTrigger("Jump");
                animator.SetBool("Fall", false);
            }
        }
    }

    void EnablePlayerMovement()
    {
        if (adamMovement != null) adamMovement.enabled = true;
        if (villageMovement != null) villageMovement.enabled = true;
    }

    void UpdateIcon()
    {
        if (playerIcon != null)
            playerIcon.SetActive(playerIsClose && !hasTalked);

        if (!hasTalked && !isTyping)
        {
            if (exclamationIcon != null) exclamationIcon.SetActive(true);
            if (talkingIcon != null) talkingIcon.SetActive(false);
        }
        else if (isTyping)
        {
            if (exclamationIcon != null) exclamationIcon.SetActive(false);
            if (talkingIcon != null) talkingIcon.SetActive(true);
        }
        else
        {
            if (exclamationIcon != null) exclamationIcon.SetActive(false);
            if (talkingIcon != null) talkingIcon.SetActive(false);
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
        isTyping = false;
        DialogPanel.SetActive(false);

        if (CameraZoom != null)
            CameraZoom.ZoomOut();

        EnablePlayerMovement();

        if (dialogueCollider != null)
            dialogueCollider.enabled = false;

        UpdateIcon();
    }

    private IEnumerator Typing()
    {
        if (index < 0 || index >= dialog.Length)
            yield break;

        isTyping = true;
        dialogText.text = "";

        if (dialog[index].characterImage != null)
            characterImage.sprite = dialog[index].characterImage;

        if (!string.IsNullOrEmpty(dialog[index].characterName))
            characterNameText.text = dialog[index].characterName;

        UpdateIcon();

        foreach (char letter in dialog[index].dialogueText.ToCharArray())
        {
            dialogText.text += letter;
            yield return new WaitForSeconds(wordSpeed);
        }

        isTyping = false;
        if (continueButton != null)
            continueButton.SetActive(true);

        UpdateIcon();
    }

    private void CompleteTyping()
    {
        StopAllCoroutines();

        if (index >= 0 && index < dialog.Length)
            dialogText.text = dialog[index].dialogueText;

        isTyping = false;

        if (continueButton != null)
            continueButton.SetActive(true);

        UpdateIcon();
    }

    public void NextLine()
    {
        if (continueButton != null)
            continueButton.SetActive(false);

        zeroText();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerIsClose = true;
            UpdateIcon();
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerIsClose = false;
            UpdateIcon();
        }
    }

    /// <summary>
    /// 확률 분포: 빨간티켓 60%, 파란티켓 30%, 노란티켓 10%
    /// </summary>
    private void RandomDialogue()
    {
        float r = Random.Range(0f, 1f);

        if (r < 0.6f)
            index = 0; // 빨간티켓
        else if (r < 0.9f)
            index = 1; // 파란티켓
        else
            index = 2; // 노란티켓
    }

    private void RandomTest()
    {
        int num1 = 0, num2 = 0, num3 = 0;
        for (int i = 0; i < 100000; i++)
        {
            RandomDialogue();
            if (index == 0) num1++;
            if (index == 1) num2++;
            if (index == 2) num3++;
        }
        Debug.Log("빨간티켓 :" + num1 + " 파란티켓 :" + num2 + " 노란티켓 :" + num3);
    }
}
