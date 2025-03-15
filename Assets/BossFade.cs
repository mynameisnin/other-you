using System.Collections;
using DG.Tweening; // DOTween ���
using UnityEngine;

public class BossFade : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;
    private Collider2D bossCollider;
    public float fadeDuration = 1.5f; // ���̵� �ƿ� ���� �ð�

    [Header("����Ʈ ����")]
    public GameObject effectPrefab; // ����Ʈ ������ (Inspector���� ����)
    public Transform effectSpawnPoint; // ����Ʈ ��ġ

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        bossCollider = GetComponent<Collider2D>();

        if (spriteRenderer == null)
        {
            Debug.LogError("[BossFade] SpriteRenderer�� �����ϴ�!");
        }

        if (bossCollider == null)
        {
            Debug.LogError("[BossFade] Collider2D�� �����ϴ�!");
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player")) // �÷��̾ Ʈ���� �ȿ� ������ ����
        {
            Debug.Log("[BossFade] �÷��̾ ������! ���� ���� ����.");
            StartCoroutine(FadeAndDestroy());
        }
    }

    private IEnumerator FadeAndDestroy()
    {
        bossCollider.enabled = false; // �浹 ����

        //  ����Ʈ ���� (���� ���)
        if (effectPrefab != null)
        {
            Vector3 spawnPos = effectSpawnPoint != null ? effectSpawnPoint.position : transform.position;
            GameObject effect = Instantiate(effectPrefab, spawnPos, Quaternion.identity);
            Destroy(effect, 2f); // 2�� �� ����Ʈ ����
        }

        //  ���� ���̵� �ƿ� ȿ��
        if (spriteRenderer != null)
        {
            spriteRenderer.DOFade(0, fadeDuration);
        }

        yield return new WaitForSeconds(fadeDuration + 0.5f); // �ִϸ��̼��� ���� ������ ���

        Debug.Log("[BossFade] ���� ���� �Ϸ�!");
        Destroy(gameObject); // ���� ����
    }
}
