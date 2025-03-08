using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class StatPanelUI : MonoBehaviour
{
    [Header("UI Elements")]
    public TextMeshProUGUI levelText;  // 레벨 표시
    public Image healthBarFill;        // 체력 게이지
    public TextMeshProUGUI healthText; // 체력 수치 텍스트 추가
    public Image energyBarFill;        // 에너지 게이지
    public TextMeshProUGUI energyText; //  에너지 수치 텍스트 추가

    private void Update()
    {
        UpdateStatPanel();
    }

    private void UpdateStatPanel()
    {
        //  [체력 UI 업데이트]
        if (HurtPlayer.Instance != null)
        {
            float healthRatio = (float)HurtPlayer.Instance.currentHealth / HurtPlayer.Instance.MaxHealth;
            healthBarFill.fillAmount = healthRatio; // 체력 게이지 업데이트
            healthText.text = $"{HurtPlayer.Instance.currentHealth} / {HurtPlayer.Instance.MaxHealth}"; //  체력 텍스트 추가
        }

        //  [레벨 & 에너지 UI 업데이트]
        if (PlayerStats.Instance != null && EnergyBarUI.Instance != null)
        {
            levelText.text = "레벨: " + PlayerStats.Instance.level;

            float energyRatio = EnergyBarUI.Instance.GetCurrentEnergy() / 100f; // maxEnergy = 100 기준
            energyBarFill.fillAmount = energyRatio; // 에너지 게이지 업데이트
            energyText.text = $"{(int)EnergyBarUI.Instance.GetCurrentEnergy()} / 100"; //  에너지 텍스트 추가
        }
    }
}
