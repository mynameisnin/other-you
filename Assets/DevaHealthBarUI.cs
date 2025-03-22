using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
public class DevaHealthBarUI : MonoBehaviour
{
    public static DevaHealthBarUI Instance; //  ? 싱글톤 적용

    public Image healthBarFill;   // ? 실시간 체력 바 (빨간색)
    public Image healthBarBack;   // ? 딜레이 감소 바 (노란색)
    public Image healthBarBorder; // ? HP 테두리 이미지

    private float maxHealth;
    private float initialWidth; // ? 초기 HP바 크기 저장

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
        initialWidth = healthBarFill.rectTransform.sizeDelta.x; // ? 초기 크기 저장
        UpdateHealthBar(maxHP, false);
        UpdateHealthBarSize(); // ? 체력바 크기 업데이트
    }

    public void UpdateHealthBar(float currentHealth, bool animate = true)
    {
        float healthRatio = currentHealth / maxHealth;

        // ? HP바 크기를 직접 조정하여 체력이 증가할 때 확장
        float newWidth = initialWidth * (maxHealth / 100f); // ? 초기 크기 대비 증가율 계산

        healthBarFill.rectTransform.DOSizeDelta(new Vector2(newWidth, healthBarFill.rectTransform.sizeDelta.y), 0.5f)
            .SetEase(Ease.OutQuad);

        // ? 테두리도 HP 바와 동기화하여 크기 조정
        healthBarBorder.rectTransform.DOSizeDelta(new Vector2(newWidth, healthBarBorder.rectTransform.sizeDelta.y), 0.5f)
            .SetEase(Ease.OutQuad);

        // ? 실시간 체력 반영
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

    // ? 최대 체력 증가 시 HP 바 크기 확장
    public void UpdateMaxHealth(float newMaxHealth)
    {
        maxHealth = newMaxHealth;
        UpdateHealthBar(PlayerStats.Instance.currentHealth, false);
        UpdateHealthBarSize(); // ? 테두리 크기 업데이트
    }

    // ? HP 테두리를 늘려서 최대 체력 증가 반영
    private void UpdateHealthBarSize()
    {
        if (healthBarBorder == null || healthBarFill == null) return;

        float newWidth = initialWidth * (maxHealth / 100f); // ? HP바 크기 기준 확장

        healthBarBorder.rectTransform.DOSizeDelta(new Vector2(newWidth, healthBarBorder.rectTransform.sizeDelta.y), 0.5f)
            .SetEase(Ease.OutQuad);

        healthBarFill.rectTransform.DOSizeDelta(new Vector2(newWidth, healthBarFill.rectTransform.sizeDelta.y), 0.5f)
            .SetEase(Ease.OutQuad);
    }
}
