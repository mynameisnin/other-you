using UnityEngine;

public class CharacterFlipController : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void FlipCharacter()
    {
        spriteRenderer.flipX = !spriteRenderer.flipX; // ���� ���� ����
    }
}