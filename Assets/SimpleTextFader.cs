using UnityEngine;
using DG.Tweening;

public class SimpleTextFader : MonoBehaviour
{
    public CanvasGroup canvasGroup;
    public float duration = 0.5f;

    public void FadeIn()
    {
        canvasGroup.DOFade(1f, duration).SetEase(Ease.OutQuad);
    }

    public void FadeOut()
    {
        canvasGroup.DOFade(0f, duration).SetEase(Ease.InQuad);
    }

    private void Reset()
    {
        canvasGroup = GetComponent<CanvasGroup>();
    }
}
