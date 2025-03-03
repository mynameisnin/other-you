using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System.Collections;

public class EnergyBarUI : MonoBehaviour
{
    public Image energyBarFill;   // ENERGY 바 (초록색)

    private float maxEnergy = 100f; // 최대 ENERGY 값
    private float currentEnergy;
    public float energyRegenRate = 15f; //  초당 회복량 증가 (기존 5 → 15)
    public float regenDelay = 2f; // 공격 후 회복이 시작되기까지의 지연 시간

    private Coroutine regenCoroutine; // 에너지 회복 코루틴

    void Start()
    {
        currentEnergy = maxEnergy;
        UpdateEnergyBar(currentEnergy, false);
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

        if (energyBarFill == null)
        {
            Debug.LogError("[EnergyBarUI] energyBarFill이 할당되지 않음!");
            return;
        }

        if (animate)
        {
            energyBarFill.DOFillAmount(energyRatio, 0.4f).SetEase(Ease.OutQuad);
        }
        else
        {
            energyBarFill.fillAmount = energyRatio;
        }
    }

    public void ReduceEnergy(float amount)
    {
        currentEnergy -= amount;
        currentEnergy = Mathf.Clamp(currentEnergy, 0, maxEnergy);
        UpdateEnergyBar(currentEnergy);

        // 공격 시 회복 중지 및 일정 시간 후 다시 회복 시작
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
        UpdateEnergyBar(currentEnergy);
    }

    public bool HasEnoughEnergy(float amount)
    {
        return currentEnergy >= amount;
    }

    private IEnumerator StartEnergyRegenAfterDelay()
    {
        yield return new WaitForSeconds(regenDelay); // 공격 후 일정 시간 대기

        while (currentEnergy < maxEnergy)
        {
            currentEnergy += energyRegenRate; //  초당 회복량을 직접 추가
            currentEnergy = Mathf.Clamp(currentEnergy, 0, maxEnergy);
            UpdateEnergyBar(currentEnergy);

            yield return new WaitForSeconds(0.1f); //  0.1초마다 회복 (기존: 매 프레임 회복)
        }

        regenCoroutine = null; // 회복 완료 후 코루틴 초기화
    }
    public bool IsEnergyEmpty()
    {
        return currentEnergy <= 0;
    }

}
