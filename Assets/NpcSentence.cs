using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NpcSentence : MonoBehaviour
{
    public string[] sentences;
    public Transform chatTr;
    public GameObject chatBoxPrefab;

    void Start()
    {
        StartCoroutine(TalkNpcCoroutine());
    }

    IEnumerator TalkNpcCoroutine()
    {
        while (true)
        {
            // ä��â ����
            GameObject go = Instantiate(chatBoxPrefab);
            ChatSyetem chat = go.GetComponent<ChatSyetem>();

            // ��� ���� ��� (2�� ����)
            foreach (string sentence in sentences)
            {
                chat.Ondialogue(new string[] { sentence }, chatTr);
                yield return new WaitForSeconds(2f); // �� ���� ���� ���
            }

            yield return new WaitForSeconds(2f); // ������ ���� �� �ణ ���
        }
    }
}
