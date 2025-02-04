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
        Invoke("TalkNpc", 5f);
    }

    // Update is called once per frame
    public void TalkNpc()
    {
        GameObject go = Instantiate(chatBoxPrefab);
        go.GetComponent<ChatSyetem>().Ondialogue(sentences,chatTr);
        Invoke("TalkNpc", 5f);
    }
}
