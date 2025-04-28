using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BTNSFX : MonoBehaviour
{
    public AudioSource clickSound1; // ù ��° Ŭ�� ����
    public AudioSource clickSound2; // �� ��° Ŭ�� ����

    private void Start()
    {
        float savedVolume = PlayerPrefs.GetFloat("ClickSoundVolume", 0.5f);
        SetClickSoundVolume(savedVolume);
    }

    // ù ��° ��ư Ŭ�� �� ȣ��
    public void ClickSound1()
    {
        if (clickSound1 != null)
        {
            clickSound1.Play();
            DontDestroyOnLoad(clickSound1);
        }
    }

    // �� ��° ��ư Ŭ�� �� ȣ��
    public void ClickSound2()
    {
        if (clickSound2 != null)
        {
            clickSound2.Play();
            DontDestroyOnLoad(clickSound2);
        }
    }

    // Ŭ�� ������� ������ �����ϴ� �޼���
    public void SetClickSoundVolume(float volume)
    {
        if (clickSound1 != null)
        {
            clickSound1.volume = volume;
            DontDestroyOnLoad(clickSound1);
        }

        if (clickSound2 != null)
        {
            clickSound2.volume = volume;
            DontDestroyOnLoad(clickSound2);
        }

        PlayerPrefs.SetFloat("ClickSoundVolume", volume);
        PlayerPrefs.Save();
    }
}
