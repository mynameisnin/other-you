using UnityEngine;
using System.Collections;

public class BossHurt : MonoBehaviour
{
    [Header("Phase Object")]
    public GameObject phase2Object; // 보스 2페이즈 오브젝트
    public GameObject phase2ObjectAnime;
    private bool phase2Triggered = false; // 중복 방지용

    private Rigidbody2D rb;
    private Collider2D col;

    public GameObject[] bloodEffectPrefabs;
    public GameObject parringEffects; // 사용하지 않지만 유지해도 무방
    public ParticleSystem bloodEffectParticle;

    private CameraShakeSystem cameraShake;

    [Header("Stats")]
    public int MaxHealth = 100;
    public int currentHealth;
    public int xpReward = 50;

    [Header("Flags")]
    private bool isDying = false;

    [Header("Hit Effect Position")]
    public Transform pos;

    private AngryGodAiCore aiCore; // AI Core 참조
    private void Awake() // Awake로 변경하여 Start보다 먼저 참조 설정
    {
        rb = GetComponent<Rigidbody2D>();
        col = GetComponent<Collider2D>();
        cameraShake = Camera.main != null ? Camera.main.GetComponent<CameraShakeSystem>() : null;
        aiCore = GetComponent<AngryGodAiCore>(); // AI Core 참조 가져오기

        if (aiCore == null) Debug.LogError("AngryGodAiCore component missing on " + gameObject.name);

        currentHealth = MaxHealth;
    }


    public void ShowBloodEffect()
    {
        if (bloodEffectPrefabs != null && bloodEffectPrefabs.Length > 0)
        {
            int randomIndex = Random.Range(0, bloodEffectPrefabs.Length);
            GameObject selectedEffect = bloodEffectPrefabs[randomIndex];

            GameObject bloodEffect = Instantiate(selectedEffect, pos.position, Quaternion.identity);
            Destroy(bloodEffect, 0.3f);

            if (bloodEffectParticle != null)
            {
                ParticleSystem bloodParticle = Instantiate(bloodEffectParticle, pos.position, Quaternion.identity);
                bloodParticle.Play();
                Destroy(bloodParticle.gameObject, bloodParticle.main.duration + 0.5f);
            }
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (isDying) return;

        if (other.CompareTag("PlayerAttack"))
        {
            DevaAttackDamage debaDamage = other.GetComponentInParent<DevaAttackDamage>();
            PlayerAttackDamage adamDamage = other.GetComponentInParent<PlayerAttackDamage>();

            int damage = 0;
            bool isFromAdam = false;
            bool isFromDeba = false;

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

            BossHurt closest = FindClosestEnemy(other.transform);
            if (closest == this && damage > 0)
            {
                if (currentHealth > 0 && !isDying)
                {
                    TakeDamage(damage, isFromAdam, isFromDeba);
                }
            }
        }
    }

    BossHurt FindClosestEnemy(Transform attacker)
    {
        BossHurt[] enemies = FindObjectsOfType<BossHurt>();
        BossHurt closest = null;
        float closestDistance = Mathf.Infinity;

        foreach (BossHurt enemy in enemies)
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

    public void TakeDamage(int damage, bool fromAdam, bool fromDeba)
    {
        if (isDying || (aiCore != null && aiCore.IsCurrentlyActingOrSkillActive())) // ★★★ 보스가 중요한 행동 중일 때는 데미지 처리 후 각성 요청을 잠시 보류할 수 있음 (선택적)
        {
            // 또는 데미지는 받되, 각성 요청 조건 검사를 아래 if문으로만 한정
            // 현재는 isDying만 체크하도록 유지
        }
        if (isDying) return;


        currentHealth -= damage;
        currentHealth = Mathf.Clamp(currentHealth, 0, MaxHealth);

        // 50% 이하 각성 트리거 처리 (단 한 번만)
        // phase2Triggered는 BossHurt 레벨에서 "각성 요청을 보냈는가"를 추적
        if (!phase2Triggered && currentHealth <= MaxHealth / 2)
        {
            phase2Triggered = true; // 각성 요청을 보낼 것임을 표시 (중복 방지)
            Debug.Log("▶ HP 50% 이하! 각성 시퀀스 요청 준비.");

            if (aiCore != null)
            {
                aiCore.RequestAwakeningSequence(); // AI Core에 각성 시퀀스 시작 요청
                                                   // phase2Object 및 phase2ObjectAnime의 활성화는 AI Core의 각성 시퀀스 내에서 처리
            }
            else
            {
                Debug.LogWarning("AI Core 참조가 없어 각성 시퀀스를 요청할 수 없습니다!");
                // AI Core가 없다면 기존처럼 여기서 직접 phase2Object 등을 활성화할 수 있지만,
                // "무조건 백대쉬 후" 라는 조건은 AI Core가 담당해야 함.
                if (phase2Object != null) phase2Object.SetActive(true);
                if (phase2ObjectAnime != null) phase2ObjectAnime.SetActive(true);
            }
        }

        if (currentHealth <= 0)
        {
            StartCoroutine(Die(fromAdam, fromDeba));
        }
        else
        {
            ShowBloodEffect();
            if (cameraShake != null) // null 체크 추가
                cameraShake.StartCoroutine(cameraShake.Shake(0.1f, 0.1f));
        }
    }



    private IEnumerator Die(bool fromAdam, bool fromDeba)
    {
        if (isDying) yield break;
        isDying = true;

        ShowBloodEffect();

        if (fromAdam && PlayerExperience.Instance != null)
        {
            PlayerExperience.Instance.GainXP(xpReward);
        }
        else if (fromDeba && DevaExperience.Instance != null)
        {
            DevaExperience.Instance.GainXP(xpReward);
        }

        if (rb != null)
        {
            rb.velocity = Vector2.zero;
            rb.bodyType = RigidbodyType2D.Kinematic;
            rb.simulated = false;
        }

        if (col != null) col.enabled = false;

        yield return new WaitForSeconds(0.6f);
        Destroy(gameObject);
    }
}
