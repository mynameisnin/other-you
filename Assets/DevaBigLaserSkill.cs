using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DevaBigLaserSkill : MonoBehaviour
{
    [Header("자원 및 쿨타임")]
    public int manaCost = 40;
    public float cooldown = 6f;
    private float cooldownEndTime = 0f;

    [Header("애니메이션")]
    public Animator animator;
    public string laserAnimName = "Cast2";  // ← 애니메이션 이름 수정됨

    [Header("UI")]
    public SkillCooldownUI cooldownUI;
    [Header("참조")]
    public DebaraMovement devaMovement; // ← 인스펙터에서 연결할 것

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.C))
        {
            TryCastLaser();
        }
    }

    public void TryCastLaser()
    {
        if (Time.time < cooldownEndTime)
        {
            Debug.Log(" 쿨타임 중");
            return;
        }

        if (!DevaStats.Instance.HasEnoughMana(manaCost))
        {
            Debug.Log(" 마나 부족");
            DevaManaBarUI.Instance?.FlashBorder();
            return;
        }

        // 마나 차감
        DevaStats.Instance.ReduceMana(manaCost);

        // 쿨타임 갱신
        cooldownEndTime = Time.time + cooldown;

        // 쿨타임 UI 시작
        cooldownUI?.StartCooldown();

        //  이동 및 행동 차단
        if (devaMovement != null)
        {
            devaMovement.StopMovement();           // 속도 멈춤
            devaMovement.isAttacking = true;       // 이동 및 공격 차단
        }

        // 애니메이션 실행
        animator.SetTrigger("Trigger_Cast12");

        Debug.Log(" [Deva] 강력 레이저 Cast2 실행!");
    }

    public void EndBigLaser()
    {
        if (devaMovement != null)
        {
            devaMovement.EndAttack();  // 이동 및 공격 가능하게 다시 풀어줌
        }
    }

    // UI에서 쿨타임 접근용 프로퍼티
    public float CooldownRemaining => Mathf.Max(0, cooldownEndTime - Time.time);
    public float CooldownDuration => cooldown;
}