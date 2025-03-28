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

        // 사용 가능한 해상도를 가져오고 중복 제거
        Resolution[] allResolutions = Screen.resolutions
            .Select(r => new Resolution { width = r.width, height = r.height }) // 주사율 제거
            .Distinct()
            .OrderByDescending(r => r.width) // 해상도를 내림차순 정렬
            .ToArray();

        Debug.Log($"사용 가능한 해상도 개수: {allResolutions.Length}");

        foreach (Resolution res in allResolutions)
        {
            resolutions.Add(res);
            Debug.Log($"추가된 해상도: {res.width} x {res.height}");
        }

        // 만약 해상도 리스트가 비어 있으면 기본 해상도 추가
        if (resolutions.Count == 0)
        {
            Debug.LogError("해상도를 찾을 수 없음. 기본 해상도를 추가합니다.");
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
            Debug.Log($"드롭다운 추가됨: {option.text}");

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
        Debug.Log($"적용된 해상도: {resolutions[resolutionNum].width} x {resolutions[resolutionNum].height}, 모드: {screenMode}");
    }
}
