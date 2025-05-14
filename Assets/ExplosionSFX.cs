using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplosionSFX : MonoBehaviour
{
    public void PlayExplosionSFX()
    {
        SFXManager.Instance?.Play(SFXType.Explosion);
    }

}
