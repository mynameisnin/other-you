using System.Collections;
using UnityEngine;

public class CharacterAttack : MonoBehaviour
{
    private Animator animator;
    private int currentComboStep = 0; // 현재 콤보 단계
    private float comboTimer = 0f; // 콤보 타이머
    private float comboMaxTime = 0.008f; // 콤보 입력 가능 시간
    private bool isAttacking = false; // 현재 공격 중인지 여부

    // 공격 상태를 다른 스크립트에서 읽기 위한 프로퍼티
    public bool IsAttacking => isAttacking;

    void Start()
    {
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        HandleComboInput();
        UpdateComboTimer();
    }

    private void HandleComboInput()
    {
        // 공격 버튼 입력 감지
        if (Input.GetKey(KeyCode.M))
        {
            TriggerAttack(); // 공격 트리거 호출
        }
    }

    public void TriggerAttack()
    {
        // 콤보 입력을 직접 트리거
        if (isAttacking)
        {
            ContinueCombo(); // 콤보 진행
        }
        else
        {
            StartCombo(); // 첫 공격
        }
    }

    private void StartCombo()
    {
        isAttacking = true;
        currentComboStep = 1; // 첫 번째 공격 단계
        comboTimer = comboMaxTime; // 콤보 타이머 초기화
        animator.SetTrigger("Attack1"); // 첫 번째 공격 애니메이션 실행
    }

    private void ContinueCombo()
    {
        if (currentComboStep == 1)
        {
            currentComboStep = 2; // 두 번째 공격 단계
            animator.SetTrigger("Attack2"); // 두 번째 공격 애니메이션 실행
        }
        else if (currentComboStep == 2)
        {
            currentComboStep = 3; // 세 번째 공격 단계
            animator.SetTrigger("Attack3"); // 세 번째 공격 애니메이션 실행
        }

        comboTimer = comboMaxTime; // 콤보 타이머 리셋
    }

    private void UpdateComboTimer()
    {
        if (isAttacking)
        {
            comboTimer -= Time.deltaTime;

            // 콤보 타이머가 종료되면 초기화
            if (comboTimer <= 0)
            {
                ResetCombo();
            }
        }
    }

    private void ResetCombo()
    {
        isAttacking = false;
        currentComboStep = 0;
        comboTimer = 0f;
        animator.ResetTrigger("Attack1");
        animator.ResetTrigger("Attack2");
        animator.ResetTrigger("Attack3");
    }

    // 애니메이션 이벤트: 공격 종료 시 호출
    public void EndAttack()
    {
        isAttacking = false; // 공격 종료
    }
}
