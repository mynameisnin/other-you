using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlashFlip : MonoBehaviour
{
    public GameObject slashEffect; // 슬래쉬 이펙트 프리팹
    private bool isFacingRight = true; // 캐릭터의 방향을 나타냄

    public void ShowSlashEffect()
    {
        // 방향에 따라 스프라이트를 뒤집음
        Vector3 slashScale = slashEffect.transform.localScale;
        if (isFacingRight)
        {
            slashScale.x = Mathf.Abs(slashScale.x); // 오른쪽 방향
        }
        else
        {
            slashScale.x = -Mathf.Abs(slashScale.x); // 왼쪽 방향
        }
        slashEffect.transform.localScale = slashScale;

        // 임팩트 위치 조정
        Vector3 position = transform.position;
        position.x += isFacingRight ? 1.0f : -1.0f; // 방향에 따라 위치 이동
        slashEffect.transform.position = position;

        slashEffect.SetActive(true); // 활성화
    }
}