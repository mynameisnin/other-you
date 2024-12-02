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
    private CharacterAttack characterAttack; // 공격 스크립트 참조

    void Start()
    {
        AdamRigidebody = GetComponent<Rigidbody2D>();
        AdamAnime = GetComponent<Animator>();
        AdamSprite = GetComponent<SpriteRenderer>();

        // CharacterAttack 스크립트 가져오기
        characterAttack = GetComponent<CharacterAttack>();
    }

    void Update()
    {
        // 공격 중에는 이동하지 않음
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
        // 현재 애니메이션 상태 확인
        AnimatorStateInfo currentState = AdamAnime.GetCurrentAnimatorStateInfo(0);

        // 공격 애니메이션 상태일 때 이동을 멈춤
        if (currentState.IsName("Attack1") || currentState.IsName("Attack2") || currentState.IsName("Attack3") ||
            currentState.IsName("Stance1") || currentState.IsName("Stance2") || currentState.IsName("Stance3"))
        {
            AdamRigidebody.velocity = Vector2.zero;
            return; // 이동 로직 종료
        }

        // 이동 처리
        float hor = Input.GetAxis("Horizontal");
        AdamRigidebody.velocity = new Vector2(hor * AdamMoveSpeed, AdamRigidebody.velocity.y);

        //점프처리
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
            Debug.Log("공격감지");
        }
    }
}
