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
    public Image manaBarFill;           // ���� ������
    public TextMeshProUGUI manaText;    // ���� ��ġ �ؽ�Ʈ

    [Header("EXP UI")]
    public TextMeshProUGUI expText;
    public Image expFill;

    [Header("Stat Upgrade Buttons")]
    public Button attackButton;
    public Button defenseButton;
    public Button healthButton;
    public Button energyButton;

    private float lastExpRatio = -1f;


    public Button manaButton;           // ���� ���׷��̵� ��ư
    private void Start()
    {
        attackButton.onClick.AddListener(() => UpgradeStat("attack"));
        defenseButton.onClick.AddListener(() => UpgradeStat("defense"));
        healthButton.onClick.AddListener(() => UpgradeStat("health"));
        energyButton.onClick.AddListener(() => UpgradeStat("energy"));
        manaButton.onClick.AddListener(() => UpgradeStat("mana")); // ���� ��ư �̺�Ʈ ����
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
        // ����
        manaText.text = $"{DevaStats.Instance.currentMana} / {DevaStats.Instance.maxMana}";
        float manaRatio = (float)DevaStats.Instance.currentMana / DevaStats.Instance.maxMana;
        manaBarFill.fillAmount = manaRatio;
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
            case "mana":
                DevaStats.Instance.IncreaseMaxMana();
                break;
        }

        UpdateStatPanel(); // UI ������Ʈ
    }
}
