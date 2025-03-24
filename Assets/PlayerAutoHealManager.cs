using UnityEngine;
using System.Collections;

public class PlayerAutoHealManager : MonoBehaviour
{
    public GameObject adam; // �ƴ� ������Ʈ
    public GameObject deva; // ���� ������Ʈ

    public float healRate = 3f; // �ʴ� ȸ����
    public float healInterval = 1f; // ȸ�� ����
    public float healDelay = 2f; // ��Ȱ��ȭ �� ȸ�� ���� ��� �ð�

    private Coroutine adamHealCoroutine;
    private Coroutine devaHealCoroutine;

    void Start()
    {
        StartCoroutine(CheckHealing());
    }

    private IEnumerator CheckHealing()
    {
        while (true)
        {
            // �ƴ� ��Ȱ��ȭ ���¿��� ȸ�� ����
            if (!adam.activeInHierarchy && adamHealCoroutine == null && PlayerStats.Instance != null)
            {
                adamHealCoroutine = StartCoroutine(HealRoutine(PlayerStats.Instance, () => adamHealCoroutine = null));
            }

            // ���� ��Ȱ��ȭ ���¿��� ȸ�� ����
            if (!deva.activeInHierarchy && devaHealCoroutine == null && DevaStats.Instance != null)
            {
                devaHealCoroutine = StartCoroutine(HealRoutine(DevaStats.Instance, () => devaHealCoroutine = null));
            }

            yield return new WaitForSeconds(1f);
        }
    }

    private IEnumerator HealRoutine(object statsObj, System.Action onFinish)
    {
        yield return new WaitForSeconds(healDelay);

        while (true)
        {
            if (statsObj is PlayerStats player)
            {
                if (player.currentHealth >= player.maxHealth)
                    break;

                player.currentHealth += Mathf.RoundToInt(healRate);
                player.currentHealth = Mathf.Clamp(player.currentHealth, 0, player.maxHealth);
                HurtPlayer.Instance?.UpdateHealthUI();
            }
            else if (statsObj is DevaStats deva)
            {
                if (deva.currentHealth >= deva.maxHealth)
                    break;

                deva.currentHealth += Mathf.RoundToInt(healRate);
                deva.currentHealth = Mathf.Clamp(deva.currentHealth, 0, deva.maxHealth);
                DevaHealthBarUI.Instance?.UpdateHealthBar(deva.currentHealth);
            }

            yield return new WaitForSeconds(healInterval);
        }

        onFinish?.Invoke();
    }
}