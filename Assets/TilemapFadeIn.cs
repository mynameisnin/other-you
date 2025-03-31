using UnityEngine;
using DG.Tweening;
using UnityEngine.Tilemaps;

public class TilemapFadeIn : MonoBehaviour
{
    public Tilemap tilemap;  // 타일맵 참조
    public float fadeDuration = 2f; // 페이드인 시간
    private Material tilemapMaterial; // 머티리얼 저장

    private void Start()
    {
        // TilemapRenderer에서 Material 가져오기
        TilemapRenderer tilemapRenderer = tilemap.GetComponent<TilemapRenderer>();

        // 원본 머티리얼을 복사하여 새로운 머티리얼을 생성
        tilemapMaterial = new Material(tilemapRenderer.material);
        tilemapRenderer.material = tilemapMaterial; // 새로운 머티리얼 적용

        // 초기 알파값을 0으로 설정 (완전 투명)
        tilemapMaterial.color = new Color(1f, 1f, 1f, 0f);

        // 페이드인 실행
        FadeIn();
    }

    void FadeIn()
    {
        // 머티리얼의 알파 값을 서서히 변경하여 페이드인
        tilemapMaterial.DOFade(1f, fadeDuration).SetEase(Ease.InOutQuad);
    }
}
