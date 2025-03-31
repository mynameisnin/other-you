using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class GraphicOption : MonoBehaviour
{
    FullScreenMode screenMode;
    public TMP_Dropdown resolutionDropdown;
    public Toggle fullscreenBtn;
    List<Resolution> resolutions = new List<Resolution>();
    int resolutionNum;

    void Start()
    {
        InitUI();
    }

    void InitUI()
    {
        screenMode = Screen.fullScreenMode;
        resolutions.Clear();

        // ��� ������ �ػ󵵸� �������� �ߺ� ����
        Resolution[] allResolutions = Screen.resolutions
            .Select(r => new Resolution { width = r.width, height = r.height }) // �ֻ��� ����
            .Distinct()
            .OrderByDescending(r => r.width) // �ػ󵵸� �������� ����
            .ToArray();

        Debug.Log($"��� ������ �ػ� ����: {allResolutions.Length}");

        foreach (Resolution res in allResolutions)
        {
            resolutions.Add(res);
            Debug.Log($"�߰��� �ػ�: {res.width} x {res.height}");
        }

        // ���� �ػ� ����Ʈ�� ��� ������ �⺻ �ػ� �߰�
        if (resolutions.Count == 0)
        {
            Debug.LogError("�ػ󵵸� ã�� �� ����. �⺻ �ػ󵵸� �߰��մϴ�.");
            resolutions.Add(new Resolution { width = 1920, height = 1080 });
            resolutions.Add(new Resolution { width = 1280, height = 720 });
        }

        resolutionDropdown.options.Clear();
        int optionNum = 0;

        foreach (Resolution res in resolutions)
        {
            TMP_Dropdown.OptionData option = new TMP_Dropdown.OptionData();
            option.text = $"{res.width} x {res.height}";
            resolutionDropdown.options.Add(option);
            Debug.Log($"��Ӵٿ� �߰���: {option.text}");

            if (res.width == Screen.width && res.height == Screen.height)
                resolutionDropdown.value = optionNum;

            optionNum++;
        }

        resolutionDropdown.RefreshShownValue();
        fullscreenBtn.isOn = (Screen.fullScreenMode == FullScreenMode.FullScreenWindow);
    }

    public void DropdownOptionChange(int x) { resolutionNum = x; }

    public void FullScreenBtn(bool isFull) { screenMode = isFull ? FullScreenMode.FullScreenWindow : FullScreenMode.Windowed; }

    public void OkBtnClick()
    {
        Screen.SetResolution(resolutions[resolutionNum].width,
            resolutions[resolutionNum].height,
            screenMode);
        Debug.Log($"����� �ػ�: {resolutions[resolutionNum].width} x {resolutions[resolutionNum].height}, ���: {screenMode}");
    }
}
