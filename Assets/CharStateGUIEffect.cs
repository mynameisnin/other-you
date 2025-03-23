using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class CharStateGUIEffect : MonoBehaviour
{
    private RectTransform panelTransform;  // GUI 패널의 RectTransform
    private Image panelImage;              // 패널의 배경 색상을 변경하기 위한 Image

    private Color defaultColor;            // 원래 색상 저장
    private Vector3 originalPosition;      //  패널의 원래 위치 저장

    void Start()
    {
        panelTransform = GetComponent<RectTransform>();
        panelImage = GetComponent<Image>();

        if (panelImage != null)
        {
            defaultColor = panelImage.color;
        }

        if (panelTransform != null)
        {
            originalPosition = panelTransform.localPosition; //  원래 위치 저장
        }
    }

    //  피격 시 GUI 흔들림 효과
    public void ShakePanel()
    {
        if (panelTransform != null)
        {
            panelTransform.DOKill(); //  기존 Tween 제거 (중복 흔들림 방지)

            panelTransform.DOShakePosition(0.3f, new Vector3(10f, 10f, 0), 10, 90, false, true)
                .SetRelative(true)
                .OnComplete(() =>
                {
                    panelTransform.localPosition = originalPosition;
                });
        }
    }


    //  피격 시 GUI 빨간색 깜빡임 효과
    public void FlashRed()
    {
        if (panelImage != null)
        {
            panelImage.DOColor(new Color(1f, 0f, 0f, 1f), 0.1f) // 빨간색으로 변함
                .OnComplete(() =>
                {
                    panelImage.DOColor(defaultColor, 0.2f); // 원래 색으로 복구
                });
        }
    }

    // 흔들림 + 빨간색 효과 함께 실행
    public void TriggerHitEffect()
    {
        ShakePanel();
        FlashRed();
    }
}
