using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; // UI 관련 네임스페이스 (HealthBarUI 등 사용 시)
using DG.Tweening;    // DOTween 사용 시
using UnityEngine.SceneManagement; // SceneManager 사용을 위해 추가

public class HurtPlayer : MonoBehaviour
{
    private Animator TestAnime;
    public GameObject[] bloodEffectPrefabs;
    public GameObject parringEffects; // 오타 수정 제안: parryingEffects
    public ParticleSystem bloodEffectParticle;

    public CameraShakeSystem cameraShake;
    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;

    // PlayerStats 싱글톤 인스턴스를 통해 체력 정보 접근
    public int CurrentHealth => PlayerStats.Instance != null ? PlayerStats.Instance.currentHealth : 0;
    public int MaxHealth => PlayerStats.Instance != null ? PlayerStats.Instance.maxHealth : 100; // 기본값 100

    public float knockbackForce = 5f;
    private bool isParrying = false;

    [Header("Hit Effect Position")]
    public Transform pos; // 피격 이펙트 생성 위치

    [Header("UI References")]
    public HealthBarUI healthBarUI; // 체력바 UI 참조 (인스펙터에서 할당 또는 Find)
    public CharStateGUIEffect charStateGUIEffect; // 캐릭터 상태 GUI 효과 참조 (인스펙터에서 할당 또는 Find)

    private bool isDead = false; // 플레이어 사망 여부

    // --- SceneUIManager 관련 ---
    private SceneUIManager currentSceneUIManager; // 현재 씬의 UI 매니저 참조
    // --- SceneUIManager 관련 끝 ---

    // --- DeathBackground 관련 (HurtPlayer가 직접 관리) ---
    [Header("Death Effect Elements (HurtPlayer Managed)")]
    public SpriteRenderer deathBackground; // 죽음 배경 SpriteRenderer (인스펙터 또는 Find로 할당)
    // --- DeathBackground 관련 끝 ---

    public static HurtPlayer Instance; // HurtPlayer 싱글톤 인스턴스

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // 플레이어 오브젝트가 씬 전환 시 파괴되지 않도록 설정
        }
        else if (Instance != this) // 이미 다른 인스턴스가 있다면 현재 오브젝트는 파괴
        {
            Destroy(gameObject);
            return;
        }

        // 플레이어의 주요 컴포넌트들 초기화
        TestAnime = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void OnEnable()
    {
        // 씬 로드 이벤트 구독
        SceneManager.sceneLoaded += OnSceneLoaded;
        // 현재 씬 (또는 첫 씬)의 의존 요소들 초기화
        InitializeSceneDependentElements();
    }

    void OnDisable()
    {
        // 씬 로드 이벤트 구독 해제 (오브젝트 비활성화 또는 파괴 시)
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    // 씬이 로드될 때마다 호출되는 메소드
    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // 새로 로드된 씬의 의존 요소들 초기화
        InitializeSceneDependentElements();
    }

    // 씬 의존적인 요소들을 초기화하고, 플레이어 상태를 리셋하는 메소드
    void InitializeSceneDependentElements()
    {
        // 현재 씬의 SceneUIManager 찾기
        currentSceneUIManager = FindObjectOfType<SceneUIManager>();
        if (currentSceneUIManager == null)
        {
            Debug.LogError("현재 씬에서 SceneUIManager를 찾을 수 없습니다! 각 씬에 SceneUIManager 오브젝트가 있고 활성화되어 있는지 확인하세요.");
        }

        // 카메라 셰이크 시스템 찾기
        FindCameraShake();
        // DeathBackground 찾고 초기화 (HurtPlayer가 관리)
        FindDeathBackground(); // 여기서 deathBackground의 알파값도 0으로 초기화됩니다.

        // ---!!! 중요: 플레이어 상태 초기화 시작 !!!---

        // 1. 체력 초기화
        if (PlayerStats.Instance != null)
        {
            PlayerStats.Instance.currentHealth = PlayerStats.Instance.maxHealth; // 현재 체력을 최대 체력으로!
            Debug.Log($"[HurtPlayer] 플레이어 체력 초기화: {PlayerStats.Instance.currentHealth}/{PlayerStats.Instance.maxHealth}");
        }

        // 2. 사망 상태 플래그 초기화
        isDead = false;
        Debug.Log("[HurtPlayer] isDead 플래그 초기화: false");

        // 3. 비활성화했던 컴포넌트들 다시 활성화
        AdamMovement movement = GetComponent<AdamMovement>(); // AdamMovement 스크립트 참조
        if (movement != null)
        {
            movement.enabled = true;
            Debug.Log("[HurtPlayer] AdamMovement 활성화됨");
        }

        CharacterAttack attack = GetComponent<CharacterAttack>(); // CharacterAttack 스크립트 참조
        if (attack != null)
        {
            attack.enabled = true;
            Debug.Log("[HurtPlayer] CharacterAttack 활성화됨");
        }
        // 만약 다른 스크립트(예: 점프, 대쉬 등)도 Die()에서 비활성화했다면 여기서 다시 활성화해야 합니다.

        // 4. Rigidbody 상태 초기화
        if (rb != null)
        {
            rb.bodyType = RigidbodyType2D.Dynamic; // 물리 타입을 Dynamic으로 되돌림
            rb.simulated = true;                   // 물리 시뮬레이션을 다시 켬
            rb.velocity = Vector2.zero;            // 혹시 모를 속도 잔여분 제거
            Debug.Log("[HurtPlayer] Rigidbody 상태 초기화 (Dynamic, Simulated)");
        }

        // 5. 애니메이터 상태 초기화
        if (TestAnime != null)
        {
            TestAnime.ResetTrigger("Die"); // "Die" 애니메이션 트리거 리셋
            // 만약 Bool 파라미터 (예: "IsDead")로 죽음 상태를 제어한다면:
            // TestAnime.SetBool("IsDead", false);
            TestAnime.Play("Idle", 0, 0f); // 기본 상태 (예: "Idle") 애니메이션으로 즉시 전환하여 죽음 애니메이션에서 벗어남
            Debug.Log("[HurtPlayer] Animator 상태 초기화 (Die 트리거 리셋, Idle 상태로 전환)");
        }

        // ---!!! 플레이어 상태 초기화 끝 !!!---

        // 체력바 UI 업데이트 로직
        if (PlayerStats.Instance != null && healthBarUI != null)
        {
            healthBarUI.Initialize(MaxHealth); // healthBarUI의 최대치도 다시 설정
            healthBarUI.UpdateHealthBar(CurrentHealth, false); // 초기화된 현재 체력으로 업데이트
        }
        else if (healthBarUI == null)
        {
            // HealthBarUI가 필수적이지 않거나 다른 방식으로 관리될 경우 경고 수준을 낮추거나 제거할 수 있습니다.
            Debug.LogWarning("HealthBarUI 참조가 없습니다. 인스펙터에서 할당되었는지 또는 씬에 존재하는지 확인하세요.");
        }
    }

    // 메인 카메라에서 CameraShakeSystem을 찾는 메소드
    void FindCameraShake()
    {
        if (Camera.main != null)
        {
            cameraShake = Camera.main.GetComponent<CameraShakeSystem>();
        }
        if (cameraShake == null)
        {
            // Debug.LogWarning("카메라에서 CameraShakeSystem 스크립트를 찾을 수 없습니다! 씬 전환 시 확인하세요.");
        }
    }

    // "DeathBackground"라는 이름의 오브젝트를 찾아 SpriteRenderer를 가져오고 초기화하는 메소드
    void FindDeathBackground()
    {
        GameObject backgroundObj = GameObject.Find("DeathBackground"); // 이름으로 찾기
        if (backgroundObj != null)
        {
            deathBackground = backgroundObj.GetComponent<SpriteRenderer>();
            if (deathBackground != null)
            {
                // 씬 로드 시 (그리고 아직 죽지 않았을 때) 투명하게 초기화
                deathBackground.gameObject.SetActive(true); // 게임 오브젝트 자체는 활성화 상태여야 함
                Color startColor = deathBackground.color;
                startColor.a = 0f; // 알파값을 0으로 (완전 투명)
                deathBackground.color = startColor;
            }
            else
            {
                Debug.LogWarning("HurtPlayer: 'DeathBackground' 오브젝트에 SpriteRenderer 컴포넌트가 없습니다.");
            }
        }
        else
        {
            Debug.LogWarning("HurtPlayer: 'DeathBackground' 이름의 오브젝트를 씬에서 찾을 수 없습니다.");
            deathBackground = null; // 못 찾았으면 참조를 null로 설정
        }
    }

    // 피격 시 혈흔 이펙트를 보여주는 메소드
    public void ShowBloodEffect()
    {
        if (bloodEffectPrefabs != null && bloodEffectPrefabs.Length > 0)
        {
            int randomIndex = Random.Range(0, bloodEffectPrefabs.Length);
            GameObject selectedEffect = bloodEffectPrefabs[randomIndex];

            // 'pos' 위치에 이펙트 생성
            GameObject bloodEffectInstance = Instantiate(selectedEffect, pos.position, Quaternion.identity);
            Destroy(bloodEffectInstance, 0.3f); // 0.3초 후 자동 파괴

            if (bloodEffectParticle != null)
            {
                ParticleSystem bloodParticleInstance = Instantiate(bloodEffectParticle, pos.position, Quaternion.identity);
                bloodParticleInstance.Play();
                // 파티클 시스템의 main 모듈 duration 값에 약간의 여유를 더해 파괴 시간 설정
                Destroy(bloodParticleInstance.gameObject, bloodParticleInstance.main.duration + bloodParticleInstance.main.startLifetime.constantMax + 0.5f);
            }
        }
        // else
        // {
        //     Debug.LogWarning("bloodEffectPrefabs 배열이 비어 있거나 할당되지 않았습니다!");
        // }
    }

    // 충돌 감지 (주로 적의 공격)
    void OnTriggerEnter2D(Collider2D other)
    {
        if (isParrying || isDead) return; // 패링 중이거나 이미 죽었으면 무시

        // 공격 주체 확인 (다양한 공격 유형 고려)
        EnemyMovement enemy = other.GetComponentInParent<EnemyMovement>(); // 근거리 적
        Arrow enemyArrow = other.GetComponent<Arrow>(); // 원거리 화살
        Thron thron = other.GetComponent<Thron>(); // 가시 함정

        // "EnemyAttack" 또는 "damageAmount" 태그를 가진 오브젝트와 충돌 시
        if (other.CompareTag("EnemyAttack") || other.CompareTag("damageAmount"))
        {
            // 플레이어 무적 상태 확인
            AdamMovement playerMovement = GetComponent<AdamMovement>();
            AdamUltimateSkill ultimateSkill = GetComponent<AdamUltimateSkill>(); // 궁극기 사용 중 무적 등

            if ((playerMovement != null && playerMovement.isInvincible) ||
                (ultimateSkill != null && ultimateSkill.isCasting)) // isCasting 같은 궁극기 사용 중 상태 변수
            {
                Debug.Log("플레이어 무적 상태! 대미지 없음.");
                return; // 대미지 처리 안 함
            }

            // 연속 공격 방지를 위한 쿨다운 (공격 주체 스크립트에 따라 다를 수 있음)
            EnemyDamageBumpAgainst damageTrigger = other.GetComponent<EnemyDamageBumpAgainst>();
            if (damageTrigger != null)
            {
                damageTrigger.TriggerDamageCooldown(0.5f); // 예시: 0.5초 쿨다운
            }

            int damage = 0;
            if (enemy != null)
            {
                damage = enemy.GetDamage(); // 적 스크립트에서 대미지 값 가져오기
            }
            else if (enemyArrow != null)
            {
                damage = enemyArrow.damage; // 화살 스크립트에서 대미지 값 가져오기
            }
            else if (thron != null)
            {
                damage = thron.damage; // 가시 스크립트에서 대미지 값 가져오기
                // Debug.Log("가시에 찔림!");
            }

            if (damage > 0) // 유효한 대미지가 있을 경우에만 처리
            {
                TakeDamage(damage); // 대미지 적용
                TestAnime.Play("Hurt", 0, 0f); // 피격 애니메이션 즉시 실행
                ShowBloodEffect(); // 피격 이펙트 실행
                Knockback(other.transform); // 넉백 효과

                if (cameraShake != null)
                {
                    StartCoroutine(cameraShake.Shake(0.15f, 0.15f)); // 카메라 흔들림
                }
            }
        }
    }

    // 플레이어가 대미지를 받는 로직
    public void TakeDamage(int damage)
    {
        if (isDead || PlayerStats.Instance == null) return; // 이미 죽었거나 PlayerStats 없으면 무시

        PlayerStats.Instance.currentHealth -= damage;
        // 체력이 0 미만으로 내려가지 않도록 Mathf.Clamp 사용
        PlayerStats.Instance.currentHealth = Mathf.Clamp(PlayerStats.Instance.currentHealth, 0, MaxHealth);

        Debug.Log($"[HurtPlayer] 체력 감소: {CurrentHealth} / {MaxHealth}");

        // 체력바 UI 업데이트
        if (healthBarUI != null)
        {
            healthBarUI.UpdateHealthBar(CurrentHealth, true); // true는 애니메이션 효과 적용 여부 등
        }

        // 캐릭터 상태 GUI 효과 발동 (화면 깜빡임 등)
        if (charStateGUIEffect != null)
        {
            charStateGUIEffect.TriggerHitEffect();
        }

        // 체력이 0 이하가 되면 사망 처리
        if (CurrentHealth <= 0)
        {
            Die();
        }
    }

    // 외부에서 플레이어 체력 UI를 강제로 업데이트해야 할 때 사용 (예: 아이템으로 회복 시)
    public void UpdateHealthUI()
    {
        if (healthBarUI != null && PlayerStats.Instance != null)
        {
            healthBarUI.UpdateHealthBar(CurrentHealth, true);
        }
    }

    // 패링 성공 시 대미지 무효화 (외부에서 호출될 수 있음)
    public void CancelDamage()
    {
        Debug.Log("패링 성공! 대미지 무효화됨.");
        TestAnime.ResetTrigger("Hurt"); // 피격 애니메이션 실행 방지
    }

    // 패링 시작 (외부에서 호출)
    public void StartParry()
    {
        isParrying = true;
        StartCoroutine(ResetParry()); // 일정 시간 후 패링 상태 해제
    }

    // 패링 상태를 짧은 시간 후 해제하는 코루틴
    private IEnumerator ResetParry()
    {
        yield return new WaitForSeconds(0.1f); // 예시: 0.1초 동안 패링 유효
        isParrying = false;
    }

    // 넉백 효과를 주는 메소드
    private void Knockback(Transform attackerTransform)
    {
        if (rb == null || rb.bodyType == RigidbodyType2D.Kinematic) return; // Rigidbody 없거나 Kinematic이면 넉백 불가

        // 공격자 위치를 기준으로 플레이어가 밀려날 방향 결정
        float direction = (transform.position.x - attackerTransform.position.x > 0) ? 1f : -1f;
        // X축으로 넉백 힘을 가하고, Y축으로는 약간 위로 띄우는 효과 추가 가능
        rb.velocity = new Vector2(knockbackForce * direction, rb.velocity.y + knockbackForce * 0.2f); // Y축 힘 조절 가능
    }

    // 플레이어 사망 처리 메소드
    private void Die()
    {
        if (isDead) return; // 이미 사망 처리 중이면 중복 실행 방지
        isDead = true; // 사망 상태로 설정

        Debug.Log($"{gameObject.name} 사망!");
        DisablePlayerControls(); // 플레이어 조작 비활성화

        // Rigidbody를 Kinematic으로 변경하여 물리적 움직임 중지
        if (rb != null)
        {
            rb.velocity = Vector2.zero; // 현재 속도 제거
            rb.bodyType = RigidbodyType2D.Kinematic; // 물리 연산 비활성화
            rb.simulated = false; // 리지드바디 시뮬레이션 중지
            Debug.Log("[HurtPlayer] 리지드바디 비활성화 완료 (Kinematic, Not Simulated)");
        }

        // DeathBackground 처리 (HurtPlayer가 직접)
        if (deathBackground != null)
        {
            // 배경을 서서히 어둡게 하는 DOTween 애니메이션
            deathBackground.DOFade(1f, 0.5f) // 0.5초 동안 알파값을 1로 (완전 불투명)
                .OnComplete(() => {
                    // 배경 어두워진 후 실행될 내용
                    TestAnime.SetTrigger("Die"); // 사망 애니메이션 실행
                    ChangeLayerOnDeath();       // 캐릭터 레이어 변경 (다른 오브젝트 위로 올라오도록)

                    // SceneUIManager를 통해 DeathPanel 활성화 요청
                    if (currentSceneUIManager != null)
                    {
                        currentSceneUIManager.ShowManagedDeathPanel();
                    }
                    else
                    {
                        Debug.LogError("Die (After Fade): currentSceneUIManager가 없습니다. DeathPanel을 표시할 수 없습니다.");
                    }
                });
        }
        else
        {
            // DeathBackground가 없으면 바로 다음 로직 실행
            TestAnime.SetTrigger("Die");
            ChangeLayerOnDeath();

            if (currentSceneUIManager != null)
            {
                currentSceneUIManager.ShowManagedDeathPanel();
            }
            else
            {
                Debug.LogError("Die (No Background): currentSceneUIManager가 없습니다. DeathPanel을 표시할 수 없습니다.");
            }
        }
        // StartCoroutine(DisableAfterDeath()); // 필요하다면 일정 시간 후 캐릭터 오브젝트 자체를 비활성화
    }

    // 플레이어 조작 관련 컴포넌트들을 비활성화하는 메소드
    private void DisablePlayerControls()
    {
        AdamMovement movement = GetComponent<AdamMovement>();
        if (movement != null) movement.enabled = false;

        CharacterAttack attack = GetComponent<CharacterAttack>();
        if (attack != null) attack.enabled = false;

        // 다른 조작 관련 컴포넌트가 있다면 여기서 비활성화 (예: 점프, 대쉬 스크립트)
        // Jump jumpScript = GetComponent<Jump>();
        // if (jumpScript != null) jumpScript.enabled = false;

        Debug.Log("플레이어 컨트롤러들 비활성화됨.");
    }

    // 사망 시 캐릭터의 SpriteRenderer Sorting Order를 변경하여 다른 오브젝트 위로 보이도록 함
    private void ChangeLayerOnDeath()
    {
        if (spriteRenderer != null)
        {
            spriteRenderer.sortingOrder = 11; // 적절한 값으로 설정 (예: UI보다는 낮고, 배경보다는 높게)
            Debug.Log($"[HurtPlayer] 캐릭터 Order in Layer 변경됨: {spriteRenderer.sortingOrder}");
        }
    }

    // 일정 시간 후 플레이어 게임 오브젝트 자체를 비활성화하는 코루틴 (선택 사항)
    private IEnumerator DisableAfterDeath()
    {
        yield return new WaitForSeconds(5f); // 예: 5초 후
        gameObject.SetActive(false);
        Debug.Log("플레이어 오브젝트 비활성화됨.");
    }
}