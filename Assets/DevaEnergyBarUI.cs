using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class DevaEnergyBarUI : MonoBehaviour
{
    public static DevaEnergyBarUI Instance;

    public Image energyBarFill;
    public Image energyBarBack;
    public Image energyBarBorder;

    public float energyRegenRate = 15f;
    public float regenDelay = 2f;

    private Color defaultBorderColor;
    private Coroutine regenCoroutine;
    private float baseWidth = 200f; // ← 기준 너비 (maxEnergy = 100 기준)

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
    }

    private void Start()
    {
        DOTween.SetTweensCapacity(500, 50);

        if (energyBarBorder != null)
            defaultBorderColor = energyBarBorder.color;

        baseWidth = energyBarFill.rectTransform.sizeDelta.x;

        RefreshFromDevaStats();
        StartEnergyRegen(); // ← 자동 회복 루프 시작
    }

    public void RefreshFromDevaStats()
    {
        UpdateEnergyBar(DevaStats.Instance.currentEnergy, false);
        ExpandEnergyBar(DevaStats.Instance.maxEnergy); // ← 바 너비도 확장
    }

    public void UpdateEnergyBar(float newEnergy, bool animate = true)
    {
        float ratio = newEnergy / DevaStats.Instance.maxEnergy;

        energyBarFill.DOKill();
        energyBarBack.DOKill();

        if (animate)
        {
            energyBarFill.DOFillAmount(ratio, 0.4f).SetEase(Ease.OutCubic);
            energyBarBack.DOFillAmount(ratio, 0.8f).SetEase(Ease.OutCubic);
        }
        else
        {
            energyBarFill.fillAmount = ratio;
            energyBarBack.fillAmount = ratio;
        }
    }

    public void UpdateEnergyBar(float newEnergy)
    {
        UpdateEnergyBar(newEnergy, true);
    }

    public void ReduceEnergy(float amount)
    {
        DevaStats.Instance.currentEnergy -= Mathf.RoundToInt(amount);
        DevaStats.Instance.currentEnergy = Mathf.Clamp(DevaStats.Instance.currentEnergy, 0, DevaStats.Instance.maxEnergy);
        UpdateEnergyBar(DevaStats.Instance.currentEnergy);

        RestartEnergyRegen(); // 회복 루프 재시작
    }

    public void RecoverEnergy(float amount)
    {
        DevaStats.Instance.currentEnergy += Mathf.RoundToInt(amount);
        DevaStats.Instance.currentEnergy = Mathf.Clamp(DevaStats.Instance.currentEnergy, 0, DevaStats.Instance.maxEnergy);
        UpdateEnergyBar(DevaStats.Instance.currentEnergy);

        RestartEnergyRegen();
    }

    public bool HasEnoughEnergy(float amount)
    {
        return DevaStats.Instance.currentEnergy >= amount;
    }

    private void StartEnergyRegen()
    {
        if (regenCoroutine != null)
            StopCoroutine(regenCoroutine);

        regenCoroutine = StartCoroutine(EnergyRegenCoroutine());
    }

    private void RestartEnergyRegen()
    {
        if (regenCoroutine != null)
            StopCoroutine(regenCoroutine);

        regenCoroutine = StartCoroutine(EnergyRegenCoroutine());
    }

    private IEnumerator EnergyRegenCoroutine()
    {
        yield return new WaitForSeconds(regenDelay);

        while (DevaStats.Instance.currentEnergy < DevaStats.Instance.maxEnergy)
        {
            DevaStats.Instance.currentEnergy += Mathf.RoundToInt(energyRegenRate * Time.deltaTime);
            DevaStats.Instance.currentEnergy = Mathf.Clamp(DevaStats.Instance.currentEnergy, 0, DevaStats.Instance.maxEnergy);
            SmoothEnergyRegen();
            yield return new WaitForSeconds(0.05f);
        }

        regenCoroutine = null;
    }

    private void SmoothEnergyRegen()
    {
        float ratio = (float)DevaStats.Instance.currentEnergy / DevaStats.Instance.maxEnergy;

        energyBarFill.DOKill();
        energyBarBack.DOKill();

        energyBarFill.DOFillAmount(ratio, 1.0f).SetEase(Ease.OutCubic);
        energyBarBack.DOFillAmount(ratio, 1.5f).SetEase(Ease.OutCubic);
    }

    public void FlashBorder()
    {
        if (energyBarBorder == null) return;

        energyBarBorder.DOKill();
        energyBarBorder.DOColor(Color.red, 0.2f)
            .SetLoops(2, LoopType.Yoyo)
            .OnComplete(() => energyBarBorder.color = defaultBorderColor);
    }

    public void UpdateMaxEnergy(float newMaxEnergy)
    {
        DevaStats.Instance.maxEnergy = Mathf.RoundToInt(newMaxEnergy);
        DevaStats.Instance.currentEnergy = DevaStats.Instance.maxEnergy;

        ExpandEnergyBar(newMaxEnergy);
        UpdateEnergyBar(DevaStats.Instance.currentEnergy, false);
    }

    private void ExpandEnergyBar(float maxEnergy)
    {
        float ratio = maxEnergy / 100f;
        float targetWidth = baseWidth * ratio;

        energyBarFill.rectTransform.sizeDelta = new Vector2(targetWidth, energyBarFill.rectTransform.sizeDelta.y);
        energyBarBack.rectTransform.sizeDelta = new Vector2(targetWidth, energyBarBack.rectTransform.sizeDelta.y);

        if (energyBarBorder != null)
        {
            energyBarBorder.rectTransform.sizeDelta = new Vector2(targetWidth, energyBarBorder.rectTransform.sizeDelta.y);
        }
    }

    public bool IsEnergyEmpty()
    {
        return DevaStats.Instance.currentEnergy <= 0;
    }

    public float GetCurrentEnergy()
    {
        return DevaStats.Instance.currentEnergy;
    }
}
