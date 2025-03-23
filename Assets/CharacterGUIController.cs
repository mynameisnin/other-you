using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class CharacterGUIController : MonoBehaviour
{
    [Header("GUI References")]
    public RectTransform adamGUI;
    public RectTransform debaGUI;
    public CanvasGroup adamCanvasGroup;
    public CanvasGroup debaCanvasGroup;

    [Header("Optional Background Overlay")]
    public Image adamBackgroundImage;
    public Image debaBackgroundImage;

    [Header("Switch Settings")]
    public float switchDuration = 0.5f;
    public float verticalOffset = 100f;
    public Vector3 shrinkScale = new Vector3(0.85f, 0.85f, 1f);
    public Vector3 normalScale = Vector3.one;

    [Header("Fade Colors")]
    public Color activeColor = Color.white;
    public Color inactiveColor = new Color(0.3f, 0.3f, 0.3f, 1f);

    [Header("Full UI Elements")]
    public List<Image> adamUIImages;
    public List<Image> debaUIImages;

    private Vector2 adamOriginalPos;
    private Vector2 debaOriginalPos;

    void Start()
    {
        adamOriginalPos = adamGUI.anchoredPosition;
        debaOriginalPos = debaGUI.anchoredPosition;
    }
    private void SetUIColor(List<Image> uiImages, Color targetColor)
    {
        foreach (var img in uiImages)
        {
            if (img != null)
            {
                img.DOColor(targetColor, switchDuration);
            }
        }
    }
    public void SwitchToAdam()
    {
        adamGUI.SetAsLastSibling();

        // 위치, 크기, 알파
        adamGUI.DOAnchorPos(adamOriginalPos, switchDuration);
        adamGUI.DOScale(normalScale, switchDuration);
        adamCanvasGroup.DOFade(1f, switchDuration);

        debaGUI.DOAnchorPos(debaOriginalPos - new Vector2(0, verticalOffset), switchDuration);
        debaGUI.DOScale(shrinkScale, switchDuration);
        debaCanvasGroup.DOFade(0.5f, switchDuration);

        // 배경 색상 어둡게/밝게
        if (adamBackgroundImage != null)
            adamBackgroundImage.DOColor(activeColor, switchDuration);
        if (debaBackgroundImage != null)
            debaBackgroundImage.DOColor(inactiveColor, switchDuration);
        SetUIColor(adamUIImages, activeColor);
        SetUIColor(debaUIImages, inactiveColor);
    }

    public void SwitchToDeba()
    {
        debaGUI.SetAsLastSibling();

        debaGUI.DOAnchorPos(debaOriginalPos, switchDuration);
        debaGUI.DOScale(normalScale, switchDuration);
        debaCanvasGroup.DOFade(1f, switchDuration);

        adamGUI.DOAnchorPos(adamOriginalPos - new Vector2(0, verticalOffset), switchDuration);
        adamGUI.DOScale(shrinkScale, switchDuration);
        adamCanvasGroup.DOFade(0.5f, switchDuration);

        if (debaBackgroundImage != null)
            debaBackgroundImage.DOColor(activeColor, switchDuration);
        if (adamBackgroundImage != null)
            adamBackgroundImage.DOColor(inactiveColor, switchDuration);
        SetUIColor(debaUIImages, activeColor);
        SetUIColor(adamUIImages, inactiveColor);
    }
}
