using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DownJump : MonoBehaviour
{
    private Collider2D platformCollider;
    private AdamMovement player; // 플레이어 스크립트 참조

    void Start()
    {
        platformCollider = GetComponent<Collider2D>();
        player = FindObjectOfType<AdamMovement>(); // 플레이어 찾기
    }

    void Update()
    {
        if (IsPlayerOnPlatform() && Input.GetKey(KeyCode.DownArrow) && Input.GetKeyDown(KeyCode.Space))
        {
            StartCoroutine(DisableCollision());
        }
    }

    bool IsPlayerOnPlatform()
    {
        return player != null && player.isGround; // 메서드 호출 X, 변수이므로 괄호 제거
    }

    System.Collections.IEnumerator DisableCollision()
    {
        player.isGround = false; // 바닥 체크 해제 (중요!)
        platformCollider.enabled = false; // 충돌 해제 (바닥 통과)

        yield return new WaitForSeconds(0.7f); // 0.3초 후 다시 충돌 활성화

        platformCollider.enabled = true;
    }
}