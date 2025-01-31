using System.Collections;
using UnityEngine;
using static UnityEditor.PlayerSettings;

public class AdamMovement : MonoBehaviour
{
    Rigidbody2D AdamRigidebody;
    Animator AdamAnime;
    SpriteRenderer AdamSprite;

    public float JumpPower = 3f;
    public float AdamMoveSpeed = 3f;
    private CharacterAttack characterAttack; // 공격 스크립트 참조

    private bool canDash = true;
    private bool isDashing;
    private bool isDashAttacking; // 대쉬 공격 상태 추가
    private bool attackInputRecently = false; // 최근 공격 입력 여부

    [SerializeField]
    private float dashingPower = 24f;
    private float dashingTime = 0.3f;
    private float dashingCooldown = 0.5f;
    [SerializeField] private TrailRenderer dashTrail;

    [SerializeField] private float afterImageInterval = 0.02f;
    [SerializeField] private float afterImageLifetime = 1f; // 잔상 유지 시간
    [SerializeField] private float attackInputCooldown = 0.2f; // 공격 후 대쉬 차단 시간
    [SerializeField] private float dashAttackDuration = 0.3f; // 대쉬 공격 지속 시간

    private bool lastKeyWasRight = true;

    //점프 변수
    bool isGround;
    public Transform JumpPos;
    public float checkRadiusJump;
    public LayerMask islayer;

    private bool isAttacking = false;

    void Start()
    {
        AdamRigidebody = GetComponent<Rigidbody2D>();
        AdamAnime = GetComponent<Animator>();
        AdamSprite = GetComponent<SpriteRenderer>();
        characterAttack = GetComponent<CharacterAttack>();
    }

    void Update()
    {

        // 현재 애니메이션 상태 확인
        AnimatorStateInfo currentState = AdamAnime.GetCurrentAnimatorStateInfo(0);

        // 공격 애니메이션 실행 중인지 확인
        bool isInAttackAnimation =  currentState.IsName("Stance") || currentState.IsName("Attack1") || currentState.IsName("Attack2")  || currentState.IsName("DashAttack") || currentState.IsName("Attack2Attack1");

        // 공격 중 또는 대쉬 공격 중이면 행동 차단
        if (isInAttackAnimation || isDashAttacking)
        {
            StopMovement(); // 이동 차단
            return; // 다른 동작 차단
        }
        
        HandleAttack();

        if (!isDashing && !attackInputRecently)
        {
            HandleMovement();
        }

        if (!isInAttackAnimation && !attackInputRecently)
        {
            HandleDash(); // 공격 애니메이션이 실행 중이 아니고 공격 입력 후 쿨다운이 아니면 대쉬 가능
        }

        HandleJump();

        AdamAnimation();
        HandleFlip();
        HandleFall();
    }

    void HandleMovement()
    {
        float hor = Input.GetAxisRaw("Horizontal");
        AdamRigidebody.velocity = new Vector2(hor * AdamMoveSpeed, AdamRigidebody.velocity.y);

        if (hor > 0)
            lastKeyWasRight = true;
        else if (hor < 0)
            lastKeyWasRight = false;
    }

    void HandleAttack()
    {
        if (Input.GetKeyDown(KeyCode.M)&&isGround) // 공격 키 입력
        {
            attackInputRecently = true; // 공격 입력 감지
            StartCoroutine(ResetAttackInputCooldown());

            if (isDashing)
            {
                StartCoroutine(DashAttack()); // 대쉬 중 공격
            }
            else if (characterAttack != null)
            {
                isAttacking = true;
                characterAttack.TriggerAttack(); // 일반 공격
            }
        }
        else
        {
            isAttacking = false;
        }
    }

    void HandleDash()
    {
        // 대쉬 불가 조건: 대쉬 중, 쿨다운 중, 또는 공격 입력 후 일정 시간 내
        if (isDashing || !canDash || attackInputRecently || !isGround)
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

        AdamAnime.SetBool("isDashing", true); // 대쉬 애니메이션 활성화

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
        AdamAnime.SetTrigger("DashAttack"); // 대쉬 공격 애니메이션 실행

        float dashDirection = lastKeyWasRight ? 1 : -1;

        // 대쉬 공격 중 일정 거리만큼 유지
        float elapsed = 0f;
        while (elapsed < dashAttackDuration)
        {
            AdamRigidebody.velocity = new Vector2(dashDirection * dashingPower, 0f);
            elapsed += Time.deltaTime;
            yield return null;
        }

        AdamRigidebody.velocity = Vector2.zero; // 대쉬 종료 시 멈춤
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
            float alpha = Mathf.Lerp(originalColor.a, 0, elapsed / fadeDuration); // 알파값 점진적 감소
            sr.color = new Color(originalColor.r, originalColor.g, originalColor.b, alpha); // 투명도 적용
            yield return null;
        }

        Destroy(sr.gameObject); // 최종적으로 잔상 오브젝트 삭제
    }

    private IEnumerator ResetAttackInputCooldown()
    {
        yield return new WaitForSeconds(attackInputCooldown);
        attackInputRecently = false; // 공격 입력 상태 초기화
    }

    void StopMovement()
    {
        AdamRigidebody.velocity = Vector2.zero; // 이동 차단
    }

    void AdamAnimation()
    {
        float hor = Input.GetAxisRaw("Horizontal");
        AdamAnime.SetBool("run", Mathf.Abs(hor) > 0.00f);
    }

    void HandleFlip()
    {
        float hor = Input.GetAxisRaw("Horizontal");
        if (hor != 0)
        {
            AdamSprite.flipX = hor < 0;
        }
    }

    void HandleJump()
    {
        isGround = Physics2D.OverlapCircle(JumpPos.position, checkRadiusJump, islayer);
        bool isJumping = AdamAnime.GetCurrentAnimatorStateInfo(0).IsName("Jump 1");
       

        if (Input.GetKeyDown(KeyCode.Space) && isGround && !isJumping)
        {
            Debug.Log("Jumping...");
            StartCoroutine(DelayedJump());
        }
    }

    private IEnumerator DelayedJump()
    {
        // 점프 애니메이션 실행
        AdamAnime.SetTrigger("Jump");

        // 0.2초 대기 후 점프 힘 적용
        yield return new WaitForSeconds(0.2f);

        AdamRigidebody.velocity = new Vector2(AdamRigidebody.velocity.x, JumpPower);

        Debug.Log("Jump executed after delay!");
    }

    void HandleFall()
    {
        if (!isGround && AdamRigidebody.velocity.y < 0)
        {
            AdamAnime.SetBool("Fall",true); // Fall 애니메이션 실행
        }
        else
        {
            AdamAnime.SetBool("Fall", false);
        }
    }

    private void OnDrawGizmos()
    {
        if (JumpPos != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(JumpPos.position, checkRadiusJump);
        }
    }
}
