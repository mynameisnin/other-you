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

    // NPC �Ӹ� ���� ǥ�õ� ������
    public GameObject exclamationIcon;
    public GameObject talkingIcon;
    public Transform iconPosition; // ������ ��ġ (NPC �Ӹ� ��)

    // �÷��̾� �Ӹ� ���� ǥ�õ� ������
    public GameObject playerIcon;

    public Collider2D dialogueCollider;

    void Start()
    {
        Debug.Log("[DialogueSystem] villageMovement ���� ����: " + (villageMovement != null));
        UpdateIcon();
        if (playerIcon != null)
        {
            playerIcon.SetActive(false);
        }

        // �������� ó������ NPC �Ӹ� ���� ǥ�õǵ��� �ʱ�ȭ
        if (exclamationIcon != null)
        {
            exclamationIcon.SetActive(true);
        }

        if (talkingIcon != null)
        {
            talkingIcon.SetActive(false);
        }

        RandomTest();
    }

    void Update()
    {
        UpdateIconPosition(); // ������ ��ġ ������Ʈ

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
                hasTalked = true; // ��ȭ�� ���۵Ǿ����� ǥ��
                UpdateIcon();
                RandomDialogue();
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
            rb.velocity = Vector2.zero; // ��� �̵� ����
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
        if (adamMovement != null) // villageMovement�� ������� adamMovement�� Ȱ��ȭ�ؾ� ��
        {
            adamMovement.enabled = true;
        }

        if (villageMovement != null) // villageMovement�� �����ϸ� Ȱ��ȭ
        {
            villageMovement.enabled = true;
        }
    }

    void UpdateIcon()
    {
        // �÷��̾� �Ӹ� �� ������ ����
        if (playerIcon != null)
        {
            playerIcon.SetActive(playerIsClose && !hasTalked); // ��ȭ�� ���� �Ŀ��� ������ ǥ�� �� ��
        }

        // NPC �Ӹ� �� ������ ����
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
        isTyping = false;
        DialogPanel.SetActive(false);

        if (CameraZoom != null)
        {
            CameraZoom.ZoomOut();
        }

        EnablePlayerMovement();

        // ���̾�αװ� ������ �ݶ��̴� ��Ȱ��ȭ �� ������ �����
        if (dialogueCollider != null)
        {
            dialogueCollider.enabled = false;
        }

        UpdateIcon(); // ���̾�α� ���� �� ������ ������Ʈ
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
        zeroText();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerIsClose = true;
            UpdateIcon(); // ������ ǥ��
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerIsClose = false;
            UpdateIcon(); // ������ ����
        }
    }

    private void RandomDialogue()
    {
        int i = Random.Range(0, 9999);

        if (i < 1000)
        {
            index = 0;
        }
        else if (i < 4000)
        {
            index = 1;
        }
        else
        {
            index = 2;
        }
    }

    private void RandomTest()
    {
        int num1 = 0, num2 = 0, num3 = 0;
        for (int i = 0; i < 100000; i++)
        {
            RandomDialogue();
            if (index == 0)
            {
                num1++;
            }
            if (index == 1)
            {
                num2++;
            }
            if (index == 2)
            {
                num3++;
            }
        }
        Debug.Log("����Ƽ�� :" + num1 + "�Ķ�Ƽ�� :" + num2 + "���Ƽ�� :" + num3);
    }
}
