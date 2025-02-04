using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Potal : MonoBehaviour
{
    public string targetScene; //�̵��� �� �̸�
    private bool isPlayerNear = false; // �÷��̾ ��Ż ���� ���� �ִ��� Ȯ��
    private FakeLoadingScreen fakeLoadingScreen; // FakeLoadingScreen ����

    void Start()
    {
        fakeLoadingScreen = FindObjectOfType<FakeLoadingScreen>(); //FakeLoadingScreen ã��
    }

    void Update()
    {
        if (isPlayerNear && Input.GetKeyDown(KeyCode.UpArrow))
        {
            if (fakeLoadingScreen != null)
            {
                fakeLoadingScreen.LoadScene(targetScene); // ��¥ �ε� ����
            }
            else
            {
                SceneManager.LoadScene(targetScene); // ���� FakeLoadingScreen�� ������ �ٷ� �� ��ȯ
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
