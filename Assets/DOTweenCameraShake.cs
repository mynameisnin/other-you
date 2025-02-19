using DG.Tweening;
using UnityEngine;

public class DOTweenCameraShake : MonoBehaviour
{
    public float defaultDuration = 0.5f;  // 흔들림 지속 시간 (전체 지속 시간)
    public float defaultStrength = 0.5f; // 흔들림 강도 (최대 흔들림 거리)
    public float shakeInterval = 0.05f; // 흔들리는 주기 (짧을수록 더 빠르게 흔들림)

    private Vector3 originalPosition;
    private bool isShaking = false;
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
        if (isShaking) return; // 이미 흔들리고 있다면 중복 실행 방지

        isShaking = true;

        shakeTween = DOTween.Sequence()
            .AppendCallback(() => ApplyShake(strength))
            .AppendInterval(shakeInterval) // 지정된 주기마다 흔들림 적용
            .SetLoops(Mathf.CeilToInt(duration / shakeInterval), LoopType.Restart)
            .OnKill(() =>
            {
                isShaking = false;
                transform.localPosition = originalPosition; // 원래 위치로 복귀
            });
    }

    private void ApplyShake(float strength)
    {
        // 랜덤한 방향으로 이동 (X, Y 방향)
        Vector3 randomOffset = new Vector3(
            Random.Range(-strength, strength),
            Random.Range(-strength, strength),
            0f // Z축 흔들림 제거
        );
        transform.localPosition = originalPosition + randomOffset;
    }

    public void StopShaking()
    {
        if (shakeTween != null && shakeTween.IsActive())
        {
            shakeTween.Kill();
        }

        isShaking = false;
        transform.localPosition = originalPosition; // 원래 위치로 복귀
    }
}
