using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class SkillCooldownUI : MonoBehaviour
{
    public Image skillIcon;             // �Ϲ� ������
    public Image cooldownOverlay;       // Fill Ÿ������ ��Ÿ�� ǥ��
    public GameObject glowEffect;       // ��Ÿ�� ������ �� ��¦��

    public float cooldownTime = 5f;
    private float currentCooldown = 0f;
    private bool isCoolingDown = false;

    void Start()
    {
        // ó���� ��Ÿ�� �������̰� ������ �ʵ��� ����
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
        skillIcon.color = new Color(1, 1, 1, 0.5f); // �����
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
