using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterSwitcher : MonoBehaviour
{
    public GameObject adamObject;
    public GameObject debaObject;

    private bool isAdamActive = true;
    public CharacterGUIController guiController; //  GUI 전환용 컨트롤러 연결
    void Start()
    {
        ActivateAdam(); // 시작은 아담으로
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

        // Deba의 현재 위치를 Adam에게 전달
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

        // Adam의 현재 위치를 Deba에게 전달
        debaObject.transform.position = adamObject.transform.position;

        adamObject.SetActive(false);
        debaObject.SetActive(true);
        //  GUI 전환
        if (guiController != null)
        {
            guiController.SwitchToDeba();
        }

    }
}