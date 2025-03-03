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

    void Start()
    {
        panelTransform = GetComponent<RectTransform>();
        panelImage = GetComponent<Image>();

        if (panelImage != null)
        {
            defaultColor = panelImage.color;
        }
    }

    //  �ǰ� �� GUI ��鸲 ȿ��
    public void ShakePanel()
    {
        if (panelTransform != null)
        {
            panelTransform.DOShakePosition(0.3f, new Vector3(10f, 10f, 0)); // 0.3�� ���� ��鸲
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