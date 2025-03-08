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

    public int maxEnergy = 100; // ? �ִ� ������ �߰�
    public int currentEnergy;   // ? ���� ������ �߰�


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
        currentEnergy = maxEnergy; // ? ���� �� �ִ� ������ ����
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

    //  PlayerExperience���� ȣ���ϴ� ������ �Լ�
    //  ������ ó��
    public void LevelUp()
    {
        level++;
        statPoints += 3; // ? ���� ����Ʈ ����

        Debug.Log($"���� ��! ���� ����: {level}, ���� ���� ����Ʈ: {statPoints}");

        //  ����ġ �ʿ䷮ ������Ʈ
        if (PlayerExperience.Instance != null)
        {
            experienceToNextLevel = PlayerExperience.Instance.xpToNextLevel;
        }
    }



    public void IncreaseMaxEnergy()
    {
        if (statPoints > 0)
        {
            maxEnergy += 10;
            currentEnergy = maxEnergy;
            statPoints--;

            Debug.Log($"�ִ� ������ ����! ���� ������: {maxEnergy}, ���� ���� ����Ʈ: {statPoints}");

            if (EnergyBarUI.Instance != null)
            {
                EnergyBarUI.Instance.UpdateMaxEnergy(maxEnergy); // ? UI ������Ʈ
            }
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
    public void IncreaseEnergy()
    {
        if (statPoints > 0)
        {
            maxEnergy += 10; //  ������ ���� ���� �ִ� ������ ����
            currentEnergy = maxEnergy; //  �ִ� �������� �����ϸ� ���� �������� ȸ��
            statPoints--;

            Debug.Log($"�ִ� ������ ����! ���� ������: {maxEnergy}, ���� ���� ����Ʈ: {statPoints}");

            //  EnergyBarUI�� ������Ʈ ��û
            if (EnergyBarUI.Instance != null)
            {
                EnergyBarUI.Instance.UpdateMaxEnergy(maxEnergy);
            }
        }
        else
        {
            Debug.Log("���� ����Ʈ�� �����մϴ�!");
        }
    }


}
