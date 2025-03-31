using UnityEngine;
using DG.Tweening;
using UnityEngine.Tilemaps;

public class TilemapFadeIn : MonoBehaviour
{
    public Tilemap tilemap;  // Ÿ�ϸ� ����
    public float fadeDuration = 2f; // ���̵��� �ð�
    private Material tilemapMaterial; // ��Ƽ���� ����

    private void Start()
    {
        // TilemapRenderer���� Material ��������
        TilemapRenderer tilemapRenderer = tilemap.GetComponent<TilemapRenderer>();

        // ���� ��Ƽ������ �����Ͽ� ���ο� ��Ƽ������ ����
        tilemapMaterial = new Material(tilemapRenderer.material);
        tilemapRenderer.material = tilemapMaterial; // ���ο� ��Ƽ���� ����

        // �ʱ� ���İ��� 0���� ���� (���� ����)
        tilemapMaterial.color = new Color(1f, 1f, 1f, 0f);

        // ���̵��� ����
        FadeIn();
    }

    void FadeIn()
    {
        // ��Ƽ������ ���� ���� ������ �����Ͽ� ���̵���
        tilemapMaterial.DOFade(1f, fadeDuration).SetEase(Ease.InOutQuad);
    }
}
