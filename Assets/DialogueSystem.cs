using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

// 대화 데이터를 저장하는 클래스
[System.Serializable]
public class Dialogue
{
    [TextArea] public string dialogueText; // 대화 내용
    public Sprite characterImage;         // 캐릭터 초상화 이미지
    public string characterName;          // 캐릭터 이름
}

public class DialogueSystem : MonoBehaviour
{
    public GameObject DialogPanel;           // 대화창 오브젝트
    public TMP_Text dialogText;              // 대화 텍스트 컴포넌트
    public Image characterImage;             // 캐릭터 초상화 이미지 컴포넌트
    public TMP_Text characterNameText;       // 캐릭터 이름 텍스트 컴포넌트
    public Dialogue[] dialog;                // 대화 배열 (텍스트와 이미지 포함)

    private int index;                       // 현재 대화 인덱스

    public GameObject continueButton;        // "계속" 버튼 오브젝트
    public float wordSpeed;                  // 글자 타이핑 속도
    public bool playerIsClose;               // 플레이어와의 거리 확인 변수

    private bool isTyping = false;           // 현재 타이핑 중인지 확인
    private bool hasTalked = false;          // NPC와 한 번이라도 대화했는지 여부
    public CamZoomSystem CameraZoom;         // CameraZoom 스크립트를 참조

    //추가: 대화 상태 아이콘 (느낌표, 말하는 중)
    public GameObject exclamationIcon;       // "!" 아이콘 (처음 대화 전)
    public GameObject talkingIcon;           // "..." 아이콘 (대화 중)
    public Transform iconPosition;           // 아이콘 위치를 지정할 Transform (NPC 머리 위)

    void Start()
    {
        UpdateIcon(); // 처음 상태 설정
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.UpArrow) && playerIsClose)
        {
            if (DialogPanel.activeInHierarchy)
            {
                if (isTyping)
                {
                    Debug.Log("CompleteTyping 호출");
                    CompleteTyping();
                }
                else
                {
                    Debug.Log("NextLine 호출");
                    NextLine();
                }
            }
            else
            {
                Debug.Log("Camera Zoom In 호출");
                DialogPanel.SetActive(true);
                CameraZoom.ZoomIn(); // 카메라 확대
                hasTalked = true; // 첫 대화 후 "!" 아이콘 제거
                UpdateIcon(); // 아이콘 업데이트
                StartCoroutine(Typing());
            }
        }

        UpdateIconPosition(); //아이콘 위치 업데이트
    }

    //추가: 아이콘 상태 업데이트
    void UpdateIcon()
    {
        if (!hasTalked && !isTyping)
        {
            // 대화를 한 적이 없고, 대화 중이 아닐 때 "!" 표시
            exclamationIcon.SetActive(true);
            talkingIcon.SetActive(false);
        }
        else if (isTyping)
        {
            // 대화 중이면 "..." 표시
            exclamationIcon.SetActive(false);
            talkingIcon.SetActive(true);
        }
        else
        {
            // 대화가 끝났으면 아이콘 숨김
            exclamationIcon.SetActive(false);
            talkingIcon.SetActive(false);
        }
    }

    //추가: 아이콘 위치를 NPC 머리 위로 고정
    void UpdateIconPosition()
    {
        if (iconPosition != null)
        {
            exclamationIcon.transform.position = iconPosition.position;
            talkingIcon.transform.position = iconPosition.position;
        }
    }

    // 대화 초기화 메서드
    public void zeroText()
    {
        dialogText.text = "";
        characterNameText.text = ""; // 이름 텍스트 초기화
        index = 0;
        isTyping = false;
        DialogPanel.SetActive(false);

        if (CameraZoom != null)
        {
            Debug.Log("Camera Zoom Out 호출");
            CameraZoom.ZoomOut(); // 카메라 복구
        }
        else
        {
            Debug.LogWarning("CameraSystem이 null입니다.");
        }

        UpdateIcon(); //추가: 아이콘 업데이트
    }

    // 타이핑 코루틴 (한 글자씩 출력)
    IEnumerator Typing()
    {
        if (index < 0 || index >= dialog.Length)
        {
            Debug.LogWarning("대화 인덱스가 유효하지 않습니다.");
            yield break;
        }

        isTyping = true;
        dialogText.text = "";

        // 현재 대사 텍스트와 이미지, 이름 설정
        if (dialog[index].characterImage != null)
        {
            characterImage.sprite = dialog[index].characterImage;
        }

        if (!string.IsNullOrEmpty(dialog[index].characterName))
        {
            characterNameText.text = dialog[index].characterName;
        }

        UpdateIcon(); //추가: 아이콘 업데이트 (대화 중 상태)

        foreach (char letter in dialog[index].dialogueText.ToCharArray())
        {
            dialogText.text += letter;
            yield return new WaitForSeconds(wordSpeed);
        }

        isTyping = false;
        if (continueButton != null)
        {
            continueButton.SetActive(true); // 타이핑 완료 후 버튼 활성화
        }

        UpdateIcon(); //추가: 아이콘 업데이트 (대화 완료 상태)
    }

    // 현재 대사를 즉시 완료하는 메서드
    private void CompleteTyping()
    {
        StopAllCoroutines(); // 모든 코루틴 중지

        if (index >= 0 && index < dialog.Length)
        {
            dialogText.text = dialog[index].dialogueText;
        }

        isTyping = false;

        if (continueButton != null)
        {
            continueButton.SetActive(true);
        }

        UpdateIcon(); //추가: 아이콘 업데이트
    }

    // 다음 대사로 넘어가는 메서드
    public void NextLine()
    {
        if (continueButton != null)
        {
            continueButton.SetActive(false);
        }

        if (index < dialog.Length - 1)
        {
            index++;
            StopAllCoroutines(); // 이전 코루틴 중지
            StartCoroutine(Typing());
        }
        else
        {
            zeroText();
        }
    }

    // 플레이어가 트리거 범위에 들어왔을 때 호출
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("PlayerAttack"))
        {
            playerIsClose = true;
        }
    }

    // 플레이어가 트리거 범위를 벗어났을 때 호출
    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("PlayerAttack"))
        {
            playerIsClose = false;
            zeroText();
        }
    }
}
