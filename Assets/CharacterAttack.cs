using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterAttack : MonoBehaviour
{
    private Animator animator;
    private int currentComboStep = 0; // 현재 콤보 단계
    private float comboTimer = 0f; // 콤보 타이머
    private float comboMaxTime = 0.02f; // 콤보 입력 가능 시간
    private bool isAttacking = false; // 현재 공격 중인지 여부

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
            if (isAttacking)
            {
                // 콤보 진행 중이면 다음 단계로
                ContinueCombo();
            }
            else
            {
                // 새로운 콤보 시작
                StartCombo();
            }
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
            Debug.Log($"Combo Timer: {comboTimer}, Current Combo Step: {currentComboStep}");

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

}