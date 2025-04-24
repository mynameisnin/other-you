using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    public void LoadMainMenu()
    {
        //  PlayerManager가 존재하면 삭제
        if (PlayerManager.Instance != null)
        {
            Destroy(PlayerManager.Instance.gameObject);
            PlayerManager.Instance = null; // 싱글톤 인스턴스도 초기화
        }

        //  메인 메뉴 씬으로 이동
        SceneManager.LoadScene("startscenes"); //  메인 메뉴 씬 이름에 맞게 수정
    }

}
