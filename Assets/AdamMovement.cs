using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AdamMovement : MonoBehaviour
{
    Rigidbody2D AdamRigidebody;
    Animator AdamAnime;
    SpriteRenderer AdamSprite;

    public float JumpPoewr = 3f;
    public float AdamMoveSpeed = 3f;
    private CharacterAttack characterAttack; // ���� ��ũ��Ʈ ����

    void Start()
    {
        AdamRigidebody = GetComponent<Rigidbody2D>();
        AdamAnime = GetComponent<Animator>();
        AdamSprite = GetComponent<SpriteRenderer>();

        // CharacterAttack ��ũ��Ʈ ��������
        characterAttack = GetComponent<CharacterAttack>();
    }

    void Update()
    {
        // ���� �߿��� �̵����� ����
        if (characterAttack != null && characterAttack.IsAttacking)
        {
            AdamRigidebody.velocity = Vector2.zero;
            return;
        }

        HandleMovement();
        AdamAnimation();
        AdamSpriteRender();
    }

    void HandleMovement()
    {
        // ���� �ִϸ��̼� ���� Ȯ��
        AnimatorStateInfo currentState = AdamAnime.GetCurrentAnimatorStateInfo(0);

        // ���� �ִϸ��̼� ������ �� �̵��� ����
        if (currentState.IsName("Attack1") || currentState.IsName("Attack2") || currentState.IsName("Attack3") ||
            currentState.IsName("Stance1") || currentState.IsName("Stance2") || currentState.IsName("Stance3"))
        {
            AdamRigidebody.velocity = Vector2.zero;
            return; // �̵� ���� ����
        }

        // �̵� ó��
        float hor = Input.GetAxis("Horizontal");
        AdamRigidebody.velocity = new Vector2(hor * AdamMoveSpeed, AdamRigidebody.velocity.y);

        //����ó��
        if(Input.GetKey(KeyCode.Space))
        {
            AdamRigidebody.velocity = Vector2.up * JumpPoewr;
        }
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

        if (AdamRigidebody.velocity.y > 0.1f)
        {
            AdamAnime.SetBool("jump", true);
        }
        else
        {
            AdamAnime.SetBool("jump", false);
        }
    }

    void AdamSpriteRender()
    {
        if (Input.GetButton("Horizontal"))
        {
            AdamSprite.flipX = Input.GetAxisRaw("Horizontal") == -1;
        }
    }

    void OnTriggerEnter2D(Collider2D TestEnemy)
    {
        if(TestEnemy.CompareTag("TestEnemy"))
        {
            Debug.Log("���ݰ���");
        }
    }
}
