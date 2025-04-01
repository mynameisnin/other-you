using System.Collections;
using UnityEngine;

public class AdamUltimateSkill : MonoBehaviour
{
    public float ultimateDuration = 1f;
    public float manaCost = 50f;
    public KeyCode ultimateKey = KeyCode.C;

    private AdamMovement adamMovement;
    private Animator adamAnimator;
    private Rigidbody2D adamRigidbody;

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
            if (PlayerStats.Instance.currentEnergy >= manaCost)
            {
                StartCoroutine(CastUltimate());
            }
            else
            {
                Debug.Log("�ñر� �ߵ� ����: ���� ����!");
                EnergyBarUI.Instance.FlashBorder();
            }
        }
    }

    IEnumerator CastUltimate()
    {
        isCasting = true;

        // ���� ����
   

        // ���� ����
        adamMovement.isInvincible = true;
        adamRigidbody.velocity = Vector2.zero;
        adamAnimator.SetTrigger("Ultimate");
        
        // �̵�, �뽬, ���� ���� �ߴ�
        adamMovement.ForceStopDash();
        adamMovement.StopMovement();

        Debug.Log("�ñر� �ߵ�!");

        // ���� Ÿ�� (�ִϸ��̼�, ����Ʈ, Ÿ�� ó�� ���� ���⼭)
        yield return new WaitForSeconds(ultimateDuration);

        // ���� ����
        adamMovement.isInvincible = false;
        isCasting = false;

        Debug.Log("�ñر� ����");
    }
}
