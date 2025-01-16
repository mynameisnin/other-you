using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumpAttack : MonoBehaviour
{
   
    private Animator AdamAnime;
    private bool isJumpAttacking = false; // ���� ���� ����
    private Rigidbody2D rd;
    [Header("���� ����")]
    public float jumpAttackCooldown = 0.5f; // �ν����Ϳ��� ���� ����

    void Start()
    {
        AdamAnime = GetComponent<Animator>();
        rd = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.M) && !IsGrounded()) // ���߿��� ���� ����
        {
            PerformJumpAttack();
        }
        AdamAnime.SetBool("IsGrounded", IsGrounded());
        AdamAnime.SetFloat("VerticalSpeed", rd.velocity.y);


    }

    private void PerformJumpAttack()
    {
        if (isJumpAttacking) return; // �̹� ���� ���� ���̸� ���� �� ��

        isJumpAttacking = true;
        AdamAnime.SetTrigger("JumpAttack"); // ���� ���� �ִϸ��̼� ����

        // ���� �ð� �� ���� ���� ���·� ����
        StartCoroutine(ResetJumpAttack());
    }

    private IEnumerator ResetJumpAttack()
    {
        yield return new WaitForSeconds(jumpAttackCooldown); // ���� ���� �ð� ���� ����
        isJumpAttacking = false; // ���� ���� �� �ٽ� ���� ����
    }

    private bool IsGrounded()
    {
        return Mathf.Abs(GetComponent<Rigidbody2D>().velocity.y) < 0.1f;
    }
}