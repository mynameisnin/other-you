using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using DG.Tweening;

public class GuardSuicideTrigger : MonoBehaviour
{
    public PlayableDirector timeline; // Ÿ�Ӷ���
    public Collider2D triggerCollider; // �÷��̾ ������ Ʈ���� �ݶ��̴�
    public Transform fallingObject; // ������ ������ ������Ʈ
    public float fallDistance = 5f; // �������� �Ÿ�
    public float fallDuration = 1.5f; // �������� �ð�

    private bool timelineStarted = false; // Ÿ�Ӷ����� ����Ǿ����� üũ

    void Start()
    {
        // Ʈ���� ��Ȱ��ȭ (�ʱ� ����)
        if (triggerCollider != null)
        {
            triggerCollider.enabled = false;
        }
    }

    void Update()
    {
        // Ÿ�Ӷ��� ���� �α� ���
        if (timeline != null)
        {
            Debug.Log($"Ÿ�Ӷ��� ����: {timeline.state}");
        }

        // Ÿ�Ӷ����� ���� ������ Ȯ�� �� Ʈ���� Ȱ��ȭ
        if (timeline != null && timeline.state == PlayState.Playing && !timelineStarted)
        {
            timelineStarted = true; // �� ���� ����ǵ��� üũ
            if (triggerCollider != null)
            {
                triggerCollider.enabled = true; // Ʈ���� Ȱ��ȭ
                Debug.Log(" Ʈ���� Ȱ��ȭ��.");
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log($"Ʈ���� ����: {other.gameObject.name}");

        if (other.CompareTag("Player")) // �÷��̾ Ʈ���ſ� ������ ����
        {
            Debug.Log(" �÷��̾ Ʈ���ſ� ������! ������Ʈ�� �ٷ� �Ʒ��� �������ϴ�.");

            if (fallingObject != null)
            {
                Debug.Log(" ������ ������Ʈ Ȯ�ε�.");

                // �Ʒ��� ������ ��ǥ ��ġ ���� (���� ��ġ���� -fallDistance ��ŭ �Ʒ�)
                float targetY = fallingObject.position.y - fallDistance;

                // DOTween �ִϸ��̼� ���� (��� ��������)
                fallingObject.DOMoveY(targetY, fallDuration)
                    .SetEase(Ease.InQuad) // �ε巴�� �������� ȿ�� (������ �������� ����)
                    .OnComplete(() => Debug.Log(" ������Ʈ ���� �Ϸ�"));
            }
            else
            {
                Debug.LogError(" ������ ������Ʈ�� �������� �ʽ��ϴ�!");
            }
        }
    }
}
