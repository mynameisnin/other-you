using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class DevaExperience : MonoBehaviour
{
    public static DevaExperience Instance;

    public int currentXP = 0;
    public int xpToNextLevel = 100;

    public Image expFillImage; // 데바 전용 EXP 바 이미지
    public Text expText;       // 데바 전용 텍스트

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
        if (Input.GetKeyDown(KeyCode.L)) // 테스트용 키: 데바에게 경험치 추가
        {
            GainXP(50);
            Debug.Log("L 키를 눌러 데바 경험치 추가");
        }
    }

    public void GainXP(int amount)
    {
        currentXP += amount;
        Debug.Log($"[데바] 경험치 획득: {amount}, 현재 경험치: {currentXP}/{xpToNextLevel}");

        while (currentXP >= xpToNextLevel)
        {
            currentXP -= xpToNextLevel;
            LevelUp();
        }

        UpdateXPUI();
    }

    private void LevelUp()
    {
        xpToNextLevel = Mathf.RoundToInt(xpToNextLevel * 1.5f);
        Debug.Log("[데바] 레벨 업! 다음 레벨 필요 경험치: " + xpToNextLevel);
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
}
