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

    public int maxMana = 100;
    public int currentMana;

    [Header("Level Up Effect Settings")]
    public GameObject levelUpEffectPrefab; //  ������ ����Ʈ ������
    public Transform levelUpEffectPosition; // ����Ʈ�� ������ ��ġ (Inspector���� ���� ����)
    public float effectDuration = 2f; // ����Ʈ ���� �ð�

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
        currentMana = maxMana; // ������ �ʱ�ȭ

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
        Debug.Log($"ü�� ȸ��! ���� ü��: {currentHealth} / {maxHealth}");
    }

    public void LevelUp()
    {
        level++;
        statPoints += 3;
        Debug.Log($"���� ��! ���� ����: {level}, ���� ���� ����Ʈ: {statPoints}");

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

            Debug.Log("������ ����Ʈ ����!");
        }
        else
        {
            Debug.LogWarning("������ ����Ʈ ������ �Ǵ� ��ġ�� �������� �ʾҽ��ϴ�.");
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
                EnergyBarUI.Instance.UpdateMaxEnergy(maxEnergy);
        }
    }

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
            currentHealth = maxHealth;

            if (HurtPlayer.Instance != null)
                HurtPlayer.Instance.UpdateHealthUI();

            if (HealthBarUI.Instance != null)
                HealthBarUI.Instance.UpdateMaxHealth(maxHealth);

            statPoints--;
            Debug.Log($"�ִ� ü�� ����! ���� ü��: {maxHealth}, ���� ���� ����Ʈ: {statPoints}");
        }
    }

    public void IncreaseEnergy()
    {
        if (statPoints > 0)
        {
            maxEnergy += 10;
            currentEnergy = maxEnergy;
            statPoints--;

            Debug.Log($"�ִ� ������ ����! ���� ������: {maxEnergy}, ���� ���� ����Ʈ: {statPoints}");

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

            Debug.Log($"�ִ� ���� ����! ���� ����: {maxMana}, ���� ���� ����Ʈ: {statPoints}");

            ManaBarUI.Instance?.UpdateMaxMana(maxMana);
        }
    }
}