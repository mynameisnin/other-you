using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening; 
public class HurtPlayer : MonoBehaviour
{
    private Animator TestAnime;
    public GameObject[] bloodEffectPrefabs;
    public GameObject parringEffects;
    public ParticleSystem bloodEffectParticle;


    private CameraShakeSystem cameraShake;
    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;
    public int MaxHealth = 100;
    public int currentHealth;

    public float knockbackForce = 5f;
    private bool isParrying = false;

    [Header("Hit Effect Position")]
    public Transform pos; //  수동으로 위치 조정 가능한 피격 이펙트 위치

    public HealthBarUI healthBarUI; //  UI 체력바 참조 추가
    public CharStateGUIEffect charStateGUIEffect;
    private bool isDead = false; //  사망 여부 확인

    [Header("Death Effect Elements")]
    public SpriteRenderer deathBackground; //  배경을 어둡게 할 오브젝트 (SpriteRenderer)

    void Start()
    {
        TestAnime = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        cameraShake = Camera.main != null ? Camera.main.GetComponent<CameraShakeSystem>() : null;

        if (cameraShake == null)
        {
            Debug.LogWarning("카메라에서 CameraShakeSystem 스크립트를 찾을 수 없습니다.");
        }

        currentHealth = MaxHealth;

        if (healthBarUI != null)
        {
            healthBarUI.Initialize(MaxHealth);
        }
        //  검은 배경의 투명도를 0으로 초기화 (완전 투명)
        if (deathBackground != null)
        {
            Color startColor = deathBackground.color;
            startColor.a = 0f;
            deathBackground.color = startColor;
        }
    }



public void ShowBloodEffect()
    {
        if (bloodEffectPrefabs != null && bloodEffectPrefabs.Length > 0)
        {
            int randomIndex = Random.Range(0, bloodEffectPrefabs.Length);
            GameObject selectedEffect = bloodEffectPrefabs[randomIndex];

            //  pos 위치에서 이펙트 생성
            GameObject bloodEffect = Instantiate(selectedEffect, pos.position, Quaternion.identity);
            Destroy(bloodEffect, 0.3f);

            if (bloodEffectParticle != null)
            {
                ParticleSystem bloodParticle = Instantiate(bloodEffectParticle, pos.position, Quaternion.identity);
                bloodParticle.Play();
                Destroy(bloodParticle.gameObject, bloodParticle.main.duration + 0.5f);
            }
        }
        else
        {
            Debug.LogWarning("bloodEffectPrefabs 배열이 비어 있습니다!");
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (isParrying || isDead) return; // 패링 상태거나 사망했으면 무시
        EnemyMovement enemy = other.GetComponentInParent<EnemyMovement>();
        if (other != null && other.CompareTag("EnemyAttack")|| other.CompareTag("damageAmount"))
        {
            //  플레이어가 대쉬 중이면 대미지 무효화
            AdamMovement playerMovement = GetComponent<AdamMovement>();
            if (playerMovement != null && playerMovement.isInvincible)
            {
                Debug.Log("무적 상태! 대미지 없음");
                return; // 대미지 처리 안 함
            }
            EnemyDamageBumpAgainst damageTrigger = other.GetComponent<EnemyDamageBumpAgainst>();
            if (damageTrigger != null)
            {
                damageTrigger.TriggerDamageCooldown(0.5f);
            }
            //  애니메이션 즉시 다시 실행
            TestAnime.Play("Hurt", 0, 0f);
            int damage = enemy.GetDamage();
            TakeDamage(damage); 

            ShowBloodEffect();
            Knockback(other.transform);

            if (cameraShake != null)
            {
                StartCoroutine(cameraShake.Shake(0.15f, 0.15f));
            }
        }
    }

    public void TakeDamage(int damage)
    {
        if (isDead) return; //  이미 사망한 상태면 대미지 무효

        currentHealth -= damage;
        currentHealth = Mathf.Clamp(currentHealth, 0, MaxHealth);

            Debug.Log($"[HurtPlayer] 체력 감소: {currentHealth} / {MaxHealth}");

        if (healthBarUI != null)
        {
            healthBarUI.UpdateHealthBar(currentHealth, true);  // 애니메이션 활성화
        }


        healthBarUI.UpdateHealthBar(currentHealth);

        if (charStateGUIEffect != null)
        {
            charStateGUIEffect.TriggerHitEffect();
        }

        if (currentHealth <= 0)
        {
            Die();
        }
    }
    public void CancelDamage()
    {
        Debug.Log(" 패링 성공! 대미지 무효화");
        TestAnime.ResetTrigger("Hurt"); // 피격 애니메이션 실행 방지
    }
    public void StartParry()
    {
        isParrying = true;
        StartCoroutine(ResetParry());
    }

    private IEnumerator ResetParry()
    {
        yield return new WaitForSeconds(0.1f);
        isParrying = false;
    }

    private void Knockback(Transform playerTransform)
    {
        if (rb == null) return;

        float direction = transform.position.x - playerTransform.position.x > 0 ? 1f : -1f;
        rb.velocity = new Vector2(knockbackForce * direction, rb.velocity.y + 1f);
    }

    private void Die()
    {
        if (isDead) return;
        isDead = true;

        Debug.Log($"{gameObject.name} 즉시 사망!");

        //  모든 입력 & 움직임 즉시 차단
        DisablePlayerControls();

        //  리지드바디 제거 (중력 영향 제거)
        if (rb != null)
        {
            Destroy(rb);
            rb = null; //  참조 제거
        }

        //  검은 배경을 서서히 어둡게 변환
        if (deathBackground != null)
        {
            deathBackground.DOFade(1f, 0.5f)
                .OnComplete(() =>
                {
                    TestAnime.SetTrigger("Die"); //  사망 애니메이션 실행
                    ChangeLayerOnDeath();
                });
        }
        else
        {
            TestAnime.SetTrigger("Die");
            ChangeLayerOnDeath();
        }

        //  3초 후 캐릭터 비활성화
        StartCoroutine(DisableAfterDeath());
    }

    private void DisablePlayerControls()
    {
        //  이동, 점프, 대쉬, 공격 등 모든 입력 차단
        AdamMovement movement = GetComponent<AdamMovement>();
        if (movement != null) movement.enabled = false;

        CharacterAttack attack = GetComponent<CharacterAttack>();
        if (attack != null) attack.enabled = false;

        Debug.Log("모든 컨트롤러 비활성화됨");
    }

    private void ChangeLayerOnDeath()
    {
        if (spriteRenderer != null)
        {
            spriteRenderer.sortingOrder = 11; //  캐릭터가 배경 위로 올라가도록 설정
            Debug.Log($"[HurtPlayer] Order in Layer 변경됨: {spriteRenderer.sortingOrder}");
        }
    }

    private IEnumerator DisableAfterDeath()
    {
        yield return new WaitForSeconds(5f);
        gameObject.SetActive(false);
    }
}