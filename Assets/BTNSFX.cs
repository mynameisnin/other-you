using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BTNSFX : MonoBehaviour
{
    public AudioSource clickSound1; // 첫 번째 클릭 사운드
    public AudioSource clickSound2; // 두 번째 클릭 사운드

    private void Start()
    {
        float savedVolume = PlayerPrefs.GetFloat("ClickSoundVolume", 0.5f);
        SetClickSoundVolume(savedVolume);
    }

    // 첫 번째 버튼 클릭 시 호출
    public void ClickSound1()
    {
        if (clickSound1 != null)
        {
            clickSound1.Play();
            DontDestroyOnLoad(clickSound1);
        }
    }

    // 두 번째 버튼 클릭 시 호출
    public void ClickSound2()
    {
        if (clickSound2 != null)
        {
            clickSound2.Play();
            DontDestroyOnLoad(clickSound2);
        }
    }

    // 클릭 사운드들의 볼륨을 설정하는 메서드
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
