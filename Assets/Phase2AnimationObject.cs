using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Phase2AnimationObject : MonoBehaviour
{
    // 애니메이션 끝에 호출될 함수
    public void DeactivateSelf()
    {
        gameObject.SetActive(false);
        Debug.Log("▶ 애니메이션 종료: Phase2Object 비활성화됨");
    }
}