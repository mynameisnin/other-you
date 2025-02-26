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
    private CharacterAttack characterAttack; // ���� ��ũ��Ʈ ����

    private bool canDash = true;
    private bool isDashing;
    private bool isDashAttacking; // �뽬 ���� ���� �߰�
    private bool attackInputRecently = false; // �ֱ� ���� �Է� ����

    [SerializeField]
    private float dashingPower = 24f;
    private float dashingTime = 0.3f;
    private float dashingCooldown = 0.5f;
    [SerializeField] private TrailRenderer dashTrail;

    [SerializeField] private float afterImageInterval = 0.02f;
    [SerializeField] private float afterImageLifetime = 1f; // �ܻ� ���� �ð�
    [SerializeField] private float attackInputCooldown = 0.2f; // ���� �� �뽬 ���� �ð�
    [SerializeField] private float dashAttackDuration = 0.3f; // �뽬 ���� ���� �ð�

    private bool lastKeyWasRight = true;

    //���� ����
    public bool isGround;
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

        // ���� �ִϸ��̼� ���� Ȯ��
        AnimatorStateInfo currentState = AdamAnime.GetCurrentAnimatorStateInfo(0);

        // ���� �ִϸ��̼� ���� ������ Ȯ��
        bool isInAttackAnimation =  currentState.IsName("Stance") || currentState.IsName("Attack1") || currentState.IsName("Attack2")  || currentState.IsName("DashAttack") || currentState.IsName("Attack2Attack1");

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
        if (Input.GetKeyDown(KeyCode.M)&&isGround) // ���� Ű �Է�
        {
            attackInputRecently = true; // ���� �Է� ����
            StartCoroutine(ResetAttackInputCooldown());

            if (isDashing)
            {
                StartCoroutine(DashAttack()); // �뽬 �� ����
            }
            else if (characterAttack != null)
            {
                isAttacking = true;
                characterAttack.TriggerAttack(); // �Ϲ� ����
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
        // �뽬 �Ұ� ����: �뽬 ��, ��ٿ� ��, ���� �Է� �� ���� �ð� ��, ���� ��
        if (isDashing || !canDash || attackInputRecently || !isGround || currentState.IsName("Jump 1"))
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
        float elapsed = 0f;

        while (elapsed < dashAttackDuration)
        {
            AdamRigidebody.velocity = new Vector2(dashDirection * dashingPower, 0f);
            elapsed += Time.deltaTime;
            yield return null;
        }

        // �̵� �ӵ��� ������ ���̴� ���
        float decelerationTime = 0.2f;
        while (decelerationTime > 0)
        {
            AdamRigidebody.velocity = new Vector2(AdamRigidebody.velocity.x * 0.8f, AdamRigidebody.velocity.y);
            decelerationTime -= Time.deltaTime;
            yield return null;
        }

        AdamRigidebody.velocity = Vector2.zero;
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
        sr.color = new Color(0.5f, 0.5f, 0.5f, 1f);
        sr.flipX = AdamSprite.flipX;
        afterImage.transform.position = transform.position;
        afterImage.transform.localScale = transform.localScale;

        StartCoroutine(FadeOutAndDestroy(sr));
    }

    private IEnumerator FadeOutAndDestroy(SpriteRenderer sr)
    {
        if (sr == null) yield break; //  SpriteRenderer�� ������ �ٷ� ����

        float fadeDuration = afterImageLifetime;
        Color originalColor = sr.color;
        float elapsed = 0f;

        while (elapsed < fadeDuration)
        {
            if (sr == null) yield break; //  ���� ���� �����Ǿ����� �ߴ�

            elapsed += Time.deltaTime;
            float alpha = Mathf.Lerp(originalColor.a, 0, elapsed / fadeDuration);
            sr.color = new Color(originalColor.r, originalColor.g, originalColor.b, alpha);
            yield return null;
        }

        if (sr != null && sr.gameObject != null) Destroy(sr.gameObject); //  null üũ �� ����
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

        // �뽬 ���̰ų� �������� ���̸� ���� �Ұ�
        if (isDashing || (!isGround && AdamRigidebody.velocity.y < 0))
        {
            return;
        }

        if (Input.GetKeyDown(KeyCode.Space) && isGround && !isJumping)
        {
            Debug.Log("Jumping...");
            AdamAnime.SetTrigger("Jump"); // ���� �ִϸ��̼� ���� (�� ����)
            AdamRigidebody.velocity = new Vector2(AdamRigidebody.velocity.x, JumpPower);
        }
    }



    void HandleFall()
    {
        
        if (!isGround && AdamRigidebody.velocity.y < 0)
        {
            AdamAnime.SetBool("Fall",true); // Fall �ִϸ��̼� ����
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
