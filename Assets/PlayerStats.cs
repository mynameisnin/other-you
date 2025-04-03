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

    public int maxEnergy = 100; // ? 최대 에너지 추가
    public int currentEnergy;   // ? 현재 에너지 추가

    public int maxMana = 100;
    public int currentMana;

    [Header("Level Up Effect Settings")]
    public GameObject levelUpEffectPrefab; //  레벨업 이펙트 프리팹
    public Transform levelUpEffectPosition; // 이펙트를 생성할 위치 (Inspector에서 설정 가능)
    public float effectDuration = 2f; // 이펙트 지속 시간

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }

    void Start()
    {
        currentHealth = maxHealth;
        currentEnergy = maxEnergy;
        currentMana = maxMana; // 마나도 초기화

        if (HurtPlayer.Instance != null)
            HurtPlayer.Instance.UpdateHealthUI();

        if (EnergyBarUI.Instance != null)
            EnergyBarUI.Instance.RefreshFromPlayerStats();

        if (ManaBarUI.Instance != null)
            ManaBarUI.Instance.UpdateManaBar(currentMana);
    }

    public void Heal(int amount)
    {
        currentHealth += amount;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
        Debug.Log($"체력 회복! 현재 체력: {currentHealth} / {maxHealth}");
    }

    public void LevelUp()
    {
        level++;
        statPoints += 3;
        Debug.Log($"레벨 업! 현재 레벨: {level}, 남은 스탯 포인트: {statPoints}");

        if (PlayerExperience.Instance != null)
            experienceToNextLevel = PlayerExperience.Instance.xpToNextLevel;

        PlayLevelUpEffect();
    }

    private void PlayLevelUpEffect()
    {
        if (levelUpEffectPrefab != null && levelUpEffectPosition != null)
        {
            GameObject effect = Instantiate(levelUpEffectPrefab, levelUpEffectPosition.position, Quaternion.identity);
            effect.transform.SetParent(levelUpEffectPosition);

            float fadeDuration = 1.5f;
            float moveDistance = 1f;

            Sequence levelUpEffectSequence = DOTween.Sequence();
            levelUpEffectSequence.Append(effect.transform.DOMoveY(effect.transform.position.y + moveDistance, fadeDuration));

            SpriteRenderer spriteRenderer = effect.GetComponent<SpriteRenderer>();
            if (spriteRenderer != null)
            {
                levelUpEffectSequence.Join(spriteRenderer.DOFade(0, fadeDuration));
            }

            levelUpEffectSequence.OnComplete(() => Destroy(effect));

            Debug.Log("레벨업 이펙트 실행!");
        }
        else
        {
            Debug.LogWarning("레벨업 이펙트 프리팹 또는 위치가 설정되지 않았습니다.");
        }
    }

    public void IncreaseMaxEnergy()
    {
        if (statPoints > 0)
        {
            maxEnergy += 10;
            currentEnergy = maxEnergy;
            statPoints--;

            Debug.Log($"최대 에너지 증가! 현재 에너지: {maxEnergy}, 남은 스탯 포인트: {statPoints}");

            if (EnergyBarUI.Instance != null)
                EnergyBarUI.Instance.UpdateMaxEnergy(maxEnergy);
        }
    }

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
            currentHealth = maxHealth;

            if (HurtPlayer.Instance != null)
                HurtPlayer.Instance.UpdateHealthUI();

            if (HealthBarUI.Instance != null)
                HealthBarUI.Instance.UpdateMaxHealth(maxHealth);

            statPoints--;
            Debug.Log($"최대 체력 증가! 현재 체력: {maxHealth}, 남은 스탯 포인트: {statPoints}");
        }
    }

    public void IncreaseEnergy()
    {
        if (statPoints > 0)
        {
            maxEnergy += 10;
            currentEnergy = maxEnergy;
            statPoints--;

            Debug.Log($"최대 에너지 증가! 현재 에너지: {maxEnergy}, 남은 스탯 포인트: {statPoints}");

            if (EnergyBarUI.Instance != null)
                EnergyBarUI.Instance.UpdateMaxEnergy(maxEnergy);
        }
    }

    public void SetCurrentEnergy(int amount)
    {
        currentEnergy = Mathf.Clamp(amount, 0, maxEnergy);
        EnergyBarUI.Instance?.RefreshFromPlayerStats();
    }

    public void ReduceMana(int amount)
    {
        currentMana -= amount;
        currentMana = Mathf.Clamp(currentMana, 0, maxMana);
        ManaBarUI.Instance?.UpdateManaBar(currentMana);
    }

    public void RecoverMana(int amount)
    {
        currentMana += amount;
        currentMana = Mathf.Clamp(currentMana, 0, maxMana);
        ManaBarUI.Instance?.UpdateManaBar(currentMana);
    }

    public bool HasEnoughMana(int amount)
    {
        return currentMana >= amount;
    }

    public void SetCurrentMana(int amount)
    {
        currentMana = Mathf.Clamp(amount, 0, maxMana);
        ManaBarUI.Instance?.UpdateManaBar(currentMana);
    }

    public void IncreaseMaxMana()
    {
        if (statPoints > 0)
        {
            maxMana += 10;
            currentMana = maxMana;
            statPoints--;

            Debug.Log($"최대 마나 증가! 현재 마나: {maxMana}, 남은 스탯 포인트: {statPoints}");

            ManaBarUI.Instance?.UpdateMaxMana(maxMana);
        }
    }
}