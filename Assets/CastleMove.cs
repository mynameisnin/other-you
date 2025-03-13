using DG.Tweening;
using UnityEngine;

public class CastleMove : MonoBehaviour
{
    public RectTransform castleUITransform; // UI 성 (RectTransform)
    public Transform castleWorldTransform; // 3D/2D 성 (Transform)
    public float moveDistance = 500f; // 이동 거리
    public float stepDuration = 0.05f; // 각 단계 이동 시간
    public float pauseDuration = 0.2f; // 각 단계 멈추는 시간
    public int steps = 5; // 총 몇 번 끊어서 이동할지

    void Start()
    {
        // DOTween이 초기화되지 않았다면 강제 초기화
        if (!DOTween.IsTweening(castleWorldTransform) && !DOTween.IsTweening(castleUITransform))
        {
            
            DOTween.Init();
        }
    }

    public void MoveCastle()
    {
        Debug.Log("MoveCastle() 실행됨!");

        if (castleUITransform == null && castleWorldTransform == null)
        {
            Debug.LogError("MoveCastle(): Transform이 설정되지 않음! Inspector에서 연결하세요.");
            return;
        }

        Sequence sequence = DOTween.Sequence();

        for (int i = 0; i < steps; i++) // `steps`번 반복하며 끊기는 효과 생성
        {
            if (castleUITransform != null) // UI 성 이동 (RectTransform)
            {
                float newY = castleUITransform.anchoredPosition.y + (moveDistance / steps);
                sequence.Append(castleUITransform.DOAnchorPosY(newY, stepDuration)
                    .SetEase(Ease.OutBack)); // 더 튕기는 느낌
            }
            else if (castleWorldTransform != null) // 3D/2D 성 이동 (Transform)
            {
                float newY = castleWorldTransform.position.y + (moveDistance / steps);
                sequence.Append(castleWorldTransform.DOMoveY(newY, stepDuration)
                    .SetEase(Ease.OutBack)); // 더 튕기는 느낌
            }

            sequence.AppendInterval(pauseDuration); // 멈추는 시간 추가
        }

        sequence.Play();
        Debug.Log("MoveCastle(): DOTween 애니메이션 실행됨!");
    }
}
