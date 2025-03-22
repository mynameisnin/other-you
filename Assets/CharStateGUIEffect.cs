using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class CharStateGUIEffect : MonoBehaviour
{
    private RectTransform panelTransform;  // GUI �г��� RectTransform
    private Image panelImage;              // �г��� ��� ������ �����ϱ� ���� Image

    private Color defaultColor;            // ���� ���� ����
    private Vector3 originalPosition;      //  �г��� ���� ��ġ ����

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
            originalPosition = panelTransform.localPosition; //  ���� ��ġ ����
        }
    }

    //  �ǰ� �� GUI ��鸲 ȿ��
    public void ShakePanel()
    {
        if (panelTransform != null)
        {
            panelTransform.DOKill(); //  ���� Tween ���� (�ߺ� ��鸲 ����)

            panelTransform.DOShakePosition(0.3f, new Vector3(10f, 10f, 0), 10, 90, false, true)
                .SetRelative(true)
                .OnComplete(() =>
                {
                    panelTransform.localPosition = originalPosition;
                });
        }
    }


    //  �ǰ� �� GUI ������ ������ ȿ��
    public void FlashRed()
    {
        if (panelImage != null)
        {
            panelImage.DOColor(new Color(1f, 0f, 0f, 1f), 0.1f) // ���������� ����
                .OnComplete(() =>
                {
                    panelImage.DOColor(defaultColor, 0.2f); // ���� ������ ����
                });
        }
    }

    // ��鸲 + ������ ȿ�� �Բ� ����
    public void TriggerHitEffect()
    {
        ShakePanel();
        FlashRed();
    }
}
