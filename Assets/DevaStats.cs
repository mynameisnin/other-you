using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class DevaStats : MonoBehaviour
{
    public static DevaStats Instance;

    public int level = 1;
    public int experience = 0;
    public int experienceToNextLevel = 100;
    public int statPoints = 0;

    public int attackPower = 8;
    public int defense = 4;
    public int maxHealth = 90;
    public int currentHealth;

    public int maxEnergy = 80;
    public int currentEnergy;

    [Header("Level Up Effect")]
    public GameObject levelUpEffectPrefab;
    public Transform levelUpEffectPosition;
    public float effectDuration = 2f;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
    }

    private void Start()
    {
        currentHealth = maxHealth;
        currentEnergy = maxEnergy;

        if (EnergyBarUI.Instance != null)
        {
            EnergyBarUI.Instance.UpdateEnergyBar(currentEnergy, false);
        }
    }

    public void Heal(int amount)
    {
        currentHealth += amount;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
    }

    public void LevelUp()
    {
        level++;
        statPoints += 3;

        experienceToNextLevel = Mathf.RoundToInt(experienceToNextLevel * 1.5f);

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

            Sequence s = DOTween.Sequence();
            s.Append(effect.transform.DOMoveY(effect.transform.position.y + moveDistance, fadeDuration));
            SpriteRenderer sr = effect.GetComponent<SpriteRenderer>();
            if (sr != null)
            {
                s.Join(sr.DOFade(0, fadeDuration));
            }

            s.OnComplete(() => Destroy(effect));
        }
    }

    // 스탯 증가 함수
    public void IncreaseAttack()
    {
        if (statPoints > 0)
        {
            attackPower += 2;
            statPoints--;
        }
    }

    public void IncreaseDefense()
    {
        if (statPoints > 0)
        {
            defense += 1;
            statPoints--;
        }
    }

    public void IncreaseMaxHealth()
    {
        if (statPoints > 0)
        {
            maxHealth += 10;
            currentHealth = maxHealth;
            statPoints--;
        }
    }

    public void IncreaseEnergy()
    {
        if (statPoints > 0)
        {
            maxEnergy += 10;
            currentEnergy = maxEnergy;
            statPoints--;

            if (EnergyBarUI.Instance != null)
            {
                EnergyBarUI.Instance.UpdateMaxEnergy(maxEnergy);
            }
        }
    }

    public void SetCurrentEnergy(int amount)
    {
        currentEnergy = Mathf.Clamp(amount, 0, maxEnergy);
        if (EnergyBarUI.Instance != null)
        {
            EnergyBarUI.Instance.UpdateEnergyBar(currentEnergy);
        }
    }
}