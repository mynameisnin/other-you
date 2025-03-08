using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class PlayerStats : MonoBehaviour
{
    public static PlayerStats Instance; // ? �̱��� ����

    [Header("Character Stats")]
    public int level = 1;
    public int experience = 0;
    public int experienceToNextLevel = 100; // ? ���� �������� �ʿ��� ����ġ
    public int statPoints = 0;  // ? ���� ����Ʈ

    public int attackPower = 10;
    public int defense = 5;
    public int maxHealth = 100;
    public int currentHealth;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }

    void Start()
    {
        //  HurtPlayer�� ü���� ����ȭ
        if (HurtPlayer.Instance != null)
        {
            HurtPlayer.Instance.currentHealth = currentHealth;
        }
    }

    //  ü�� ȸ�� �Լ� �߰�
    public void Heal(int amount)
    {
        currentHealth += amount;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

        //  HurtPlayer�� ü�µ� ������Ʈ
        if (HurtPlayer.Instance != null)
        {
            HurtPlayer.Instance.currentHealth = currentHealth;
        }

        Debug.Log($"ü�� ȸ��! ���� ü��: {currentHealth} / {maxHealth}");
    }

    // ? PlayerExperience���� ȣ���ϴ� ������ �Լ�
    public void LevelUp()
    {
        level++;
        statPoints += 3; // ? ���� ����Ʈ ����

        Debug.Log($"���� ��! ���� ����: {level}, ���� ���� ����Ʈ: {statPoints}");

        // ? PlayerExperience���� �ݿ� (����ġ �ʿ䷮ ������Ʈ)
        if (PlayerExperience.Instance != null)
        {
            experienceToNextLevel = PlayerExperience.Instance.xpToNextLevel;
        }
    }

    // ? ���� ���� �Լ�
    public void IncreaseAttack()
    {
        if (statPoints > 0)
        {
            attackPower += 2;
            statPoints--;
            Debug.Log($"���ݷ� ����! ���� ���ݷ�: {attackPower}, ���� ���� ����Ʈ: {statPoints}");
        }
    }

    public void IncreaseDefense()
    {
        if (statPoints > 0)
        {
            defense += 1;
            statPoints--;
            Debug.Log($"���� ����! ���� ����: {defense}, ���� ���� ����Ʈ: {statPoints}");
        }
    }

    public void IncreaseMaxHealth()
{
    if (statPoints > 0)
    {
        maxHealth += 10;
        currentHealth = maxHealth; // ? �ִ� ü���� �����ϸ� ���� ü�µ� ȸ��

        // ? HurtPlayer�� �ִ� ü�µ� ���� ����
        if (HurtPlayer.Instance != null)
        {
            HurtPlayer.Instance.MaxHealth = maxHealth;
            HurtPlayer.Instance.currentHealth = currentHealth;
            HurtPlayer.Instance.UpdateHealthUI();
        }

        // ? HealthBarUI���� �ݿ�
        if (HealthBarUI.Instance != null)
        {
            HealthBarUI.Instance.UpdateMaxHealth(maxHealth);
        }

        statPoints--;
        Debug.Log($"�ִ� ü�� ����! ���� ü��: {maxHealth}, ���� ���� ����Ʈ: {statPoints}");
    }
}

}
