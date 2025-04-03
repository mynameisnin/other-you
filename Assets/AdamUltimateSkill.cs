using System.Collections;
using UnityEngine;

public class AdamUltimateSkill : MonoBehaviour
{
    public float ultimateDuration = 1f;
    public float manaCost = 50f;
    public KeyCode ultimateKey = KeyCode.C;
    public float cooldownDuration = 8f; // ��: 8�� ��Ÿ��
    private float cooldownEndTime = 0f;
    private AdamMovement adamMovement;
    private Animator adamAnimator;
    private Rigidbody2D adamRigidbody;
    [SerializeField] private SkillCooldownUI ultimateCooldownUI; // �ν����� ����
    public bool isCasting = false;

    void Start()
    {
        adamMovement = GetComponent<AdamMovement>();
        adamAnimator = GetComponent<Animator>();
        adamRigidbody = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        if (Input.GetKeyDown(ultimateKey) && !isCasting)
        {
            // ��Ÿ�� üũ
            if (Time.time < cooldownEndTime)
            {
                Debug.Log("�ñر� ��Ÿ�� ��");
                return;
            }

            if (PlayerStats.Instance.HasEnoughMana((int)manaCost))
            {
                StartCoroutine(CastUltimate());
            }
            else
            {
                Debug.Log("�ñر� �ߵ� ����: ���� ����!");
                ManaBarUI.Instance?.FlashBorder();
            }
        }
    }

    IEnumerator CastUltimate()
    {
        isCasting = true;

        PlayerStats.Instance.ReduceMana((int)manaCost);

        adamMovement.isInvincible = true;
        adamRigidbody.velocity = Vector2.zero;
        adamAnimator.SetTrigger("Ultimate");

        adamMovement.ForceStopDash();
        adamMovement.StopMovement();

        Debug.Log("�ñر� �ߵ�!");

        yield return new WaitForSeconds(ultimateDuration);

        adamMovement.isInvincible = false;
        isCasting = false;

        // ��Ÿ�� ����
        cooldownEndTime = Time.time + cooldownDuration;
        // ��Ÿ�� ����
        cooldownEndTime = Time.time + cooldownDuration;

        if (ultimateCooldownUI != null)
        {
            ultimateCooldownUI.cooldownTime = cooldownDuration;
            ultimateCooldownUI.StartCooldown();
        }
        Debug.Log("�ñر� ����");
    }
    public void CancelUltimate()
    {
        if (!isCasting) return;

        Debug.Log("�ñر� ���� ����� (ĳ���� ����ġ)");

        StopAllCoroutines(); // �ñر� �ڷ�ƾ ����
        isCasting = false;
        adamMovement.isInvincible = false;
        adamMovement.ForceStopDash();
        adamMovement.StopMovement();
        adamAnimator.ResetTrigger("Ultimate");
    }
}
