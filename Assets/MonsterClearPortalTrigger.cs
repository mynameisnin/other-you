using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MonsterClearPortalTrigger : MonoBehaviour
{
    [Header("몬스터 리스트")]
    public List<GameObject> fieldMonsters = new List<GameObject>();

    [Header("포탈")]
    public GameObject portal; // 비활성화 상태로 시작

    [Header("경고 텍스트")]
    public GameObject warningText; // "필드 몬스터가 남아있습니다" 텍스트 오브젝트

    [Header("텍스트 표시 시간")]
    public float warningDisplayTime = 2f;

    private bool playerInRange = false;

    private void Start()
    {
        if (portal != null)
            portal.SetActive(false); // 시작 시 비활성화

        if (warningText != null)
            warningText.SetActive(false);
    }

    private void Update()
    {
        // 몬스터가 모두 제거되었는지 확인
        fieldMonsters.RemoveAll(monster => monster == null);

        if (fieldMonsters.Count == 0 && portal != null && !portal.activeSelf)
        {
            portal.SetActive(true); // 몬스터 클리어 시 포탈 활성화
            Debug.Log("[포탈] 모든 몬스터 처치됨 - 포탈 열림");
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;

            if (fieldMonsters.Count > 0)
            {
                Debug.Log("[포탈] 몬스터가 남아있어 포탈 사용 불가");
                StartCoroutine(ShowWarningText());
            }
        }
    }

    private IEnumerator ShowWarningText()
    {
        if (warningText != null)
        {
            warningText.SetActive(true);
            yield return new WaitForSeconds(warningDisplayTime);
            warningText.SetActive(false);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
        }
    }
}
