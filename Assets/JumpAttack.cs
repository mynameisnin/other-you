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

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.M) && !IsGrounded()) // 공중에서 공격 가능
        {
            PerformJumpAttack();
        }
        AdamAnime.SetBool("IsGrounded", IsGrounded());
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
}