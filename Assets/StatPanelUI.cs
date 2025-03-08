using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening; //  DOTween���� EXP �ٸ� �ε巴�� ������Ʈ

public class StatPanelUI : MonoBehaviour
{
    [Header("UI Elements")]
    public TextMeshProUGUI levelText;  // ���� ǥ��
    public TextMeshProUGUI statPointsText; //  ��� ������ ���� ����Ʈ ǥ��
    public Image healthBarFill;        // ü�� ������
    public TextMeshProUGUI healthText; // ü�� ��ġ �ؽ�Ʈ
    public Image energyBarFill;        // ������ ������
    public TextMeshProUGUI energyText; // ������ ��ġ �ؽ�Ʈ

    [Header("EXP UI")]
    public TextMeshProUGUI expText; //  ����ġ ��ġ ǥ�� (��: "EXP: 50 / 100")
    public Image expFill; //  ����ġ �� UI �߰�

    [Header("Stat Upgrade Buttons")]
    public Button attackButton;
    public Button defenseButton;
    public Button healthButton;

    private void Start()
    {
        //  ��ư Ŭ�� �̺�Ʈ ����
        attackButton.onClick.AddListener(() => UpgradeStat("attack"));
        defenseButton.onClick.AddListener(() => UpgradeStat("defense"));
        healthButton.onClick.AddListener(() => UpgradeStat("health"));
    }

    private void Update()
    {
        UpdateStatPanel();
    }
    private float lastExpRatio = -1f; // ? ���� ����ġ ���� ���� (���ʿ��� Tween ȣ�� ����)

    private void UpdateStatPanel()
    {
        if (PlayerStats.Instance == null) return;

        // ? ����ġ �� ������Ʈ (���� ���� �ٸ� ���� Tween ����)
        float expRatio = (float)PlayerStats.Instance.experience / PlayerStats.Instance.experienceToNextLevel;
        if (Mathf.Abs(expRatio - lastExpRatio) > 0.001f) // ? ���� ���� ���� Tween ����
        {
            expFill.DOFillAmount(expRatio, 0.5f).SetEase(Ease.OutQuad);
            lastExpRatio = expRatio; // ? ������ �� ����
        }

        // ? ����ġ �ؽ�Ʈ ������Ʈ
        expText.text = $"EXP: {PlayerStats.Instance.experience} / {PlayerStats.Instance.experienceToNextLevel}";

        // ? ���� & ���� ����Ʈ ������Ʈ
        levelText.text = "����: " + PlayerStats.Instance.level;
        statPointsText.text = "���� ����Ʈ: " + PlayerStats.Instance.statPoints;

        // ? ü�� & ������ ������Ʈ
        healthText.text = $"{HurtPlayer.Instance.currentHealth} / {HurtPlayer.Instance.MaxHealth}";
        float healthRatio = (float)HurtPlayer.Instance.currentHealth / HurtPlayer.Instance.MaxHealth;
        healthBarFill.fillAmount = healthRatio;

        energyText.text = $"{(int)EnergyBarUI.Instance.GetCurrentEnergy()} / 100";
        float energyRatio = EnergyBarUI.Instance.GetCurrentEnergy() / 100f;
        energyBarFill.fillAmount = energyRatio;
    }

    private void UpgradeStat(string statType)
    {
        if (PlayerStats.Instance == null) return;

        switch (statType)
        {
            case "attack":
                PlayerStats.Instance.IncreaseAttack(); //  ���ݷ� ���� �Լ� ȣ��
                break;
            case "defense":
                PlayerStats.Instance.IncreaseDefense(); //  ���� ���� �Լ� ȣ��
                break;
            case "health":
                PlayerStats.Instance.IncreaseMaxHealth(); //  �ִ� ü�� ���� �Լ� ȣ��
                break;
        }

        UpdateStatPanel(); //  UI ������Ʈ
    }
}
