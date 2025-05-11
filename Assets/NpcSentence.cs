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
            // 채팅창 생성
            GameObject go = Instantiate(chatBoxPrefab);
            ChatSyetem chat = go.GetComponent<ChatSyetem>();

            // 모든 문장 출력 (2초 간격)
            foreach (string sentence in sentences)
            {
                chat.Ondialogue(new string[] { sentence }, chatTr);
                yield return new WaitForSeconds(2f); // 각 문장 사이 대기
            }

            yield return new WaitForSeconds(2f); // 마지막 문장 후 약간 대기
        }
    }
}
