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

    void HandleAttack()
    {
        if (Input.GetKeyDown(KeyCode.LeftControl) && isGround)
        {
            attackInputRecently = true;
            StartCoroutine(ResetAttackInputCooldown());

            if (magicAttack != null)
            {
                isAttacking = true;
                magicAttack.CastMagic();
            }
        }
        else
        {
            isAttacking = false;
        }
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
        Vector2 targetPosition = new Vector2(transform.position.x + (teleportDistance * teleportDirection), transform.position.y);

        transform.position = targetPosition;

        yield return new WaitForSeconds(0.2f);

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
    }

    void HandleFlip()
    {
        float hor = Input.GetAxisRaw("Horizontal");
        if (hor != 0)
        {
            DebaraSprite.flipX = hor < 0;
        }
    }

    void HandleJump()
    {
        isGround = Physics2D.OverlapCircle(JumpPos.position, checkRadiusJump, isLayer);
        bool isJumping = DebaraAnime.GetCurrentAnimatorStateInfo(0).IsName("Jump 1");

        if (isTeleporting || (!isGround && DebaraRigidbody.velocity.y < 0))
        {
            return;
        }

        if (Input.GetKeyDown(KeyCode.Space) && isGround && !isJumping)
        {
            Debug.Log("Jumping...");
            DebaraAnime.SetTrigger("Jump");
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
    }
}
