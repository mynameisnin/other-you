using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Bgmcontrol : MonoBehaviour
{
    public static Bgmcontrol Instance; // 싱글톤 인스턴스
    public AudioSource bgmAudioSource; // 메인 BGM AudioSource
    public AudioSource subAudioSource; // 마을 BGM용 서브 AudioSource
    public AudioClip townBGM;          // 마을 씬에서 재생할 BGM

    private const string BGMVolumeKey = "BGMVolume"; // 볼륨 저장 키

    private void Awake()
    {
        // 싱글톤 설정
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        // 볼륨 불러오기
        float savedVolume = PlayerPrefs.GetFloat(BGMVolumeKey, 0.5f);
        if (bgmAudioSource != null) bgmAudioSource.volume = savedVolume;
        if (subAudioSource != null) subAudioSource.volume = savedVolume;

        SceneManager.sceneLoaded += OnSceneLoaded;

        // 메인 BGM 시작
        if (!bgmAudioSource.isPlaying) bgmAudioSource.Play();
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        string sceneName = scene.name;

        if (sceneName == "map_village")
        {
            if (bgmAudioSource.isPlaying)
            {
                bgmAudioSource.Pause(); // 메인 음악 일시정지
            }

            if (subAudioSource.clip != townBGM)
            {
                subAudioSource.clip = townBGM;
            }

            if (!subAudioSource.isPlaying)
            {
                subAudioSource.Play(); // 마을 음악 재생
            }
        }
        else if (sceneName == "Startscenes" || sceneName == "Storyscenes")
        {
            if (subAudioSource.isPlaying)
            {
                subAudioSource.Stop(); // 마을 음악 정지
            }

            if (!bgmAudioSource.isPlaying)
            {
                bgmAudioSource.UnPause(); // 메인 음악 이어서 재생
            }
        }
    }
}
