using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeteorImpactHandler : MonoBehaviour
{
    [SerializeField] private GameObject explosionEffectPrefab;
    [SerializeField] private string[] targetTags = { "Player", "DevaPlayer", "Ground" };

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
        if (explosionEffectPrefab != null)
        {
            GameObject effect = Instantiate(explosionEffectPrefab, transform.position, Quaternion.identity);

            float duration = 1f; // 기본값 (예비)
            Animator anim = effect.GetComponent<Animator>();
            if (anim != null && anim.runtimeAnimatorController != null)
            {
                AnimationClip[] clips = anim.runtimeAnimatorController.animationClips;
                if (clips.Length > 0)
                    duration = clips[0].length;
            }

            Destroy(effect, duration); // 애니메이션 종료 후 파괴
        }

        Destroy(gameObject); // 메테오 바로 제거
    }
}