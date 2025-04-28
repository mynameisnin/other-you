using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SfxControl : MonoBehaviour
{
    public Slider volumeSlider; // 볼륨 조절을 위한 슬라이더
    public BTNSFX btnSound; // BTNSFX 인스턴스
    private const string VolumeKey = "SfxVolume"; // PlayerPrefs에 저장할 키

    private void Start()
    {
        // PlayerPrefs에서 볼륨 값을 불러오기
        // 불러올 수 없는 경우 기본값 0.5f로 설정
        float savedVolume = PlayerPrefs.GetFloat(VolumeKey, 0.5f);

        // 슬라이더의 초기 값을 저장된 볼륨 값으로 설정
        volumeSlider.value = savedVolume;

        // 클릭 사운드의 초기 볼륨을 설정
        btnSound.SetClickSoundVolume(savedVolume);

        // 슬라이더의 값이 변경될 때 SetVolume 메서드를 호출하도록 설정
        volumeSlider.onValueChanged.AddListener(SetVolume);
    }

    // 슬라이더의 값이 변경될 때 호출되는 메서드
    private void SetVolume(float value)
    {
        Debug.Log("SFX슬라이더 값: " + value); // 슬라이더의 현재 값을 로그에 출력

        // 클릭 사운드의 볼륨을 슬라이더의 값으로 설정
        btnSound.SetClickSoundVolume(value);

        // 현재 볼륨 값을 PlayerPrefs에 저장
        PlayerPrefs.SetFloat(VolumeKey, value);

        // 변경된 PlayerPrefs 값을 저장
        PlayerPrefs.Save();
    }
}
