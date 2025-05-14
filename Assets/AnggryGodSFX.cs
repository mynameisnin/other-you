using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class AnggryGodSFX : MonoBehaviour
{
    public void AngryGodAttckSFX()
    {
        SFXManager.Instance?.Play(SFXType.AngryGodAttack);
    }
    public void AngryGodFrameSFX()
    {
        SFXManager.Instance?.Play(SFXType.AngryGodFrame);
    }
    public void AngryGodMeteorSFX()
    {
        SFXManager.Instance?.Play(SFXType.AngryGodmMteor);
    }
    public void AngryGodMeteorFallingSFX()
    {
        SFXManager.Instance?.Play(SFXType.AngryGodMeteorFalling);
    }
    public void AngryGodActiveSFX()
    {
        SFXManager.Instance?.Play(SFXType.AngryGodActive);
    }
    public void AngryGodDashSFX()
    {
        SFXManager.Instance?.Play(SFXType.AngryGodDash);
    }
    public void AngryGodEvasionSFX()
    {
        SFXManager.Instance?.Play(SFXType.AngryGodEvasion);
    }
    public void AngryGodSpawnSkillSFX()
    {
        SFXManager.Instance?.Play(SFXType.AngryGodSpawnSkill);
    }
}
