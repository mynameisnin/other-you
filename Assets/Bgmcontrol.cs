using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bgmcontrol : MonoBehaviour
{
    public static Bgmcontrol Instance; // �̱��� �ν��Ͻ�
    public AudioSource bgmAudioSource; // ��������� ����� AudioSource
    private const string BGMVolumeKey = "BGMVolume"; // ���� ���� Ű

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
            Destroy(gameObject); // �̹� �ν��Ͻ��� �����ϸ� �ߺ� ������ ����
            return;
        }

        // ����� ���� ���� ������ �ҷ����� (������ �⺻�� 0.5)
        float savedVolume = PlayerPrefs.GetFloat(BGMVolumeKey, 0.5f);
        if (bgmAudioSource != null)
        {
            bgmAudioSource.volume = savedVolume;
        }
    }
}
