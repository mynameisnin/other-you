using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlashFlip : MonoBehaviour
{
    public GameObject slashEffect; // ������ ����Ʈ ������
    private bool isFacingRight = true; // ĳ������ ������ ��Ÿ��

    public void ShowSlashEffect()
    {
        // ���⿡ ���� ��������Ʈ�� ������
        Vector3 slashScale = slashEffect.transform.localScale;
        if (isFacingRight)
        {
            slashScale.x = Mathf.Abs(slashScale.x); // ������ ����
        }
        else
        {
            slashScale.x = -Mathf.Abs(slashScale.x); // ���� ����
        }
        slashEffect.transform.localScale = slashScale;

        // ����Ʈ ��ġ ����
        Vector3 position = transform.position;
        position.x += isFacingRight ? 1.0f : -1.0f; // ���⿡ ���� ��ġ �̵�
        slashEffect.transform.position = position;

        slashEffect.SetActive(true); // Ȱ��ȭ
    }
}