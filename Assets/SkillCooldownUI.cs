using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class SkillCooldownUI : MonoBehaviour
{
    [System.Serializable]
    public class SkillSlot
    {
        public Image skillIcon;           // 아이콘 이미지
        public Image cooldownOverlay;     // Fill로 쿨타임 표시
        public GameObject glowEffect;     // 쿨타임 종료 시 이펙트

        [HideInInspector] public float cooldownTime; // 외부에서 연결
        private float currentCooldown = 0f;
        private bool isCoolingDown = false;

        public void StartCooldown(float skillCooldownTime)
        {
            if (isCoolingDown) return;

            cooldownTime = skillCooldownTime;
            currentCooldown = cooldownTime;
            isCoolingDown = true;

            if (cooldownOverlay != null)
                cooldownOverlay.fillAmount = 1f;

            if (skillIcon != null)
                skillIcon.color = new Color(1, 1, 1, 0.5f); // 흐리게

            if (glowEffect != null)
                glowEffect.SetActive(false);
        }

        public void UpdateCooldown()
        {
            if (!isCoolingDown) return;

            currentCooldown -= Time.deltaTime;
            if (cooldownOverlay != null)
                cooldownOverlay.fillAmount = currentCooldown / cooldownTime;

            if (currentCooldown <= 0f)
            {
                isCoolingDown = false;
                if (cooldownOverlay != null)
                    cooldownOverlay.fillAmount = 0f;

                if (skillIcon != null)
                    skillIcon.color = Color.white;

                if (glowEffect != null)
                {
                    glowEffect.SetActive(true);
                    glowEffect.transform.localScale = Vector3.one;
                    glowEffect.transform.DOPunchScale(Vector3.one * 0.3f, 0.4f, 8, 1f);

                    // 투명 -> 불투명 -> 투명
                    Image glowImage = glowEffect.GetComponent<Image>();
                    if (glowImage != null)
                    {
                        glowImage.color = new Color(1, 1, 1, 0);
                        glowImage.DOFade(1f, 0.1f).OnComplete(() =>
                        {
                            glowImage.DOFade(0f, 0.3f);
                        });
                    }
                }
            }
        }
    }

    [Header("Skill Slots")]
    public SkillSlot[] skillSlots = new SkillSlot[3];

    void Start()
    {
        foreach (var slot in skillSlots)
        {
            if (slot.cooldownOverlay != null)
                slot.cooldownOverlay.fillAmount = 0f;
            if (slot.glowEffect != null)
                slot.glowEffect.SetActive(false);
        }
    }

    void Update()
    {
        foreach (var slot in skillSlots)
        {
            slot.UpdateCooldown();
        }
    }

    // 외부 스킬 스크립트에서 호출
    public void TriggerSkillCooldown(int index, float cooldownTime)
    {
        if (index < 0 || index >= skillSlots.Length) return;
        skillSlots[index].StartCooldown(cooldownTime);
    }
}
