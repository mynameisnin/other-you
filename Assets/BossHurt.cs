using UnityEngine;
using System.Collections;
using UnityEngine.Playables;
public class BossHurt : MonoBehaviour
{
    [Header("Phase Object")]
    public GameObject phase2Object; // ���� 2������ ������Ʈ
    public GameObject phase2ObjectAnime;
    private bool phase2Triggered = false; // �ߺ� ������

    private Rigidbody2D rb;
    private Collider2D col;

    public GameObject[] bloodEffectPrefabs;
    public GameObject parringEffects; // ������� ������ �����ص� ����
    public ParticleSystem bloodEffectParticle;

    private CameraShakeSystem cameraShake;
    [Header("�ִϸ�����")]
    public Animator bossAnimator; // ���� �ִϸ����� �߰�
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
    private AngryGodAiCore aiCore; // AI Core ����
    public GameObject adamCharacter; // �ν����Ϳ��� �ƴ� ĳ���� �Ҵ�
    public GameObject devaCharacter; // �ν����Ϳ��� ���� ĳ���� �Ҵ� (���ٴ� ���� �̸� ��Ȱ��ȭ ���·� �ΰų�, �������� ��쿣 �Ʒ� Instantiate ��� ���)
    public float devaSpawnOffsetDistance = 1.0f;
    private void Awake() // Awake�� �����Ͽ� Start���� ���� ���� ����
    {
        rb = GetComponent<Rigidbody2D>();
        col = GetComponent<Collider2D>();
        cameraShake = Camera.main != null ? Camera.main.GetComponent<CameraShakeSystem>() : null;
        aiCore = GetComponent<AngryGodAiCore>(); // AI Core ���� ��������

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
        if (isDying || (aiCore != null && aiCore.IsCurrentlyActingOrSkillActive())) // �ڡڡ� ������ �߿��� �ൿ ���� ���� ������ ó�� �� ���� ��û�� ��� ������ �� ���� (������)
        {
            // �Ǵ� �������� �޵�, ���� ��û ���� �˻縦 �Ʒ� if�����θ� ����
            // ����� isDying�� üũ�ϵ��� ����
        }
        if (isDying) return;


        currentHealth -= damage;
        currentHealth = Mathf.Clamp(currentHealth, 0, MaxHealth);

        // 50% ���� ���� Ʈ���� ó�� (�� �� ����)
        // phase2Triggered�� BossHurt �������� "���� ��û�� ���´°�"�� ����
        if (!phase2Triggered && currentHealth <= MaxHealth / 2)
        {
            phase2Triggered = true; // ���� ��û�� ���� ������ ǥ�� (�ߺ� ����)
            Debug.Log("�� HP 50% ����! ���� ������ ��û �غ�.");

            if (aiCore != null)
            {
                aiCore.RequestAwakeningSequence(); // AI Core�� ���� ������ ���� ��û
                                                   // phase2Object �� phase2ObjectAnime�� Ȱ��ȭ�� AI Core�� ���� ������ ������ ó��
            }
            else
            {
                Debug.LogWarning("AI Core ������ ���� ���� �������� ��û�� �� �����ϴ�!");
                // AI Core�� ���ٸ� ����ó�� ���⼭ ���� phase2Object ���� Ȱ��ȭ�� �� ������,
                // "������ ��뽬 ��" ��� ������ AI Core�� ����ؾ� ��.
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
            if (cameraShake != null) // null üũ �߰�
                cameraShake.StartCoroutine(cameraShake.Shake(0.1f, 0.1f));
        }
    }



    private IEnumerator Die(bool fromAdam, bool fromDeba)
    {
        // ... (���� isDying, ���� ó��, ����ġ ���� ��) ...

        // �ƴ� �̵� Ÿ�Ӷ��� ���
        if (characterMoveTimeline != null)
        {
            Debug.Log("�ƴ� �̵� Ÿ�Ӷ��� ���!");
            characterMoveTimeline.Play();
            // ���� �ƴ� Ÿ�Ӷ����� ���� �Ŀ� ���ٰ� ������ �Ϸ���, ���⼭ Ÿ�Ӷ��� ���� ������ ��ٷ��� ��.
            // yield return new WaitForSeconds((float)characterMoveTimeline.duration);
        }

        // �ڡڡ� ���ٸ� �ƴ��� ���� ��ġ�� ��Ÿ���� �ϱ� �ڡڡ�
        if (devaCharacter != null && adamCharacter != null)
        {
            // 1. ���� ��ġ ����
            //    ��Ȯ�� �ƴ� ��ġ, �Ǵ� �ƴ� ���� ��¦ ��������.
            Vector3 spawnPosition = adamCharacter.transform.position;

            // �ƴ��� ������ ���� ������ �Ϸ��� (�ɼ�)
            spawnPosition += adamCharacter.transform.right * devaSpawnOffsetDistance;
            // �Ǵ� �ƴ��� ���ʿ� ������ �Ϸ��� (�ɼ�)
            // spawnPosition += adamCharacter.transform.forward * devaSpawnOffsetDistance;

            devaCharacter.transform.position = spawnPosition;

            // (���û���) ���ٰ� �ƴ��� �ٶ󺸰� �Ϸ���
            // devaCharacter.transform.LookAt(adamCharacter.transform);
            // �Ǵ� �׳� �ƴ�� ���� ������ ���� �Ϸ���
            devaCharacter.transform.rotation = adamCharacter.transform.rotation;


            // 2. ���� Ȱ��ȭ (���� ��Ȱ��ȭ ���¿��ٸ�)
            devaCharacter.SetActive(true);
            Debug.Log("���ٰ� �ƴ� ��ġ(" + spawnPosition + ")�� ��Ÿ��!");

            // 3. (���û���) ���� ���� �ִϸ��̼�/����Ʈ ���
            Animator devaAnimator = devaCharacter.GetComponent<Animator>();
            if (devaAnimator != null)
            {
                // devaAnimator.SetTrigger("AppearTrigger"); // ����: ���� �ִϸ��̼� Ʈ����
            }
            // ��ƼŬ ����Ʈ ���� �͵� ���⼭ Instantiate ����
        }
        else
        {
            if (devaCharacter == null) Debug.LogWarning("���� ĳ���Ͱ� �Ҵ���� �ʾҽ��ϴ�!");
            if (adamCharacter == null) Debug.LogWarning("�ƴ� ĳ���Ͱ� �Ҵ���� �ʾҽ��ϴ�!");
        }

        if (bossAnimator != null)
        {
            bossAnimator.SetTrigger("Die");
            Debug.Log("[BossHurt] ���� �״� �ִϸ��̼� �����.");
        }

        yield return new WaitForSeconds(1f); // �ִϸ��̼� ��� �ð�
        Destroy(gameObject);
    }
}