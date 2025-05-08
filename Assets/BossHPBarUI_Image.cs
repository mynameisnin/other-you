using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class BossHPBarUI_Image : MonoBehaviour
{
    public Image hpBarImage;               // ü�¹� �̹��� (Fill Amount)
    public BossHurt bossHurtTarget;        // ����� ���� ��ũ��Ʈ
    public CanvasGroup canvasGroup;        // HP �� ������� �� �� ���

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

            // ü�� �ε巴�� ���̱�
            hpTween = hpBarImage.DOFillAmount(targetFill, 0.3f).SetEase(Ease.OutQuad);

            // ���� ������ ȿ�� (���� �� ���� ��)
            Color originalColor = hpBarImage.color;
            flashTween = hpBarImage.DOColor(Color.red, 0.1f)
                .OnComplete(() => hpBarImage.DOColor(originalColor, 0.2f));

            currentFill = targetFill;

            // ü�� 0�̸� HP �� �������
            if (Mathf.Approximately(targetFill, 0f))
            {
                HideHPBar();
            }
        }
    }

    private void HideHPBar()
    {
        isHidden = true;

        // canvasGroup ���� �ε巴�� ���� 0 ó��
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
