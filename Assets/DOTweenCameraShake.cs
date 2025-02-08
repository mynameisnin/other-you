using DG.Tweening;
using UnityEngine;

public class DOTweenCameraShake : MonoBehaviour
{
    public float defaultDuration = 0.5f;  // ��鸲 ���� �ð�
    public float defaultStrength = 0.5f; // ��鸲 ����
    public int vibrato = 10;  // ���� Ƚ�� (Ŭ���� ���� ��鸲)
    public float randomness = 90f; // ������ ������
    public bool fadeOut = false; //  fadeOut�� false�� �����Ͽ� ���� �پ���� �ʵ��� ��

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
        // ���� ��鸲�� ���� �ִٸ� ���� �� ����
        if (shakeTween != null && shakeTween.IsActive())
        {
            shakeTween.Kill();
        }

        // fadeOut�� false�� �����Ͽ� ��鸲�� ������ �پ���� �ʰ� ��
        shakeTween = transform.DOShakePosition(duration, strength, vibrato, randomness, fadeOut)
                              .OnComplete(() => transform.localPosition = originalPosition); // ��鸲 ���� �� ���� ��ġ�� ����
    }
}
