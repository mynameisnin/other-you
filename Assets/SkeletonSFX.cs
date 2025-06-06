using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class SkeletonSFX : MonoBehaviour
{

    public void SkeletonAttackSFX()
    {
        SFXManager.Instance?.Play(SFXType.SkeletonAttack);
    }

    public void SkeletonAttack2SFX()
    {
        SFXManager.Instance?.Play(SFXType.SkeletonAttack2);
    }
}
