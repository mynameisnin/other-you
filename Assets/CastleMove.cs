using DG.Tweening;
using UnityEngine;

public class CastleMove : MonoBehaviour
{
    public RectTransform castleUITransform; // UI �� (RectTransform)
    public Transform castleWorldTransform; // 3D/2D �� (Transform)
    public float moveDistance = 500f; // �̵� �Ÿ�
    public float stepDuration = 0.05f; // �� �ܰ� �̵� �ð�
    public float pauseDuration = 0.2f; // �� �ܰ� ���ߴ� �ð�
    public int steps = 5; // �� �� �� ��� �̵�����

    void Start()
    {
        // DOTween�� �ʱ�ȭ���� �ʾҴٸ� ���� �ʱ�ȭ
        if (!DOTween.IsTweening(castleWorldTransform) && !DOTween.IsTweening(castleUITransform))
        {
            
            DOTween.Init();
        }
    }

    public void MoveCastle()
    {
        Debug.Log("MoveCastle() �����!");

        if (castleUITransform == null && castleWorldTransform == null)
        {
            Debug.LogError("MoveCastle(): Transform�� �������� ����! Inspector���� �����ϼ���.");
            return;
        }

        Sequence sequence = DOTween.Sequence();

        for (int i = 0; i < steps; i++) // `steps`�� �ݺ��ϸ� ����� ȿ�� ����
        {
            if (castleUITransform != null) // UI �� �̵� (RectTransform)
            {
                float newY = castleUITransform.anchoredPosition.y + (moveDistance / steps);
                sequence.Append(castleUITransform.DOAnchorPosY(newY, stepDuration)
                    .SetEase(Ease.OutBack)); // �� ƨ��� ����
            }
            else if (castleWorldTransform != null) // 3D/2D �� �̵� (Transform)
            {
                float newY = castleWorldTransform.position.y + (moveDistance / steps);
                sequence.Append(castleWorldTransform.DOMoveY(newY, stepDuration)
                    .SetEase(Ease.OutBack)); // �� ƨ��� ����
            }

            sequence.AppendInterval(pauseDuration); // ���ߴ� �ð� �߰�
        }

        sequence.Play();
        Debug.Log("MoveCastle(): DOTween �ִϸ��̼� �����!");
    }
}
