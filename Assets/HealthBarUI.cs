using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;  // DOTween 사용
public class HealthBarUI : MonoBehaviour
{
    public Image healthBarFill;   // 실시간 체력 바 (빨간색)
    public Image healthBarBack;   // 딜레이 감소 바 (노란색)

    private float maxHealth;

    public void Initialize(float maxHP)
    {
        maxHealth = maxHP;
        UpdateHealthBar(maxHP, false);
    }

    public void UpdateHealthBar(float currentHealth, bool animate = true)
    {
        float healthRatio = currentHealth / maxHealth;

        //  1) 빨간 체력 바는 즉시 감소
        healthBarFill.fillAmount = healthRatio;

        if (animate)
        {
            //  2) 노란색 딜레이 바는 DOTween으로 천천히 감소
            healthBarBack.DOFillAmount(healthRatio, 0.6f).SetEase(Ease.OutQuad);
        }
        else
        {
            // 애니메이션 없이 바로 변경 (체력 초기화 시 사용)
            healthBarBack.fillAmount = healthRatio;
        }


    }

}