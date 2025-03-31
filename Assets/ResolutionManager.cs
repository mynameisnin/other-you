using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class ResolutionManager : MonoBehaviour
{
    public TMP_Text resolutionText;  // TMP 텍스트 사용
    public Button prevButton;
    public Button nextButton;
    public Toggle fullscreenToggle;  // 체크박스 (전체화면 토글)

    private Resolution[] resolutions;
    private int currentIndex = 0;

    void Start()
    {
        resolutions = Screen.resolutions;
        currentIndex = GetCurrentResolutionIndex();
        UpdateResolutionText();

        prevButton.onClick.AddListener(() => ChangeResolution(-1));
        nextButton.onClick.AddListener(() => ChangeResolution(1));
        fullscreenToggle.onValueChanged.AddListener(SetFullscreen);  // 체크박스 이벤트 추가

        // 현재 전체화면 상태 반영
        fullscreenToggle.isOn = Screen.fullScreen;
    }

    void ChangeResolution(int direction)
    {
        currentIndex = Mathf.Clamp(currentIndex + direction, 0, resolutions.Length - 1);
        ApplyResolution();
    }

    void ApplyResolution()
    {
        Resolution res = resolutions[currentIndex];
        Screen.SetResolution(res.width, res.height, Screen.fullScreen);
        UpdateResolutionText();
    }

    void SetFullscreen(bool isFullscreen)
    {
        Screen.fullScreen = isFullscreen;
    }

    void UpdateResolutionText()
    {
        resolutionText.text = $"{resolutions[currentIndex].width}x{resolutions[currentIndex].height}";
    }

    int GetCurrentResolutionIndex()
    {
        for (int i = 0; i < resolutions.Length; i++)
        {
            if (Screen.currentResolution.width == resolutions[i].width &&
                Screen.currentResolution.height == resolutions[i].height)
            {
                return i;
            }
        }
        return 0; // 기본값
    }
}
