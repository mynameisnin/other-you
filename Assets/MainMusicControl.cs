using UnityEngine;
using UnityEngine.UI;

public class MainMusicControl : MonoBehaviour
{
    public Slider bgmSlider; // 볼륨 조절을 위한 슬라이더
    private const string BGMVolumeKey = "BGMVolume"; // PlayerPrefs에 저장할 키

    private void Start()
    {
        if (Bgmcontrol.Instance != null)
        {
            // 저장된 볼륨 불러오기 (없으면 기본값 0.5)
            float savedVolume = PlayerPrefs.GetFloat(BGMVolumeKey, 0.5f);

            // 슬라이더 값 설정 전에 리스너 추가 (UI 갱신을 위해)
            bgmSlider.onValueChanged.AddListener(ChangebgmVolume);

            // 슬라이더의 초기 값을 설정
            bgmSlider.value = savedVolume;

            // 배경음 볼륨을 설정
            Bgmcontrol.Instance.bgmAudioSource.volume = savedVolume;
        }
    }

    // 슬라이더 값 변경 시 호출되는 메서드
    public void ChangebgmVolume(float value)
    {
        if (Bgmcontrol.Instance != null)
        {
            // 배경음 볼륨 설정
            Bgmcontrol.Instance.bgmAudioSource.volume = value;

            // 볼륨 값 저장
            PlayerPrefs.SetFloat(BGMVolumeKey, value);
            PlayerPrefs.Save();
        }
    }
}
