using System.Collections;
using UnityEngine;

public class DevaBigLaserSkill : MonoBehaviour
{
    [Header("자원 및 쿨타임")]
    public int manaCost = 40;
    public float cooldown = 6f;
    private float cooldownEndTime = 0f;

    [Header("애니메이션")]
    public Animator animator;
    public string laserAnimName = "Cast2";  // ← 애니메이션 이름

    [Header("UI")]
    public SkillCooldownUI cooldownUI;

    [Header("참조")]
    public DebaraMovement devaMovement; // ← 움직임 멈춤 처리

    [Header("데미지 관련")]
    public int damage = 50;
    public string targetTag = "Enemy";
    public bool fromAdam = false;
    public bool fromDeba = true;

    [Header("콜라이더 연결")]
    public Collider2D laserCollider; // ← 인스펙터에 연결할 것

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
        cooldownUI?.StartCooldown();

        // 이동 차단
        if (devaMovement != null)
        {
            devaMovement.StopMovement();
            devaMovement.isAttacking = true;
        }

        // 콜라이더 비활성화 (애니메이션에서 Enable로 조절)
        if (laserCollider != null)
            laserCollider.enabled = false;

        // 애니메이션 시작
        animator.SetTrigger("Trigger_Cast12");

        Debug.Log(" Deva 레이저 스킬 Cast2 시작!");
    }

    //  애니메이션 이벤트에서 호출됨
    public void EnableLaserCollider()
    {
        if (laserCollider != null)
            laserCollider.enabled = true;
    }

    public void DisableLaserCollider()
    {
        if (laserCollider != null)
            laserCollider.enabled = false;
    }

    public void EndBigLaser()
    {
        if (devaMovement != null)
        {
            devaMovement.EndAttack();
        }

        if (laserCollider != null)
        {
            laserCollider.enabled = false;
        }

        Debug.Log(" 레이저 스킬 종료됨");
    }

    // 데미지 처리
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (laserCollider == null || !laserCollider.enabled) return;

        // 일반 적
        enemyTest enemy = other.GetComponent<enemyTest>();
        if (enemy != null)
        {
            Debug.Log("Deva 레이저 명중! (적)");
            enemy.TakeDamage(damage, fromAdam, fromDeba, transform.root);
            return;
        }

        // 보스
        BossHurt boss = other.GetComponent<BossHurt>();
        if (boss != null)
        {
            Debug.Log("Deva 레이저 명중! (보스)");
            boss.TakeDamage(damage, fromAdam, fromDeba); // 3개 인자
            return;
        }
    }

    public float CooldownRemaining => Mathf.Max(0, cooldownEndTime - Time.time);
    public float CooldownDuration => cooldown;
}
