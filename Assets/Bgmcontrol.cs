using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bgmcontrol : MonoBehaviour
{
    public static Bgmcontrol Instance; // �̱��� �ν��Ͻ�
    public AudioSource bgmAudioSource; // ��������� ����� AudioSource

    private void Awake()
    {
        // �̱��� ���� ����
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // ���� ��ȯ�Ǿ ������Ʈ�� �ı����� ����
        }
        else
        {
            Destroy(this.gameObject); // �̹� �ν��Ͻ��� �����ϸ� �ߺ� ������ ����
        }
    }
}