using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class DialogueManager : MonoBehaviour
{
    [SerializeField] private GameObject[] monsters; // 삭제 여부를 체크할 몬스터들
    [SerializeField] private GameObject objectToActivate; // 다이얼로그 오브젝트

    private bool hasActivated = false;

    private void Start()
    {
        objectToActivate.SetActive(false); // 시작할 때 비활성화
    }

    private void Update()
    {
        if (hasActivated) return; // 이미 활성화됐으면 중복 방지

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
            Debug.Log("모든 몬스터가 삭제되었습니다!");
            objectToActivate.SetActive(true);
            hasActivated = true;
        }
    }
}