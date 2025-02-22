using UnityEngine;
using UnityEngine.Rendering.Universal; // Light2D ����� ���� �ʿ�

public class StatueInteraction : MonoBehaviour
{
    private Light2D statueLight;
    private bool isActivated = false; // �� �� ���������� �ٲ������ Ȯ���ϴ� ����
    private bool isPlayerNearby = false; // �÷��̾ ��ó�� �ִ��� Ȯ��

    public Color blueColor = Color.blue;
    public Color redColor = Color.red;
    public ParticleSystem featherEffect; // ���� ��ƼŬ �ý���

    void Start()
    {
        statueLight = GetComponent<Light2D>();
        statueLight.color = blueColor; // �ʱ� ���� ����

        if (featherEffect != null)
        {
            featherEffect.Stop(); // ������ �� ��ƼŬ ����
        }
    }

    void Update()
    {
        // �÷��̾ ��ó�� �ְ� �� Ű�� ������ Ȱ��ȭ
        if (isPlayerNearby && Input.GetKeyDown(KeyCode.UpArrow) && !isActivated)
        {
            statueLight.color = redColor; // ���������� ����
            isActivated = true; // �� �� Ȱ��ȭ�Ǹ� ����

            if (featherEffect != null)
            {
                featherEffect.Play(); // ���� ȿ�� ����
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("PlayerAttack")) // �÷��̾ ���� ��ó�� ���� ���� üũ
        {
            isPlayerNearby = true; // �÷��̾ ��ó�� ������ ǥ��
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("PlayerAttack")) // �÷��̾ �־�����
        {
            isPlayerNearby = false; // �������� ���
        }
    }
}
