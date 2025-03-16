using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyActivator : MonoBehaviour
{
    private void Awake()
    {
        gameObject.SetActive(false); // 처음에는 비활성화
    }

    // 타임라인의 Signal Receiver에서 이 함수를 실행
    public void Activate()
    {
        gameObject.SetActive(true);
        Debug.Log("[EnemyActivator] 적 활성화됨!");
    }
}