using UnityEngine;
using System.Collections;

public class PlayerAutoHealManager : MonoBehaviour
{
    public GameObject adam; // 아담 오브젝트
    public GameObject deva; // 데바 오브젝트

    public float healRate = 3f; // 초당 회복량
    public float healInterval = 1f; // 회복 간격
    public float healDelay = 2f; // 비활성화 후 회복 시작 대기 시간

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
            // 아담 비활성화 상태에서 회복 시작
            if (!adam.activeInHierarchy && adamHealCoroutine == null && PlayerStats.Instance != null)
            {
                adamHealCoroutine = StartCoroutine(HealRoutine(PlayerStats.Instance, () => adamHealCoroutine = null));
            }

            // 데바 비활성화 상태에서 회복 시작
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