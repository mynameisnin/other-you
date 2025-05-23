using System.Collections;
using UnityEngine;


public class AdamMovement : MonoBehaviour
{
    Rigidbody2D AdamRigidebody;
    Animator AdamAnime;
    SpriteRenderer AdamSprite;

    public float JumpPower = 3f;
    public float AdamMoveSpeed = 3f;
    private CharacterAttack characterAttack; // 공격 스크립트 참조
    private EnergyBarUI energyBarUI; //  에너지 UI 추가

    private bool canDash = true;
    private bool isDashing;
    private bool isDashAttacking; // 대쉬 공격 상태 추가
    private bool attackInputRecently = false; // 최근 공격 입력 여부

    [SerializeField]
    private float dashingPower = 24f;
    private float dashingTime = 0.3f;
    private float dashingCooldown = 0.5f;
    [SerializeField] private TrailRenderer dashTrail;

    [SerializeField] private float dashEnergyCost = 15f; //  대쉬 시 에너지 소모량
    [SerializeField] private float afterImageInterval = 0.02f;
    [SerializeField] private float afterImageLifetime = 1f; // 잔상 유지 시간
    public float attackInputCooldown = 0.2f; // 공격 후 대쉬 차단 시간
    [SerializeField] private float dashAttackDuration = 0.3f; // 대쉬 공격 지속 시간

    public bool lastKeyWasRight = true;

    //점프 변수
    public bool isGround;
    public Transform JumpPos;
    public float checkRadiusJump;
    public LayerMask islayer;

    public bool isAttacking = false;
    public bool isInvincible { get; set; }


    public BladeExhaustSkill bladeSkill;

    private DownJump currentPlatformComponent;
    private Transform currentPlatformPosition;
    private Vector3 lastPlatformPos;
    void Start()
    {
        AdamRigidebody = GetComponent<Rigidbody2D>();
        AdamAnime = GetComponent<Animator>();
        AdamSprite = GetComponent<SpriteRenderer>();
        characterAttack = GetComponent<CharacterAttack>();

    }

    void Update()
    {
        if (GetComponent<AdamUltimateSkill>()?.enabled == true &&
       GetComponent<AdamUltimateSkill>().isActiveAndEnabled &&
       GetComponent<AdamUltimateSkill>().isCasting)
        {
            StopMovement();
            return;
        }
        if (bladeSkill != null)
        {
            // Debug.Log($"[AdamMovement] isSlashing: {bladeSkill.isSlashing}");
        }

        if (bladeSkill != null && bladeSkill.isSlashing)
        {
            Debug.Log("[AdamMovement] 블레이드 슬래시 중이라 이동 차단됨");
            StopMovement();
            return;
        }
        if (Input.GetKeyDown(KeyCode.X))
        {
            if (bladeSkill != null)
                bladeSkill.StartBladeSlash();
        }

        // 현재 애니메이션 상태 확인
        AnimatorStateInfo currentState = AdamAnime.GetCurrentAnimatorStateInfo(0);

        // 공격 애니메이션 실행 중인지 확인
        bool isInAttackAnimation = currentState.IsName("Stance") || currentState.IsName("Attack1") || currentState.IsName("Attack2") || currentState.IsName("DashAttack") || currentState.IsName("Attack2Attack1");

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
        FollowPlatform();
    }

    float currentSpeed = 0f;
    float acceleration = 10f;
    float deceleration = 15f;
    public float maxSpeed = 6f;

    void HandleMovement()
    {
        float hor = Input.GetAxisRaw("Horizontal");

        if (isDashAttacking) //  공격 중일 때 강제 정지
        {
            currentSpeed = 0f;
            return;
        }

        if (hor != 0)
        {
            currentSpeed = Mathf.Lerp(currentSpeed, hor * maxSpeed, Time.deltaTime * acceleration);
        }
        else
        {
            //  감속 속도를 빠르게 해서 공격 후 불필요한 이동을 방지
            currentSpeed = Mathf.Lerp(currentSpeed, 0, Time.deltaTime * (deceleration * 2f));
        }

        AdamRigidebody.velocity = new Vector2(currentSpeed, AdamRigidebody.velocity.y);
    }

    void HandleAttack()
    {
        if (Input.GetKeyDown(KeyCode.LeftControl) && isGround) // 공격 키 입력
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
        AnimatorStateInfo currentState = AdamAnime.GetCurrentAnimatorStateInfo(0);

        if (isDashing || !canDash || attackInputRecently || !isGround || currentState.IsName("Jump 1"))
        {
            return;
        }

        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            float currentEnergy = PlayerStats.Instance.currentEnergy;

            if (currentEnergy < dashEnergyCost)
            {
                Debug.Log("대쉬 불가: ENERGY 부족!");
                EnergyBarUI.Instance.FlashBorder(); // 테두리 깜빡이기
                return;
            }

            //  에너지가 충분하면 대쉬 실행
            StartCoroutine(Dash());
        }
    }

    private IEnumerator Dash()
    {
        EnergyBarUI.Instance.ReduceEnergy(dashEnergyCost);

        canDash = false;
        isDashing = true;
        isInvincible = true;

        AdamAnime.SetBool("isDashing", true);

        if (dashTrail != null) dashTrail.emitting = true;

        StartCoroutine(LeaveAfterImage());

        float originalGravity = AdamRigidebody.gravityScale;
        AdamRigidebody.gravityScale = 0f;

        //  **현재 입력을 기반으로 방향 결정!**
        float hor = Input.GetAxisRaw("Horizontal");
        float dashDirection = (hor != 0) ? hor : (AdamSprite.flipX ? -1 : 1);

        AdamRigidebody.velocity = new Vector2(dashDirection * dashingPower, 0f);

        yield return new WaitForSeconds(dashingTime);

        if (dashTrail != null) dashTrail.emitting = false;

        AdamRigidebody.gravityScale = originalGravity;
        isDashing = false;
        isInvincible = false;

        AdamAnime.SetBool("isDashing", false);

        yield return new WaitForSeconds(dashingCooldown);
        canDash = true;
    }

    private IEnumerator DashAttack()
    {
        isDashAttacking = true;
        isInvincible = true;

        AdamAnime.SetTrigger("DashAttack");

        float dashDirection = AdamSprite.flipX ? -1f : 1f;

        if (dashTrail != null)
            dashTrail.emitting = true;

        float elapsed = 0f;
        while (elapsed < dashAttackDuration)
        {
            AdamRigidebody.velocity = new Vector2(dashDirection * dashingPower, 0f);
            elapsed += Time.deltaTime;
            yield return null;
        }

        // 감속
        float decelerationTime = 0.2f;
        while (decelerationTime > 0)
        {
            AdamRigidebody.velocity = new Vector2(AdamRigidebody.velocity.x * 0.8f, AdamRigidebody.velocity.y);
            decelerationTime -= Time.deltaTime;
            yield return null;
        }

        //  여기서 isDashAttacking 먼저 해제 (이제 이동 가능)
        isDashAttacking = false;

        if (dashTrail != null)
            dashTrail.emitting = false;

        AdamRigidebody.velocity = Vector2.zero;

        //  무적은 0.3초 더 유지 (이동은 가능하게)
        yield return new WaitForSeconds(0.3f);

        isInvincible = false;
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
        afterImage.tag = "AfterImage";
        SpriteRenderer sr = afterImage.AddComponent<SpriteRenderer>();

        sr.sprite = AdamSprite.sprite;
        sr.color = new Color(0.5f, 0.5f, 0.5f, 1f);
        sr.flipX = AdamSprite.flipX;
        afterImage.transform.position = transform.position;
        afterImage.transform.localScale = transform.localScale;

        StartCoroutine(FadeOutAndDestroy(sr));
    }

    private IEnumerator FadeOutAndDestroy(SpriteRenderer sr)
    {
        if (sr == null) yield break; //  SpriteRenderer가 없으면 바로 종료

        float fadeDuration = afterImageLifetime;
        Color originalColor = sr.color;
        float elapsed = 0f;

        while (elapsed < fadeDuration)
        {
            if (sr == null) yield break; //  실행 도중 삭제되었으면 중단

            elapsed += Time.deltaTime;
            float alpha = Mathf.Lerp(originalColor.a, 0, elapsed / fadeDuration);
            sr.color = new Color(originalColor.r, originalColor.g, originalColor.b, alpha);
            yield return null;
        }

        if (sr != null && sr.gameObject != null) Destroy(sr.gameObject); //  null 체크 후 삭제
    }

    private IEnumerator ResetAttackInputCooldown()
    {
        yield return new WaitForSeconds(attackInputCooldown);
        attackInputRecently = false; // 공격 입력 상태 초기화
    }

    public void StopMovement()
    {
        AdamRigidebody.velocity = Vector2.zero; // 이동 차단
        currentSpeed = 0f; //  남아있는 이동 속도 제거
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

        // 대쉬 중이거나 떨어지는 중이면 점프 불가
        if (isDashing || (!isGround && AdamRigidebody.velocity.y < 0))
        {
            return;
        }

        if (Input.GetKey(KeyCode.DownArrow) && Input.GetKeyDown(KeyCode.Space))
        {
            if (currentPlatformComponent != null)
            {
                Debug.Log("플레이어가 밟은 발판 작동!");
                isGround = false;
                currentPlatformComponent.TriggerDownJump();  // 발판에서 실행할 함수 호출
            }
        }

        if (Input.GetKeyDown(KeyCode.Space) && isGround && !isJumping)
        {
            Debug.Log("Jumping...");
            AdamAnime.SetTrigger("Jump"); // 점프 애니메이션 실행 (한 번만)
            AdamRigidebody.velocity = new Vector2(AdamRigidebody.velocity.x, JumpPower);
        }
    }

    void FollowPlatform()
    {
        if (currentPlatformPosition != null) 
        {
            Vector3 platformDelta = currentPlatformPosition.position - lastPlatformPos;
            transform.position += platformDelta; // 발판이 움직인 만큼 따라감
            lastPlatformPos = currentPlatformPosition.position;
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Platform"))
        {
            Debug.Log("발판감지:" + collision.gameObject);
            currentPlatformComponent = collision.gameObject.GetComponent<DownJump>();
            currentPlatformPosition = collision.transform;
            lastPlatformPos = currentPlatformPosition.position;
        }
    }

    void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Platform"))
        {
            currentPlatformComponent = null;
            currentPlatformPosition = null;
        }
    }


    void HandleFall()
    {

        if (!isGround && AdamRigidebody.velocity.y < 0)
        {
            AdamAnime.SetBool("Fall", true); // Fall 애니메이션 실행
        }
        else
        {
            AdamAnime.SetBool("Fall", false);
        }
    }
    public void ForceStopDash()
    {
        isDashing = false;
        isDashAttacking = false;
        isInvincible = false;
        canDash = true;

        // 이동 정지
        AdamRigidebody.velocity = Vector2.zero;
        currentSpeed = 0f;

        // Trail 꺼주기
        if (dashTrail != null)
        {
            dashTrail.emitting = false;
        }

        // 애니메이션 상태 리셋
        if (AdamAnime != null)
        {
            AdamAnime.SetBool("isDashing", false);
            AdamAnime.ResetTrigger("DashAttack");
            AdamAnime.SetBool("run", false);
        }

        //  코루틴 강제 중지
        StopAllCoroutines();

        //  남아 있는 AfterImage 오브젝트 제거
        var afterImages = GameObject.FindGameObjectsWithTag("AfterImage");
        foreach (var img in afterImages)
        {
            Destroy(img);
        }
    }
    [Header("텔레포트 거리")]
    public float slashTeleportDistance = 2.5f;

    public void PerformSlashTeleport()
    {
        float direction = AdamSprite.flipX ? -1f : 1f;
        Vector2 origin = (Vector2)transform.position + new Vector2(0, 0.5f) + new Vector2(direction * 0.3f, 0); // 약간 앞쪽에서 쏨
        float maxDistance = slashTeleportDistance;

        RaycastHit2D hit = Physics2D.Raycast(origin, Vector2.right * direction, maxDistance, islayer);

        Vector2 targetPosition;

        if (hit.collider != null)
        {
            float safeDistance = Mathf.Max(0f, hit.distance - 0.05f); // 벽 전까지
            targetPosition = (Vector2)transform.position + new Vector2(direction * safeDistance, 0);
            Debug.Log($"[슬래시텔레포트] 벽 감지됨 → 안전 거리만큼 이동: {safeDistance:F2}");
        }
        else
        {
            targetPosition = (Vector2)transform.position + new Vector2(direction * maxDistance, 0);
            Debug.Log($"[슬래시텔레포트] 벽 없음 → 최대 거리 이동: {maxDistance:F2}");
        }

        transform.position = targetPosition;

        // 디버그용 레이 그리기
        Debug.DrawRay(origin, Vector2.right * direction * maxDistance, Color.yellow, 1f);
    }

    [Header("스킬 먼지 이펙트")]
    public GameObject dustEffectPrefab;
    public Transform dustSpawnPoint;
    public void SpawnDustBeforeTeleport()
    {
        if (dustEffectPrefab != null && dustSpawnPoint != null)
        {
            Vector2 spawnPosition = dustSpawnPoint.position;

            if (AdamSprite != null && AdamSprite.flipX)
            {
                float offsetX = dustSpawnPoint.position.x - transform.position.x;
                spawnPosition.x = transform.position.x - offsetX;
            }

            GameObject dust = Instantiate(dustEffectPrefab, spawnPosition, Quaternion.identity);
            Destroy(dust, 0.7f);

            SpriteRenderer dustSprite = dust.GetComponent<SpriteRenderer>();
            if (dustSprite != null)
            {
                dustSprite.flipX = AdamSprite != null && AdamSprite.flipX;
            }
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
