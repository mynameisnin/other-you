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
    private DevaEnergyBarUI DevaEnergyBarUI; // ������ UI �߰�

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
    private Vector2? pendingTeleportTarget = null;
    public bool isControllable = true;
    private DownJump currentPlatform;
    void Start()
    {
        DebaraRigidbody = GetComponent<Rigidbody2D>();
        DebaraAnime = GetComponent<Animator>();
        DebaraSprite = GetComponent<SpriteRenderer>();
        magicAttack = GetComponent<MagicAttack>();
        DevaEnergyBarUI = FindObjectOfType<DevaEnergyBarUI>(); // EnergyBarUI ã��
    }

    void Update()
    {
        if (!isControllable) return;
        AnimatorStateInfo currentState = DebaraAnime.GetCurrentAnimatorStateInfo(0);

        bool isInAttackAnimation = currentState.IsName("Cast1") || currentState.IsName("Cast2");

        if (isInAttackAnimation && isAttacking) // �� ��¥ ���� ���� ���� ����
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
        DebaraAnime.SetBool("isGrounded", isGround);

        
    }

    float currentSpeed = 0f;
    float acceleration = 8f;
    float deceleration = 12f;
    public float maxSpeed = 4f;

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
            float currentEnergy = DevaEnergyBarUI != null ? DevaEnergyBarUI.GetCurrentEnergy() : 0f;

            // ?? ������ ���� �� �ڷ���Ʈ �Ұ� ó��
            if (currentEnergy < teleportEnergyCost && DevaEnergyBarUI != null)
            {
                Debug.Log("�ڷ���Ʈ �Ұ�: ENERGY ����!");
                DevaEnergyBarUI.FlashBorder(); // UI ������ ȿ��
                return;
            }

            StartCoroutine(Teleport()); // �ڷ���Ʈ ����
        }
    }
    public GameObject teleportStartEffectPrefab; // ��� ����Ʈ ������
    public GameObject teleportEndEffectPrefab; // ���� ����Ʈ ������
    public Transform teleportStartEffectPosition; // ��� ����Ʈ ��ġ ������ �� ������Ʈ
    public Transform teleportEndEffectPosition; // ���� ����Ʈ ��ġ ������ �� ������Ʈ

    private IEnumerator Teleport()
    {
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
        Vector2 direction = new Vector2(teleportDirection, 0);
        float distance = teleportDistance;

        // Ray ���� ��ġ: ĳ���� �߽ɿ��� ���� + ������ ��¦ �̵�
        Vector2 origin = (Vector2)transform.position + new Vector2(0, 0.5f) + direction * 0.3f;
        RaycastHit2D hit = Physics2D.Raycast(origin, direction, distance, isLayer);

        // ����� �� ǥ��
        Debug.DrawRay(origin, direction * distance, Color.red, 1f);

        // ����� �ð�ȭ


        Vector2 targetPosition;

        if (hit.collider != null)
        {
            float safeDistance = Mathf.Max(0f, hit.distance - 0.05f);
            targetPosition = (Vector2)transform.position + direction * safeDistance;
            Debug.Log($"[����] �� ������: {hit.collider.name} | �Ÿ�: {hit.distance:F2} �� �̵��Ÿ�: {safeDistance:F2}");
        }
        else
        {
            targetPosition = (Vector2)transform.position + direction * distance;
            Debug.Log($"[����] �� ���� �� ��ü �Ÿ� �̵�: {distance:F2}");
        }

        pendingTeleportTarget = targetPosition;

        // ��� ����Ʈ
        if (teleportStartEffectPrefab != null && teleportStartEffectPosition != null)
        {
            GameObject startEffect = Instantiate(teleportStartEffectPrefab, teleportStartEffectPosition.position, Quaternion.identity);
            Destroy(startEffect, 0.3f);
        }

        yield return new WaitForSeconds(0.2f); // �ڷ���Ʈ ������

        transform.position = targetPosition;
        pendingTeleportTarget = null;

        // ���� ����Ʈ
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

        if (Input.GetKey(KeyCode.DownArrow) && Input.GetKeyDown(KeyCode.Space))
        {
            if (currentPlatform != null)
            {
                Debug.Log("�÷��̾ ���� ���� �۵�!");
                isGround = false;
                currentPlatform.TriggerDownJump();  // ���ǿ��� ������ �Լ� ȣ��
            }
        }

        if (Input.GetKeyDown(KeyCode.Space) && isGround && !isJumping)
        {
            Debug.Log("Jumping...");
            DebaraRigidbody.velocity = new Vector2(DebaraRigidbody.velocity.x, JumpPower);
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Platform"))
        {
            Debug.Log("���ǰ���:" + collision.gameObject);
            currentPlatform = collision.gameObject.GetComponent<DownJump>();
        }
    }

    void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Platform"))
        {
            currentPlatform = null;
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

        // ������ ����
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
            DebaraAnime.Play("DevaIdle"); // �� Ȱ��ȭ ������ ���� ����
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

            // ��ġ ����
            if (pendingTeleportTarget.HasValue)
            {
                transform.position = pendingTeleportTarget.Value;
                pendingTeleportTarget = null;
            }

            StopAllCoroutines();
        }
    }
    [SerializeField] private GameObject laserPrefab;
    [SerializeField] private Transform laserSpawnOrigin; // ���� ��ġ (Deva ����)

    [SerializeField] private int laserCount = 6; // �� �� ��ȯ���� (��: 6��)
    [SerializeField] private float interval = 0.1f; // ��ȯ ����
    [SerializeField] private float spawnDistanceStep = 0.5f; // �� ��ȯ���� �������� �󸶳� �̵�����

    [SerializeField] private float laserManaCost = 40f; // ������ ��ų ���� �Ҹ�

    [Header("UI ����")]
    public SkillCooldownUI laserCooldownUI; // ������ ��ų ��Ÿ�� UI ����

    [Header("��Ÿ�� ����")]
    public float laserCooldown = 6f; // ������ ��ų ��Ÿ�� �ð�
    private bool isLaserOnCooldown = false;
    private float laserCooldownEndTime = 0f;



    public void CastLaserSkill()
    {
        // ? ��Ÿ�� üũ�� Time.time ����
        if (Time.time < laserCooldownEndTime)
        {
            Debug.Log("������ ��ų ��Ÿ�� ��");
            return;
        }

        if (!isGround)
        {
            Debug.Log("���߿����� ��ų ��� �Ұ�");
            return;
        }

        if (!DevaStats.Instance.HasEnoughMana((int)laserManaCost))
        {
            Debug.Log("���� ����!");
            if (DevaManaBarUI.Instance != null)
                DevaManaBarUI.Instance.FlashBorder();
            return;
        }

        // ���� ����
        DevaStats.Instance.ReduceMana((int)laserManaCost);

        // ��Ÿ�� ���� (�ð� ����)
        laserCooldownEndTime = Time.time + laserCooldown;

        if (laserCooldownUI != null)
        {
            laserCooldownUI.cooldownTime = laserCooldown;
            laserCooldownUI.StartCooldown(); // �������
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
                // ��������Ʈ ����
                Vector3 scale = laser.transform.localScale;
                scale.x = Mathf.Abs(scale.x) * -1f;
                laser.transform.localScale = scale;

                // ��ü ������Ʈ ���� (ȸ��)
                laser.transform.rotation = Quaternion.Euler(0, 180f, 0);
            }
            else
            {
                // �������� ���� �⺻ ����
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
        StopAllCoroutines(); // ������ �ڷ�ƾ ����

        if (laserCooldownUI != null)
        {
            // FillAmount ��� �ʱ�ȭ
            laserCooldownUI.cooldownTime = 0f;
        }

        if (DebaraAnime != null && gameObject.activeInHierarchy)
        {
            DebaraAnime.ResetTrigger("Cast1");
            DebaraAnime.Play("DevaIdle");
        }

        Debug.Log("Deba ������ ��ų ���� �ʱ�ȭ �Ϸ�");
    }
    [SerializeField] private DevaBigLaserSkill bigLaserSkill; // �ν����� ����

    public void EndBigLaserFromAnimation()
    {
        if (bigLaserSkill != null)
            bigLaserSkill.EndBigLaser();
    }

}
