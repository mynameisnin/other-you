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

        // ����ġ ����
        float expRatio = (float)DevaStats.Instance.experience / DevaStats.Instance.experienceToNextLevel;
        if (Mathf.Abs(expRatio - lastExpRatio) > 0.001f)
        {
            expFill.DOFillAmount(expRatio, 0.5f).SetEase(Ease.OutQuad);
            lastExpRatio = expRatio;
        }

        // ����ġ �ؽ�Ʈ
        expText.text = $"EXP: {DevaStats.Instance.experience} / {DevaStats.Instance.experienceToNextLevel}";

        // ���� �� ���� ����Ʈ
        levelText.text = $"����: {DevaStats.Instance.level}";
        statPointsText.text = $"���� ����Ʈ: {DevaStats.Instance.statPoints}";

        // ü��
        float healthRatio = (float)DevaStats.Instance.currentHealth / DevaStats.Instance.maxHealth;
        healthBarFill.fillAmount = healthRatio;
        healthText.text = $"{DevaStats.Instance.currentHealth} / {DevaStats.Instance.maxHealth}";

        // ������
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

        UpdateStatPanel(); // UI ������Ʈ
    }
}
