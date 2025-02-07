using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DOTweenCameraShake : MonoBehaviour
{
    public float defaultDuration = 0.5f;
    public float defaultStrength = 0.5f;
    public int vibrato = 10;  // ���� �� (Ŭ���� ���� ��鸲)
    public float randomness = 90f; // ������ ������

    private Vector3 originalPosition;

    private void Start()
    {
        originalPosition = transform.localPosition;
    }

    public void ShakeCamera()
    {
        ShakeCamera(defaultDuration, defaultStrength);
    }

    public void ShakeCamera(float duration, float strength)
    {
        transform.DOShakePosition(duration, strength, vibrato, randomness)
                 .OnComplete(() => transform.localPosition = originalPosition); // ��鸲 ���� �� ���� ��ġ�� ����
    }
}
