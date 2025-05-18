using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement; // 맨 위에 추가 필요
public class BossHurt : MonoBehaviour
{
    [Header("Phase Object")]
    public GameObject phase2Object;
    public GameObject phase2ObjectAnime;
    private bool phase2Triggered = false;

    private Rigidbody2D rb;
    private Collider2D col;

    public GameObject[] bloodEffectPrefabs;
    public GameObject parringEffects;
    public ParticleSystem bloodEffectParticle;

    private CameraShakeSystem cameraShake;

    [Header("애니메이터")]
    public Animator bossAnimator;

    [Header("Stats")]
    public int MaxHealth = 100;
    public int currentHealth;
    public int xpReward = 50;

    [Header("Flags")]
    private bool isDying = false;

    [Header("Hit Effect Position")]
    public Transform pos;

    private AngryGodAiCore aiCore;

    public GameObject adamCharacter;
    public GameObject devaCharacter;
    public float devaSpawnOffsetDistance = 1.0f;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        col = GetComponent<Collider2D>();
        cameraShake = Camera.main != null ? Camera.main.GetComponent<CameraShakeSystem>() : null;
        aiCore = GetComponent<AngryGodAiCore>();

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
        if (isDying || (aiCore != null && aiCore.IsCurrentlyActingOrSkillActive()))
        {
        }
        if (isDying) return;

        currentHealth -= damage;
        currentHealth = Mathf.Clamp(currentHealth, 0, MaxHealth);

        if (!phase2Triggered && currentHealth <= MaxHealth / 2)
        {
            phase2Triggered = true;
            Debug.Log("▶ HP 50% 이하! 각성 시퀀스 요청 준비.");

            if (aiCore != null)
            {
                aiCore.RequestAwakeningSequence();
            }
            else
            {
                Debug.LogWarning("AI Core 참조가 없어 각성 시퀀스를 요청할 수 없습니다!");
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
            if (cameraShake != null)
                cameraShake.StartCoroutine(cameraShake.Shake(0.1f, 0.1f));
        }
    }

    private IEnumerator Die(bool fromAdam, bool fromDeba)
    {
        if (isDying) yield break;
        isDying = true;

        Time.timeScale = 0.2f;
        Time.fixedDeltaTime = 0.02f * Time.timeScale;

        Debug.Log("▶ 슬로우 모션 시작");

        yield return new WaitForSecondsRealtime(1.5f);

        Time.timeScale = 1f;
        Time.fixedDeltaTime = 0.02f;

        Debug.Log("▶ 슬로우 모션 복구");



 

        yield return new WaitForSeconds(1f);
        Debug.Log("▶ 씬 이동 중...");
        SceneManager.LoadScene("end"); // 여기에 전환할 씬 이름을 넣으세요
    }
}
