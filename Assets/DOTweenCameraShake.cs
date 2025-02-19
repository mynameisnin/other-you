using DG.Tweening;
using UnityEngine;

public class DOTweenCameraShake : MonoBehaviour
{
    public float defaultDuration = 0.5f;  // ��鸲 ���� �ð� (��ü ���� �ð�)
    public float defaultStrength = 0.5f; // ��鸲 ���� (�ִ� ��鸲 �Ÿ�)
    public float shakeInterval = 0.05f; // ��鸮�� �ֱ� (ª������ �� ������ ��鸲)

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
        if (isShaking) return; // �̹� ��鸮�� �ִٸ� �ߺ� ���� ����

        isShaking = true;

        shakeTween = DOTween.Sequence()
            .AppendCallback(() => ApplyShake(strength))
            .AppendInterval(shakeInterval) // ������ �ֱ⸶�� ��鸲 ����
            .SetLoops(Mathf.CeilToInt(duration / shakeInterval), LoopType.Restart)
            .OnKill(() =>
            {
                isShaking = false;
                transform.localPosition = originalPosition; // ���� ��ġ�� ����
            });
    }

    private void ApplyShake(float strength)
    {
        // ������ �������� �̵� (X, Y ����)
        Vector3 randomOffset = new Vector3(
            Random.Range(-strength, strength),
            Random.Range(-strength, strength),
            0f // Z�� ��鸲 ����
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
        transform.localPosition = originalPosition; // ���� ��ġ�� ����
    }
}
