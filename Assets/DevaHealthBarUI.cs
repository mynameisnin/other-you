using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class DevaHealthBarUI : MonoBehaviour
{
    public static DevaHealthBarUI Instance;

    public Image healthBarFill;
    public Image healthBarBack;
    public Image healthBarBorder;

    [SerializeField] private float baseWidth = 200f; // ← GUI 디자인 기준 (HP 100일 때)

    private float maxHealth;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    public void Initialize(float maxHP)
    {
        maxHealth = maxHP;
        ExpandHealthBar(maxHealth); // 초기 HP 기준 GUI 확장
        UpdateHealthBar(maxHP, false);
    }

    public void UpdateHealthBar(float currentHealth, bool animate = true)
    {
        float healthRatio = currentHealth / maxHealth;

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
        ExpandHealthBar(maxHealth);
        UpdateHealthBar(DevaStats.Instance.currentHealth, false);
    }

    private void ExpandHealthBar(float maxHP)
    {
        float ratio = maxHP / 100f;
        float targetWidth = baseWidth * ratio;

        healthBarFill.rectTransform.DOSizeDelta(
            new Vector2(targetWidth, healthBarFill.rectTransform.sizeDelta.y),
            0.5f
        ).SetEase(Ease.OutQuad);

        healthBarBack.rectTransform.DOSizeDelta(
            new Vector2(targetWidth, healthBarBack.rectTransform.sizeDelta.y),
            0.5f
        ).SetEase(Ease.OutQuad);

        if (healthBarBorder != null)
        {
            healthBarBorder.rectTransform.DOSizeDelta(
                new Vector2(targetWidth, healthBarBorder.rectTransform.sizeDelta.y),
                0.5f
            ).SetEase(Ease.OutQuad);
        }
    }
}
