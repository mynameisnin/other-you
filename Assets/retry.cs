using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class retry : MonoBehaviour
{

    public void Retry()
    {
        Time.timeScale = 1f; // Ȥ�� ���� �� �Ͻ����� ������ �ٽ� ��������
        SceneManager.LoadScene(SceneManager.GetActiveScene().name); // ���� �� �ٽ� �ε�
    }
}
