using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AdamMovement : MonoBehaviour
{
    Rigidbody2D AdamRigidebody;
    Animator AdamAnime;
    SpriteRenderer AdamSprite;

    public float AdamMoveSpeed = 3f;
    public float attackMoveForce = 10f; // ���� �� ���� �ӵ�
    private bool isAttacking = false; // ���� ���� Ȯ��

    void Start()
    {
        AdamRigidebody = GetComponent<Rigidbody2D>();
        AdamAnime = GetComponent<Animator>();
        AdamSprite = GetComponent<SpriteRenderer>();

    }

    void Update()
    {
        // ���� �߿��� �̵��� ����
        if (isAttacking)
            return;

        HandleMovement();
        AdamAnimation();
        AdamSpriteRender();
    }

    void AdamMove()
    {
        float hor = Input.GetAxis("Horizontal");
        AdamRigidebody.velocity = new Vector2(hor * AdamMoveSpeed, AdamRigidebody.velocity.y);
    }

    void AdamAnimation()
    {
        float hor = Input.GetAxis("Horizontal");
        if (Mathf.Abs(hor) > 0.01f)
        {
            AdamAnime.SetBool("run", true);
        }
        else
        {
            AdamAnime.SetBool("run", false);
        }

        if (Input.GetKey(KeyCode.LeftAlt))
        {
            AdamAnime.SetBool("hurt", true);
        }
        else
        {
            AdamAnime.SetBool("hurt", false);
        }
    }

    void AdamSpriteRender()
    {
        if (Input.GetButton("Horizontal"))
        {
            AdamSprite.flipX = Input.GetAxisRaw("Horizontal") == -1;
        }
    }

    void HandleMovement()
    {
        // ���� �ִϸ��̼� ���� Ȯ��
        AnimatorStateInfo currentState = AdamAnime.GetCurrentAnimatorStateInfo(0);

        // Attack1, Attack2, Attack3 ������ �� �̵��� ����
        if (currentState.IsName("Attack1") || currentState.IsName("Attack2") || currentState.IsName("Attack3") || currentState.IsName("Stance1") || currentState.IsName("Stance2") || currentState.IsName("Stance3"))
        {
            AdamRigidebody.velocity = Vector2.zero;
            return; // �̵� ���� ����
        }

        // �̵� ó��
        AdamMove();
    }

    // === �ִϸ��̼� �̺�Ʈ�� ȣ��Ǵ� �޼��� ===
    public void StartAttack()
    {
        isAttacking = true; // ���� ����
    }

    public void EndAttack()
    {
        isAttacking = false; // ���� ����
    }

    public void MoveForwardDuringAttack()
    {
        float direction = AdamSprite.flipX ? -1f : 1f; // ĳ���� ����
        AdamRigidebody.velocity = new Vector2(direction * attackMoveForce, AdamRigidebody.velocity.y);
    }
}
