using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class PlayerStats : MonoBehaviour
{
    public static PlayerStats Instance; // ? 싱글톤 적용

    [Header("Character Stats")]
    public int level = 1;
    public int experience = 0;
    public int experienceToNextLevel = 100; // ? 다음 레벨까지 필요한 경험치
    public int statPoints = 0;  // ? 스탯 포인트

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
        //  HurtPlayer의 체력을 동기화
        if (HurtPlayer.Instance != null)
        {
            HurtPlayer.Instance.currentHealth = currentHealth;
        }
    }

    //  체력 회복 함수 추가
    public void Heal(int amount)
    {
        currentHealth += amount;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

        //  HurtPlayer의 체력도 업데이트
        if (HurtPlayer.Instance != null)
        {
            HurtPlayer.Instance.currentHealth = currentHealth;
        }

        Debug.Log($"체력 회복! 현재 체력: {currentHealth} / {maxHealth}");
    }

    // ? PlayerExperience에서 호출하는 레벨업 함수
    public void LevelUp()
    {
        level++;
        statPoints += 3; // ? 스탯 포인트 지급

        Debug.Log($"레벨 업! 현재 레벨: {level}, 남은 스탯 포인트: {statPoints}");

        // ? PlayerExperience에도 반영 (경험치 필요량 업데이트)
        if (PlayerExperience.Instance != null)
        {
            experienceToNextLevel = PlayerExperience.Instance.xpToNextLevel;
        }
    }

    // ? 스탯 증가 함수
    public void IncreaseAttack()
    {
        if (statPoints > 0)
        {
            attackPower += 2;
            statPoints--;
            Debug.Log($"공격력 증가! 현재 공격력: {attackPower}, 남은 스탯 포인트: {statPoints}");
        }
    }

    public void IncreaseDefense()
    {
        if (statPoints > 0)
        {
            defense += 1;
            statPoints--;
            Debug.Log($"방어력 증가! 현재 방어력: {defense}, 남은 스탯 포인트: {statPoints}");
        }
    }

    public void IncreaseMaxHealth()
{
    if (statPoints > 0)
    {
        maxHealth += 10;
        currentHealth = maxHealth; // ? 최대 체력이 증가하면 현재 체력도 회복

        // ? HurtPlayer의 최대 체력도 같이 증가
        if (HurtPlayer.Instance != null)
        {
            HurtPlayer.Instance.MaxHealth = maxHealth;
            HurtPlayer.Instance.currentHealth = currentHealth;
            HurtPlayer.Instance.UpdateHealthUI();
        }

        // ? HealthBarUI에도 반영
        if (HealthBarUI.Instance != null)
        {
            HealthBarUI.Instance.UpdateMaxHealth(maxHealth);
        }

        statPoints--;
        Debug.Log($"최대 체력 증가! 현재 체력: {maxHealth}, 남은 스탯 포인트: {statPoints}");
    }
}

}
