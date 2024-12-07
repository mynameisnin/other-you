using System.Collections;
using UnityEngine;

public class AdamMovement : MonoBehaviour
{
    Rigidbody2D AdamRigidebody;
    Animator AdamAnime;
    SpriteRenderer AdamSprite;

    public float JumpPower = 3f;
    public float AdamMoveSpeed = 3f;
    private CharacterAttack characterAttack; // ���� ��ũ��Ʈ ����

    private bool canDash = true;
    private bool isDashing;
    private bool isDashAttacking; // �뽬 ���� ���� �߰�
    [SerializeField]
    private float dashingPower = 24f;
    private float dashingTime = 0.2f;
    private float dashingCooldown = 1f;
    [SerializeField] private TrailRenderer dashTrail;

    [SerializeField] private float afterImageInterval = 0.05f;
    [SerializeField] private float afterImageLifetime = 1f;

    private bool lastKeyWasRight = true;

    void Start()
    {
        AdamRigidebody = GetComponent<Rigidbody2D>();
        AdamAnime = GetComponent<Animator>();
        AdamSprite = GetComponent<SpriteRenderer>();

        characterAttack = GetComponent<CharacterAttack>();
    }

    void Update()
    {
        // �뽬 ���� ���°� �ƴϸ� �Ϲ� ���ݰ� �̵� ó��
        if (!isDashAttacking)
        {
            HandleAttack();
            if (!characterAttack.IsAttacking && !isDashing)
            {
                HandleMovement();
            }
            HandleDash();
        }

        AdamAnimation();
        HandleFlip();
    }

    void HandleMovement()
    {
        float hor = Input.GetAxisRaw("Horizontal");
        AdamRigidebody.velocity = new Vector2(hor * AdamMoveSpeed, AdamRigidebody.velocity.y);

        if (Input.GetKey(KeyCode.Space))
        {
            AdamRigidebody.velocity = Vector2.up * JumpPower;
        }

        if (hor > 0)
            lastKeyWasRight = true;
        else if (hor < 0)
            lastKeyWasRight = false;
    }

    void HandleAttack()
    {
        if (Input.GetKeyDown(KeyCode.M)) // ���� Ű �Է�
        {
            if (isDashing)
            {
                StartCoroutine(DashAttack()); // �뽬 �� ����
            }
            else if (characterAttack != null)
            {
                characterAttack.TriggerAttack(); // �Ϲ� ����
            }
        }
    }

    void HandleDash()
    {
        if (isDashing || !canDash) return;

        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            StartCoroutine(Dash());
        }
    }

    private IEnumerator Dash()
    {
        canDash = false;
        isDashing = true;

        AdamAnime.SetBool("isDashing", true); // �뽬 �ִϸ��̼� Ȱ��ȭ

        if (dashTrail != null) dashTrail.emitting = true;

        StartCoroutine(LeaveAfterImage());

        float originalGravity = AdamRigidebody.gravityScale;
        AdamRigidebody.gravityScale = 0f;

        float dashDirection = lastKeyWasRight ? 1 : -1;
        AdamRigidebody.velocity = new Vector2(dashDirection * dashingPower, 0f);

        yield return new WaitForSeconds(dashingTime);

        if (dashTrail != null) dashTrail.emitting = false;

        AdamRigidebody.gravityScale = originalGravity;
        isDashing = false;

        AdamAnime.SetBool("isDashing", false);

        yield return new WaitForSeconds(dashingCooldown);
        canDash = true;
    }

    private IEnumerator DashAttack()
    {
        isDashAttacking = true;
        AdamAnime.SetTrigger("DashAttack"); // �뽬 ���� �ִϸ��̼� ����

        float originalVelocity = AdamRigidebody.velocity.x; // �뽬 ���� �� �ӵ� ����
        float dashDirection = lastKeyWasRight ? 1 : -1;
        AdamRigidebody.velocity = new Vector2(dashDirection * dashingPower * 1.5f, 0f); // �뽬 ���� �ӵ� ����

        yield return new WaitForSeconds(0.3f); // �뽬 ���� ���� �ð�

        AdamRigidebody.velocity = new Vector2(originalVelocity, AdamRigidebody.velocity.y); // ���� �ӵ��� ����
        isDashAttacking = false;
    }

    private IEnumerator LeaveAfterImage()
    {
        while (isDashing)
        {
            CreateAfterImage();
            yield return new WaitForSeconds(afterImageInterval);
        }
    }

    void CreateAfterImage()
    {
        GameObject afterImage = new GameObject("AfterImage");
        SpriteRenderer sr = afterImage.AddComponent<SpriteRenderer>();

        sr.sprite = AdamSprite.sprite;
        sr.color = new Color(1f, 1f, 1f, 0.5f);
        sr.flipX = AdamSprite.flipX;
        afterImage.transform.position = transform.position;
        afterImage.transform.localScale = transform.localScale;

        Destroy(afterImage, afterImageLifetime);
    }

    void AdamAnimation()
    {
        float hor = Input.GetAxisRaw("Horizontal");
        AdamAnime.SetBool("run", Mathf.Abs(hor) > 0.01f);
        AdamAnime.SetBool("jump", AdamRigidebody.velocity.y > 0.1f);
    }

    void HandleFlip()
    {
        float hor = Input.GetAxisRaw("Horizontal");
        if (hor != 0)
        {
            AdamSprite.flipX = hor < 0;
        }
    }
}
