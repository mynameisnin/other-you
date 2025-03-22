using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterSwitcher : MonoBehaviour
{
    public GameObject adamObject;
    public GameObject debaObject;

    private bool isAdamActive = true;
    public CharacterGUIController guiController; //  GUI ��ȯ�� ��Ʈ�ѷ� ����
    void Start()
    {
        ActivateAdam(); // ������ �ƴ�����
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            SwitchCharacter();
        }
    }

    void SwitchCharacter()
    {
        if (isAdamActive)
        {
            ActivateDeba();
        }
        else
        {
            ActivateAdam();
        }
    }

    void ActivateAdam()
    {
        isAdamActive = true;

        // Deba�� ���� ��ġ�� Adam���� ����
        adamObject.transform.position = debaObject.transform.position;

        adamObject.SetActive(true);
        debaObject.SetActive(false);
        if (guiController != null)
        {
            guiController.SwitchToAdam();
        }
    }

    void ActivateDeba()
    {
        isAdamActive = false;

        // Adam�� ���� ��ġ�� Deba���� ����
        debaObject.transform.position = adamObject.transform.position;

        adamObject.SetActive(false);
        debaObject.SetActive(true);
        //  GUI ��ȯ
        if (guiController != null)
        {
            guiController.SwitchToDeba();
        }

    }
}