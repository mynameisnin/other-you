using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Potal : MonoBehaviour
{
    public string targetScene; //이동할 씬 이름
    private bool isPlayerNear = false; // 플레이어가 포탈 범위 내에 있는지 확인
    private FakeLoadingScreen fakeLoadingScreen; // FakeLoadingScreen 참조

    void Start()
    {
        fakeLoadingScreen = FindObjectOfType<FakeLoadingScreen>(); //FakeLoadingScreen 찾기
    }

    void Update()
    {
        if (isPlayerNear && Input.GetKeyDown(KeyCode.UpArrow))
        {
            if (fakeLoadingScreen != null)
            {
                fakeLoadingScreen.LoadScene(targetScene); // 가짜 로딩 실행
            }
            else
            {
                SceneManager.LoadScene(targetScene); // 만약 FakeLoadingScreen이 없으면 바로 씬 전환
            }
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("PlayerAttack"))
        {
            isPlayerNear = true;
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("PlayerAttack"))
        {
            isPlayerNear = false;
        }
    }
}
