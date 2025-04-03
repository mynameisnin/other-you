using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class ResolutionManager : MonoBehaviour
{
    public TMP_Text resolutionText;  // TMP �ؽ�Ʈ ���
    public Button prevButton;
    public Button nextButton;
    public Button applyButton; // ���� ��ư �߰�
    public Toggle fullscreenToggle;  // üũ�ڽ� (��üȭ�� ���)

    private Resolution[] resolutions;
    private int currentIndex = 0;
    private bool isFullscreen;

    void Start()
    {
        resolutions = Screen.resolutions;
        currentIndex = GetCurrentResolutionIndex();
        isFullscreen = Screen.fullScreen;

        UpdateResolutionText();

        prevButton.onClick.AddListener(() => ChangeResolution(-1));
        nextButton.onClick.AddListener(() => ChangeResolution(1));
        applyButton.onClick.AddListener(ApplyResolution);
        fullscreenToggle.onValueChanged.AddListener(value => isFullscreen = value);

        fullscreenToggle.isOn = isFullscreen;
    }

    void ChangeResolution(int direction)
    {
        currentIndex = Mathf.Clamp(currentIndex + direction, 0, resolutions.Length - 1);
        UpdateResolutionText();
    }

    void ApplyResolution()
    {
        Resolution res = resolutions[currentIndex];
        Screen.SetResolution(res.width, res.height, isFullscreen);
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
        return 0; // �⺻��
    }
}
