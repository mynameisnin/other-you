using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VillageMovement : MonoBehaviour
{
    Rigidbody2D rb;
    Animator animator;
    SpriteRenderer spriteRenderer;

    public float moveSpeed = 3f;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator.ResetTrigger("DashAttack"); // 대시 트리거 해제
        animator.SetBool("isDashing", false); // 대시 애니메이션 강제 비활성화

    }

    void Update()
    {
        HandleMovement();
        HandleFlip();
        HandleAnimation();
    }

    void HandleMovement()
    {
        float hor = Input.GetAxisRaw("Horizontal");
        rb.velocity = new Vector2(hor * moveSpeed, rb.velocity.y);
    }

    void HandleFlip()
    {
        float hor = Input.GetAxisRaw("Horizontal");
        if (hor != 0)
        {
            spriteRenderer.flipX = hor < 0;
        }
    }

    void HandleAnimation()
    {
        float hor = Input.GetAxisRaw("Horizontal");
        animator.SetBool("run", Mathf.Abs(hor) > 0);
    }
}
