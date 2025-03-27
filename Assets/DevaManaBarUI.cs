using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class DevaManaBarUI : MonoBehaviour
{
    public static DevaManaBarUI Instance;

    public Image manaBarFill;
    public Image manaBarBack;
    public Image manaBarBorder;

    [SerializeField] private float baseWidth = 180f;

    private Color defaultBorderColor;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
    }

    private void Start()
    {
        DOTween.SetTweensCapacity(500, 50);

        if (manaBarBorder != null)
            defaultBorderColor = manaBarBorder.color;

        RefreshFromDevaStats();
    }

    public void RefreshFromDevaStats()
    {
        UpdateManaBar(DevaStats.Instance.currentMana, false);         // ? currentMana 사용
        ExpandManaBar(DevaStats.Instance.maxMana);                    // ? maxMana 사용
    }

    public void UpdateManaBar(float currentMana, bool animate = true)
    {
        float ratio = currentMana / DevaStats.Instance.maxMana;       // ? maxMana 기준

        manaBarFill.DOKill();
        manaBarBack.DOKill();

        if (animate)
        {
            manaBarFill.DOFillAmount(ratio, 0.4f).SetEase(Ease.OutCubic);
            manaBarBack.DOFillAmount(ratio, 0.8f).SetEase(Ease.OutCubic);
        }
        else
        {
            manaBarFill.fillAmount = ratio;
            manaBarBack.fillAmount = ratio;
        }
    }

    public void UpdateManaBar(float currentMana)
    {
        UpdateManaBar(currentMana, true);
    }

    public void FlashBorder()
    {
        if (manaBarBorder == null) return;

        manaBarBorder.DOKill();
        manaBarBorder.DOColor(Color.cyan, 0.2f)
            .SetLoops(2, LoopType.Yoyo)
            .OnComplete(() => manaBarBorder.color = defaultBorderColor);
    }

    public void UpdateMaxMana(float newMaxMana)
    {
        DevaStats.Instance.maxMana = Mathf.RoundToInt(newMaxMana);
        DevaStats.Instance.currentMana = DevaStats.Instance.maxMana;

        ExpandManaBar(newMaxMana);
        UpdateManaBar(DevaStats.Instance.currentMana, false);
    }

    private void ExpandManaBar(float maxMana)
    {
        float ratio = maxMana / 100f;
        float targetWidth = baseWidth * ratio;

        manaBarFill.rectTransform.DOSizeDelta(
            new Vector2(targetWidth, manaBarFill.rectTransform.sizeDelta.y),
            0.4f
        ).SetEase(Ease.OutCubic);

        manaBarBack.rectTransform.DOSizeDelta(
            new Vector2(targetWidth, manaBarBack.rectTransform.sizeDelta.y),
            0.4f
        ).SetEase(Ease.OutCubic);

        if (manaBarBorder != null)
        {
            manaBarBorder.rectTransform.DOSizeDelta(
                new Vector2(targetWidth, manaBarBorder.rectTransform.sizeDelta.y),
                0.4f
            ).SetEase(Ease.OutCubic);
        }
    }

    public float GetCurrentMana()
    {
        return DevaStats.Instance.currentMana;
    }

    public bool HasEnoughMana(float amount)
    {
        return DevaStats.Instance.currentMana >= amount;
    }
}

