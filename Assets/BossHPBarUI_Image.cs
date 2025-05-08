using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class BossHPBarUI_Image : MonoBehaviour
{
    public Image hpBarImage;               // 체력바 이미지 (Fill Amount)
    public BossHurt bossHurtTarget;        // 연결된 보스 스크립트
    public CanvasGroup canvasGroup;        // HP 바 사라지게 할 때 사용

    private float currentFill = 1f;
    private Tweener hpTween;
    private Tweener flashTween;
    private bool isHidden = false;

    private void Start()
    {
        if (bossHurtTarget != null)
        {
            currentFill = (float)bossHurtTarget.currentHealth / bossHurtTarget.MaxHealth;
            hpBarImage.fillAmount = currentFill;
        }
    }

    private void Update()
    {
        if (bossHurtTarget == null || hpBarImage == null || isHidden) return;

        float targetFill = (float)bossHurtTarget.currentHealth / bossHurtTarget.MaxHealth;

        if (!Mathf.Approximately(currentFill, targetFill))
        {
            hpTween?.Kill();
            flashTween?.Kill();

            // 체력 부드럽게 줄이기
            hpTween = hpBarImage.DOFillAmount(targetFill, 0.3f).SetEase(Ease.OutQuad);

            // 색상 깜빡임 효과 (빨강 → 원래 색)
            Color originalColor = hpBarImage.color;
            flashTween = hpBarImage.DOColor(Color.red, 0.1f)
                .OnComplete(() => hpBarImage.DOColor(originalColor, 0.2f));

            currentFill = targetFill;

            // 체력 0이면 HP 바 사라지기
            if (Mathf.Approximately(targetFill, 0f))
            {
                HideHPBar();
            }
        }
    }

    private void HideHPBar()
    {
        isHidden = true;

        // canvasGroup 통해 부드럽게 투명도 0 처리
        if (canvasGroup != null)
        {
            canvasGroup.DOFade(0f, 0.5f).OnComplete(() =>
            {
                gameObject.SetActive(false);
            });
        }
        else
        {
            gameObject.SetActive(false);
        }
    }
}
