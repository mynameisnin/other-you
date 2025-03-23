using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
public class DevaHealthBarUI : MonoBehaviour
{
    public static DevaHealthBarUI Instance; //  ? �̱��� ����

    public Image healthBarFill;   // ? �ǽð� ü�� �� (������)
    public Image healthBarBack;   // ? ������ ���� �� (�����)
    public Image healthBarBorder; // ? HP �׵θ� �̹���

    private float maxHealth;
    private float initialWidth; // ? �ʱ� HP�� ũ�� ����

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
        initialWidth = healthBarFill.rectTransform.sizeDelta.x; // ? �ʱ� ũ�� ����
        UpdateHealthBar(maxHP, false);
        UpdateHealthBarSize(); // ? ü�¹� ũ�� ������Ʈ
    }

    public void UpdateHealthBar(float currentHealth, bool animate = true)
    {
        float healthRatio = currentHealth / maxHealth;

        // ? HP�� ũ�⸦ ���� �����Ͽ� ü���� ������ �� Ȯ��
        float newWidth = initialWidth * (maxHealth / 100f); // ? �ʱ� ũ�� ��� ������ ���

        healthBarFill.rectTransform.DOSizeDelta(new Vector2(newWidth, healthBarFill.rectTransform.sizeDelta.y), 0.5f)
            .SetEase(Ease.OutQuad);

        // ? �׵θ��� HP �ٿ� ����ȭ�Ͽ� ũ�� ����
        healthBarBorder.rectTransform.DOSizeDelta(new Vector2(newWidth, healthBarBorder.rectTransform.sizeDelta.y), 0.5f)
            .SetEase(Ease.OutQuad);

        // ? �ǽð� ü�� �ݿ�
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

    // ? �ִ� ü�� ���� �� HP �� ũ�� Ȯ��
    public void UpdateMaxHealth(float newMaxHealth)
    {
        maxHealth = newMaxHealth;
        UpdateHealthBar(PlayerStats.Instance.currentHealth, false);
        UpdateHealthBarSize(); // ? �׵θ� ũ�� ������Ʈ
    }

    // ? HP �׵θ��� �÷��� �ִ� ü�� ���� �ݿ�
    private void UpdateHealthBarSize()
    {
        if (healthBarBorder == null || healthBarFill == null) return;

        float newWidth = initialWidth * (maxHealth / 100f); // ? HP�� ũ�� ���� Ȯ��

        healthBarBorder.rectTransform.DOSizeDelta(new Vector2(newWidth, healthBarBorder.rectTransform.sizeDelta.y), 0.5f)
            .SetEase(Ease.OutQuad);

        healthBarFill.rectTransform.DOSizeDelta(new Vector2(newWidth, healthBarFill.rectTransform.sizeDelta.y), 0.5f)
            .SetEase(Ease.OutQuad);
    }
}
