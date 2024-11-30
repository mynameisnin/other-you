using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AdamMovement : MonoBehaviour
{
    Rigidbody2D AdamRigidebody;
    Animator AdamAnime;
    SpriteRenderer AdamSprite;

    public float AdamMoveSpeed = 3f;
    public float attackMoveForce = 10f; // 공격 시 전진 속도
    private bool isAttacking = false; // 공격 상태 확인

    void Start()
    {
        AdamRigidebody = GetComponent<Rigidbody2D>();
        AdamAnime = GetComponent<Animator>();
        AdamSprite = GetComponent<SpriteRenderer>();

    }

    void Update()
    {
        // 공격 중에는 이동을 멈춤
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
        // 현재 애니메이션 상태 확인
        AnimatorStateInfo currentState = AdamAnime.GetCurrentAnimatorStateInfo(0);

        // Attack1, Attack2, Attack3 상태일 때 이동을 멈춤
        if (currentState.IsName("Attack1") || currentState.IsName("Attack2") || currentState.IsName("Attack3") || currentState.IsName("Stance1") || currentState.IsName("Stance2") || currentState.IsName("Stance3"))
        {
            AdamRigidebody.velocity = Vector2.zero;
            return; // 이동 로직 종료
        }

        // 이동 처리
        AdamMove();
    }

    // === 애니메이션 이벤트로 호출되는 메서드 ===
    public void StartAttack()
    {
        isAttacking = true; // 공격 시작
    }

    public void EndAttack()
    {
        isAttacking = false; // 공격 종료
    }

    public void MoveForwardDuringAttack()
    {
        float direction = AdamSprite.flipX ? -1f : 1f; // 캐릭터 방향
        AdamRigidebody.velocity = new Vector2(direction * attackMoveForce, AdamRigidebody.velocity.y);
    }
}
