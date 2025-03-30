using System.Collections; // 코루틴 사용을 위한 네임스페이스
using UnityEngine; // Unity 엔진 관련 기능 사용

public class enemyTest : MonoBehaviour // 적의 피격 및 사망 처리 스크립트
{
    // 컴포넌트 참조
    private Animator TestAnime; // 애니메이터 참조
    private Rigidbody2D rb; // 리지드바디 참조
    private Collider2D col; // 콜라이더 참조

    // 이펙트 프리팹
    public GameObject[] bloodEffectPrefabs; // 피격 시 생성할 혈흔 이펙트 프리팹 배열
    public GameObject parringEffects; // 패링 시 사용할 이펙트
    public ParticleSystem bloodEffectParticle; // 파티클 타입 혈흔 이펙트

    // 기타 설정
    private CameraShakeSystem cameraShake; // 카메라 흔들기 스크립트 참조

    [Header("Stats")]
    public int MaxHealth = 100; // 최대 체력
    public int currentHealth; // 현재 체력
    public float knockbackForce = 5f; // 넉백 힘
    public int xpReward = 50; // 죽었을 때 줄 경험치량

    [Header("Flags")]
    private bool isDying = false; // 죽는 중인지 여부
    private bool isParrying = false; // 패링 중인지 여부

    [Header("Hit Effect Position")]
    public Transform pos; // 피격 이펙트를 표시할 위치

    private void Start()
    {
        // 컴포넌트 초기화
        TestAnime = GetComponent<Animator>(); // 애니메이터 가져오기
        rb = GetComponent<Rigidbody2D>(); // 리지드바디 가져오기
        col = GetComponent<Collider2D>(); // 콜라이더 가져오기
        cameraShake = Camera.main != null ? Camera.main.GetComponent<CameraShakeSystem>() : null; // 메인 카메라에서 카메라 흔들기 컴포넌트 가져오기

        // 체력 초기화
        currentHealth = MaxHealth;
    }

    // 피격 이펙트를 보여주는 함수
    public void ShowBloodEffect()
    {
        if (bloodEffectPrefabs != null && bloodEffectPrefabs.Length > 0) // 프리팹이 존재할 경우
        {
            int randomIndex = Random.Range(0, bloodEffectPrefabs.Length); // 랜덤 이펙트 선택
            GameObject selectedEffect = bloodEffectPrefabs[randomIndex]; // 선택된 이펙트

            GameObject bloodEffect = Instantiate(selectedEffect, pos.position, Quaternion.identity); // 이펙트 생성
            Destroy(bloodEffect, 0.3f); // 잠시 후 파괴

            // 파티클 이펙트도 재생
            if (bloodEffectParticle != null)
            {
                ParticleSystem bloodParticle = Instantiate(bloodEffectParticle, pos.position, Quaternion.identity); // 파티클 생성
                bloodParticle.Play(); // 재생
                Destroy(bloodParticle.gameObject, bloodParticle.main.duration + 0.5f); // 지속 시간 이후 파괴
            }
        }
    }

    // 공격 충돌 처리 함수
    void OnTriggerEnter2D(Collider2D other)
    {
        if (isParrying || isDying) return; // 패링 중이거나 죽는 중이면 처리 안 함

        if (other.CompareTag("PlayerAttack")) // 공격 태그 충돌 시
        {
            // 공격한 대상 확인
            DevaAttackDamage debaDamage = other.GetComponentInParent<DevaAttackDamage>(); // 데바 공격 컴포넌트
            PlayerAttackDamage adamDamage = other.GetComponentInParent<PlayerAttackDamage>(); // 아담 공격 컴포넌트

            int damage = 0; // 데미지 초기화
            bool isFromAdam = false; // 아담 공격 여부
            bool isFromDeba = false; // 데바 공격 여부

            // 어떤 플레이어가 공격했는지 확인
            if (adamDamage != null)
            {
                damage = adamDamage.GetNomalAttackDamage(); // 아담 데미지
                isFromAdam = true;
            }
            else if (debaDamage != null)
            {
                damage = debaDamage.GetMagicDamage(); // 데바 데미지
                isFromDeba = true;
            }

            // 가장 가까운 적만 피격 처리
            enemyTest closestEnemy = FindClosestEnemy(other.transform); // 가장 가까운 적 찾기
            if (closestEnemy == this && damage > 0) // 내가 가장 가깝고 데미지가 있을 경우
            {
                if (currentHealth > 0 && !isDying) // 아직 살아있는 경우
                {
                    TestAnime.Play("Hurt", 0, 0f); // 피격 애니메이션 실행
                    TakeDamage(damage, isFromAdam, isFromDeba); // 데미지 처리
                    ShowBloodEffect(); // 이펙트 출력
                    Knockback(other.transform); // 넉백 처리

                    if (cameraShake != null)
                    {
                        StartCoroutine(cameraShake.Shake(0.1f, 0.1f)); // 카메라 흔들기
                    }
                }
            }
        }
    }

    // 공격자 기준으로 가장 가까운 적 찾기
    enemyTest FindClosestEnemy(Transform attacker)
    {
        enemyTest[] enemies = FindObjectsOfType<enemyTest>(); // 씬에 존재하는 모든 enemyTest 객체
        enemyTest closest = null; // 가장 가까운 적 참조
        float closestDistance = Mathf.Infinity; // 초기 거리 설정

        foreach (enemyTest enemy in enemies)
        {
            float dist = Vector2.Distance(attacker.position, enemy.transform.position); // 거리 계산
            if (dist < closestDistance)
            {
                closestDistance = dist;
                closest = enemy;
            }
        }

        return closest; // 가장 가까운 적 반환
    }

    // 패링 시작 처리
    public void StartParry()
    {
        isParrying = true; // 패링 상태 설정
        StartCoroutine(ResetParry()); // 짧은 시간 후 초기화
    }

    // 패링 상태 리셋
    private IEnumerator ResetParry()
    {
        yield return new WaitForSeconds(0.1f); // 잠깐 패링 유지
        isParrying = false; // 패링 종료
    }

    // 데미지를 처리하는 함수
    public void TakeDamage(int damage, bool fromAdam, bool fromDeba, Transform attacker = null)
    {
        if (isDying) return; // 이미 죽는 중이면 무시

        currentHealth -= damage; // 체력 감소
        currentHealth = Mathf.Clamp(currentHealth, 0, MaxHealth); // 범위 제한

        if (currentHealth <= 0)
        {
            StartCoroutine(Die(fromAdam, fromDeba)); // 사망 처리 시작
        }
        else
        {
            TestAnime.Play("Hurt", 0, 0f); // 피격 애니메이션
            ShowBloodEffect(); // 이펙트 출력
            Knockback(attacker); // 넉백 처리
            cameraShake?.StartCoroutine(cameraShake.Shake(0.1f, 0.1f)); // 카메라 흔들기
        }
    }

    // 넉백 처리 함수
    private void Knockback(Transform attacker)
    {
        if (rb == null || attacker == null) return; // 넉백 불가 조건

        float dir = Mathf.Sign(transform.position.x - attacker.position.x); // 공격자와의 상대 방향 계산
        if (dir == 0) dir = Random.Range(0, 2) == 0 ? -1f : 1f; // 완전히 같은 위치일 경우 랜덤 방향

        float finalKnockback = knockbackForce; // 기본 넉백

        if (attacker.CompareTag("AdamSkill")) // 아담 스킬일 경우
        {
            finalKnockback *= 0.6f; // 넉백 감소
        }
        else if (attacker.CompareTag("DevaSkill")) // 데바 스킬일 경우
        {
            finalKnockback *= 0.6f;
        }

        rb.velocity = new Vector2(finalKnockback * dir, rb.velocity.y + 1f); // 넉백 방향 및 위로 튀기기
    }

    // 사망 처리 함수
    private IEnumerator Die(bool fromAdam, bool fromDeba)
    {
        if (isDying) yield break; // 이미 사망 중이면 중복 처리 방지
        isDying = true; // 사망 상태 설정

        ShowBloodEffect(); // 이펙트 한 번만 출력

        // 경험치 지급 처리
        if (fromAdam && PlayerExperience.Instance != null)
        {
            PlayerExperience.Instance.GainXP(xpReward); // 아담에게 경험치 지급
        }
        else if (fromDeba && DevaExperience.Instance != null)
        {
            DevaExperience.Instance.GainXP(xpReward); // 데바에게 경험치 지급
        }

        // 물리 정지
        if (rb != null)
        {
            rb.velocity = Vector2.zero; // 속도 정지
            rb.bodyType = RigidbodyType2D.Kinematic; // 물리 비활성화
            rb.simulated = false;
        }

        // 충돌 비활성화
        if (col != null) col.enabled = false;

        // 죽음 애니메이션 실행
        TestAnime.SetTrigger("Die");

        yield return new WaitForSeconds(0.6f); // 애니메이션 재생 시간 대기
        Destroy(gameObject); // 적 오브젝트 파괴
    }
}
