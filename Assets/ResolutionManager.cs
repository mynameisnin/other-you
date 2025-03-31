using UnityEngine;
using TMPro; // TextMeshPro 네임스페이스 추가
using UnityEngine.UI;

public class ResolutionManager : MonoBehaviour
{
    public TMP_Text resolutionText; // TMP_Text로 변경
    public Button prevButton;
    public Button nextButton;

    private Resolution[] resolutions;
    private int currentIndex = 0;

    void Start()
    {
        resolutions = Screen.resolutions;
        currentIndex = GetCurrentResolutionIndex();
        UpdateResolutionText();

        prevButton.onClick.AddListener(() => ChangeResolution(-1));
        nextButton.onClick.AddListener(() => ChangeResolution(1));
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
