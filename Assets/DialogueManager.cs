using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class DialogueManager : MonoBehaviour
{
    [SerializeField] private GameObject[] monsters; // ���� ���θ� üũ�� ���͵�
    [SerializeField] private GameObject objectToActivate; // ���̾�α� ������Ʈ

    private bool hasActivated = false;

    private void Start()
    {
        objectToActivate.SetActive(false); // ������ �� ��Ȱ��ȭ
    }

    private void Update()
    {
        if (hasActivated) return; // �̹� Ȱ��ȭ������ �ߺ� ����

        bool allDead = true;

        foreach (GameObject monster in monsters)
        {
            if (monster != null)
            {
                allDead = false;
                break;
            }
        }

        if (allDead)
        {
            Debug.Log("��� ���Ͱ� �����Ǿ����ϴ�!");
            objectToActivate.SetActive(true);
            hasActivated = true;
        }
    }
}