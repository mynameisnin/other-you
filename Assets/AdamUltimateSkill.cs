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
            // 마나 확인
            if (PlayerStats.Instance.HasEnoughMana((int)manaCost))
            {
                StartCoroutine(CastUltimate());
            }
            else
            {
                Debug.Log("궁극기 발동 실패: 마나 부족!");
                ManaBarUI.Instance?.FlashBorder(); // 마나 테두리 깜빡임 추가했다면 사용
            }
        }
    }

    IEnumerator CastUltimate()
    {
        isCasting = true;

        // 마나 차감
        PlayerStats.Instance.ReduceMana((int)manaCost);

        // 상태 설정
        adamMovement.isInvincible = true;
        adamRigidbody.velocity = Vector2.zero;
        adamAnimator.SetTrigger("Ultimate");

        adamMovement.ForceStopDash();
        adamMovement.StopMovement();

        Debug.Log("궁극기 발동!");

        yield return new WaitForSeconds(ultimateDuration);

        adamMovement.isInvincible = false;
        isCasting = false;

        Debug.Log("궁극기 종료");
    }
    public void CancelUltimate()
    {
        if (!isCasting) return;

        Debug.Log("궁극기 강제 종료됨 (캐릭터 스위치)");

        StopAllCoroutines(); // 궁극기 코루틴 종료
        isCasting = false;
        adamMovement.isInvincible = false;
        adamMovement.ForceStopDash();
        adamMovement.StopMovement();
        adamAnimator.ResetTrigger("Ultimate");
    }
}
