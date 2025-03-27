using UnityEngine;
using System.Collections;

public class PlayerAutoHealManager : MonoBehaviour
{
    public GameObject adam; // �ƴ� ������Ʈ
    public GameObject deva; // ���� ������Ʈ

    [Header("Healing Settings")]
    public float healthHealRate = 3f; // ü�� �ʴ� ȸ����
    public float manaHealRate = 5f;   // ���� �ʴ� ȸ����
    public float healInterval = 1f;   // ȸ�� ����
    public float healDelay = 2f;      // ��Ȱ��ȭ �� ȸ�� ���� ��� �ð�

    private Coroutine adamHealCoroutine;
    private Coroutine devaHealCoroutine;

    void Start()
    {
        StartCoroutine(CheckHealing());
        Debug.Log($"[AutoHeal] Deva active? {deva.activeInHierarchy}, currentMana: {DevaStats.Instance.currentMana}/{DevaStats.Instance.maxMana}");
    }

    private IEnumerator CheckHealing()
    {
        while (true)
        {
            // �ƴ� ��Ȱ��ȭ �� ȸ�� ����
            if (!adam.activeInHierarchy && adamHealCoroutine == null && PlayerStats.Instance != null)
            {
                adamHealCoroutine = StartCoroutine(HealRoutine(PlayerStats.Instance, () => adamHealCoroutine = null));
            }
            // �ƴ� Ȱ��ȭ �� ȸ�� �ߴ�
            else if (adam.activeInHierarchy && adamHealCoroutine != null)
            {
                StopCoroutine(adamHealCoroutine);
                adamHealCoroutine = null;
            }

            // ���� ��Ȱ��ȭ �� ȸ�� ����
            if (!deva.activeInHierarchy && devaHealCoroutine == null && DevaStats.Instance != null)
            {
                devaHealCoroutine = StartCoroutine(HealRoutine(DevaStats.Instance, () => devaHealCoroutine = null));
            }
            // ���� Ȱ��ȭ �� ȸ�� �ߴ�
            else if (deva.activeInHierarchy && devaHealCoroutine != null)
            {
                StopCoroutine(devaHealCoroutine);
                devaHealCoroutine = null;
            }

            yield return new WaitForSeconds(1f);
        }
    }


    private IEnumerator HealRoutine(object statsObj, System.Action onFinish)
    {
        yield return new WaitForSeconds(healDelay);

        while (true)
        {
            bool isFullyHealed = true;

            if (statsObj is PlayerStats player)
            {
                if (player.currentHealth < player.maxHealth)
                {
                    player.currentHealth += Mathf.RoundToInt(healthHealRate);
                    player.currentHealth = Mathf.Clamp(player.currentHealth, 0, player.maxHealth);
                    HurtPlayer.Instance?.UpdateHealthUI();
                    isFullyHealed = false;
                }
            }
            else if (statsObj is DevaStats deva)
            {
                // ü�� ȸ��
                if (deva.currentHealth < deva.maxHealth)
                {
                    deva.currentHealth += Mathf.RoundToInt(healthHealRate);
                    deva.currentHealth = Mathf.Clamp(deva.currentHealth, 0, deva.maxHealth);
                    DevaHealthBarUI.Instance?.UpdateHealthBar(deva.currentHealth);
                    isFullyHealed = false;
                }

                // ���� ȸ��
                if (deva.currentMana < deva.maxMana)
                {
                    deva.currentMana += Mathf.RoundToInt(manaHealRate);
                    deva.currentMana = Mathf.Clamp(deva.currentMana, 0, deva.maxMana);
                    DevaManaBarUI.Instance?.UpdateManaBar(deva.currentMana);
                    isFullyHealed = false;
                }
            }

            if (isFullyHealed)
                break;

            yield return new WaitForSeconds(healInterval);
        }

        onFinish?.Invoke();
    }
}
