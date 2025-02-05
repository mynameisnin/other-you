using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Potal : MonoBehaviour
{
    public List<Collider2D> npcColliders; // 모든 NPC의 콜라이더 리스트 (Inspector에서 NPC 추가)
    public string targetScene; // 이동할 씬 이름
    private bool isPlayerNear = false; // 플레이어가 포탈 범위 내에 있는지 확인
    private FakeLoadingScreen fakeLoadingScreen; // FakeLoadingScreen 참조

    private bool allNpcTalked = false; // 모든 대화 완료 여부 체크 
    public GameObject warningPanel; // 경고메시지 출력하는 캔버스 
    private CanvasGroup warningCanvasGroup; // CanvasGroup 참조
    private bool isWarningActive = false; // 경고 캔버스 활성화 여부 (연타 방지)

    void Start()
    {
        fakeLoadingScreen = FindObjectOfType<FakeLoadingScreen>(); // FakeLoadingScreen 찾기

        if (warningPanel != null)
        {
            warningCanvasGroup = warningPanel.GetComponent<CanvasGroup>();

            if (warningCanvasGroup == null)
            {
                warningCanvasGroup = warningPanel.AddComponent<CanvasGroup>(); // 없으면 추가
            }

            warningCanvasGroup.alpha = 0f; // 처음엔 보이지 않게 설정
            warningPanel.SetActive(false);
        }
    }

    void Update()
    {
        CheckAllNpcTalked(); // NPC 대화 완료 여부 체크

        // 플레이어가 포탈 범위 안에 있고, 위쪽 방향키를 눌렀을 때
        if (isPlayerNear && Input.GetKeyDown(KeyCode.UpArrow))
        {
            if (allNpcTalked)
            {
                if (fakeLoadingScreen != null)
                {
                    fakeLoadingScreen.LoadScene(targetScene); // 가짜 로딩 실행
                    Debug.Log("가짜 로딩 실행 중...");
                }
                else
                {
                    SceneManager.LoadScene(targetScene); // 가짜 로딩이 없으면 바로 씬 전환
                    Debug.Log("바로 씬 전환!");
                }
            }
            else
            {
                if (!isWarningActive) // 경고 캔버스가 이미 활성화된 경우 중복 호출 방지
                {
                    ShowWarningCanvas();
                }
            }
        }
    }

    void CheckAllNpcTalked()
    {
        foreach (Collider2D npc in npcColliders)
        {
            if (npc.enabled) // 하나라도 활성화된 콜라이더가 있으면 아직 대화 안 한 NPC가 있음
            {
                allNpcTalked = false;
                return;
            }
        }

        // 모든 NPC의 콜라이더가 비활성화되었을 때만 true
        allNpcTalked = true;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("PlayerAttack"))
        {
            isPlayerNear = true; // 플레이어가 포탈 범위 내에 있음을 체크
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("PlayerAttack"))
        {
            isPlayerNear = false; //  포탈 범위를 벗어나면 false
        }
    }

    void ShowWarningCanvas()
    {
        if (warningPanel != null)
        {
            isWarningActive = true; // 경고 캔버스 활성화 (연타 방지)
            warningPanel.SetActive(true);
            StartCoroutine(FadeCanvasGroup(warningCanvasGroup, 0f, 1f, 0.5f)); // 0.5초 동안 나타나기
            Debug.Log("경고 캔버스 표시");
            StartCoroutine(HideWarningCanvas()); // 2초 후 자동으로 숨김
        }
    }

    IEnumerator HideWarningCanvas()
    {
        yield return new WaitForSeconds(2f);
        StartCoroutine(FadeCanvasGroup(warningCanvasGroup, 1f, 0f, 0.5f)); // 0.5초 동안 사라지기
        yield return new WaitForSeconds(0.5f);
        warningPanel.SetActive(false); // 완전히 사라진 후 비활성화
        isWarningActive = false; // 경고 캔버스 종료 후 다시 호출 가능
    }

    IEnumerator FadeCanvasGroup(CanvasGroup canvasGroup, float startAlpha, float endAlpha, float duration)
    {
        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(startAlpha, endAlpha, elapsed / duration);
            yield return null;
        }
        canvasGroup.alpha = endAlpha;
    }
}
