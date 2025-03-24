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
    private float fullBarWidth; // 현재 설정된 바의 너비 (100%)

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

        // 현재 UI 바의 width를 기준으로 최대 크기 저장 (디자인상 100%)
        fullBarWidth = energyBarFill.rectTransform.sizeDelta.x;

        RefreshFromDevaStats();
    }

    public void RefreshFromDevaStats()
    {
        UpdateEnergyBar(DevaStats.Instance.currentEnergy, false);
    }

    public void UpdateEnergyBar(float newEnergy, bool animate = true)
    {
        float ratio = (float)newEnergy / DevaStats.Instance.maxEnergy;

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

        if (regenCoroutine != null)
            StopCoroutine(regenCoroutine);

        regenCoroutine = StartCoroutine(StartEnergyRegenAfterDelay());
    }

    public void RecoverEnergy(float amount)
    {
        DevaStats.Instance.currentEnergy += Mathf.RoundToInt(amount);
        DevaStats.Instance.currentEnergy = Mathf.Clamp(DevaStats.Instance.currentEnergy, 0, DevaStats.Instance.maxEnergy);
        SmoothEnergyRegen();
    }

    public bool HasEnoughEnergy(float amount)
    {
        return DevaStats.Instance.currentEnergy >= amount;
    }

    private IEnumerator StartEnergyRegenAfterDelay()
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

        ExpandEnergyBar(newMaxEnergy); //  바 길이 늘리기

        UpdateEnergyBar(DevaStats.Instance.currentEnergy, false);
    }
    private void ExpandEnergyBar(float newMaxEnergy)
    {
        float baseWidth = fullBarWidth; // 초기 width 기준 (디자인상 maxEnergy = 100)
        float targetWidth = (newMaxEnergy / 100f) * baseWidth;

        //  Fill 바 확장
        energyBarFill.rectTransform.DOSizeDelta(
            new Vector2(targetWidth, energyBarFill.rectTransform.sizeDelta.y),
            0.4f
        ).SetEase(Ease.OutCubic);

        //  Back 바 확장
        energyBarBack.rectTransform.DOSizeDelta(
            new Vector2(targetWidth, energyBarBack.rectTransform.sizeDelta.y),
            0.4f
        ).SetEase(Ease.OutCubic);

        //  Border 확장 (있는 경우)
        if (energyBarBorder != null)
        {
            energyBarBorder.rectTransform.DOSizeDelta(
                new Vector2(targetWidth, energyBarBorder.rectTransform.sizeDelta.y),
                0.4f
            ).SetEase(Ease.OutCubic);
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
