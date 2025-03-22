using System.Collections;
using UnityEngine;

public class CharacterSwitcher : MonoBehaviour
{
    public GameObject adamObject;
    public GameObject debaObject;

    public Animator switchAnimator;
    public GameObject switchEffectObject;

    public CharacterGUIController guiController;

    [Header("Switch Settings")]
    public string triggerAdamToDeba = "Play_AdamToDeba";
    public string triggerDebaToAdam = "Play_DebaToAdam";
    public float switchDelay = 0.7f;  // 애니메이션 재생 시간
    public float switchCooldown = 1.5f; // 쿨타임

    private bool isAdamActive = true;
    private bool canSwitch = true;

    void Start()
    {
        ActivateAdamImmediate();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab) && canSwitch)
        {
            StartCoroutine(PlaySwitchSequence());
        }
    }

    IEnumerator PlaySwitchSequence()
    {
        canSwitch = false;

        Vector3 currentPos;

        if (isAdamActive)
        {
            currentPos = adamObject.transform.position;
            adamObject.SetActive(false);
        }
        else
        {
            currentPos = debaObject.transform.position;
            debaObject.SetActive(false);
        }

        switchEffectObject.transform.position = currentPos;

        if (switchAnimator != null)
        {
            switchEffectObject.SetActive(true);

            string triggerToUse = isAdamActive ? triggerAdamToDeba : triggerDebaToAdam;
            switchAnimator.SetTrigger(triggerToUse);
        }

        yield return new WaitForSeconds(switchDelay);

        if (isAdamActive)
            ActivateDebaDelayed();
        else
            ActivateAdamDelayed();

        switchEffectObject.SetActive(false);

        // 쿨타임 대기
        yield return new WaitForSeconds(switchCooldown - switchDelay);
        canSwitch = true;
    }

    void ActivateAdamImmediate()
    {
        isAdamActive = true;
        adamObject.SetActive(true);
        debaObject.SetActive(false);
        guiController?.SwitchToAdam();
    }

    void ActivateDebaDelayed()
    {
        isAdamActive = false;
        debaObject.transform.position = switchEffectObject.transform.position;
        debaObject.SetActive(true);
        guiController?.SwitchToDeba();
    }

    void ActivateAdamDelayed()
    {
        isAdamActive = true;
        adamObject.transform.position = switchEffectObject.transform.position;
        adamObject.SetActive(true);
        guiController?.SwitchToAdam();
    }
}
