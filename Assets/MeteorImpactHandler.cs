using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeteorImpactHandler : MonoBehaviour
{
    [SerializeField] private GameObject explosionEffectPrefab;
    [SerializeField] private string[] targetTags = {  "Ground" };

    [SerializeField] private GameObject fireEffectPrefab; // �� �ִϸ��̼� ������
    private void OnTriggerEnter2D(Collider2D collision)
    {
        foreach (string tag in targetTags)
        {
            if (collision.CompareTag(tag))
            {
                TriggerExplosion();
                break;
            }
        }
    }

    private void TriggerExplosion()
    {
        // 1. ���� ���� ����Ʈ
        if (explosionEffectPrefab != null)
        {
            GameObject effect = Instantiate(explosionEffectPrefab, transform.position, Quaternion.identity);

            float duration = 1f;
            Animator anim = effect.GetComponent<Animator>();
            if (anim != null && anim.runtimeAnimatorController != null)
            {
                AnimationClip[] clips = anim.runtimeAnimatorController.animationClips;
                if (clips.Length > 0)
                    duration = clips[0].length;
            }

            Destroy(effect, duration);
        }

        // 2. �� �ִϸ��̼� �߰�
        if (fireEffectPrefab != null)
        {
            GameObject fire = Instantiate(fireEffectPrefab, transform.position, Quaternion.identity);
           
            Destroy(fire, 10f);
        }

        // 3. ���׿� �ı�
        Destroy(gameObject);
    }

}