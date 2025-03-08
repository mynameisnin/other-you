using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening; //  DOTween으로 EXP 바를 부드럽게 업데이트

public class StatPanelUI : MonoBehaviour
{
    [Header("UI Elements")]
    public TextMeshProUGUI levelText;  // 레벨 표시
    public TextMeshProUGUI statPointsText; //  사용 가능한 스탯 포인트 표시
    public Image healthBarFill;        // 체력 게이지
    public TextMeshProUGUI healthText; // 체력 수치 텍스트
    public Image energyBarFill;        // 에너지 게이지
    public TextMeshProUGUI energyText; // 에너지 수치 텍스트

    [Header("EXP UI")]
    public TextMeshProUGUI expText; //  경험치 수치 표시 (예: "EXP: 50 / 100")
    public Image expFill; //  경험치 바 UI 추가

    [Header("Stat Upgrade Buttons")]
    public Button attackButton;
    public Button defenseButton;
    public Button healthButton;

    private void Start()
    {
        //  버튼 클릭 이벤트 연결
        attackButton.onClick.AddListener(() => UpgradeStat("attack"));
        defenseButton.onClick.AddListener(() => UpgradeStat("defense"));
        healthButton.onClick.AddListener(() => UpgradeStat("health"));
    }

    private void Update()
    {
        UpdateStatPanel();
    }
    private float lastExpRatio = -1f; // ? 이전 경험치 비율 저장 (불필요한 Tween 호출 방지)

    private void UpdateStatPanel()
    {
        if (PlayerStats.Instance == null) return;

        // ? 경험치 바 업데이트 (이전 값과 다를 때만 Tween 실행)
        float expRatio = (float)PlayerStats.Instance.experience / PlayerStats.Instance.experienceToNextLevel;
        if (Mathf.Abs(expRatio - lastExpRatio) > 0.001f) // ? 값이 변할 때만 Tween 실행
        {
            expFill.DOFillAmount(expRatio, 0.5f).SetEase(Ease.OutQuad);
            lastExpRatio = expRatio; // ? 마지막 값 저장
        }

        // ? 경험치 텍스트 업데이트
        expText.text = $"EXP: {PlayerStats.Instance.experience} / {PlayerStats.Instance.experienceToNextLevel}";

        // ? 레벨 & 스탯 포인트 업데이트
        levelText.text = "레벨: " + PlayerStats.Instance.level;
        statPointsText.text = "스탯 포인트: " + PlayerStats.Instance.statPoints;

        // ? 체력 & 에너지 업데이트
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
                PlayerStats.Instance.IncreaseAttack(); //  공격력 증가 함수 호출
                break;
            case "defense":
                PlayerStats.Instance.IncreaseDefense(); //  방어력 증가 함수 호출
                break;
            case "health":
                PlayerStats.Instance.IncreaseMaxHealth(); //  최대 체력 증가 함수 호출
                break;
        }

        UpdateStatPanel(); //  UI 업데이트
    }
}
