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
    public float switchDelay = 0.7f;  // �ִϸ��̼� ��� �ð�
    public float switchCooldown = 1.5f; // ��Ÿ��

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
            DeactivateAdam();
            adamObject.SetActive(false);
        }
        else
        {
            currentPos = debaObject.transform.position;
            DeactivateDeba();
            debaObject.SetActive(false);
        }

        // ����ġ ����Ʈ ��ġ ����
        switchEffectObject.transform.position = currentPos;

        // ����Ʈ �ִϸ��̼� ����
        if (switchAnimator != null)
        {
            switchEffectObject.SetActive(true);

            string triggerToUse = isAdamActive ? triggerAdamToDeba : triggerDebaToAdam;
            switchAnimator.SetTrigger(triggerToUse);

            //   GUI ��ȯ�� ����Ʈ�� ���ÿ� ����
            if (guiController != null)
            {
                if (isAdamActive)
                    guiController.SwitchToDeba();  // Deba�� �ٲ�� ����Ʈ�ϱ� Deba GUI Ȱ��ȭ
                else
                    guiController.SwitchToAdam();
            }
        }

        yield return new WaitForSeconds(switchDelay);

        // ĳ���� Ȱ��ȭ�� ������ �� ����
        if (isAdamActive)
            ActivateDebaDelayed();
        else
            ActivateAdamDelayed();

        switchEffectObject.SetActive(false);

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
        // ����ġ ���� ���� �ʱ�ȭ
        var debaMovement = debaObject.GetComponent<DebaraMovement>();
        if (debaMovement != null)
        {
            debaMovement.ResetState(); // isTeleporting, attackInputRecently, velocity �� �ʱ�ȭ
        }

   

    }

    void ActivateAdamDelayed()
    {
        isAdamActive = true;
        adamObject.transform.position = switchEffectObject.transform.position;
        adamObject.SetActive(true);
      
    }

    void DeactivateAdam()
    {
        var adamMovement = adamObject.GetComponent<AdamMovement>();
        if (adamMovement != null)
        {
            adamMovement.ForceStopDash();
            var buff = adamObject.GetComponent<AdamAttackSpeedBuff>();
            buff?.ResetSkillState();
            var bladeSkill = adamObject.GetComponentInChildren<BladeExhaustSkill>();
            if (bladeSkill != null)
                bladeSkill.ResetSkillState();
            Rigidbody2D rb = adamObject.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                rb.velocity = Vector2.zero;
                rb.angularVelocity = 0f;
            }
        }

        adamObject.SetActive(false);
    }
    void DeactivateDeba()
    {
        var debaMovement = debaObject.GetComponent<DebaraMovement>();
        if (debaMovement != null)
        {
            debaMovement.ForceEndAttack();
            debaMovement.ForceCancelTeleport();
            debaMovement.ResetLaserSkill();

            //  Deva�� Rigidbody �ʱ�ȭ
            Rigidbody2D rb = debaObject.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                rb.velocity = Vector2.zero;
                rb.angularVelocity = 0f;
            }
        }

        debaObject.SetActive(false);
    }

}
