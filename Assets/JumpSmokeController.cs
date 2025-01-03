using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumpSmokeController : MonoBehaviour
{
    [SerializeField] private GameObject jumpSmokeObject; // �̸� ������ ���� ������Ʈ
    [SerializeField] private Transform smokeSpawnPoint; // ���� ��ġ

    public void ActivateJumpSmoke()
    {
        if (jumpSmokeObject != null && smokeSpawnPoint != null)
        {
            // ���� ������Ʈ�� ���� ��ġ�� �̵� �� Ȱ��ȭ
            jumpSmokeObject.transform.position = smokeSpawnPoint.position;
            jumpSmokeObject.SetActive(true);

            // ���� �ð� �� ��Ȱ��ȭ
            Invoke("DeactivateJumpSmoke", 1f); // 1�� �� ��Ȱ��ȭ
        }
    }

    private void DeactivateJumpSmoke()
    {
        if (jumpSmokeObject != null)
        {
            jumpSmokeObject.SetActive(false);
        }
    }
}