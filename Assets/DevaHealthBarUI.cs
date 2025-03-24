using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class DevaHealthBarUI : MonoBehaviour
{
    public static DevaHealthBarUI Instance;

    public Image healthBarFill;   // 실시간 체력 바 (빨간색)
    public Image healthBarBack;   // 딜레이 감소 바 (노란색)
    public Image healthBarBorder; // 테두리

    private float maxHealth;
    private float initialWidth;

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
        initialWidth = healthBarFill.rectTransform.sizeDelta.x;
        UpdateHealthBar(maxHP, false);
        UpdateHealthBarSize();
    }

    public void UpdateHealthBar(float currentHealth, bool animate = true)
    {
        float healthRatio = currentHealth / maxHealth;

        float newWidth = initialWidth * (maxHealth / 100f);

        healthBarFill.rectTransform.DOSizeDelta(
            new Vector2(newWidth, healthBarFill.rectTransform.sizeDelta.y), 0.5f)
            .SetEase(Ease.OutQuad);

        if (healthBarBorder != null)
        {
            healthBarBorder.rectTransform.DOSizeDelta(
                new Vector2(newWidth, healthBarBorder.rectTransform.sizeDelta.y), 0.5f)
                .SetEase(Ease.OutQuad);
        }

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
        UpdateHealthBarSize();
    }

    private void UpdateHealthBarSize()
    {
        if (healthBarBorder == null || healthBarFill == null) return;

        float newWidth = initialWidth * (maxHealth / 100f);

        healthBarBorder.rectTransform.DOSizeDelta(
            new Vector2(newWidth, healthBarBorder.rectTransform.sizeDelta.y), 0.5f)
            .SetEase(Ease.OutQuad);

        healthBarFill.rectTransform.DOSizeDelta(
            new Vector2(newWidth, healthBarFill.rectTransform.sizeDelta.y), 0.5f)
            .SetEase(Ease.OutQuad);
    }
}
