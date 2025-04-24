using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    public void LoadMainMenu()
    {
        //  PlayerManager�� �����ϸ� ����
        if (PlayerManager.Instance != null)
        {
            Destroy(PlayerManager.Instance.gameObject);
            PlayerManager.Instance = null; // �̱��� �ν��Ͻ��� �ʱ�ȭ
        }

        //  ���� �޴� ������ �̵�
        SceneManager.LoadScene("startscenes"); //  ���� �޴� �� �̸��� �°� ����
    }

}
