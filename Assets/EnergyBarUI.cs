using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System.Collections;

public class EnergyBarUI : MonoBehaviour
{
    public static EnergyBarUI Instance;
    public Image energyBarFill;    // ENERGY 바 (초록색)
    public Image energyBarBack;    // 에너지 딜레이 바 (천천히 감소, 노란색)
    public Image energyBarBorder;  //  테두리 이미지 추가

    private Color defaultBorderColor; // 기본 테두리 색상 저장

    private float maxEnergy = 100f; // 최대 ENERGY 값
    private float currentEnergy;
    public float energyRegenRate = 15f; // 초당 회복량 증가
    public float regenDelay = 2f; // 공격 후 회복 시작까지의 지연 시간

    private Coroutine regenCoroutine; // 에너지 회복 코루틴

    void Start()
    {
        DOTween.SetTweensCapacity(500, 50);
        currentEnergy = maxEnergy;
        UpdateEnergyBar(currentEnergy, false);

        if (energyBarBorder != null)
            defaultBorderColor = energyBarBorder.color; // 기본 테두리 색상 저장
    }

    public void Initialize(float maxEN)
    {
        maxEnergy = maxEN;
        currentEnergy = maxEN;
        UpdateEnergyBar(currentEnergy, false);
    }

    public void UpdateEnergyBar(float newEnergy, bool animate = true)
    {
        float energyRatio = newEnergy / maxEnergy;

        if (energyBarFill == null || energyBarBack == null || energyBarBorder == null)
        {
            Debug.LogError("[EnergyBarUI] UI 요소가 할당되지 않음!");
            return;
        }

        energyBarFill.DOKill();
        energyBarBack.DOKill();

        if (newEnergy > currentEnergy) // 에너지가 증가하는 경우 (회복)
        {
            energyBarFill.DOFillAmount(energyRatio, 0.6f).SetEase(Ease.OutCubic);
        }
        else // 에너지가 감소하는 경우 (즉시 반영)
        {
            energyBarFill.fillAmount = energyRatio;
        }

        if (animate)
        {
            energyBarBack.DOFillAmount(energyRatio, 1.0f).SetEase(Ease.OutCubic);
        }
        else
        {
            energyBarBack.fillAmount = energyRatio;
        }
    }

    public void ReduceEnergy(float amount)
    {
        currentEnergy -= amount;
        currentEnergy = Mathf.Clamp(currentEnergy, 0, maxEnergy);
        UpdateEnergyBar(currentEnergy);

        if (regenCoroutine != null)
        {
            StopCoroutine(regenCoroutine);
        }
        regenCoroutine = StartCoroutine(StartEnergyRegenAfterDelay());
    }

    public void RecoverEnergy(float amount)
    {
        currentEnergy += amount;
        currentEnergy = Mathf.Clamp(currentEnergy, 0, maxEnergy);
        SmoothEnergyRegen(currentEnergy);
    }

    public bool HasEnoughEnergy(float amount)
    {
        return currentEnergy >= amount;
    }

    private IEnumerator StartEnergyRegenAfterDelay()
    {
        yield return new WaitForSeconds(regenDelay);

        while (currentEnergy < maxEnergy)
        {
            currentEnergy += energyRegenRate * Time.deltaTime;
            currentEnergy = Mathf.Clamp(currentEnergy, 0, maxEnergy);
            SmoothEnergyRegen(currentEnergy);

            yield return new WaitForSeconds(0.05f);
        }

        regenCoroutine = null;
    }

    private void SmoothEnergyRegen(float targetEnergy)
    {
        float energyRatio = targetEnergy / maxEnergy;

        if (energyBarFill == null || energyBarBack == null) return;

        energyBarFill.DOKill();
        energyBarBack.DOKill();

        energyBarFill.DOFillAmount(energyRatio, 1.0f).SetEase(Ease.OutCubic);
        energyBarBack.DOFillAmount(energyRatio, 1.5f).SetEase(Ease.OutCubic);
    }

    public bool IsEnergyEmpty()
    {
        return currentEnergy <= 0;
    }
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }
    public float GetCurrentEnergy()
    {
        return currentEnergy;
    }
    //  에너지가 부족하면 테두리 색상을 빨간색으로 깜빡이게 함
    public void FlashBorder()
    {
        if (energyBarBorder == null) return;

        energyBarBorder.DOKill(); // 기존 트윈 제거
        energyBarBorder.DOColor(Color.red, 0.2f)
            .SetLoops(2, LoopType.Yoyo) // 2번 깜빡 (빨강 -> 원래색)
            .OnComplete(() => energyBarBorder.color = defaultBorderColor);
    }



}
