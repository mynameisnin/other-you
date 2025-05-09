using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeteorImpactHandler : MonoBehaviour
{
    [SerializeField] private GameObject explosionEffectPrefab;
    [SerializeField] private string[] targetTags = { "Ground" };

    [SerializeField] private GameObject fireEffectPrefab; // 불 애니메이션 프리팹

    private void OnTriggerEnter2D(Collider2D collision)
    {
        foreach (string tag in targetTags)
        {
            if (collision.CompareTag(tag))
            {
                TriggerExplosion();
                break; // Important: break after triggering to avoid multiple explosions if meteor has multiple colliders or hits multiple tagged objects at once
            }
        }
    }

    private void TriggerExplosion()
    {
        // 1. 기존 폭발 이펙트
        if (explosionEffectPrefab != null)
        {
            GameObject effect = Instantiate(explosionEffectPrefab, transform.position, Quaternion.identity);

            float duration = 1f; // Default duration
            Animator anim = effect.GetComponent<Animator>();
            if (anim != null && anim.runtimeAnimatorController != null)
            {
                AnimationClip[] clips = anim.runtimeAnimatorController.animationClips;
                if (clips.Length > 0)
                {
                    duration = clips[0].length;
                }
            }
            Destroy(effect, duration);
        }

        // 2. 보스 체력 확인 및 불 애니메이션 추가
        bool spawnFire = false;

        // Find all BossHurt components in the scene
        BossHurt[] bosses = FindObjectsOfType<BossHurt>(); // Use FindObjectsOfType if there can be multiple bosses

        if (bosses.Length > 0)
        {
            foreach (BossHurt boss in bosses)
            {
                // Check if any boss is below 50% health
                // The BossHurt script already has logic for phase2Triggered when health is <= MaxHealth / 2
                // We can either use that flag (if it were public) or re-check the condition.
                // Re-checking the condition here is safer and doesn't require modifying BossHurt to make phase2Triggered public.
                if (boss != null && boss.currentHealth <= boss.MaxHealth / 2)
                {
                    spawnFire = true;
                    Debug.Log($"Boss {boss.gameObject.name} is below 50% HP. Spawning fire effect.");
                    break; // Found a boss below 50% HP, no need to check others
                }
            }
        }
        else
        {
            Debug.Log("No BossHurt component found in the scene. Fire effect will not be spawned based on boss health.");
            // If you want the fire to spawn regardless of boss health if no boss is present,
            // you could set spawnFire = true here or handle it differently.
            // For now, it only spawns if a boss is found and is below 50% HP.
        }


        if (fireEffectPrefab != null && spawnFire)
        {
            GameObject fire = Instantiate(fireEffectPrefab, transform.position, Quaternion.identity);
            // Consider parenting the fire to the ground or an environment object if it should persist
            // and not be tied to the meteor's (soon to be destroyed) transform's original position logic.
            // For simplicity, keeping it at the impact point.
            Destroy(fire, 10f); // Destroy after 10 seconds
        }

        // 3. 메테오 파괴
        Destroy(gameObject);
    }
}