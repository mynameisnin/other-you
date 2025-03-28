using System.Collections;
using UnityEngine;

public class AdamAttackSpeedBuff : MonoBehaviour
{
    [Header("Buff Settings")]
    public float attackSpeedMultiplier = 1.5f; // 1.5배 빠르게
    public float buffDuration = 5f;
    public int manaCost = 20;

    [Header("Effect")]
    public GameObject buffEffect;

    private bool isBuffActive = false;
    private Coroutine buffCoroutine;

    private AdamMovement adamMovement;
    private Animator adamAnimator;

    void Start()
    {
        adamMovement = GetComponent<AdamMovement>();
        adamAnimator = GetComponent<Animator>();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            TryActivateBuff();
        }
    }

    public void TryActivateBuff()
    {
        if (isBuffActive)
        {
            Debug.Log("버프 이미 활성화 중");
            return;
        }

        if (!PlayerStats.Instance.HasEnoughMana(manaCost))
        {
            Debug.Log("마나 부족");
            ManaBarUI.Instance?.FlashBorder();
            return;
        }

        PlayerStats.Instance.ReduceMana(manaCost);

        if (buffCoroutine != null)
            StopCoroutine(buffCoroutine);

        buffCoroutine = StartCoroutine(ApplyBuff());
    }

    private IEnumerator ApplyBuff()
    {
        isBuffActive = true;

        // 기존 값 저장
        float originalAttackDelay = adamMovement.attackInputCooldown;
        float originalAnimatorSpeed = adamAnimator.speed;

        // 버프 적용
        adamMovement.attackInputCooldown *= 1f / attackSpeedMultiplier; // 입력 간격 줄이기
        adamAnimator.speed = attackSpeedMultiplier; // 애니메이션 속도 증가

        if (buffEffect != null)
            buffEffect.SetActive(true);

        Debug.Log("?? 공격 속도 + 애니메이션 속도 버프 적용!");

        yield return new WaitForSeconds(buffDuration);

        // 복원
        adamMovement.attackInputCooldown = originalAttackDelay;
        adamAnimator.speed = originalAnimatorSpeed;

        if (buffEffect != null)
            buffEffect.SetActive(false);

        isBuffActive = false;
        Debug.Log("? 공격 속도 버프 종료");
    }
}
