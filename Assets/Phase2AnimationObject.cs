using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Phase2AnimationObject : MonoBehaviour
{
    // �ִϸ��̼� ���� ȣ��� �Լ�
    public void DeactivateSelf()
    {
        gameObject.SetActive(false);
        Debug.Log("�� �ִϸ��̼� ����: Phase2Object ��Ȱ��ȭ��");
    }
}