using System.Collections;
using UnityEngine;

public class DevaMovement : MonoBehaviour
{
    Rigidbody2D DevaRigidbody;
    Animator DevaAnime;
    SpriteRenderer DevaSprite;

    public float DevaMoveSpeed = 3f;
    public float jumpForce = 5f; // ���� ���� ����
    public LayerMask groundLayer; // �ٴ��� ������ ���̾� ����
    public Transform groundCheck; // �ٴ� ������ ���� ��ġ
    private bool isGrounded; // �ٴڿ� ��� �ִ��� ���� Ȯ��

    // �뽬 ����
    private bool canDash = true;
    private bool isDashing;
    private float dashingPower = 24f;
    private float dashingTime = 0.3f;
    private float dashingCooldown = 0.5f;
    private bool lastKeyWasRight = true;

    [SerializeField] private TrailRenderer dashTrail; // �뽬 �ܻ� ȿ��

    // �ܻ� ȿ�� ����
    [SerializeField] private float afterImageInterval = 0.02f; // �ܻ� ���� ����
    [SerializeField] private float afterImageLifetime = 0.5f; // �ܻ� ���� �ð�

    // Start is called before the first frame update
    void Start()
    {
        DevaRigidbody = GetComponent<Rigidbody2D>();
        DevaAnime = GetComponent<Animator>();
        DevaSprite = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        CheckGrounded(); // �ٴ� ���� ������Ʈ

        if (!isDashing)
        {
            DevaMove();
            HandleDash();
        }

        DevaAnimation();
        JumpMoveMent();
        DevaSpriteRender();
    }

    void DevaMove()
    {
        float hor = Input.GetAxis("Horizontal");
        DevaRigidbody.velocity = new Vector2(hor * DevaMoveSpeed, DevaRigidbody.velocity.y);

        if (hor > 0)
            lastKeyWasRight = true;
        else if (hor < 0)
            lastKeyWasRight = false;
    }

    void HandleDash()
    {
        if (isDashing || !canDash || !isGrounded)
            return;

        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            StartCoroutine(Dash());
        }
    }

    private IEnumerator Dash()
    {
        canDash = false;
        isDashing = true;
        DevaAnime.SetBool("isDashing", true); // �뽬 �ִϸ��̼� ����

        if (dashTrail != null) dashTrail.emitting = true;

        StartCoroutine(LeaveAfterImage()); // �ܻ� ���� ����

        float originalGravity = DevaRigidbody.gravityScale;
        DevaRigidbody.gravityScale = 0f;

        float dashDirection = lastKeyWasRight ? 1 : -1;
        DevaRigidbody.velocity = new Vector2(dashDirection * dashingPower, 0f);

        yield return new WaitForSeconds(dashingTime);

        if (dashTrail != null) dashTrail.emitting = false;

        DevaRigidbody.gravityScale = originalGravity;
        isDashing = false;
        DevaAnime.SetBool("isDashing", false);

        yield return new WaitForSeconds(dashingCooldown);
        canDash = true;
    }

    // �ܻ� ���� (�뽬 �� ���������� ȣ��)
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

        sr.sprite = DevaSprite.sprite;
        sr.color = new Color(1f, 1f, 1f, 0.8f); // ������ ���
        sr.flipX = DevaSprite.flipX;
        afterImage.transform.position = transform.position;
        afterImage.transform.localScale = transform.localScale;

        StartCoroutine(FadeOutAndDestroy(sr)); // �ܻ� ���� ������� ó��
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

    void DevaAnimation()
    {
        float hor = Input.GetAxis("Horizontal");

        if (Mathf.Abs(hor) > 0.00f)
        {
            DevaAnime.SetBool("run", true);
        }
        else
        {
            DevaAnime.SetBool("run", false);
        }

        if (DevaRigidbody.velocity.y > 0.1f)
        {
            DevaAnime.SetBool("jump", true);
            DevaAnime.SetBool("fall", false);
        }
        else if (DevaRigidbody.velocity.y < -0.1f)
        {
            DevaAnime.SetBool("jump", false);
            DevaAnime.SetBool("fall", true);
        }
        else if (isGrounded)
        {
            DevaAnime.SetBool("jump", false);
            DevaAnime.SetBool("fall", false);
        }
    }

    void JumpMoveMent()
    {
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            StartCoroutine(DelayedJump()); // ���� �غ� ���� �� ���� ����
        }
    }

    void DevaSpriteRender()
    {
        if (Input.GetButton("Horizontal"))
        {
            DevaSprite.flipX = Input.GetAxisRaw("Horizontal") == -1;
        }
    }

    void CheckGrounded()
    {
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, 0.1f, groundLayer);
    }
    private IEnumerator DelayedJump()
    {
        // ���� �غ� ���� ���� (�ɱ� �ִϸ��̼�)
        DevaAnime.SetTrigger("Crouch");

        // 0.3�� ���� �غ� ���� (�� ���� �ʿ信 ���� ���� ����)
        yield return new WaitForSeconds(0.15f);

        // ���� �ִϸ��̼� ����
        DevaAnime.SetTrigger("Jump");

        // ���� ����
        DevaRigidbody.velocity = new Vector2(DevaRigidbody.velocity.x, jumpForce);
    }
}
