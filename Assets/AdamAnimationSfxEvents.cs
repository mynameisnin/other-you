using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class AdamAnimationSfxEvents : MonoBehaviour
{
    // �ִϸ��̼� �̺�Ʈ���� ȣ���
    public void PlaySwordSwingSFX()
    {
        SFXManager.Instance?.Play(SFXType.SwordSwing);
    }
    public void PlaySwordSwingSFX2()
    {
        SFXManager.Instance?.Play(SFXType.SwordSwing2);
    }
    public void PlayeFootStepSFX()
    {
        SFXManager.Instance?.Play(SFXType.FootStep);
    }
    public void PlayeBuffSkillSFX()
    {
        SFXManager.Instance?.Play(SFXType.BuffSkill);
    }
    public void PlayBladeSkillSFX()
    {
        SFXManager.Instance?.Play(SFXType.BladeSkill);
    }
    public void PlayAdamJumpSFX()
    {
        SFXManager.Instance?.Play(SFXType.AdamJump);
    }
}