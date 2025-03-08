using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class HealthBarUI : MonoBehaviour
{
    public static HealthBarUI Instance; //  싱글톤 적용

    public Image healthBarFill;   // 실시간 체력 바 (빨간색)
    public Image healthBarBack;   // 딜레이 감소 바 (노란색)
    public Image healthBarBorder; //  HP 테두리 이미지

    private float maxHealth;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject); //  중복 방지
        }
    }

    public void Initialize(float maxHP)
    {
        maxHealth = maxHP;
        UpdateHealthBar(maxHP, false);
        UpdateHealthBarSize(); //  체력바 크기 업데이트
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

    //  최대 체력 증가 시 호출 (체력바 크기 업데이트)
    public void UpdateMaxHealth(float newMaxHealth)
    {
        maxHealth = newMaxHealth;
        UpdateHealthBar(PlayerStats.Instance.currentHealth, false);
        UpdateHealthBarSize(); //  테두리 크기 업데이트
    }

    //  HP 테두리를 늘려서 최대 체력 증가 반영
    private void UpdateHealthBarSize()
    {
        if (healthBarBorder != null)
        {
            float baseSize = 100f; // ? 최소 크기
            float maxSize = 350f; // ? 최대 크기 증가 (300 → 350)
            float referenceHealth = Mathf.Max(200f, maxHealth); // ? maxHealth보다 조금 더 크게 기준 설정

            float newWidth = Mathf.Lerp(baseSize, maxSize, maxHealth / referenceHealth);
            newWidth = Mathf.Clamp(newWidth, baseSize, maxSize); // ? 최소 100, 최대 350으로 제한

            healthBarBorder.rectTransform.DOSizeDelta(new Vector2(newWidth, healthBarBorder.rectTransform.sizeDelta.y), 0.5f)
                .SetEase(Ease.OutQuad); // ? DOTween으로 부드럽게 애니메이션 적용
        }
    }


}
