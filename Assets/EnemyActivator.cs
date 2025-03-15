using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyActivator : MonoBehaviour
{
    private void Awake()
    {
        gameObject.SetActive(false); // ó������ ��Ȱ��ȭ
    }

    // Ÿ�Ӷ����� Signal Receiver���� �� �Լ��� ����
    public void Activate()
    {
        gameObject.SetActive(true);
        Debug.Log("[EnemyActivator] �� Ȱ��ȭ��!");
    }
}