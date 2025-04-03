using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

public class StatPanelUI : MonoBehaviour
{
    [Header("UI Elements")]
    public TextMeshProUGUI levelText;
    public TextMeshProUGUI statPointsText;
    public Image healthBarFill;
    public TextMeshProUGUI healthText;
    public Image energyBarFill;
    public TextMeshProUGUI energyText;
    public Image manaBarFill; //  마나 UI 추가
    public TextMeshProUGUI manaText;

    [Header("EXP UI")]
    public TextMeshProUGUI expText;
    public Image expFill;

    [Header("Stat Upgrade Buttons")]
    public Button attackButton;
    public Button defenseButton;
    public Button healthButton;
    public Button energyButton;
    public Button manaButton; //  마나 버튼 추가

    private float lastExpRatio = -1f;

    private void Start()
    {
        attackButton.onClick.AddListener(() => UpgradeStat("attack"));
        defenseButton.onClick.AddListener(() => UpgradeStat("defense"));
        healthButton.onClick.AddListener(() => UpgradeStat("health"));
        energyButton.onClick.AddListener(() => UpgradeStat("energy"));
        manaButton.onClick.AddListener(() => UpgradeStat("mana")); // ? 마나 이벤트 등록
    }

    private void Update()
    {
        UpdateStatPanel();
    }

    private void UpdateStatPanel()
    {
        if (PlayerStats.Instance == null) return;

        // 경험치 Tween
        float expRatio = (float)PlayerStats.Instance.experience / PlayerStats.Instance.experienceToNextLevel;
        if (Mathf.Abs(expRatio - lastExpRatio) > 0.001f)
        {
            expFill.DOFillAmount(expRatio, 0.5f).SetEase(Ease.OutQuad);
            lastExpRatio = expRatio;
        }

        // EXP, 레벨, 스탯 포인트
        expText.text = $"EXP: {PlayerStats.Instance.experience} / {PlayerStats.Instance.experienceToNextLevel}";
        levelText.text = $"레벨: {PlayerStats.Instance.level}";
        statPointsText.text = $"스탯 포인트: {PlayerStats.Instance.statPoints}";

        // 체력
        float healthRatio = (float)PlayerStats.Instance.currentHealth / PlayerStats.Instance.maxHealth;
        healthBarFill.fillAmount = healthRatio;
        healthText.text = $"{PlayerStats.Instance.currentHealth} / {PlayerStats.Instance.maxHealth}";

        // 에너지
        float energyRatio = (float)PlayerStats.Instance.currentEnergy / PlayerStats.Instance.maxEnergy;
        energyBarFill.fillAmount = energyRatio;
        energyText.text = $"{PlayerStats.Instance.currentEnergy} / {PlayerStats.Instance.maxEnergy}";

        // ? 마나
        float manaRatio = (float)PlayerStats.Instance.currentMana / PlayerStats.Instance.maxMana;
        manaBarFill.fillAmount = manaRatio;
        manaText.text = $"{PlayerStats.Instance.currentMana} / {PlayerStats.Instance.maxMana}";
    }

    private void UpgradeStat(string statType)
    {
        if (PlayerStats.Instance == null) return;

        switch (statType)
        {
            case "attack":
                PlayerStats.Instance.IncreaseAttack();
                break;
            case "defense":
                PlayerStats.Instance.IncreaseDefense();
                break;
            case "health":
                PlayerStats.Instance.IncreaseMaxHealth();
                break;
            case "energy":
                PlayerStats.Instance.IncreaseMaxEnergy();
                break;
            case "mana": //  마나 업그레이드
                PlayerStats.Instance.IncreaseMaxMana();
                break;
        }

        UpdateStatPanel(); // UI 갱신
    }
}
