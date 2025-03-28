using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using DG.Tweening;
using UnityEngine;

public class ManaBarUI : MonoBehaviour
{
    public static ManaBarUI Instance;

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

        RefreshFromPlayerStats();
    }

    public void RefreshFromPlayerStats()
    {
        UpdateManaBar(PlayerStats.Instance.currentMana, false);
        ExpandManaBar(PlayerStats.Instance.maxMana);
    }

    public void UpdateManaBar(float currentMana, bool animate = true)
    {
        float ratio = currentMana / PlayerStats.Instance.maxMana;

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
        PlayerStats.Instance.maxMana = Mathf.RoundToInt(newMaxMana);
        PlayerStats.Instance.currentMana = PlayerStats.Instance.maxMana;

        ExpandManaBar(newMaxMana);
        UpdateManaBar(PlayerStats.Instance.currentMana, false);
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
        return PlayerStats.Instance.currentMana;
    }

    public bool HasEnoughMana(float amount)
    {
        return PlayerStats.Instance.currentMana >= amount;
    }
}
