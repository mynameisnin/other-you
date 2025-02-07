using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DOTweenCameraShake : MonoBehaviour
{
    public float defaultDuration = 0.5f;
    public float defaultStrength = 0.5f;
    public int vibrato = 10;  // 진동 수 (클수록 자주 흔들림)
    public float randomness = 90f; // 방향의 랜덤성

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
                 .OnComplete(() => transform.localPosition = originalPosition); // 흔들림 종료 후 원래 위치로 복귀
    }
}
