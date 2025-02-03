using UnityEngine;
using UnityEngine.SceneManagement;

public class Potal : MonoBehaviour
{
    public string targetScene; // 이동할 씬 이름

    private bool isPlayerNear = false; // 플레이어가 포탈 범위 내에 있는지 확인

    void Update()
    {
        // 플레이어가 포탈 범위 안에 있을 때 ↑(위쪽 방향키) 입력 감지
        if (isPlayerNear && Input.GetKeyDown(KeyCode.UpArrow))
        {
            LoadTargetScene();
        }
    }

    void LoadTargetScene()
    {
        if (!string.IsNullOrEmpty(targetScene)) // 씬 이름이 설정되어 있다면
        {
            SceneManager.LoadScene(targetScene); // 해당 씬으로 이동
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("PlayerAttack")) // 플레이어가 포탈에 들어오면
        {
            isPlayerNear = true; // 플레이어가 포탈 범위 내에 있음
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("PlayerAttack")) // 플레이어가 포탈에서 벗어나면
        {
            isPlayerNear = false; // 플레이어가 포탈 범위를 벗어남
        }
    }
}

