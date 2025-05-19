using UnityEngine;
using System.Collections;

public class GameStartPause : MonoBehaviour
{
    public float pauseDuration = 2f; // �Ͻ����� �ð� (��)

    void Start()
    {
        StartCoroutine(PauseAtGameStart());
    }

    IEnumerator PauseAtGameStart()
    {
        Time.timeScale = 0f; // ���� �Ͻ�����
        float pauseTime = pauseDuration;

        // Time.unscaledDeltaTime�� ����� ���� �ð� �������� ��ٸ�
        while (pauseTime > 0)
        {
            pauseTime -= Time.unscaledDeltaTime;
            yield return null;
        }

        Time.timeScale = 1f; // ���� �����
    }
}