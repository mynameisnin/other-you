using UnityEngine;

public class ShadowFollowAnimation : MonoBehaviour
{
    public Animator characterAnimator; // 캐릭터의 Animator
    private Animator shadowAnimator;   // 그림자의 Animator
    public SpriteRenderer characterRenderer; // 캐릭터의 SpriteRenderer
    private SpriteRenderer shadowRenderer;
    void Start()
    {
        shadowAnimator = GetComponent<Animator>();
        shadowRenderer = GetComponent<SpriteRenderer>();
    }


    void Update()
    {
        // 캐릭터의 애니메이션 상태를 그림자 애니메이터에 복사
        shadowAnimator.Play(characterAnimator.GetCurrentAnimatorStateInfo(0).shortNameHash);
        shadowRenderer.sprite = characterRenderer.sprite;
    }
}