using UnityEngine;
using UnityEngine.UI;

public class MainMusicControl : MonoBehaviour
{
    public Slider bgmSlider; // 볼륨 조절을 위한 슬라이더
    private const string BGMVolumeKey = "BGMVolume"; // PlayerPrefs에 저장할 키

    private void Start()
    {
        // Bgmcontrol 인스턴스가 존재하는지 확인
        if (Bgmcontrol.Instance != null)
        {
            // PlayerPrefs에서 BGM 볼륨 값을 불러오기, 기본값은 0.5f
            float savedVolume = PlayerPrefs.GetFloat(BGMVolumeKey, 0.5f);
            bgmSlider.value = savedVolume; // 슬라이더의 초기 값을 설정

            // BgmSource의 볼륨을 슬라이더의 값으로 설정
            Bgmcontrol.Instance.bgmAudioSource.volume = bgmSlider.value;

            // 슬라이더의 값이 변경될 때 ChangeVolume 메서드를 호출
            bgmSlider.onValueChanged.AddListener(ChangebgmVolume);
        }
    }

    // 슬라이더의 값이 변경될 때 호출되는 메서드
    public void ChangebgmVolume(float value)
    {
        Debug.Log("New Volume: " + value); // 새로운 볼륨 값을 로그에 출력
        if (Bgmcontrol.Instance != null)
        {
            // bgmcontrol의 AudioSource 볼륨을 슬라이더의 값으로 설정
            Bgmcontrol.Instance.bgmAudioSource.volume = value;

            // 현재 볼륨 값을 PlayerPrefs에 저장
            PlayerPrefs.SetFloat(BGMVolumeKey, value);
            PlayerPrefs.Save(); // 변경 사항 저장
        }
    }
}