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
}