using DG.Tweening;
using UnityEngine;

public class DOTweenCameraShake : MonoBehaviour
{
    public float defaultDuration = 0.5f;  // 흔들림 지속 시간
    public float defaultStrength = 0.5f; // 흔들림 강도
    public int vibrato = 10;  // 진동 횟수 (클수록 자주 흔들림)
    public float randomness = 90f; // 방향의 랜덤성
    public bool fadeOut = false; //  fadeOut을 false로 설정하여 힘이 줄어들지 않도록 함

    private Vector3 originalPosition;
    private Tween shakeTween;

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
        // 기존 흔들림이 남아 있다면 제거 후 실행
        if (shakeTween != null && shakeTween.IsActive())
        {
            shakeTween.Kill();
        }

        // fadeOut을 false로 설정하여 흔들림의 강도가 줄어들지 않게 함
        shakeTween = transform.DOShakePosition(duration, strength, vibrato, randomness, fadeOut)
                              .OnComplete(() => transform.localPosition = originalPosition); // 흔들림 종료 후 원래 위치로 복귀
    }
}
