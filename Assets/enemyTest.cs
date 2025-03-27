using System.Collections;
using UnityEngine;

public class enemyTest : MonoBehaviour
{
    //  컴포넌트 참조
    private Animator TestAnime;
    private Rigidbody2D rb;
    private Collider2D col;

    //  이펙트 프리팹
    public GameObject[] bloodEffectPrefabs;
    public GameObject parringEffects;
    public ParticleSystem bloodEffectParticle;

    //  기타 설정
    private CameraShakeSystem cameraShake;

    [Header("Stats")]
    public int MaxHealth = 100;
    public int currentHealth;
    public float knockbackForce = 5f;
    public int xpReward = 50; //  적 처치 시 지급할 경험치

    [Header("Flags")]
    private bool isDying = false;
    private bool isParrying = false;

    [Header("Hit Effect Position")]
    public Transform pos; //  피격 이펙트 위치

    private void Start()
    {
        // 컴포넌트 초기화
        TestAnime = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        col = GetComponent<Collider2D>();
        cameraShake = Camera.main != null ? Camera.main.GetComponent<CameraShakeSystem>() : null;

        // 체력 초기화
        currentHealth = MaxHealth;
    }

    //  피격 이펙트 재생
    public void ShowBloodEffect()
    {
        if (bloodEffectPrefabs != null && bloodEffectPrefabs.Length > 0)
        {
            int randomIndex = Random.Range(0, bloodEffectPrefabs.Length);
            GameObject selectedEffect = bloodEffectPrefabs[randomIndex];

            GameObject bloodEffect = Instantiate(selectedEffect, pos.position, Quaternion.identity);
            Destroy(bloodEffect, 0.3f);

            // 파티클 연출도 함께
            if (bloodEffectParticle != null)
            {
                ParticleSystem bloodParticle = Instantiate(bloodEffectParticle, pos.position, Quaternion.identity);
                bloodParticle.Play();
                Destroy(bloodParticle.gameObject, bloodParticle.main.duration + 0.5f);
            }
        }
    }

    //  공격 충돌 처리
    void OnTriggerEnter2D(Collider2D other)
    {
        if (isParrying || isDying) return;

        if (other.CompareTag("PlayerAttack"))
        {
            // 공격 주체 확인
            DevaAttackDamage debaDamage = other.GetComponentInParent<DevaAttackDamage>();
            PlayerAttackDamage adamDamage = other.GetComponentInParent<PlayerAttackDamage>();

            int damage = 0;
            bool isFromAdam = false;
            bool isFromDeba = false;

            // 누가 공격했는지 판단
            if (adamDamage != null)
            {
                damage = adamDamage.GetNomalAttackDamage();
                isFromAdam = true;
            }
            else if (debaDamage != null)
            {
                damage = debaDamage.GetMagicDamage();
                isFromDeba = true;
            }

            // 가장 가까운 적인지 확인 (동시에 여러 적 피격 방지)
            enemyTest closestEnemy = FindClosestEnemy(other.transform);
            if (closestEnemy == this && damage > 0)
            {
                if (currentHealth > 0 && !isDying)
                {
                    TestAnime.Play("Hurt", 0, 0f);
                    TakeDamage(damage, isFromAdam, isFromDeba);
                    ShowBloodEffect();
                    Knockback(other.transform);

                    // 카메라 흔들기 연출
                    if (cameraShake != null)
                    {
                        StartCoroutine(cameraShake.Shake(0.1f, 0.1f));
                    }
                }
            }
        }
    }

    //  플레이어 기준 가장 가까운 enemy 찾기
    enemyTest FindClosestEnemy(Transform attacker)
    {
        enemyTest[] enemies = FindObjectsOfType<enemyTest>();
        enemyTest closest = null;
        float closestDistance = Mathf.Infinity;

        foreach (enemyTest enemy in enemies)
        {
            float dist = Vector2.Distance(attacker.position, enemy.transform.position);
            if (dist < closestDistance)
            {
                closestDistance = dist;
                closest = enemy;
            }
        }

        return closest;
    }

    //  패링 시작
    public void StartParry()
    {
        isParrying = true;
        StartCoroutine(ResetParry());
    }

    private IEnumerator ResetParry()
    {
        yield return new WaitForSeconds(0.1f); // 잠깐만 패링 상태 유지
        isParrying = false;
    }

    //  데미지 처리
    public void TakeDamage(int damage, bool fromAdam, bool fromDeba, Transform attacker = null)
    {
        if (isDying) return;

        currentHealth -= damage;
        currentHealth = Mathf.Clamp(currentHealth, 0, MaxHealth);

        if (currentHealth <= 0)
        {
            StartCoroutine(Die(fromAdam, fromDeba));
        }
        else
        {
            TestAnime.Play("Hurt", 0, 0f);
            ShowBloodEffect();
            Knockback(attacker); // ← 위치 기반 넉백
            cameraShake?.StartCoroutine(cameraShake.Shake(0.1f, 0.1f));
        }
    }


    //  넉백 처리
private void Knockback(Transform attacker)
{
    if (rb == null || attacker == null) return;

    float dir = Mathf.Sign(transform.position.x - attacker.position.x);
    if (dir == 0) dir = Random.Range(0, 2) == 0 ? -1f : 1f;

    float finalKnockback = knockbackForce;

    if (attacker.CompareTag("AdamSkill"))
    {
        finalKnockback *= 0.6f;
    }
    else if (attacker.CompareTag("DevaSkill"))
    {
        finalKnockback *= 0.6f;
    }

    rb.velocity = new Vector2(finalKnockback * dir, rb.velocity.y + 1f);
}



    //  사망 처리 + 경험치 지급
    private IEnumerator Die(bool fromAdam, bool fromDeba)
    {
        if (isDying) yield break;
        isDying = true;

        ShowBloodEffect(); // ?? 여기서 딱 한 번만 실행!

        // 경험치 지급
        if (fromAdam && PlayerExperience.Instance != null)
        {
            PlayerExperience.Instance.GainXP(xpReward);
        }
        else if (fromDeba && DevaExperience.Instance != null)
        {
            DevaExperience.Instance.GainXP(xpReward);
        }

        // 물리 정지
        if (rb != null)
        {
            rb.velocity = Vector2.zero;
            rb.bodyType = RigidbodyType2D.Kinematic;
            rb.simulated = false;
        }

        // 충돌 비활성화
        if (col != null) col.enabled = false;

        // 죽음 애니메이션
        TestAnime.SetTrigger("Die");

        yield return new WaitForSeconds(0.6f);
        Destroy(gameObject);
    }

}
