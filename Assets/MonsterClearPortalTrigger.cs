using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MonsterClearPortalTrigger : MonoBehaviour
{
    [Header("���� ����Ʈ")]
    public List<GameObject> fieldMonsters = new List<GameObject>();

    [Header("��Ż")]
    public GameObject portal; // ��Ȱ��ȭ ���·� ����

    [Header("��� �ؽ�Ʈ")]
    public GameObject warningText; // "�ʵ� ���Ͱ� �����ֽ��ϴ�" �ؽ�Ʈ ������Ʈ

    [Header("�ؽ�Ʈ ǥ�� �ð�")]
    public float warningDisplayTime = 2f;

    private bool playerInRange = false;

    private void Start()
    {
        if (portal != null)
            portal.SetActive(false); // ���� �� ��Ȱ��ȭ

        if (warningText != null)
            warningText.SetActive(false);
    }

    private void Update()
    {
        // ���Ͱ� ��� ���ŵǾ����� Ȯ��
        fieldMonsters.RemoveAll(monster => monster == null);

        if (fieldMonsters.Count == 0 && portal != null && !portal.activeSelf)
        {
            portal.SetActive(true); // ���� Ŭ���� �� ��Ż Ȱ��ȭ
            Debug.Log("[��Ż] ��� ���� óġ�� - ��Ż ����");
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;

            if (fieldMonsters.Count > 0)
            {
                Debug.Log("[��Ż] ���Ͱ� �����־� ��Ż ��� �Ұ�");
                StartCoroutine(ShowWarningText());
            }
        }
    }

    private IEnumerator ShowWarningText()
    {
        if (warningText != null)
        {
            warningText.SetActive(true);
            yield return new WaitForSeconds(warningDisplayTime);
            warningText.SetActive(false);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
        }
    }
}
