using System.Collections;
using DG.Tweening; // DOTween 사용
using UnityEngine;

public class BossFade : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;
    private Collider2D bossCollider;
    public float fadeDuration = 1.5f; // 페이드 아웃 지속 시간

    [Header("이펙트 설정")]
    public GameObject effectPrefab; // 이펙트 프리팹 (Inspector에서 설정)
    public Transform effectSpawnPoint; // 이펙트 위치

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        bossCollider = GetComponent<Collider2D>();

        if (spriteRenderer == null)
        {
            Debug.LogError("[BossFade] SpriteRenderer가 없습니다!");
        }

        if (bossCollider == null)
        {
            Debug.LogError("[BossFade] Collider2D가 없습니다!");
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player")) // 플레이어가 트리거 안에 들어오면 실행
        {
            Debug.Log("[BossFade] 플레이어가 감지됨! 보스 삭제 시작.");
            StartCoroutine(FadeAndDestroy());
        }
    }

    private IEnumerator FadeAndDestroy()
    {
        bossCollider.enabled = false; // 충돌 막기

        //  이펙트 생성 (있을 경우)
        if (effectPrefab != null)
        {
            Vector3 spawnPos = effectSpawnPoint != null ? effectSpawnPoint.position : transform.position;
            GameObject effect = Instantiate(effectPrefab, spawnPos, Quaternion.identity);
            Destroy(effect, 2f); // 2초 후 이펙트 삭제
        }

        //  보스 페이드 아웃 효과
        if (spriteRenderer != null)
        {
            spriteRenderer.DOFade(0, fadeDuration);
        }

        yield return new WaitForSeconds(fadeDuration + 0.5f); // 애니메이션이 끝날 때까지 대기

        Debug.Log("[BossFade] 보스 삭제 완료!");
        Destroy(gameObject); // 보스 삭제
    }
}
