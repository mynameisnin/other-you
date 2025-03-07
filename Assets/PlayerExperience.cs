using UnityEngine;
using UnityEngine.UI;

public class PlayerExperience : MonoBehaviour
{
    public int currentXP = 0;   // ���� ����ġ
    public int level = 1;       // ���� ����
    public int xpToNextLevel = 100; // ���� �������� �ʿ��� ����ġ

    public Image expFillImage; // EXP �� �̹��� (Inspector���� ����)

    private void Start()
    {
        UpdateXPUI();
    }
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.P)) // PŰ�� ������ ���� ����ġ ����
        {
            GainXP(50);
            Debug.LogError("P Ű�� ������ ������ ����ġ �߰�");
        }
    }

    public void GainXP(int amount)
    {
        Debug.LogError("GainXP �Լ��� ȣ���!"); // ?? ���� ���

        currentXP += amount;
        Debug.Log($"����ġ ȹ��: {amount}, ���� ����ġ: {currentXP}/{xpToNextLevel}");

        if (currentXP >= xpToNextLevel)
        {
            LevelUp();
        }

        UpdateXPUI();
    }


    private void LevelUp()
    {
        level++;
        currentXP -= xpToNextLevel; // �ʰ��� ����ġ ����
        xpToNextLevel += Mathf.RoundToInt(xpToNextLevel * 0.2f); // �ʿ� ����ġ 20% ����

        Debug.Log($"���� ��! ���� ����: {level}, ���� ���� �ʿ� ����ġ: {xpToNextLevel}");

        UpdateXPUI();
    }

    private void UpdateXPUI()
    {
        if (expFillImage != null)
        {
            expFillImage.fillAmount = (float)currentXP / xpToNextLevel; //  Fill Amount �� ������Ʈ
        }
    }
}
