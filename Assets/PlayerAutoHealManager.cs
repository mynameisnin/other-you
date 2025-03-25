using UnityEngine;
using System.Collections;

public class PlayerAutoHealManager : MonoBehaviour
{
    public GameObject adam; // 아담 오브젝트
    public GameObject deva; // 데바 오브젝트

    [Header("Healing Settings")]
    public float healthHealRate = 3f; // 체력 초당 회복량
    public float manaHealRate = 5f;   // 마나 초당 회복량
    public float healInterval = 1f;   // 회복 간격
    public float healDelay = 2f;      // 비활성화 후 회복 시작 대기 시간

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
            if (!adam.activeInHierarchy && adamHealCoroutine == null && PlayerStats.Instance != null)
            {
                adamHealCoroutine = StartCoroutine(HealRoutine(PlayerStats.Instance, () => adamHealCoroutine = null));
            }

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
                // 체력 회복
                if (deva.currentHealth < deva.maxHealth)
                {
                    deva.currentHealth += Mathf.RoundToInt(healthHealRate);
                    deva.currentHealth = Mathf.Clamp(deva.currentHealth, 0, deva.maxHealth);
                    DevaHealthBarUI.Instance?.UpdateHealthBar(deva.currentHealth);
                    isFullyHealed = false;
                }

                // 마나 회복
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
