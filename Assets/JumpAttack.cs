using System.Collections;
using UnityEngine;

public class JumpAttack : MonoBehaviour
{
    private Animator AdamAnime;
    private bool isJumpAttacking = false;
    private Rigidbody2D rd;

    [Header("공격 설정")]
    public float jumpAttackCooldown = 0.5f;

    [Header("에너지 소모 설정")]
    public float jumpAttackEnergyCost = 15f;
    private bool energyConsumed = false;

    void Start()
    {
        AdamAnime = GetComponent<Animator>();
        rd = GetComponent<Rigidbody2D>();
    }

    private bool wasGrounded = false;

    void Update()
    {
        bool isGrounded = IsGrounded();

        if (!wasGrounded && isGrounded && isJumpAttacking)
        {
            isJumpAttacking = false;
            AdamAnime.ResetTrigger("JumpAttack");
            Debug.Log("착지로 인해 점프 어택 초기화됨");
        }

        wasGrounded = isGrounded;

        if (Input.GetKeyDown(KeyCode.LeftControl) && !isGrounded)
        {
            PerformJumpAttack();
        }

        AdamAnime.SetBool("IsGrounded", isGrounded);
        AdamAnime.SetFloat("VerticalSpeed", rd.velocity.y);
    }

    private void PerformJumpAttack()
    {
        if (isJumpAttacking) return;

        float currentEnergy = PlayerStats.Instance.currentEnergy;

        if (currentEnergy <= 0)
        {
            Debug.Log("점프 공격 불가: ENERGY 부족!");
            EnergyBarUI.Instance?.FlashBorder();
            return;
        }

        isJumpAttacking = true;
        energyConsumed = false;
        AdamAnime.SetTrigger("JumpAttack");

        StartCoroutine(ResetJumpAttack());
        StartCoroutine(ConsumeEnergyAfterAnimation());
    }

    private IEnumerator ResetJumpAttack()
    {
        yield return new WaitForSeconds(jumpAttackCooldown);
        isJumpAttacking = false;
    }

    private IEnumerator ConsumeEnergyAfterAnimation()
    {
        yield return new WaitUntil(() =>
            AdamAnime.GetCurrentAnimatorStateInfo(0).IsName("JumpAttack"));

        if (!energyConsumed)
        {
            float currentEnergy = PlayerStats.Instance.currentEnergy;
            float energyToConsume = Mathf.Min(jumpAttackEnergyCost, currentEnergy);

            EnergyBarUI.Instance?.ReduceEnergy(energyToConsume);
            energyConsumed = true;
        }
    }

    private bool IsGrounded()
    {
        return Mathf.Abs(GetComponent<Rigidbody2D>().velocity.y) < 0.1f;
    }

    public void ResetJumpAttackState()
    {
        isJumpAttacking = false;

        if (AdamAnime != null && gameObject.activeInHierarchy)
        {
            AdamAnime.ResetTrigger("JumpAttack");
            AdamAnime.SetBool("IsGrounded", true);
        }

        StopAllCoroutines();
        Debug.Log("JumpAttack 상태 초기화됨 (캐릭터 전환 등)");
    }
}
