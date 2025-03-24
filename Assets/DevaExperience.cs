using UnityEngine;
using UnityEngine.UI;

public class DevaExperience : MonoBehaviour
{
    public static DevaExperience Instance;

    public int currentXP = 0;
    public int xpToNextLevel = 100;

    public Image expFillImage;
    public Text expText;

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
        if (Input.GetKeyDown(KeyCode.L))
        {
            GainXP(50);
            Debug.Log("L Ű�� ���� ���� ����ġ �߰�");
        }
    }

    public void GainXP(int amount)
    {
        currentXP += amount;
        Debug.Log($"[����] ����ġ ȹ��: {amount}, ���� ����ġ: {currentXP}/{xpToNextLevel}");

        while (currentXP >= xpToNextLevel)
        {
            currentXP -= xpToNextLevel;

            if (DevaStats.Instance != null)
                DevaStats.Instance.LevelUp(); //  �������� DevaStats���� �ñ��

            xpToNextLevel = Mathf.RoundToInt(xpToNextLevel * 1.5f);
        }

        UpdateXPUI();
        SyncWithDevaStats();
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

    private void SyncWithDevaStats()
    {
        if (DevaStats.Instance != null)
        {
            DevaStats.Instance.experience = currentXP;
            DevaStats.Instance.experienceToNextLevel = xpToNextLevel;
        }
    }
}
