using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenueLoad : MonoBehaviour
{
    public void LoadNextScene()
    {
        //  PlayerManager ����
        if (PlayerManager.Instance != null)
        {
            Destroy(PlayerManager.Instance.gameObject);
            PlayerManager.Instance = null;
        }
        else
        {
            //  Adam ĳ���� ����
            GameObject adam = GameObject.Find("Adam");
            if (adam != null)
            {
                Destroy(adam);
            }
        }

        //  ���� Ÿ��Ʋ ������ �̵�
        SceneManager.LoadScene("startscenes");
    }
}
