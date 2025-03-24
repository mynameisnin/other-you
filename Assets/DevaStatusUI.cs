using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

public class DevaStatPanelUI : MonoBehaviour
{
    [Header("UI Elements")]
    public TextMeshProUGUI levelText;
    public TextMeshProUGUI statPointsText;
    public Image healthBarFill;
    public TextMeshProUGUI healthText;
    public Image energyBarFill;
    public TextMeshProUGUI energyText;

    [Header("EXP UI")]
    public TextMeshProUGUI expText;
    public Image expFill;

    [Header("Stat Upgrade Buttons")]
    public Button attackButton;
    public Button defenseButton;
    public Button healthButton;
    public Button energyButton;

    private float lastExpRatio = -1f;

    private void Start()
    {
        attackButton.onClick.AddListener(() => UpgradeStat("attack"));
        defenseButton.onClick.AddListener(() => UpgradeStat("defense"));
        healthButton.onClick.AddListener(() => UpgradeStat("health"));
        energyButton.onClick.AddListener(() => UpgradeStat("energy"));
    }

    private void Update()
    {
        UpdateStatPanel();
    }

    private void UpdateStatPanel()
    {
        if (DevaStats.Instance == null) return;

        // 경험치 비율
        float expRatio = (float)DevaStats.Instance.experience / DevaStats.Instance.experienceToNextLevel;
        if (Mathf.Abs(expRatio - lastExpRatio) > 0.001f)
        {
            expFill.DOFillAmount(expRatio, 0.5f).SetEase(Ease.OutQuad);
            lastExpRatio = expRatio;
        }

        // 경험치 텍스트
        expText.text = $"EXP: {DevaStats.Instance.experience} / {DevaStats.Instance.experienceToNextLevel}";

        // 레벨 및 스탯 포인트
        levelText.text = $"레벨: {DevaStats.Instance.level}";
        statPointsText.text = $"스탯 포인트: {DevaStats.Instance.statPoints}";

        // 체력
        float healthRatio = (float)DevaStats.Instance.currentHealth / DevaStats.Instance.maxHealth;
        healthBarFill.fillAmount = healthRatio;
        healthText.text = $"{DevaStats.Instance.currentHealth} / {DevaStats.Instance.maxHealth}";

        // 에너지
        float energyRatio = (float)DevaStats.Instance.currentEnergy / DevaStats.Instance.maxEnergy;
        energyBarFill.fillAmount = energyRatio;
        energyText.text = $"{DevaStats.Instance.currentEnergy} / {DevaStats.Instance.maxEnergy}";
    }

    private void UpgradeStat(string statType)
    {
        if (DevaStats.Instance == null) return;

        switch (statType)
        {
            case "attack":
                DevaStats.Instance.IncreaseAttack();
                break;
            case "defense":
                DevaStats.Instance.IncreaseDefense();
                break;
            case "health":
                DevaStats.Instance.IncreaseMaxHealth();
                break;
            case "energy":
                DevaStats.Instance.IncreaseEnergy();
                break;
        }

        UpdateStatPanel(); // UI 업데이트
    }
}
