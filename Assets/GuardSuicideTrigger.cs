using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using DG.Tweening;

public class GuardSuicideTrigger : MonoBehaviour
{
    [Header("Ÿ�Ӷ��� ���� ���")]
    public PlayableDirector timeline;            // (�ɼ�) Ÿ�Ӷ���
    public Collider2D triggerCollider;           // �÷��̾ ������ Ʈ���� �ݶ��̴�
    public Transform fallingObject;              // ������ ������ ������Ʈ
    public float fallDistance = 5f;              // �������� �Ÿ�
    public float fallDuration = 1.5f;            // �������� �ð�

    [Header("���� ����Ʈ")]
    public List<GameObject> fieldMonsters = new List<GameObject>();

    private bool triggerReady = false;           // Ʈ���Ÿ� ����� �� �ִ� ��������
    private bool hasTriggered = false;           // Ʈ���� �� ���� ����

    void Start()
    {
        if (triggerCollider != null)
            triggerCollider.enabled = false;

        // �ʱ� ���� ����Ʈ ���� (���� ������Ʈ ����)
        fieldMonsters.RemoveAll(m => m == null);
    }

    void Update()
    {
        // ���� ����Ʈ���� ���� ���� ����
        fieldMonsters.RemoveAll(m => m == null);

        // ���Ͱ� ��� �׾���, ���� Ʈ���Ÿ� �غ����� �ʾҴٸ�
        if (fieldMonsters.Count == 0 && !triggerReady)
        {
            triggerReady = true;
            Debug.Log("[GuardSuicideTrigger] ��� ���Ͱ� ���ŵ�, Ʈ���� Ȱ��ȭ");

            if (triggerCollider != null)
                triggerCollider.enabled = true;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!triggerReady || hasTriggered) return;

        if (other.CompareTag("Player"))
        {
            hasTriggered = true;
            Debug.Log("[GuardSuicideTrigger] �÷��̾ Ʈ���ſ� ������, ������Ʈ ���� ����");

            if (fallingObject != null)
            {
                float targetY = fallingObject.position.y - fallDistance;
                fallingObject.DOMoveY(targetY, fallDuration)
                    .SetEase(Ease.InQuad)
                    .OnComplete(() => Debug.Log("[GuardSuicideTrigger] ������Ʈ ���� �Ϸ�"));
            }
            else
            {
                Debug.LogError("[GuardSuicideTrigger] ���� ������Ʈ�� �����ϴ�.");
            }
        }
    }
}
