using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DevaAnimationSfxEvents : MonoBehaviour
{
    // Start is called before the first frame update
    public void PlayTeleportSFX()
    {
        SFXManager.Instance?.Play(SFXType.Teleport);
    }
    public void DevaNomalAttackSFX()
    {
        SFXManager.Instance?.Play(SFXType.NomalAttackSFX);
    }
    public void DevaSkillAttackSFX()
    {
        SFXManager.Instance?.Play(SFXType.DevaskillAttacSFX);

    }
    public void DevaSkillBigLaser()
    {
        SFXManager.Instance?.Play(SFXType.SkillBigerLaser);
    }
}
