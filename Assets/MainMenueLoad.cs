using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenueLoad : MonoBehaviour
{
    public void LoadNextScene()
    {
        //  PlayerManager 삭제
        if (PlayerManager.Instance != null)
        {
            Destroy(PlayerManager.Instance.gameObject);
            PlayerManager.Instance = null;
        }
        else
        {
            //  Adam 캐릭터 삭제
            GameObject adam = GameObject.Find("Adam");
            if (adam != null)
            {
                Destroy(adam);
            }
        }

        //  메인 타이틀 씬으로 이동
        SceneManager.LoadScene("startscenes");
    }
}
