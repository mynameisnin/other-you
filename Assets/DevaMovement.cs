using System.Collections;
using UnityEngine;

public class DebaraMovement : MonoBehaviour
{
    Rigidbody2D DebaraRigidbody;
    Animator DebaraAnime;
    SpriteRenderer DebaraSprite;

    public float JumpPower = 3f;
    public float MoveSpeed = 3f;
    private MagicAttack magicAttack; // 마법 공격 스크립트 참조
    private EnergyBarUI energyBarUI; // 에너지 UI 추가

    private bool canTeleport = true;
    private bool isTeleporting;
    private bool attackInputRecently = false;

    [SerializeField]
    private float teleportDistance = 5f;
    private float teleportCooldown = 1.0f;
    [SerializeField] private TrailRenderer teleportTrail;

    [SerializeField] private float teleportEnergyCost = 20f;
    [SerializeField] private float attackInputCooldown = 0.2f;

    public bool lastKeyWasRight = true;

    // 점프 변수
    public bool isGround;
    public Transform JumpPos;
    public float checkRadiusJump;
    public LayerMask isLayer;

    public bool isAttacking = false;
    public bool isInvincible { get; private set; }
    [SerializeField] private Transform jumpAttackCheckPos; // Ray 시작 지점
    [SerializeField] private float jumpAttackRayLength = 1.5f; // 레이 길이
    [SerializeField] private LayerMask jumpAttackBlockLayer; // 감지할 레이어
    void Start()
    {
        DebaraRigidbody = GetComponent<Rigidbody2D>();
        DebaraAnime = GetComponent<Animator>();
        DebaraSprite = GetComponent<SpriteRenderer>();
        magicAttack = GetComponent<MagicAttack>();
        energyBarUI = FindObjectOfType<EnergyBarUI>(); // EnergyBarUI 찾기
    }

    void Update()
    {
        AnimatorStateInfo currentState = DebaraAnime.GetCurrentAnimatorStateInfo(0);

        bool isInAttackAnimation = currentState.IsName("Cast1") || currentState.IsName("Cast2");

        if (isInAttackAnimation)
        {
            StopMovement();
            return;
        }

        HandleAttack();

        if (!isTeleporting && !attackInputRecently)
        {
            HandleMovement();
        }

        if (!isInAttackAnimation && !attackInputRecently)
        {
            HandleTeleport();
        }

        HandleJump();
        DebaraAnimation();
        HandleFlip();
        HandleFall();
    }

    float currentSpeed = 0f;
    float acceleration = 8f;
    float deceleration = 12f;
    float maxSpeed = 4f;

    void HandleMovement()
    {
        if (isAttacking) return; //  공격 중에는 이동 불가

        float hor = Input.GetAxisRaw("Horizontal");

        if (hor != 0)
        {
            currentSpeed = Mathf.Lerp(currentSpeed, hor * maxSpeed, Time.deltaTime * acceleration);
        }
        else
        {
            currentSpeed = Mathf.Lerp(currentSpeed, 0, Time.deltaTime * (deceleration * 2f));
        }

        DebaraRigidbody.velocity = new Vector2(currentSpeed, DebaraRigidbody.velocity.y);
    }
    bool IsJumpAttackBlocked()
    {
        if (jumpAttackCheckPos == null) return false;

        RaycastHit2D hit = Physics2D.Raycast(jumpAttackCheckPos.position, Vector2.down, jumpAttackRayLength, jumpAttackBlockLayer);
        return hit.collider != null;
    }
    void HandleAttack()
    {
        if (isAttacking) return;

        if (Input.GetKeyDown(KeyCode.LeftControl))
        {
            attackInputRecently = true;
            isAttacking = true;
            StartCoroutine(ResetAttackInputCooldown());

            if (magicAttack != null)
            {
                if (!isGround)
                {
                    if (IsJumpAttackBlocked())
                    {
                        isAttacking = false;
                        attackInputRecently = false;
                        Debug.Log("Jump Attack Blocked by Raycast!");
                        return;
                    }

                    DebaraAnime.Play("JumpAttack", 0, 0);
                    // 공중 공격 시 StopMovement 호출 X
                }
                else
                {
                    DebaraAnime.Play("Attack", 0, 0);
                    StopMovement(); // 지상에서는 움직임 멈추도록 유지
                }
            }
        }
    }



    public void EndAttack()
    {
        isAttacking = false; // 공격 상태 해제
    }
    void HandleTeleport()
    {
        if (isTeleporting || !canTeleport || attackInputRecently || !isGround)
        {
            return;
        }

        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            float currentEnergy = energyBarUI != null ? energyBarUI.GetCurrentEnergy() : 0f;

            if (currentEnergy < teleportEnergyCost && energyBarUI != null)
            {
                Debug.Log("텔레포트 불가: ENERGY 부족!");
                energyBarUI.FlashBorder();
                return;
            }

            StartCoroutine(Teleport());
        }
    }

    public GameObject teleportStartEffectPrefab; // 출발 이펙트 프리팹
    public GameObject teleportEndEffectPrefab; // 도착 이펙트 프리팹
    public Transform teleportStartEffectPosition; // 출발 이펙트 위치 지정용 빈 오브젝트
    public Transform teleportEndEffectPosition; // 도착 이펙트 위치 지정용 빈 오브젝트
    private IEnumerator Teleport()
    {
        if (energyBarUI != null)
        {
            energyBarUI.ReduceEnergy(teleportEnergyCost);
        }

        canTeleport = false;
        isTeleporting = true;
        isInvincible = true;

        DebaraAnime.SetTrigger("Teleport");

        if (teleportTrail != null) teleportTrail.emitting = true;

        float teleportDirection = DebaraSprite.flipX ? -1f : 1f;
        Vector2 startPosition = transform.position; // 기존 위치 유지
        Vector2 targetPosition = new Vector2(transform.position.x + (teleportDistance * teleportDirection), transform.position.y);

        //  출발 위치에 이펙트 생성 (빈 오브젝트 위치 기반)
        if (teleportStartEffectPrefab != null && teleportStartEffectPosition != null)
        {
            GameObject startEffect = Instantiate(teleportStartEffectPrefab, teleportStartEffectPosition.position, Quaternion.identity);
            Destroy(startEffect, 0.3f); // 1.5초 후 자동 삭제
        }

        yield return new WaitForSeconds(0.2f); // 텔레포트 딜레이

        transform.position = targetPosition; // 텔레포트 위치는 유지

        //  도착 위치에 이펙트 생성 (빈 오브젝트 위치 기반)
        if (teleportEndEffectPrefab != null && teleportEndEffectPosition != null)
        {
            GameObject endEffect = Instantiate(teleportEndEffectPrefab, teleportEndEffectPosition.position, Quaternion.identity);
            Destroy(endEffect, 0.5f); // 1.5초 후 자동 삭제
        }

        if (teleportTrail != null) teleportTrail.emitting = false;

        isTeleporting = false;
        isInvincible = false;

        yield return new WaitForSeconds(teleportCooldown);
        canTeleport = true;
    }

    private IEnumerator ResetAttackInputCooldown()
    {
        yield return new WaitForSeconds(attackInputCooldown);
        attackInputRecently = false;
    }

    void StopMovement()
    {
        DebaraRigidbody.velocity = Vector2.zero;
        currentSpeed = 0f;
    }

    void DebaraAnimation()
    {
        float hor = Input.GetAxisRaw("Horizontal");
        DebaraAnime.SetBool("run", Mathf.Abs(hor) > 0.00f);
        bool rising = DebaraRigidbody.velocity.y > 0.1f && !isGround && !(DebaraRigidbody.velocity.y < 0);
        DebaraAnime.SetBool("isJumping", rising);
    }

    void HandleFlip()
    {
        if (isAttacking) return; // 공격 중에는 방향 전환 금지

        float hor = Input.GetAxisRaw("Horizontal");
        if (hor != 0)
        {
            DebaraSprite.flipX = hor < 0;
        }
    }

    void HandleJump()
    {
        isGround = Physics2D.OverlapCircle(JumpPos.position, checkRadiusJump, isLayer);
        AnimatorStateInfo currentState = DebaraAnime.GetCurrentAnimatorStateInfo(0);
        bool isJumping = currentState.IsName("Jump");

        //  공격 중이면 점프 금지
        if (isAttacking)
        {
            return;
        }

        //  일반 공격 중에는 점프 금지
        if (currentState.IsName("Attack"))
        {
            return;
        }

        if (isTeleporting || (!isGround && DebaraRigidbody.velocity.y < 0))
        {
            return;
        }

        if (Input.GetKeyDown(KeyCode.Space) && isGround && !isJumping)
        {
            Debug.Log("Jumping...");
            DebaraRigidbody.velocity = new Vector2(DebaraRigidbody.velocity.x, JumpPower);
        }
    }



    void HandleFall()
    {
        if (!isGround && DebaraRigidbody.velocity.y < 0)
        {
            DebaraAnime.SetBool("Fall", true);
        }
        else
        {
            DebaraAnime.SetBool("Fall", false);
        }
    }

    private void OnDrawGizmos()
    {
        if (JumpPos != null)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(JumpPos.position, checkRadiusJump);
        }

        if (jumpAttackCheckPos != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(jumpAttackCheckPos.position, jumpAttackCheckPos.position + Vector3.down * jumpAttackRayLength);
        }
    }
}
