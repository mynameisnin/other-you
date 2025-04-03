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
    private DevaEnergyBarUI DevaEnergyBarUI; // 에너지 UI 추가

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
    private Vector2? pendingTeleportTarget = null;
    void Start()
    {
        DebaraRigidbody = GetComponent<Rigidbody2D>();
        DebaraAnime = GetComponent<Animator>();
        DebaraSprite = GetComponent<SpriteRenderer>();
        magicAttack = GetComponent<MagicAttack>();
        DevaEnergyBarUI = FindObjectOfType<DevaEnergyBarUI>(); // EnergyBarUI 찾기
    }

    void Update()
    {
        AnimatorStateInfo currentState = DebaraAnime.GetCurrentAnimatorStateInfo(0);

        bool isInAttackAnimation = currentState.IsName("Cast1") || currentState.IsName("Cast2");

        if (isInAttackAnimation && isAttacking) // ← 진짜 공격 중일 때만 막음
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
        if (Input.GetKeyDown(KeyCode.X) && !isAttacking)
        {
            CastLaserSkill();
        }
    }

    float currentSpeed = 0f;
    float acceleration = 8f;
    float deceleration = 12f;
    public float maxSpeed = 4f;

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
            float currentEnergy = DevaEnergyBarUI != null ? DevaEnergyBarUI.GetCurrentEnergy() : 0f;

            // ?? 에너지 부족 시 텔레포트 불가 처리
            if (currentEnergy < teleportEnergyCost && DevaEnergyBarUI != null)
            {
                Debug.Log("텔레포트 불가: ENERGY 부족!");
                DevaEnergyBarUI.FlashBorder(); // UI 깜빡임 효과
                return;
            }

            StartCoroutine(Teleport()); // 텔레포트 실행
        }
    }
    public GameObject teleportStartEffectPrefab; // 출발 이펙트 프리팹
    public GameObject teleportEndEffectPrefab; // 도착 이펙트 프리팹
    public Transform teleportStartEffectPosition; // 출발 이펙트 위치 지정용 빈 오브젝트
    public Transform teleportEndEffectPosition; // 도착 이펙트 위치 지정용 빈 오브젝트

    private IEnumerator Teleport()
    {
        // ? 에너지 차감
        if (DevaEnergyBarUI != null)
        {
            DevaEnergyBarUI.ReduceEnergy(teleportEnergyCost);
        }

        canTeleport = false;
        isTeleporting = true;
        isInvincible = true;

        

        if (teleportTrail != null)
            teleportTrail.emitting = true;

        float teleportDirection = DebaraSprite.flipX ? -1f : 1f;
        Vector2 targetPosition = new Vector2(transform.position.x + (teleportDistance * teleportDirection), transform.position.y);
        pendingTeleportTarget = targetPosition;
        // 출발 이펙트
        if (teleportStartEffectPrefab != null && teleportStartEffectPosition != null)
        {
            GameObject startEffect = Instantiate(teleportStartEffectPrefab, teleportStartEffectPosition.position, Quaternion.identity);
            Destroy(startEffect, 0.3f);
        }

        yield return new WaitForSeconds(0.2f); // 텔레포트 딜레이

        transform.position = targetPosition;
        pendingTeleportTarget = null;
        // 도착 이펙트
        if (teleportEndEffectPrefab != null && teleportEndEffectPosition != null)
        {
            GameObject endEffect = Instantiate(teleportEndEffectPrefab, teleportEndEffectPosition.position, Quaternion.identity);
            Destroy(endEffect, 0.5f);
        }

        if (teleportTrail != null)
            teleportTrail.emitting = false;

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

    public void StopMovement()
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
    public void ResetState()
    {
        isAttacking = false;
        attackInputRecently = false;
        isTeleporting = false;
        isInvincible = false;
        canTeleport = true;
        currentSpeed = 0f;

        if (DebaraAnime != null)
        {
            DebaraAnime.ResetTrigger("Attack");
            DebaraAnime.SetBool("run", false);
            DebaraAnime.SetBool("isJumping", false);
            DebaraAnime.SetBool("Fall", false);
        }

        // 움직임 정지
        if (DebaraRigidbody != null)
        {
            DebaraRigidbody.velocity = Vector2.zero;
        }
    }
    public void ForceEndAttack()
    {
        isAttacking = false;
        attackInputRecently = false;

        if (DebaraAnime != null && gameObject.activeInHierarchy)
        {
            DebaraAnime.ResetTrigger("Attack");
            DebaraAnime.Play("DevaIdle"); // ← 활성화 상태일 때만 실행
        }

        if (magicAttack != null)
        {
            magicAttack.EndAttacks();
        }
    }
    public void ForceCancelTeleport()
    {
        if (isTeleporting)
        {
            isTeleporting = false;
            canTeleport = true;
            isInvincible = false;

            if (teleportTrail != null)
            {
                teleportTrail.emitting = false;
            }

            // 위치 보정
            if (pendingTeleportTarget.HasValue)
            {
                transform.position = pendingTeleportTarget.Value;
                pendingTeleportTarget = null;
            }

            StopAllCoroutines();
        }
    }
    [SerializeField] private GameObject laserPrefab;
    [SerializeField] private Transform laserSpawnOrigin; // 시작 위치 (Deva 앞쪽)

    [SerializeField] private int laserCount = 6; // 몇 번 소환할지 (예: 6번)
    [SerializeField] private float interval = 0.1f; // 소환 간격
    [SerializeField] private float spawnDistanceStep = 0.5f; // 각 소환마다 앞쪽으로 얼마나 이동할지

    [SerializeField] private float laserManaCost = 40f; // 레이저 스킬 마나 소모량

    [Header("UI 연결")]
    public SkillCooldownUI laserCooldownUI; // 레이저 스킬 쿨타임 UI 연결

    [Header("쿨타임 설정")]
    public float laserCooldown = 6f; // 레이저 스킬 쿨타임 시간
    private bool isLaserOnCooldown = false;
    private float laserCooldownEndTime = 0f;



    public void CastLaserSkill()
    {
        // ? 쿨타임 체크는 Time.time 기준
        if (Time.time < laserCooldownEndTime)
        {
            Debug.Log("레이저 스킬 쿨타임 중");
            return;
        }

        if (!isGround)
        {
            Debug.Log("공중에서는 스킬 사용 불가");
            return;
        }

        if (!DevaStats.Instance.HasEnoughMana((int)laserManaCost))
        {
            Debug.Log("마나 부족!");
            if (DevaManaBarUI.Instance != null)
                DevaManaBarUI.Instance.FlashBorder();
            return;
        }

        // 마나 차감
        DevaStats.Instance.ReduceMana((int)laserManaCost);

        // 쿨타임 시작 (시간 설정)
        laserCooldownEndTime = Time.time + laserCooldown;

        if (laserCooldownUI != null)
        {
            laserCooldownUI.cooldownTime = laserCooldown;
            laserCooldownUI.StartCooldown(); // 기존대로
        }

        DebaraAnime.Play("Cast1");
        isAttacking = true;
    }




    [SerializeField] private float offsetX = 0.5f;
    [SerializeField] private float offsetY = 0f;

    private IEnumerator SpawnLaserSequence()
    {
        isAttacking = true;

        Vector3 direction = DebaraSprite.flipX ? Vector3.left : Vector3.right;
        Vector3 startPos = transform.position + new Vector3((DebaraSprite.flipX ? -1 : 1) * offsetX, offsetY, 0);

        for (int i = 0; i < laserCount; i++)
        {
            Vector3 spawnPos = startPos + direction * spawnDistanceStep * i;
            GameObject laser = Instantiate(laserPrefab, spawnPos, Quaternion.identity);

            if (DebaraSprite.flipX)
            {
                // 스프라이트 반전
                Vector3 scale = laser.transform.localScale;
                scale.x = Mathf.Abs(scale.x) * -1f;
                laser.transform.localScale = scale;

                // 전체 오브젝트 방향 (회전)
                laser.transform.rotation = Quaternion.Euler(0, 180f, 0);
            }
            else
            {
                // 오른쪽일 때는 기본 방향
                Vector3 scale = laser.transform.localScale;
                scale.x = Mathf.Abs(scale.x);
                laser.transform.localScale = scale;

                laser.transform.rotation = Quaternion.identity;
            }

            yield return new WaitForSeconds(interval);
        }

        
        StartCoroutine(ResetAttackInputCooldown());
    }
    public void FireLaser()
    {
        StartCoroutine(SpawnLaserSequence());
    }
    public void EndLaserAttack()
    {
        isAttacking = false;
    }
    public void ResetLaserSkill()
    {
        isLaserOnCooldown = false;
        isAttacking = false;
        StopAllCoroutines(); // 레이저 코루틴 정지

        if (laserCooldownUI != null)
        {
            // FillAmount 즉시 초기화
            laserCooldownUI.cooldownTime = 0f;
        }

        if (DebaraAnime != null && gameObject.activeInHierarchy)
        {
            DebaraAnime.ResetTrigger("Cast1");
            DebaraAnime.Play("DevaIdle");
        }

        Debug.Log("Deba 레이저 스킬 상태 초기화 완료");
    }
    [SerializeField] private DevaBigLaserSkill bigLaserSkill; // 인스펙터 연결

    public void EndBigLaserFromAnimation()
    {
        if (bigLaserSkill != null)
            bigLaserSkill.EndBigLaser();
    }

}
