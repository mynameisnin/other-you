using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumpSmokeController : MonoBehaviour
{
    [SerializeField] private GameObject jumpSmokeObject; // 미리 생성된 연기 오브젝트
    [SerializeField] private Transform smokeSpawnPoint; // 연기 위치

    public void ActivateJumpSmoke()
    {
        if (jumpSmokeObject != null && smokeSpawnPoint != null)
        {
            // 연기 오브젝트를 점프 위치로 이동 및 활성화
            jumpSmokeObject.transform.position = smokeSpawnPoint.position;
            jumpSmokeObject.SetActive(true);

            // 일정 시간 후 비활성화
            Invoke("DeactivateJumpSmoke", 1f); // 1초 후 비활성화
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