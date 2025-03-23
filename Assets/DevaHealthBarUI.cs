using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class DevaHealthBarUI : MonoBehaviour
{
    public static DevaHealthBarUI Instance;

    public Image healthBarFill;
    public Image healthBarBack;
    public Image healthBarBorder;

    private float maxHealth;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void Initialize(float maxHP)
    {
        maxHealth = maxHP;
        UpdateHealthBar(maxHP, false);
    }

    public void UpdateHealthBar(float currentHealth, bool animate = true)
    {
        float healthRatio = (float)currentHealth / maxHealth;

        healthBarFill.DOKill();
        healthBarBack.DOKill();

        healthBarFill.fillAmount = healthRatio;

        if (animate)
        {
            healthBarBack.DOFillAmount(healthRatio, 0.6f).SetEase(Ease.OutQuad);
        }
        else
        {
            healthBarBack.fillAmount = healthRatio;
        }
    }

    public void UpdateMaxHealth(float newMaxHealth)
    {
        maxHealth = newMaxHealth;
        UpdateHealthBar(DevaStats.Instance.currentHealth, false);
    }
}
