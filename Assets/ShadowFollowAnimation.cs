using UnityEngine;

public class ShadowFollowAnimation : MonoBehaviour
{
    public Animator characterAnimator; // ĳ������ Animator
    private Animator shadowAnimator;   // �׸����� Animator
    public SpriteRenderer characterRenderer; // ĳ������ SpriteRenderer
    private SpriteRenderer shadowRenderer;
    void Start()
    {
        shadowAnimator = GetComponent<Animator>();
        shadowRenderer = GetComponent<SpriteRenderer>();
    }


    void Update()
    {
        // ĳ������ �ִϸ��̼� ���¸� �׸��� �ִϸ����Ϳ� ����
        shadowAnimator.Play(characterAnimator.GetCurrentAnimatorStateInfo(0).shortNameHash);
        shadowRenderer.sprite = characterRenderer.sprite;
    }
}