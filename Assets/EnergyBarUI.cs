using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System.Collections;

public class EnergyBarUI : MonoBehaviour
{
    public static EnergyBarUI Instance;

    public Image energyBarFill;
    public Image energyBarBack;
    public Image energyBarBorder;

    public float energyRegenRate = 15f;
    public float regenDelay = 2f;

    private Coroutine regenCoroutine;
    private Color defaultBorderColor;

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

        RefreshFromPlayerStats();
        StartEnergyRegen(); // ? 자동 회복 루프 시작
    }

    public void RefreshFromPlayerStats()
    {
        UpdateEnergyBar();
    }

    public void UpdateEnergyBar(bool animate = true)
    {
        float ratio = (float)PlayerStats.Instance.currentEnergy / PlayerStats.Instance.maxEnergy;

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

    public void ReduceEnergy(float amount)
    {
        PlayerStats.Instance.currentEnergy -= Mathf.RoundToInt(amount);
        PlayerStats.Instance.currentEnergy = Mathf.Clamp(PlayerStats.Instance.currentEnergy, 0, PlayerStats.Instance.maxEnergy);
        UpdateEnergyBar();

        RestartEnergyRegen(); // ? 회복 중단 및 재시작
    }

    public void RecoverEnergy(float amount)
    {
        PlayerStats.Instance.currentEnergy += Mathf.RoundToInt(amount);
        PlayerStats.Instance.currentEnergy = Mathf.Clamp(PlayerStats.Instance.currentEnergy, 0, PlayerStats.Instance.maxEnergy);
        UpdateEnergyBar();

        RestartEnergyRegen(); // ? 회복 중단 및 재시작
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

        while (PlayerStats.Instance.currentEnergy < PlayerStats.Instance.maxEnergy)
        {
            PlayerStats.Instance.currentEnergy += Mathf.RoundToInt(energyRegenRate * Time.deltaTime);
            PlayerStats.Instance.currentEnergy = Mathf.Clamp(PlayerStats.Instance.currentEnergy, 0, PlayerStats.Instance.maxEnergy);
            SmoothEnergyRegen();
            yield return new WaitForSeconds(0.05f);
        }

        regenCoroutine = null;
    }

    private void SmoothEnergyRegen()
    {
        float ratio = (float)PlayerStats.Instance.currentEnergy / PlayerStats.Instance.maxEnergy;

        energyBarFill.DOKill();
        energyBarBack.DOKill();

        energyBarFill.DOFillAmount(ratio, 1.0f).SetEase(Ease.OutCubic);
        energyBarBack.DOFillAmount(ratio, 1.5f).SetEase(Ease.OutCubic);
    }

    public bool IsEnergyEmpty()
    {
        return PlayerStats.Instance.currentEnergy <= 0;
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
        PlayerStats.Instance.maxEnergy = Mathf.RoundToInt(newMaxEnergy);
        PlayerStats.Instance.currentEnergy = PlayerStats.Instance.maxEnergy;
        UpdateEnergyBar(false);
    }
}
