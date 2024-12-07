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
    private bool attackInputRecently = false; // �ֱ� ���� �Է� ����

    [SerializeField]
    private float dashingPower = 24f;
    private float dashingTime = 0.2f;
    private float dashingCooldown = 1f;
    [SerializeField] private TrailRenderer dashTrail;

    [SerializeField] private float afterImageInterval = 0.05f;
    [SerializeField] private float afterImageLifetime = 1f; // �ܻ� ���� �ð�
    [SerializeField] private float attackInputCooldown = 0.2f; // ���� �� �뽬 ���� �ð�
    [SerializeField] private float dashAttackDuration = 0.3f; // �뽬 ���� ���� �ð�

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
        // ���� �ִϸ��̼� ���� Ȯ��
        AnimatorStateInfo currentState = AdamAnime.GetCurrentAnimatorStateInfo(0);

        // ���� �ִϸ��̼� ���� ������ Ȯ��
        bool isInAttackAnimation = currentState.IsName("Attack1") || currentState.IsName("Attack2") || currentState.IsName("Attack3") || currentState.IsName("DashAttack");

        // ���� �� �Ǵ� �뽬 ���� ���̸� �ൿ ����
        if (isInAttackAnimation || isDashAttacking)
        {
            StopMovement(); // �̵� ����
            return; // �ٸ� ���� ����
        }

        HandleAttack();

        if (!isDashing && !attackInputRecently)
        {
            HandleMovement();
        }

        if (!isInAttackAnimation && !attackInputRecently)
        {
            HandleDash(); // ���� �ִϸ��̼��� ���� ���� �ƴϰ� ���� �Է� �� ��ٿ��� �ƴϸ� �뽬 ����
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
            attackInputRecently = true; // ���� �Է� ����
            StartCoroutine(ResetAttackInputCooldown());

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
        // �뽬 �Ұ� ����: �뽬 ��, ��ٿ� ��, �Ǵ� ���� �Է� �� ���� �ð� ��
        if (isDashing || !canDash || attackInputRecently)
        {
            return;
        }

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

        float dashDirection = lastKeyWasRight ? 1 : -1;

        // �뽬 ���� �� ���� �Ÿ���ŭ ����
        float elapsed = 0f;
        while (elapsed < dashAttackDuration)
        {
            AdamRigidebody.velocity = new Vector2(dashDirection * dashingPower, 0f);
            elapsed += Time.deltaTime;
            yield return null;
        }

        AdamRigidebody.velocity = Vector2.zero; // �뽬 ���� �� ����
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
        sr.color = new Color(0.2f, 0.2f, 0.2f, 0.8f);
        sr.flipX = AdamSprite.flipX;
        afterImage.transform.position = transform.position;
        afterImage.transform.localScale = transform.localScale;

        StartCoroutine(FadeOutAndDestroy(sr));
    }

    private IEnumerator FadeOutAndDestroy(SpriteRenderer sr)
    {
        float fadeDuration = afterImageLifetime;
        Color originalColor = sr.color;
        float elapsed = 0f;

        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            float alpha = Mathf.Lerp(originalColor.a, 0, elapsed / fadeDuration); // ���İ� ������ ����
            sr.color = new Color(originalColor.r, originalColor.g, originalColor.b, alpha); // ���� ����
            yield return null;
        }

        Destroy(sr.gameObject); // ���������� �ܻ� ������Ʈ ����
    }

    private IEnumerator ResetAttackInputCooldown()
    {
        yield return new WaitForSeconds(attackInputCooldown);
        attackInputRecently = false; // ���� �Է� ���� �ʱ�ȭ
    }

    void StopMovement()
    {
        AdamRigidebody.velocity = Vector2.zero; // �̵� ����
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
