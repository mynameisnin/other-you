using System.Collections;
using UnityEngine;

public class DevaMovement : MonoBehaviour
{
    Rigidbody2D DevaRigidbody;
    Animator DevaAnime;
    SpriteRenderer DevaSprite;

    public float DevaMoveSpeed = 3f;
    public float jumpForce = 5f; // 점프 높이 설정
    public LayerMask groundLayer; // 바닥을 감지할 레이어 설정
    public Transform groundCheck; // 바닥 감지를 위한 위치
    private bool isGrounded; // 바닥에 닿아 있는지 여부 확인

    // 대쉬 변수
    private bool canDash = true;
    private bool isDashing;
    private float dashingPower = 24f;
    private float dashingTime = 0.3f;
    private float dashingCooldown = 0.5f;
    private bool lastKeyWasRight = true;

    [SerializeField] private TrailRenderer dashTrail; // 대쉬 잔상 효과

    // 잔상 효과 변수
    [SerializeField] private float afterImageInterval = 0.02f; // 잔상 생성 간격
    [SerializeField] private float afterImageLifetime = 0.5f; // 잔상 유지 시간

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
        CheckGrounded(); // 바닥 감지 업데이트

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
        DevaAnime.SetBool("isDashing", true); // 대쉬 애니메이션 실행

        if (dashTrail != null) dashTrail.emitting = true;

        StartCoroutine(LeaveAfterImage()); // 잔상 생성 시작

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

    // 잔상 생성 (대쉬 중 지속적으로 호출)
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
        sr.color = new Color(1f, 1f, 1f, 0.8f); // 반투명 흰색
        sr.flipX = DevaSprite.flipX;
        afterImage.transform.position = transform.position;
        afterImage.transform.localScale = transform.localScale;

        StartCoroutine(FadeOutAndDestroy(sr)); // 잔상 점점 사라지게 처리
    }

    private IEnumerator FadeOutAndDestroy(SpriteRenderer sr)
    {
        float fadeDuration = afterImageLifetime;
        Color originalColor = sr.color;
        float elapsed = 0f;

        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            float alpha = Mathf.Lerp(originalColor.a, 0, elapsed / fadeDuration); // 알파값 점진적 감소
            sr.color = new Color(originalColor.r, originalColor.g, originalColor.b, alpha); // 투명도 적용
            yield return null;
        }

        Destroy(sr.gameObject); // 최종적으로 잔상 오브젝트 삭제
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
            StartCoroutine(DelayedJump()); // 점프 준비 동작 후 점프 실행
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
        // 점프 준비 동작 실행 (앉기 애니메이션)
        DevaAnime.SetTrigger("Crouch");

        // 0.3초 동안 준비 동작 (이 값은 필요에 따라 조정 가능)
        yield return new WaitForSeconds(0.15f);

        // 점프 애니메이션 실행
        DevaAnime.SetTrigger("Jump");

        // 점프 적용
        DevaRigidbody.velocity = new Vector2(DevaRigidbody.velocity.x, jumpForce);
    }
}
