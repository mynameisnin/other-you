using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;  // DOTween ���
public class HealthBarUI : MonoBehaviour
{
    public Image healthBarFill;   // �ǽð� ü�� �� (������)
    public Image healthBarBack;   // ������ ���� �� (�����)

    private float maxHealth;

    public void Initialize(float maxHP)
    {
        maxHealth = maxHP;
        UpdateHealthBar(maxHP, false);
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

}