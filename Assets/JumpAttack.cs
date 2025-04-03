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

    private bool wasGrounded = false; // ���� �������� ���� ���� ����

    void Update()
    {
        bool isGrounded = IsGrounded();

        // ���� �� ���� ���� ���� �ʱ�ȭ
        if (!wasGrounded && isGrounded && isJumpAttacking)
        {
            isJumpAttacking = false;
            AdamAnime.ResetTrigger("JumpAttack"); // Ȥ�� �𸣴� Ʈ���ŵ� �ʱ�ȭ
            Debug.Log("������ ���� ���� ���� �ʱ�ȭ��");
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
    public void ResetJumpAttackState()
    {
        isJumpAttacking = false;

        // ���� ���� Ʈ���� �ʱ�ȭ
        if (AdamAnime != null && gameObject.activeInHierarchy)
        {
            AdamAnime.ResetTrigger("JumpAttack");
            AdamAnime.SetBool("IsGrounded", true); // �����ϰ� ó��
        }

        // ���� ���� �ڷ�ƾ�� �ִٸ� ���� (���⼱ StopAllCoroutines ���)
        StopAllCoroutines();

        Debug.Log("JumpAttack ���� �ʱ�ȭ�� (ĳ���� ��ȯ ��)");
    }
}