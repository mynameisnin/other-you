using UnityEngine;
using UnityEngine.SceneManagement;

public class Potal : MonoBehaviour
{
    public string targetScene; // �̵��� �� �̸�

    private bool isPlayerNear = false; // �÷��̾ ��Ż ���� ���� �ִ��� Ȯ��

    void Update()
    {
        // �÷��̾ ��Ż ���� �ȿ� ���� �� ��(���� ����Ű) �Է� ����
        if (isPlayerNear && Input.GetKeyDown(KeyCode.UpArrow))
        {
            LoadTargetScene();
        }
    }

    void LoadTargetScene()
    {
        if (!string.IsNullOrEmpty(targetScene)) // �� �̸��� �����Ǿ� �ִٸ�
        {
            SceneManager.LoadScene(targetScene); // �ش� ������ �̵�
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("PlayerAttack")) // �÷��̾ ��Ż�� ������
        {
            isPlayerNear = true; // �÷��̾ ��Ż ���� ���� ����
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("PlayerAttack")) // �÷��̾ ��Ż���� �����
        {
            isPlayerNear = false; // �÷��̾ ��Ż ������ ���
        }
    }
}

