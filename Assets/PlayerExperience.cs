using UnityEngine;
using UnityEngine.UI;

public class PlayerExperience : MonoBehaviour
{
    public static PlayerExperience Instance; // ? 싱글톤 적용 (다른 스크립트에서 쉽게 접근)

    public int currentXP = 0;   // 현재 경험치
    public int xpToNextLevel = 100; // 다음 레벨까지 필요한 경험치

    public Image expFillImage; // EXP 바 이미지 (Inspector에서 연결)
    public Text expText; // ? 경험치 텍스트 추가

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
        if (Input.GetKeyDown(KeyCode.P)) // ? P키를 누르면 강제 경험치 증가
        {
            GainXP(50);
            Debug.Log("P 키를 눌러서 경험치 추가");
        }
    }

    public void GainXP(int amount)
    {
        currentXP += amount;
        Debug.Log($"경험치 획득: {amount}, 현재 경험치: {currentXP}/{xpToNextLevel}");

        while (currentXP >= xpToNextLevel)
        {
            currentXP -= xpToNextLevel;
            LevelUp();
        }

        UpdateXPUI();
        SyncWithPlayerStats(); // ? PlayerStats에 경험치 동기화
    }

    private void LevelUp()
    {
        if (PlayerStats.Instance != null)
        {
            PlayerStats.Instance.LevelUp(); // ? PlayerStats의 레벨업 실행 (에너지 증가 포함)
        }

        xpToNextLevel = Mathf.RoundToInt(xpToNextLevel * 1.5f); // 다음 레벨 필요 경험치 증가
        Debug.Log($"레벨 업! 다음 레벨 필요 경험치: {xpToNextLevel}");

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

    // ? 경험치를 PlayerStats와 동기화
    private void SyncWithPlayerStats()
    {
        if (PlayerStats.Instance != null)
        {
            PlayerStats.Instance.experience = currentXP;
            PlayerStats.Instance.experienceToNextLevel = xpToNextLevel;
        }
    }
}
