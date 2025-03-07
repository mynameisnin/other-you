using UnityEngine;
using UnityEngine.UI;

public class PlayerExperience : MonoBehaviour
{
    public int currentXP = 0;   // 현재 경험치
    public int level = 1;       // 현재 레벨
    public int xpToNextLevel = 100; // 다음 레벨까지 필요한 경험치

    public Image expFillImage; // EXP 바 이미지 (Inspector에서 연결)

    private void Start()
    {
        UpdateXPUI();
    }
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.P)) // P키를 누르면 강제 경험치 증가
        {
            GainXP(50);
            Debug.LogError("P 키를 눌러서 강제로 경험치 추가");
        }
    }

    public void GainXP(int amount)
    {
        Debug.LogError("GainXP 함수가 호출됨!"); // ?? 강제 출력

        currentXP += amount;
        Debug.Log($"경험치 획득: {amount}, 현재 경험치: {currentXP}/{xpToNextLevel}");

        if (currentXP >= xpToNextLevel)
        {
            LevelUp();
        }

        UpdateXPUI();
    }


    private void LevelUp()
    {
        level++;
        currentXP -= xpToNextLevel; // 초과된 경험치 유지
        xpToNextLevel += Mathf.RoundToInt(xpToNextLevel * 0.2f); // 필요 경험치 20% 증가

        Debug.Log($"레벨 업! 현재 레벨: {level}, 다음 레벨 필요 경험치: {xpToNextLevel}");

        UpdateXPUI();
    }

    private void UpdateXPUI()
    {
        if (expFillImage != null)
        {
            expFillImage.fillAmount = (float)currentXP / xpToNextLevel; //  Fill Amount 값 업데이트
        }
    }
}
