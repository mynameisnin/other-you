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
                Debug.Log("궁극기 발동 실패: 마나 부족!");
                EnergyBarUI.Instance.FlashBorder();
            }
        }
    }

    IEnumerator CastUltimate()
    {
        isCasting = true;

        // 마나 차감
   

        // 상태 설정
        adamMovement.isInvincible = true;
        adamRigidbody.velocity = Vector2.zero;
        adamAnimator.SetTrigger("Ultimate");
        
        // 이동, 대쉬, 공격 강제 중단
        adamMovement.ForceStopDash();
        adamMovement.StopMovement();

        Debug.Log("궁극기 발동!");

        // 연출 타임 (애니메이션, 이펙트, 타격 처리 등은 여기서)
        yield return new WaitForSeconds(ultimateDuration);

        // 상태 복원
        adamMovement.isInvincible = false;
        isCasting = false;

        Debug.Log("궁극기 종료");
    }
}
