using UnityEngine;
using UnityEngine.UI;

public class PlayerExperience : MonoBehaviour
{
    public static PlayerExperience Instance; // ? �̱��� ���� (�ٸ� ��ũ��Ʈ���� ���� ����)

    public int currentXP = 0;   // ���� ����ġ
    public int xpToNextLevel = 100; // ���� �������� �ʿ��� ����ġ

    public Image expFillImage; // EXP �� �̹��� (Inspector���� ����)
    public Text expText; // ? ����ġ �ؽ�Ʈ �߰�

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }

    private void Start()
    {
        UpdateXPUI();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.P)) // ? PŰ�� ������ ���� ����ġ ����
        {
            GainXP(50);
            Debug.Log("P Ű�� ������ ����ġ �߰�");
        }
    }

    public void GainXP(int amount)
    {
        currentXP += amount;
        Debug.Log($"����ġ ȹ��: {amount}, ���� ����ġ: {currentXP}/{xpToNextLevel}");

        while (currentXP >= xpToNextLevel)
        {
            currentXP -= xpToNextLevel;
            LevelUp();
        }

        UpdateXPUI();
        SyncWithPlayerStats(); // ? PlayerStats�� ����ġ ����ȭ
    }

    private void LevelUp()
    {
        if (PlayerStats.Instance != null)
        {
            PlayerStats.Instance.LevelUp(); // ? PlayerStats�� ������ ���� (������ ���� ����)
        }

        xpToNextLevel = Mathf.RoundToInt(xpToNextLevel * 1.5f); // ���� ���� �ʿ� ����ġ ����
        Debug.Log($"���� ��! ���� ���� �ʿ� ����ġ: {xpToNextLevel}");

        UpdateXPUI();
    }


    private void UpdateXPUI()
    {
        if (expFillImage != null)
        {
            float expRatio = (float)currentXP / xpToNextLevel;
            expFillImage.fillAmount = expRatio;
        }

        if (expText != null)
        {
            expText.text = $"EXP: {currentXP} / {xpToNextLevel}";
        }
    }

    // ? ����ġ�� PlayerStats�� ����ȭ
    private void SyncWithPlayerStats()
    {
        if (PlayerStats.Instance != null)
        {
            PlayerStats.Instance.experience = currentXP;
            PlayerStats.Instance.experienceToNextLevel = xpToNextLevel;
        }
    }
}
