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

}
