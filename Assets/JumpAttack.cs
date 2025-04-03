using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumpAttack : MonoBehaviour
{
   
    private Animator AdamAnime;
    private bool isJumpAttacking = false; // 점프 공격 여부
    private Rigidbody2D rd;
    [Header("공격 설정")]
    public float jumpAttackCooldown = 0.5f; // 인스펙터에서 조절 가능

    void Start()
    {
        AdamAnime = GetComponent<Animator>();
        rd = GetComponent<Rigidbody2D>();
    }

    private bool wasGrounded = false; // 이전 프레임의 착지 상태 저장

    void Update()
    {
        bool isGrounded = IsGrounded();

        // 착지 시 점프 어택 상태 초기화
        if (!wasGrounded && isGrounded && isJumpAttacking)
        {
            isJumpAttacking = false;
            AdamAnime.ResetTrigger("JumpAttack"); // 혹시 모르니 트리거도 초기화
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
        if (isJumpAttacking) return; // 이미 점프 공격 중이면 실행 안 함

        isJumpAttacking = true;
        AdamAnime.SetTrigger("JumpAttack"); // 점프 공격 애니메이션 실행

        // 일정 시간 후 공격 가능 상태로 변경
        StartCoroutine(ResetJumpAttack());
    }

    private IEnumerator ResetJumpAttack()
    {
        yield return new WaitForSeconds(jumpAttackCooldown); // 공격 지속 시간 조절 가능
        isJumpAttacking = false; // 공격 종료 후 다시 공격 가능
    }

    private bool IsGrounded()
    {
        return Mathf.Abs(GetComponent<Rigidbody2D>().velocity.y) < 0.1f;
    }
    public void ResetJumpAttackState()
    {
        isJumpAttacking = false;

        // 점프 어택 트리거 초기화
        if (AdamAnime != null && gameObject.activeInHierarchy)
        {
            AdamAnime.ResetTrigger("JumpAttack");
            AdamAnime.SetBool("IsGrounded", true); // 안전하게 처리
        }

        // 실행 중인 코루틴이 있다면 정지 (여기선 StopAllCoroutines 사용)
        StopAllCoroutines();

        Debug.Log("JumpAttack 상태 초기화됨 (캐릭터 전환 등)");
    }
}