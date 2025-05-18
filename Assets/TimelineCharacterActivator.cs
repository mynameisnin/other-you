using UnityEngine;
using System.Collections.Generic; // Dictionary ����� ���� (������, ������ ���� ����)

public class TimelineCharacterActivator : MonoBehaviour
{
    [Header("���� ĳ���� (Ÿ�Ӷ��� ���� ���)")]
    public GameObject originalAdam;
    public GameObject originalDeva;
    // �ʿ��ϴٸ� �ٸ� ���� ĳ���͵鵵 �߰�

    [Header("���̾�α׿� ĳ���� ������")]
    public GameObject dialogueAdamPrefab;
    public GameObject dialogueDevaPrefab;
    // �ʿ��ϴٸ� �ٸ� ���̾�α׿� ĳ���� ������ �߰�

    // ������ ĳ���� �ν��Ͻ� �����
    private GameObject spawnedDialogueAdam;
    private GameObject spawnedDialogueDeva;

    // ���� ĳ������ ������ ��ġ/ȸ�� ����� (���� �� ���)
    // �� �������� ���� CleanupAfterDialogue���� ������� �����Ƿ�, ���� ������ ó���ص� ��
    // private Vector3 lastAdamPos;
    // private Quaternion lastAdamRot;
    // private Vector3 lastDevaPos;
    // private Quaternion lastDevaRot;
    // private bool wasAdamOriginallyActive;
    // private bool wasDevaOriginallyActive;


    // �� �Լ��� Ÿ�Ӷ����� Signal Emitter���� "���̾�α� ����" ������ ȣ��
    // (TimelineDialogueSystem�� StartDialogue�� ���� �ñ׳��� �޵��� ���� ����)
    public void PrepareCharactersForDialogue()
    {
        Debug.Log("TimelineCharacterActivator: PrepareCharactersForDialogue() ȣ���");

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

        // ���̾�α׿� ĳ���� ���� (������ Ȱ��ȭ ���¿��� ����)
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
        // �ٸ� ĳ���͵鵵 �ʿ��ϴٸ� ���⿡ �߰�
    }

    // �� �Լ��� "���̾�α� ����" ������ ȣ�� (������ �ñ׳� �Ǵ� TimelineDialogueSystem�� EndDialogue���� ȣ��)
    public void CleanupDialogueCharacters()
    {
        Debug.Log("TimelineCharacterActivator: CleanupDialogueCharacters() ȣ���");

        // ������ ���̾�α׿� ĳ���� �ı�
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

        // ���� ĳ���ʹ� �������� ����
        Debug.Log("Original characters remain deactivated as per request.");
    }
}