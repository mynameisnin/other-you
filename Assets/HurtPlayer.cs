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


    public CameraShakeSystem cameraShake;
    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;
    public int CurrentHealth => PlayerStats.Instance != null ? PlayerStats.Instance.currentHealth : 0;
    public int MaxHealth => PlayerStats.Instance != null ? PlayerStats.Instance.maxHealth : 100;


    public float knockbackForce = 5f;
    private bool isParrying = false;

    [Header("Hit Effect Position")]
    public Transform pos; //  수동으로 위치 조정 가능한 피격 이펙트 위치

    public HealthBarUI healthBarUI; //  UI 체력바 참조 추가
    public CharStateGUIEffect charStateGUIEffect;
    private bool isDead = false; //  사망 여부 확인

    [Header("Death Effect Elements")]
    public SpriteRenderer deathBackground; //  배경을 어둡게 할 오브젝트 (SpriteRenderer)
    public static HurtPlayer Instance; // 싱글톤 인스턴스 추가
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
        FindCameraShake();
        FindDeathBackground(); //  씬 시작 시 deathBackground 찾기

    }

    void Update()
    {
        if (cameraShake == null)
        {
            FindCameraShake(); //  씬이 바뀌었을 경우 다시 찾음
        }
        if (deathBackground == null)
        {
            FindDeathBackground(); //  씬 변경 시 다시 deathBackground 찾기
        }
    }
    void FindDeathBackground()
    {
        GameObject backgroundObj = GameObject.Find("DeathBackground");
        if (backgroundObj != null)
        {
            deathBackground = backgroundObj.GetComponent<SpriteRenderer>();
        }
        else
        {
            Debug.LogWarning("DeathBackground를 찾을 수 없습니다! 씬 전환 시 확인하세요.");
        }
    }
    //  카메라 셰이크 시스템을 다시 찾는 함수 추가
    void FindCameraShake()
    {
        cameraShake = Camera.main != null ? Camera.main.GetComponent<CameraShakeSystem>() : null;

        if (cameraShake == null)
        {
            Debug.LogWarning("카메라에서 CameraShakeSystem 스크립트를 찾을 수 없습니다! 씬 전환 시 확인하세요.");
        }
    }
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
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

        // 근거리 적 공격인지 검사
        EnemyMovement enemy = other.GetComponentInParent<EnemyMovement>();
        // 원거리 공격(화살)인지 검사
        Arrow enemyArrow = other.GetComponent<Arrow>();
        // 가시인지 검사
        Thron thron = other.GetComponent<Thron>();

        // 공격이 "EnemyAttack" 또는 "damageAmount" 태그를 가진 경우 실행
        if (other.CompareTag("EnemyAttack") || other.CompareTag("damageAmount"))
        {
            // 플레이어가 무적 상태인지 확인
            AdamMovement playerMovement = GetComponent<AdamMovement>();
            AdamUltimateSkill ultimateSkill = GetComponent<AdamUltimateSkill>();

            if ((playerMovement != null && playerMovement.isInvincible) ||
                (ultimateSkill != null && ultimateSkill.isCasting))
            {
                Debug.Log("무적 상태! 대미지 없음");
                return; // 대미지 처리 안 함
            }
            // 0.5초 동안 추가 대미지를 받지 않도록 설정 (연속 공격 방지)
            EnemyDamageBumpAgainst damageTrigger = other.GetComponent<EnemyDamageBumpAgainst>();
            if (damageTrigger != null)
            {
                damageTrigger.TriggerDamageCooldown(0.5f);
            }

            //  근거리 적 공격인지 확인 후 대미지 적용
            int damage = 0;
            if (enemy != null)
            {
                damage = enemy.GetDamage(); // 적이 주는 대미지를 가져옴
            }
            //  원거리 공격(화살)인지 확인 후 대미지 적용
            else if (enemyArrow != null)
            {
                damage = enemyArrow.damage; // 화살이 가진 대미지 적용
            }
            //  가시인지 확인 후 대미지 적용
            else if (thron != null)
            {
                damage = thron.damage; // 가시 대미지 적용
                Debug.Log("가시");
            }

            // 대미지 적용 (피격 처리)
            TakeDamage(damage);

            //  피격 애니메이션 즉시 실행
            TestAnime.Play("Hurt", 0, 0f);

            // 피격 이펙트 실행
            ShowBloodEffect();

            //  넉백(충격) 효과 실행
            Knockback(other.transform);

            //  카메라 흔들림 (카메라 셰이크)
            if (cameraShake != null)
            {
                StartCoroutine(cameraShake.Shake(0.15f, 0.15f)); // 0.15초 동안 화면 흔들림
            }
        }
    }


    public void TakeDamage(int damage)
    {
        if (isDead || PlayerStats.Instance == null) return;

        PlayerStats.Instance.currentHealth -= damage;
        PlayerStats.Instance.currentHealth = Mathf.Clamp(PlayerStats.Instance.currentHealth, 0, PlayerStats.Instance.maxHealth);

        Debug.Log($"[HurtPlayer] 체력 감소: {PlayerStats.Instance.currentHealth} / {PlayerStats.Instance.maxHealth}");

        if (healthBarUI != null)
        {
            healthBarUI.UpdateHealthBar(PlayerStats.Instance.currentHealth, true);
        }

        if (charStateGUIEffect != null)
        {
            charStateGUIEffect.TriggerHitEffect();
        }

        if (PlayerStats.Instance.currentHealth <= 0)
        {
            Die();
        }
    }

    public void UpdateHealthUI()
    {
        if (healthBarUI != null)
        {
            healthBarUI.UpdateHealthBar(PlayerStats.Instance.currentHealth, true);
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
            rb.velocity = Vector2.zero; // 현재 속도 제거
            rb.bodyType = RigidbodyType2D.Kinematic; // 물리 연산 비활성화
            rb.simulated = false; // 리지드바디 연산 중지
            Debug.Log("[HurtPlayer] 리지드바디 비활성화 완료!");
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