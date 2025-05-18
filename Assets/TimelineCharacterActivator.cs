using UnityEngine;
using System.Collections.Generic; // Dictionary 사용을 위해 (선택적, 지금은 직접 저장)

public class TimelineCharacterActivator : MonoBehaviour
{
    [Header("원본 캐릭터 (타임라인 제어 대상)")]
    public GameObject originalAdam;
    public GameObject originalDeva;
    // 필요하다면 다른 원본 캐릭터들도 추가

    [Header("다이얼로그용 캐릭터 프리팹")]
    public GameObject dialogueAdamPrefab;
    public GameObject dialogueDevaPrefab;
    // 필요하다면 다른 다이얼로그용 캐릭터 프리팹 추가

    // 스폰된 캐릭터 인스턴스 저장용
    private GameObject spawnedDialogueAdam;
    private GameObject spawnedDialogueDeva;

    // 원본 캐릭터의 마지막 위치/회전 저장용 (스폰 시 사용)
    // 이 변수들은 이제 CleanupAfterDialogue에서 사용하지 않으므로, 로컬 변수로 처리해도 됨
    // private Vector3 lastAdamPos;
    // private Quaternion lastAdamRot;
    // private Vector3 lastDevaPos;
    // private Quaternion lastDevaRot;
    // private bool wasAdamOriginallyActive;
    // private bool wasDevaOriginallyActive;


    // 이 함수를 타임라인의 Signal Emitter에서 "다이얼로그 시작" 시점에 호출
    // (TimelineDialogueSystem의 StartDialogue와 같은 시그널을 받도록 설정 가능)
    public void PrepareCharactersForDialogue()
    {
        Debug.Log("TimelineCharacterActivator: PrepareCharactersForDialogue() 호출됨");

        Vector3 adamPos = Vector3.zero;
        Quaternion adamRot = Quaternion.identity;
        bool adamWasActive = false;

        if (originalAdam != null)
        {
            adamWasActive = originalAdam.activeSelf;
            if (adamWasActive)
            {
                adamPos = originalAdam.transform.position;
                adamRot = originalAdam.transform.rotation;
                originalAdam.SetActive(false);
                Debug.Log($"Original Adam deactivated. Pos: {adamPos}");
            }
        }

        Vector3 devaPos = Vector3.zero;
        Quaternion devaRot = Quaternion.identity;
        bool devaWasActive = false;

        if (originalDeva != null)
        {
            devaWasActive = originalDeva.activeSelf;
            if (devaWasActive)
            {
                devaPos = originalDeva.transform.position;
                devaRot = originalDeva.transform.rotation;
                originalDeva.SetActive(false);
                Debug.Log($"Original Deva deactivated. Pos: {devaPos}");
            }
        }

        // 다이얼로그용 캐릭터 스폰 (원본이 활성화 상태였을 때만)
        if (dialogueAdamPrefab != null && adamWasActive)
        {
            spawnedDialogueAdam = Instantiate(dialogueAdamPrefab, adamPos, adamRot);
            Debug.Log("Dialogue Adam spawned.");
        }

        if (dialogueDevaPrefab != null && devaWasActive)
        {
            spawnedDialogueDeva = Instantiate(dialogueDevaPrefab, devaPos, devaRot);
            Debug.Log("Dialogue Deva spawned.");
        }
        // 다른 캐릭터들도 필요하다면 여기에 추가
    }

    // 이 함수를 "다이얼로그 종료" 시점에 호출 (별도의 시그널 또는 TimelineDialogueSystem의 EndDialogue에서 호출)
    public void CleanupDialogueCharacters()
    {
        Debug.Log("TimelineCharacterActivator: CleanupDialogueCharacters() 호출됨");

        // 스폰된 다이얼로그용 캐릭터 파괴
        if (spawnedDialogueAdam != null)
        {
            Destroy(spawnedDialogueAdam);
            spawnedDialogueAdam = null;
            Debug.Log("Dialogue Adam destroyed.");
        }
        if (spawnedDialogueDeva != null)
        {
            Destroy(spawnedDialogueDeva);
            spawnedDialogueDeva = null;
            Debug.Log("Dialogue Deva destroyed.");
        }

        // 원본 캐릭터는 복원하지 않음
        Debug.Log("Original characters remain deactivated as per request.");
    }
}