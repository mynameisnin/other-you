using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class DevaExperience : MonoBehaviour
{
    public static DevaExperience Instance;

    public int currentXP = 0;
    public int xpToNextLevel = 100;

    public Image expFillImage; // ���� ���� EXP �� �̹���
    public Text expText;       // ���� ���� �ؽ�Ʈ

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
        if (Input.GetKeyDown(KeyCode.L)) // �׽�Ʈ�� Ű: ���ٿ��� ����ġ �߰�
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
            LevelUp();
        }

        UpdateXPUI();
    }

    private void LevelUp()
    {
        xpToNextLevel = Mathf.RoundToInt(xpToNextLevel * 1.5f);
        Debug.Log("[����] ���� ��! ���� ���� �ʿ� ����ġ: " + xpToNextLevel);
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
