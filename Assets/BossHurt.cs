using UnityEngine;
using System.Collections;
using UnityEngine.Playables;
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
    [Header("애니메이터")]
    public Animator bossAnimator; // 보스 애니메이터 추가
    [Header("Stats")]
    public int MaxHealth = 100;
    public int currentHealth;
    public int xpReward = 50;

    [Header("Flags")]
    private bool isDying = false;

    [Header("Hit Effect Position")]
    public Transform pos;
    [Header("Timelines")]
    public PlayableDirector characterMoveTimeline;
    private AngryGodAiCore aiCore; // AI Core 참조
    public GameObject adamCharacter; // 인스펙터에서 아담 캐릭터 할당
    public GameObject devaCharacter; // 인스펙터에서 데바 캐릭터 할당 (데바는 씬에 미리 비활성화 상태로 두거나, 프리팹일 경우엔 아래 Instantiate 방식 사용)
    public float devaSpawnOffsetDistance = 1.0f;
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
        // ... (기존 isDying, 보스 처리, 경험치 지급 등) ...

        // 아담 이동 타임라인 재생
        if (characterMoveTimeline != null)
        {
            Debug.Log("아담 이동 타임라인 재생!");
            characterMoveTimeline.Play();
            // 만약 아담 타임라인이 끝난 후에 데바가 나오게 하려면, 여기서 타임라인 끝날 때까지 기다려야 함.
            // yield return new WaitForSeconds((float)characterMoveTimeline.duration);
        }

        // ★★★ 데바를 아담의 현재 위치에 나타나게 하기 ★★★
        if (devaCharacter != null && adamCharacter != null)
        {
            // 1. 데바 위치 설정
            //    정확히 아담 위치, 또는 아담 옆에 살짝 떨어져서.
            Vector3 spawnPosition = adamCharacter.transform.position;

            // 아담의 오른쪽 옆에 나오게 하려면 (옵션)
            spawnPosition += adamCharacter.transform.right * devaSpawnOffsetDistance;
            // 또는 아담의 앞쪽에 나오게 하려면 (옵션)
            // spawnPosition += adamCharacter.transform.forward * devaSpawnOffsetDistance;

            devaCharacter.transform.position = spawnPosition;

            // (선택사항) 데바가 아담을 바라보게 하려면
            // devaCharacter.transform.LookAt(adamCharacter.transform);
            // 또는 그냥 아담과 같은 방향을 보게 하려면
            devaCharacter.transform.rotation = adamCharacter.transform.rotation;


            // 2. 데바 활성화 (만약 비활성화 상태였다면)
            devaCharacter.SetActive(true);
            Debug.Log("데바가 아담 위치(" + spawnPosition + ")에 나타남!");

            // 3. (선택사항) 데바 등장 애니메이션/이펙트 재생
            Animator devaAnimator = devaCharacter.GetComponent<Animator>();
            if (devaAnimator != null)
            {
                // devaAnimator.SetTrigger("AppearTrigger"); // 예시: 등장 애니메이션 트리거
            }
            // 파티클 이펙트 같은 것도 여기서 Instantiate 가능
        }
        else
        {
            if (devaCharacter == null) Debug.LogWarning("데바 캐릭터가 할당되지 않았습니다!");
            if (adamCharacter == null) Debug.LogWarning("아담 캐릭터가 할당되지 않았습니다!");
        }

        if (bossAnimator != null)
        {
            bossAnimator.SetTrigger("Die");
            Debug.Log("[BossHurt] 보스 죽는 애니메이션 실행됨.");
        }

        yield return new WaitForSeconds(1f); // 애니메이션 대기 시간
        Destroy(gameObject);
    }
}