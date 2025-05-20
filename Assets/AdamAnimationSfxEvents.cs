using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class AdamAnimationSfxEvents : MonoBehaviour
{
    // 애니메이션 이벤트에서 호출됨
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
    public void PlayParrySFX()
    {
        SFXManager.Instance?.Play(SFXType.Parry);
    }
    public void PlaySwichSFX()
    {
        SFXManager.Instance?.Play(SFXType.Swich);
    }
    public void PlayhitSFX()
    {
        SFXManager.Instance?.Play(SFXType.hit);
    }
    public void Playhit1SFX()
    {
        SFXManager.Instance?.Play(SFXType.hit1);
    }
    public void PlayDashSFX()
    {
        SFXManager.Instance?.Play(SFXType.Dash);
    }
    public void PlayDeathSFX()
    {
        SFXManager.Instance?.Play(SFXType.Death);
    }
}