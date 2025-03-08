using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System.Collections;

public class EnergyBarUI : MonoBehaviour
{
    public static EnergyBarUI Instance;
    public Image energyBarFill;    // ENERGY �� (�ʷϻ�)
    public Image energyBarBack;    // ������ ������ �� (õõ�� ����, �����)
    public Image energyBarBorder;  //  �׵θ� �̹��� �߰�

    private Color defaultBorderColor; // �⺻ �׵θ� ���� ����

    private float maxEnergy = 100f; // �ִ� ENERGY ��
    private float currentEnergy;
    public float energyRegenRate = 15f; // �ʴ� ȸ���� ����
    public float regenDelay = 2f; // ���� �� ȸ�� ���۱����� ���� �ð�

    private Coroutine regenCoroutine; // ������ ȸ�� �ڷ�ƾ

    void Start()
    {
        DOTween.SetTweensCapacity(500, 50);
        currentEnergy = maxEnergy;
        UpdateEnergyBar(currentEnergy, false);

        if (energyBarBorder != null)
            defaultBorderColor = energyBarBorder.color; // �⺻ �׵θ� ���� ����
    }

    public void Initialize(float maxEN)
    {
        maxEnergy = maxEN;
        currentEnergy = maxEN;
        UpdateEnergyBar(currentEnergy, false);
    }

    public void UpdateEnergyBar(float newEnergy, bool animate = true)
    {
        float energyRatio = newEnergy / maxEnergy;

        if (energyBarFill == null || energyBarBack == null || energyBarBorder == null)
        {
            Debug.LogError("[EnergyBarUI] UI ��Ұ� �Ҵ���� ����!");
            return;
        }

        energyBarFill.DOKill();
        energyBarBack.DOKill();

        if (newEnergy > currentEnergy) // �������� �����ϴ� ��� (ȸ��)
        {
            energyBarFill.DOFillAmount(energyRatio, 0.6f).SetEase(Ease.OutCubic);
        }
        else // �������� �����ϴ� ��� (��� �ݿ�)
        {
            energyBarFill.fillAmount = energyRatio;
        }

        if (animate)
        {
            energyBarBack.DOFillAmount(energyRatio, 1.0f).SetEase(Ease.OutCubic);
        }
        else
        {
            energyBarBack.fillAmount = energyRatio;
        }
    }

    public void ReduceEnergy(float amount)
    {
        currentEnergy -= amount;
        currentEnergy = Mathf.Clamp(currentEnergy, 0, maxEnergy);
        UpdateEnergyBar(currentEnergy);

        if (regenCoroutine != null)
        {
            StopCoroutine(regenCoroutine);
        }
        regenCoroutine = StartCoroutine(StartEnergyRegenAfterDelay());
    }

    public void RecoverEnergy(float amount)
    {
        currentEnergy += amount;
        currentEnergy = Mathf.Clamp(currentEnergy, 0, maxEnergy);
        SmoothEnergyRegen(currentEnergy);
    }

    public bool HasEnoughEnergy(float amount)
    {
        return currentEnergy >= amount;
    }

    private IEnumerator StartEnergyRegenAfterDelay()
    {
        yield return new WaitForSeconds(regenDelay);

        while (currentEnergy < maxEnergy)
        {
            currentEnergy += energyRegenRate * Time.deltaTime;
            currentEnergy = Mathf.Clamp(currentEnergy, 0, maxEnergy);
            SmoothEnergyRegen(currentEnergy);

            yield return new WaitForSeconds(0.05f);
        }

        regenCoroutine = null;
    }

    private void SmoothEnergyRegen(float targetEnergy)
    {
        float energyRatio = targetEnergy / maxEnergy;

        if (energyBarFill == null || energyBarBack == null) return;

        energyBarFill.DOKill();
        energyBarBack.DOKill();

        energyBarFill.DOFillAmount(energyRatio, 1.0f).SetEase(Ease.OutCubic);
        energyBarBack.DOFillAmount(energyRatio, 1.5f).SetEase(Ease.OutCubic);
    }

    public bool IsEnergyEmpty()
    {
        return currentEnergy <= 0;
    }
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }
    public float GetCurrentEnergy()
    {
        return currentEnergy;
    }
    //  �������� �����ϸ� �׵θ� ������ ���������� �����̰� ��
    public void FlashBorder()
    {
        if (energyBarBorder == null) return;

        energyBarBorder.DOKill(); // ���� Ʈ�� ����
        energyBarBorder.DOColor(Color.red, 0.2f)
            .SetLoops(2, LoopType.Yoyo) // 2�� ���� (���� -> ������)
            .OnComplete(() => energyBarBorder.color = defaultBorderColor);
    }



}
