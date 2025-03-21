using System.Collections;
using UnityEngine;

public class DebaraMovement : MonoBehaviour
{
    Rigidbody2D DebaraRigidbody;
    Animator DebaraAnime;
    SpriteRenderer DebaraSprite;

    public float JumpPower = 3f;
    public float MoveSpeed = 3f;
    private MagicAttack magicAttack; // ���� ���� ��ũ��Ʈ ����
    private EnergyBarUI energyBarUI; // ������ UI �߰�

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

    // ���� ����
    public bool isGround;
    public Transform JumpPos;
    public float checkRadiusJump;
    public LayerMask isLayer;

    public bool isAttacking = false;
    public bool isInvincible { get; private set; }
    [SerializeField] private Transform jumpAttackCheckPos; // Ray ���� ����
    [SerializeField] private float jumpAttackRayLength = 1.5f; // ���� ����
    [SerializeField] private LayerMask jumpAttackBlockLayer; // ������ ���̾�
    void Start()
    {
        DebaraRigidbody = GetComponent<Rigidbody2D>();
        DebaraAnime = GetComponent<Animator>();
        DebaraSprite = GetComponent<SpriteRenderer>();
        magicAttack = GetComponent<MagicAttack>();
        energyBarUI = FindObjectOfType<EnergyBarUI>(); // EnergyBarUI ã��
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
        if (isAttacking) return; //  ���� �߿��� �̵� �Ұ�

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
                    // ���� ���� �� StopMovement ȣ�� X
                }
                else
                {
                    DebaraAnime.Play("Attack", 0, 0);
                    StopMovement(); // ���󿡼��� ������ ���ߵ��� ����
                }
            }
        }
    }



    public void EndAttack()
    {
        isAttacking = false; // ���� ���� ����
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
                Debug.Log("�ڷ���Ʈ �Ұ�: ENERGY ����!");
                energyBarUI.FlashBorder();
                return;
            }

            StartCoroutine(Teleport());
        }
    }

    public GameObject teleportStartEffectPrefab; // ��� ����Ʈ ������
    public GameObject teleportEndEffectPrefab; // ���� ����Ʈ ������
    public Transform teleportStartEffectPosition; // ��� ����Ʈ ��ġ ������ �� ������Ʈ
    public Transform teleportEndEffectPosition; // ���� ����Ʈ ��ġ ������ �� ������Ʈ
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
        Vector2 startPosition = transform.position; // ���� ��ġ ����
        Vector2 targetPosition = new Vector2(transform.position.x + (teleportDistance * teleportDirection), transform.position.y);

        //  ��� ��ġ�� ����Ʈ ���� (�� ������Ʈ ��ġ ���)
        if (teleportStartEffectPrefab != null && teleportStartEffectPosition != null)
        {
            GameObject startEffect = Instantiate(teleportStartEffectPrefab, teleportStartEffectPosition.position, Quaternion.identity);
            Destroy(startEffect, 0.3f); // 1.5�� �� �ڵ� ����
        }

        yield return new WaitForSeconds(0.2f); // �ڷ���Ʈ ������

        transform.position = targetPosition; // �ڷ���Ʈ ��ġ�� ����

        //  ���� ��ġ�� ����Ʈ ���� (�� ������Ʈ ��ġ ���)
        if (teleportEndEffectPrefab != null && teleportEndEffectPosition != null)
        {
            GameObject endEffect = Instantiate(teleportEndEffectPrefab, teleportEndEffectPosition.position, Quaternion.identity);
            Destroy(endEffect, 0.5f); // 1.5�� �� �ڵ� ����
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
        if (isAttacking) return; // ���� �߿��� ���� ��ȯ ����

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

        //  ���� ���̸� ���� ����
        if (isAttacking)
        {
            return;
        }

        //  �Ϲ� ���� �߿��� ���� ����
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
