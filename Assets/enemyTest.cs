using System.Collections;
using UnityEngine;

public class enemyTest : MonoBehaviour
{
    //  ������Ʈ ����
    private Animator TestAnime;
    private Rigidbody2D rb;
    private Collider2D col;

    //  ����Ʈ ������
    public GameObject[] bloodEffectPrefabs;
    public GameObject parringEffects;
    public ParticleSystem bloodEffectParticle;

    //  ��Ÿ ����
    private CameraShakeSystem cameraShake;

    [Header("Stats")]
    public int MaxHealth = 100;
    public int currentHealth;
    public float knockbackForce = 5f;
    public int xpReward = 50; //  �� óġ �� ������ ����ġ

    [Header("Flags")]
    private bool isDying = false;
    private bool isParrying = false;

    [Header("Hit Effect Position")]
    public Transform pos; //  �ǰ� ����Ʈ ��ġ

    private void Start()
    {
        // ������Ʈ �ʱ�ȭ
        TestAnime = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        col = GetComponent<Collider2D>();
        cameraShake = Camera.main != null ? Camera.main.GetComponent<CameraShakeSystem>() : null;

        // ü�� �ʱ�ȭ
        currentHealth = MaxHealth;
    }

    //  �ǰ� ����Ʈ ���
    public void ShowBloodEffect()
    {
        if (bloodEffectPrefabs != null && bloodEffectPrefabs.Length > 0)
        {
            int randomIndex = Random.Range(0, bloodEffectPrefabs.Length);
            GameObject selectedEffect = bloodEffectPrefabs[randomIndex];

            GameObject bloodEffect = Instantiate(selectedEffect, pos.position, Quaternion.identity);
            Destroy(bloodEffect, 0.3f);

            // ��ƼŬ ���⵵ �Բ�
            if (bloodEffectParticle != null)
            {
                ParticleSystem bloodParticle = Instantiate(bloodEffectParticle, pos.position, Quaternion.identity);
                bloodParticle.Play();
                Destroy(bloodParticle.gameObject, bloodParticle.main.duration + 0.5f);
            }
        }
    }

    //  ���� �浹 ó��
    void OnTriggerEnter2D(Collider2D other)
    {
        if (isParrying || isDying) return;

        if (other.CompareTag("PlayerAttack"))
        {
            // ���� ��ü Ȯ��
            DevaAttackDamage debaDamage = other.GetComponentInParent<DevaAttackDamage>();
            PlayerAttackDamage adamDamage = other.GetComponentInParent<PlayerAttackDamage>();

            int damage = 0;
            bool isFromAdam = false;
            bool isFromDeba = false;

            // ���� �����ߴ��� �Ǵ�
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

            // ���� ����� ������ Ȯ�� (���ÿ� ���� �� �ǰ� ����)
            enemyTest closestEnemy = FindClosestEnemy(other.transform);
            if (closestEnemy == this && damage > 0)
            {
                if (currentHealth > 0 && !isDying)
                {
                    TestAnime.Play("Hurt", 0, 0f);
                    TakeDamage(damage, isFromAdam, isFromDeba);
                    ShowBloodEffect();
                    Knockback(other.transform);

                    // ī�޶� ���� ����
                    if (cameraShake != null)
                    {
                        StartCoroutine(cameraShake.Shake(0.1f, 0.1f));
                    }
                }
            }
        }
    }

    //  �÷��̾� ���� ���� ����� enemy ã��
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

    //  �и� ����
    public void StartParry()
    {
        isParrying = true;
        StartCoroutine(ResetParry());
    }

    private IEnumerator ResetParry()
    {
        yield return new WaitForSeconds(0.1f); // ��� �и� ���� ����
        isParrying = false;
    }

    //  ������ ó��
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
            Knockback(attacker); // �� ��ġ ��� �˹�
            cameraShake?.StartCoroutine(cameraShake.Shake(0.1f, 0.1f));
        }
    }


    //  �˹� ó��
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



    //  ��� ó�� + ����ġ ����
    private IEnumerator Die(bool fromAdam, bool fromDeba)
    {
        if (isDying) yield break;
        isDying = true;

        ShowBloodEffect(); // ?? ���⼭ �� �� ���� ����!

        // ����ġ ����
        if (fromAdam && PlayerExperience.Instance != null)
        {
            PlayerExperience.Instance.GainXP(xpReward);
        }
        else if (fromDeba && DevaExperience.Instance != null)
        {
            DevaExperience.Instance.GainXP(xpReward);
        }

        // ���� ����
        if (rb != null)
        {
            rb.velocity = Vector2.zero;
            rb.bodyType = RigidbodyType2D.Kinematic;
            rb.simulated = false;
        }

        // �浹 ��Ȱ��ȭ
        if (col != null) col.enabled = false;

        // ���� �ִϸ��̼�
        TestAnime.SetTrigger("Die");

        yield return new WaitForSeconds(0.6f);
        Destroy(gameObject);
    }

}
