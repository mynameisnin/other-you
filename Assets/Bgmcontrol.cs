using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bgmcontrol : MonoBehaviour
{
    public static Bgmcontrol Instance; // 싱글톤 인스턴스
    public AudioSource bgmAudioSource; // 배경음악을 재생할 AudioSource
    private const string BGMVolumeKey = "BGMVolume"; // 볼륨 저장 키

    private void Awake()
    {
        // 싱글톤 패턴 설정
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // 씬이 전환되어도 오브젝트가 파괴되지 않음
        }
        else
        {
            Destroy(gameObject); // 이미 인스턴스가 존재하면 중복 생성을 방지
            return;
        }

        // 저장된 볼륨 값이 있으면 불러오기 (없으면 기본값 0.5)
        float savedVolume = PlayerPrefs.GetFloat(BGMVolumeKey, 0.5f);
        if (bgmAudioSource != null)
        {
            bgmAudioSource.volume = savedVolume;
        }
    }
}
