using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class StatPanelUI : MonoBehaviour
{
    [Header("UI Elements")]
    public TextMeshProUGUI levelText;  // ���� ǥ��
    public Image healthBarFill;        // ü�� ������
    public TextMeshProUGUI healthText; // ü�� ��ġ �ؽ�Ʈ �߰�
    public Image energyBarFill;        // ������ ������
    public TextMeshProUGUI energyText; //  ������ ��ġ �ؽ�Ʈ �߰�

    private void Update()
    {
        UpdateStatPanel();
    }

    private void UpdateStatPanel()
    {
        //  [ü�� UI ������Ʈ]
        if (HurtPlayer.Instance != null)
        {
            float healthRatio = (float)HurtPlayer.Instance.currentHealth / HurtPlayer.Instance.MaxHealth;
            healthBarFill.fillAmount = healthRatio; // ü�� ������ ������Ʈ
            healthText.text = $"{HurtPlayer.Instance.currentHealth} / {HurtPlayer.Instance.MaxHealth}"; //  ü�� �ؽ�Ʈ �߰�
        }

        //  [���� & ������ UI ������Ʈ]
        if (PlayerStats.Instance != null && EnergyBarUI.Instance != null)
        {
            levelText.text = "����: " + PlayerStats.Instance.level;

            float energyRatio = EnergyBarUI.Instance.GetCurrentEnergy() / 100f; // maxEnergy = 100 ����
            energyBarFill.fillAmount = energyRatio; // ������ ������ ������Ʈ
            energyText.text = $"{(int)EnergyBarUI.Instance.GetCurrentEnergy()} / 100"; //  ������ �ؽ�Ʈ �߰�
        }
    }
}
