using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System.Collections;

public class EnergyBarUI : MonoBehaviour
{
    public Image energyBarFill;   // ENERGY �� (�ʷϻ�)

    private float maxEnergy = 100f; // �ִ� ENERGY ��
    private float currentEnergy;
    public float energyRegenRate = 15f; //  �ʴ� ȸ���� ���� (���� 5 �� 15)
    public float regenDelay = 2f; // ���� �� ȸ���� ���۵Ǳ������ ���� �ð�

    private Coroutine regenCoroutine; // ������ ȸ�� �ڷ�ƾ

    void Start()
    {
        currentEnergy = maxEnergy;
        UpdateEnergyBar(currentEnergy, false);
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

        if (energyBarFill == null)
        {
            Debug.LogError("[EnergyBarUI] energyBarFill�� �Ҵ���� ����!");
            return;
        }

        if (animate)
        {
            energyBarFill.DOFillAmount(energyRatio, 0.4f).SetEase(Ease.OutQuad);
        }
        else
        {
            energyBarFill.fillAmount = energyRatio;
        }
    }

    public void ReduceEnergy(float amount)
    {
        currentEnergy -= amount;
        currentEnergy = Mathf.Clamp(currentEnergy, 0, maxEnergy);
        UpdateEnergyBar(currentEnergy);

        // ���� �� ȸ�� ���� �� ���� �ð� �� �ٽ� ȸ�� ����
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
        UpdateEnergyBar(currentEnergy);
    }

    public bool HasEnoughEnergy(float amount)
    {
        return currentEnergy >= amount;
    }

    private IEnumerator StartEnergyRegenAfterDelay()
    {
        yield return new WaitForSeconds(regenDelay); // ���� �� ���� �ð� ���

        while (currentEnergy < maxEnergy)
        {
            currentEnergy += energyRegenRate; //  �ʴ� ȸ������ ���� �߰�
            currentEnergy = Mathf.Clamp(currentEnergy, 0, maxEnergy);
            UpdateEnergyBar(currentEnergy);

            yield return new WaitForSeconds(0.1f); //  0.1�ʸ��� ȸ�� (����: �� ������ ȸ��)
        }

        regenCoroutine = null; // ȸ�� �Ϸ� �� �ڷ�ƾ �ʱ�ȭ
    }
    public bool IsEnergyEmpty()
    {
        return currentEnergy <= 0;
    }

}
