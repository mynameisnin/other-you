using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class SkillCooldownUI : MonoBehaviour
{
    public Image skillIcon;             // 일반 아이콘
    public Image cooldownOverlay;       // Fill 타입으로 쿨타임 표현
    public GameObject glowEffect;       // 쿨타임 끝났을 때 반짝임

    public float cooldownTime = 5f;
    private float currentCooldown = 0f;
    private bool isCoolingDown = false;

    void Start()
    {
        // 처음엔 쿨타임 오버레이가 보이지 않도록 설정
        if (cooldownOverlay != null)
        {
            cooldownOverlay.fillAmount = 0f;
        }

        if (glowEffect != null)
        {
            glowEffect.SetActive(false);
        }
    }
    public void StartCooldown()
    {
        if (isCoolingDown) return;
        currentCooldown = cooldownTime;
        isCoolingDown = true;

        cooldownOverlay.fillAmount = 1f;
        skillIcon.color = new Color(1, 1, 1, 0.5f); // 흐려짐
        if (glowEffect != null) glowEffect.SetActive(false);
    }

    void Update()
    {
        if (!isCoolingDown) return;

        currentCooldown -= Time.deltaTime;
        cooldownOverlay.fillAmount = currentCooldown / cooldownTime;

        if (currentCooldown <= 0f)
        {
            isCoolingDown = false;
            cooldownOverlay.fillAmount = 0f;

            skillIcon.color = Color.white;

            if (glowEffect != null)
            {
                glowEffect.SetActive(true);
                glowEffect.transform.DOPunchScale(Vector3.one * 0.3f, 0.4f, 8, 1f);
            }
        }
    }
}
