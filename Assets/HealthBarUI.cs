using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class HealthBarUI : MonoBehaviour
{
    public static HealthBarUI Instance; //  �̱��� ����

    public Image healthBarFill;   // �ǽð� ü�� �� (������)
    public Image healthBarBack;   // ������ ���� �� (�����)
    public Image healthBarBorder; //  HP �׵θ� �̹���

    private float maxHealth;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject); //  �ߺ� ����
        }
    }

    public void Initialize(float maxHP)
    {
        maxHealth = maxHP;
        UpdateHealthBar(maxHP, false);
        UpdateHealthBarSize(); //  ü�¹� ũ�� ������Ʈ
    }

    public void UpdateHealthBar(float currentHealth, bool animate = true)
    {
        float healthRatio = currentHealth / maxHealth;

        //  1) ���� ü�� �ٴ� ��� ����
        healthBarFill.fillAmount = healthRatio;

        if (animate)
        {
            //  2) ����� ������ �ٴ� DOTween���� õõ�� ����
            healthBarBack.DOFillAmount(healthRatio, 0.6f).SetEase(Ease.OutQuad);
        }
        else
        {
            // �ִϸ��̼� ���� �ٷ� ���� (ü�� �ʱ�ȭ �� ���)
            healthBarBack.fillAmount = healthRatio;
        }
    }

    //  �ִ� ü�� ���� �� ȣ�� (ü�¹� ũ�� ������Ʈ)
    public void UpdateMaxHealth(float newMaxHealth)
    {
        maxHealth = newMaxHealth;
        UpdateHealthBar(PlayerStats.Instance.currentHealth, false);
        UpdateHealthBarSize(); //  �׵θ� ũ�� ������Ʈ
    }

    //  HP �׵θ��� �÷��� �ִ� ü�� ���� �ݿ�
    private void UpdateHealthBarSize()
    {
        if (healthBarBorder != null)
        {
            float baseSize = 100f; // ? �ּ� ũ��
            float maxSize = 350f; // ? �ִ� ũ�� ���� (300 �� 350)
            float referenceHealth = Mathf.Max(200f, maxHealth); // ? maxHealth���� ���� �� ũ�� ���� ����

            float newWidth = Mathf.Lerp(baseSize, maxSize, maxHealth / referenceHealth);
            newWidth = Mathf.Clamp(newWidth, baseSize, maxSize); // ? �ּ� 100, �ִ� 350���� ����

            healthBarBorder.rectTransform.DOSizeDelta(new Vector2(newWidth, healthBarBorder.rectTransform.sizeDelta.y), 0.5f)
                .SetEase(Ease.OutQuad); // ? DOTween���� �ε巴�� �ִϸ��̼� ����
        }
    }


}
