using System.Collections; // �ڷ�ƾ ����� ���� ���ӽ����̽�
using UnityEngine; // Unity ���� ���� ��� ���

public class enemyTest : MonoBehaviour // ���� �ǰ� �� ��� ó�� ��ũ��Ʈ
{
    // ������Ʈ ����
    private Animator TestAnime; // �ִϸ����� ����
    private Rigidbody2D rb; // ������ٵ� ����
    private Collider2D col; // �ݶ��̴� ����

    // ����Ʈ ������
    public GameObject[] bloodEffectPrefabs; // �ǰ� �� ������ ���� ����Ʈ ������ �迭
    public GameObject parringEffects; // �и� �� ����� ����Ʈ
    public ParticleSystem bloodEffectParticle; // ��ƼŬ Ÿ�� ���� ����Ʈ

    // ��Ÿ ����
    private CameraShakeSystem cameraShake; // ī�޶� ���� ��ũ��Ʈ ����

    [Header("Stats")]
    public int MaxHealth = 100; // �ִ� ü��
    public int currentHealth; // ���� ü��
    public float knockbackForce = 5f; // �˹� ��
    public int xpReward = 50; // �׾��� �� �� ����ġ��

    [Header("Flags")]
    private bool isDying = false; // �״� ������ ����
    private bool isParrying = false; // �и� ������ ����

    [Header("Hit Effect Position")]
    public Transform pos; // �ǰ� ����Ʈ�� ǥ���� ��ġ

    private void Start()
    {
        // ������Ʈ �ʱ�ȭ
        TestAnime = GetComponent<Animator>(); // �ִϸ����� ��������
        rb = GetComponent<Rigidbody2D>(); // ������ٵ� ��������
        col = GetComponent<Collider2D>(); // �ݶ��̴� ��������
        cameraShake = Camera.main != null ? Camera.main.GetComponent<CameraShakeSystem>() : null; // ���� ī�޶󿡼� ī�޶� ���� ������Ʈ ��������

        // ü�� �ʱ�ȭ
        currentHealth = MaxHealth;
    }

    // �ǰ� ����Ʈ�� �����ִ� �Լ�
    public void ShowBloodEffect()
    {
        if (bloodEffectPrefabs != null && bloodEffectPrefabs.Length > 0) // �������� ������ ���
        {
            int randomIndex = Random.Range(0, bloodEffectPrefabs.Length); // ���� ����Ʈ ����
            GameObject selectedEffect = bloodEffectPrefabs[randomIndex]; // ���õ� ����Ʈ

            GameObject bloodEffect = Instantiate(selectedEffect, pos.position, Quaternion.identity); // ����Ʈ ����
            Destroy(bloodEffect, 0.3f); // ��� �� �ı�

            // ��ƼŬ ����Ʈ�� ���
            if (bloodEffectParticle != null)
            {
                ParticleSystem bloodParticle = Instantiate(bloodEffectParticle, pos.position, Quaternion.identity); // ��ƼŬ ����
                bloodParticle.Play(); // ���
                Destroy(bloodParticle.gameObject, bloodParticle.main.duration + 0.5f); // ���� �ð� ���� �ı�
            }
        }
    }

    // ���� �浹 ó�� �Լ�
    void OnTriggerEnter2D(Collider2D other)
    {
        if (isParrying || isDying) return; // �и� ���̰ų� �״� ���̸� ó�� �� ��

        if (other.CompareTag("PlayerAttack")) // ���� �±� �浹 ��
        {
            // ������ ��� Ȯ��
            DevaAttackDamage debaDamage = other.GetComponentInParent<DevaAttackDamage>(); // ���� ���� ������Ʈ
            PlayerAttackDamage adamDamage = other.GetComponentInParent<PlayerAttackDamage>(); // �ƴ� ���� ������Ʈ

            int damage = 0; // ������ �ʱ�ȭ
            bool isFromAdam = false; // �ƴ� ���� ����
            bool isFromDeba = false; // ���� ���� ����

            // � �÷��̾ �����ߴ��� Ȯ��
            if (adamDamage != null)
            {
                damage = adamDamage.GetNomalAttackDamage(); // �ƴ� ������
                isFromAdam = true;
            }
            else if (debaDamage != null)
            {
                damage = debaDamage.GetMagicDamage(); // ���� ������
                isFromDeba = true;
            }

            // ���� ����� ���� �ǰ� ó��
            enemyTest closestEnemy = FindClosestEnemy(other.transform); // ���� ����� �� ã��
            if (closestEnemy == this && damage > 0) // ���� ���� ������ �������� ���� ���
            {
                if (currentHealth > 0 && !isDying) // ���� ����ִ� ���
                {
                    TestAnime.Play("Hurt", 0, 0f); // �ǰ� �ִϸ��̼� ����
                    TakeDamage(damage, isFromAdam, isFromDeba); // ������ ó��
                    ShowBloodEffect(); // ����Ʈ ���
                    Knockback(other.transform); // �˹� ó��

                    if (cameraShake != null)
                    {
                        StartCoroutine(cameraShake.Shake(0.1f, 0.1f)); // ī�޶� ����
                    }
                }
            }
        }
    }

    // ������ �������� ���� ����� �� ã��
    enemyTest FindClosestEnemy(Transform attacker)
    {
        enemyTest[] enemies = FindObjectsOfType<enemyTest>(); // ���� �����ϴ� ��� enemyTest ��ü
        enemyTest closest = null; // ���� ����� �� ����
        float closestDistance = Mathf.Infinity; // �ʱ� �Ÿ� ����

        foreach (enemyTest enemy in enemies)
        {
            float dist = Vector2.Distance(attacker.position, enemy.transform.position); // �Ÿ� ���
            if (dist < closestDistance)
            {
                closestDistance = dist;
                closest = enemy;
            }
        }

        return closest; // ���� ����� �� ��ȯ
    }

    // �и� ���� ó��
    public void StartParry()
    {
        isParrying = true; // �и� ���� ����
        StartCoroutine(ResetParry()); // ª�� �ð� �� �ʱ�ȭ
    }

    // �и� ���� ����
    private IEnumerator ResetParry()
    {
        yield return new WaitForSeconds(0.1f); // ��� �и� ����
        isParrying = false; // �и� ����
    }

    // �������� ó���ϴ� �Լ�
    public void TakeDamage(int damage, bool fromAdam, bool fromDeba, Transform attacker = null)
    {
        if (isDying) return; // �̹� �״� ���̸� ����

        currentHealth -= damage; // ü�� ����
        currentHealth = Mathf.Clamp(currentHealth, 0, MaxHealth); // ���� ����

        if (currentHealth <= 0)
        {
            StartCoroutine(Die(fromAdam, fromDeba)); // ��� ó�� ����
        }
        else
        {
            TestAnime.Play("Hurt", 0, 0f); // �ǰ� �ִϸ��̼�
            ShowBloodEffect(); // ����Ʈ ���
            Knockback(attacker); // �˹� ó��
            cameraShake?.StartCoroutine(cameraShake.Shake(0.1f, 0.1f)); // ī�޶� ����
        }
    }

    // �˹� ó�� �Լ�
    private void Knockback(Transform attacker)
    {
        if (rb == null || attacker == null) return; // �˹� �Ұ� ����

        float dir = Mathf.Sign(transform.position.x - attacker.position.x); // �����ڿ��� ��� ���� ���
        if (dir == 0) dir = Random.Range(0, 2) == 0 ? -1f : 1f; // ������ ���� ��ġ�� ��� ���� ����

        float finalKnockback = knockbackForce; // �⺻ �˹�

        if (attacker.CompareTag("AdamSkill")) // �ƴ� ��ų�� ���
        {
            finalKnockback *= 0.6f; // �˹� ����
        }
        else if (attacker.CompareTag("DevaSkill")) // ���� ��ų�� ���
        {
            finalKnockback *= 0.6f;
        }

        rb.velocity = new Vector2(finalKnockback * dir, rb.velocity.y + 1f); // �˹� ���� �� ���� Ƣ���
    }

    // ��� ó�� �Լ�
    private IEnumerator Die(bool fromAdam, bool fromDeba)
    {
        if (isDying) yield break; // �̹� ��� ���̸� �ߺ� ó�� ����
        isDying = true; // ��� ���� ����

        ShowBloodEffect(); // ����Ʈ �� ���� ���

        // ����ġ ���� ó��
        if (fromAdam && PlayerExperience.Instance != null)
        {
            PlayerExperience.Instance.GainXP(xpReward); // �ƴ㿡�� ����ġ ����
        }
        else if (fromDeba && DevaExperience.Instance != null)
        {
            DevaExperience.Instance.GainXP(xpReward); // ���ٿ��� ����ġ ����
        }

        // ���� ����
        if (rb != null)
        {
            rb.velocity = Vector2.zero; // �ӵ� ����
            rb.bodyType = RigidbodyType2D.Kinematic; // ���� ��Ȱ��ȭ
            rb.simulated = false;
        }

        // �浹 ��Ȱ��ȭ
        if (col != null) col.enabled = false;

        // ���� �ִϸ��̼� ����
        TestAnime.SetTrigger("Die");

        yield return new WaitForSeconds(0.6f); // �ִϸ��̼� ��� �ð� ���
        Destroy(gameObject); // �� ������Ʈ �ı�
    }
}
