using System.Collections;
using UnityEngine;

public class AdamUltimateSkill : MonoBehaviour
{
    public float ultimateDuration = 1f;
    public float manaCost = 50f;
    public KeyCode ultimateKey = KeyCode.C;
    public float cooldownDuration = 8f; // øπ: 8√  ƒ≈∏¿”
    private float cooldownEndTime = 0f;
    private AdamMovement adamMovement;
    private Animator adamAnimator;
    private Rigidbody2D adamRigidbody;
    [SerializeField] private SkillCooldownUI ultimateCooldownUI; // ¿ŒΩ∫∆Â≈Õ ø¨∞·
    public bool isCasting = false;

    void Start()
    {
        adamMovement = GetComponent<AdamMovement>();
        adamAnimator = GetComponent<Animator>();
        adamRigidbody = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        if (Input.GetKeyDown(ultimateKey) && !isCasting)
        {
            // ƒ≈∏¿” √º≈©
            if (Time.time < cooldownEndTime)
            {
                Debug.Log("±√±ÿ±‚ ƒ≈∏¿” ¡ﬂ");
                return;
            }

            if (PlayerStats.Instance.HasEnoughMana((int)manaCost))
            {
                StartCoroutine(CastUltimate());
            }
            else
            {
                Debug.Log("±√±ÿ±‚ πﬂµø Ω«∆–: ∏∂≥™ ∫Œ¡∑!");
                ManaBarUI.Instance?.FlashBorder();
            }
        }
    }

    IEnumerator CastUltimate()
    {
        isCasting = true;

        PlayerStats.Instance.ReduceMana((int)manaCost);

        adamMovement.isInvincible = true;
        adamRigidbody.velocity = Vector2.zero;
        adamAnimator.SetTrigger("Ultimate");

        adamMovement.ForceStopDash();
        adamMovement.StopMovement();

        Debug.Log("±√±ÿ±‚ πﬂµø!");

        yield return new WaitForSeconds(ultimateDuration);

        adamMovement.isInvincible = false;
        isCasting = false;

        // ƒ≈∏¿” Ω√¿€
        cooldownEndTime = Time.time + cooldownDuration;
        // ƒ≈∏¿” Ω√¿€
        cooldownEndTime = Time.time + cooldownDuration;

        if (ultimateCooldownUI != null)
        {
            ultimateCooldownUI.cooldownTime = cooldownDuration;
            ultimateCooldownUI.StartCooldown();
        }
        Debug.Log("±√±ÿ±‚ ¡æ∑·");
    }
    public void CancelUltimate()
    {
        if (!isCasting) return;

        Debug.Log("±√±ÿ±‚ ∞≠¡¶ ¡æ∑·µ  (ƒ≥∏Ø≈Õ Ω∫¿ßƒ°)");

        StopAllCoroutines(); // ±√±ÿ±‚ ƒ⁄∑Á∆æ ¡æ∑·
        isCasting = false;
        adamMovement.isInvincible = false;
        adamMovement.ForceStopDash();
        adamMovement.StopMovement();
        adamAnimator.ResetTrigger("Ultimate");
    }
}
