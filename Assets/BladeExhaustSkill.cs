using System.Collections;
using UnityEngine;

public class BladeExhaustSkill : MonoBehaviour
{
    private Rigidbody2D rb;
    private Animator animator;
    private SpriteRenderer sprite;
    private AdamMovement movement;

    public float slashTeleportDistance = 2.5f;
    public float energyCost = 25f;
    public string slashAnimationName = "AdamSlash";

    private bool isSlashing = false;

    [Header("데미지 관련")]
    public int damage = 35;
    public string targetTag = "Enemy";
    public bool fromAdam = true;
    public bool fromDeba = false;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        sprite = GetComponent<SpriteRenderer>();
        movement = GetComponent<AdamMovement>();
    }

    void Update()
    {
        if (isSlashing) return;
        if (!movement.isGround) return;

        if (Input.GetKeyDown(KeyCode.X)) // 스킬 키
        {
            StartCoroutine(StartSlash());
        }
    }

    private IEnumerator StartSlash()
    {
        isSlashing = true;
        movement.StopMovement(); // 이동 차단
        movement.isInvincible = true;

        animator.Play(slashAnimationName, 0, 0); // 애니메이션 재생

        float timeout = 3f;
        while (timeout > 0 && animator.GetCurrentAnimatorStateInfo(0).IsName(slashAnimationName))
        {
            timeout -= Time.deltaTime;
            yield return null;
        }

        movement.isInvincible = false;
        isSlashing = false;
    }

    public void PerformSlashTeleport()
    {
        float direction = sprite.flipX ? -1f : 1f;
        Vector2 newPosition = new Vector2(transform.position.x + slashTeleportDistance * direction, transform.position.y);
        transform.position = newPosition;
    }

    [Header("먼지 이펙트")]
    public GameObject dustEffectPrefab;
    public Transform dustSpawnPoint;
    public SpriteRenderer adamSprite;

    public void SpawnDustBeforeTeleport()
    {
        if (dustEffectPrefab != null && dustSpawnPoint != null)
        {
            Vector2 spawnPosition = dustSpawnPoint.position;

            if (adamSprite != null && adamSprite.flipX)
            {
                float offsetX = dustSpawnPoint.position.x - transform.position.x;
                spawnPosition.x = transform.position.x - offsetX;
            }

            GameObject dust = Instantiate(dustEffectPrefab, spawnPosition, Quaternion.identity);
            Destroy(dust, 0.7f);

            SpriteRenderer dustSprite = dust.GetComponent<SpriteRenderer>();
            if (dustSprite != null)
            {
                dustSprite.flipX = adamSprite != null && adamSprite.flipX;
            }
        }
    }

    //  적과 충돌 시 데미지 처리
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag(targetTag))
        {
            enemyTest enemy = other.GetComponent<enemyTest>();
            if (enemy != null)
            {
                enemy.TakeDamage(damage, fromAdam, fromDeba, transform); // ← transform도 전달
            }
        }
    }
}
