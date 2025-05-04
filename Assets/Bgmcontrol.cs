using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Bgmcontrol : MonoBehaviour
{
    public static Bgmcontrol Instance; // 싱글톤 인스턴스
    public AudioSource bgmAudioSource; // 메인 BGM AudioSource
    public AudioSource subAudioSource; // 마을 BGM용 서브 AudioSource
    public AudioSource TutorialAudioSource; // 튜토리얼 BGM용 서브 AudioSource
    public AudioSource fightAudioSource; // 챕터1-3 BGM용 서브 AudioSource
    public AudioSource fireAudioSource; // 챕터1-3 불타는소리용 서브 AudioSource
    public AudioClip townBGM;          // 마을 씬에서 재생할 BGM
    public AudioClip fightBGM;          // 챕터1-3 씬에서 재생할 BGM

    private const string BGMVolumeKey = "BGMVolume"; // 볼륨 저장 키

    private void Awake()
    {
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

        float savedVolume = PlayerPrefs.GetFloat(BGMVolumeKey, 0.5f);
        if (bgmAudioSource != null) bgmAudioSource.volume = savedVolume;
        if (subAudioSource != null) subAudioSource.volume = savedVolume;
        if (TutorialAudioSource != null) TutorialAudioSource.volume = savedVolume;
        if (fightAudioSource != null) fightAudioSource.volume = savedVolume;
        if (fireAudioSource != null) fireAudioSource.volume = savedVolume;

        SceneManager.sceneLoaded += OnSceneLoaded;

        if (!bgmAudioSource.isPlaying) bgmAudioSource.Play();
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        string sceneName = scene.name;

        // 마을 씬
        if (sceneName == "map_village" || sceneName == "map_village2")
        {
            if (bgmAudioSource.isPlaying) bgmAudioSource.Pause();
            if (TutorialAudioSource.isPlaying) TutorialAudioSource.Stop();

            if (subAudioSource.clip != townBGM)
            {
                subAudioSource.clip = townBGM;
            }

            if (!subAudioSource.isPlaying)
            {
                subAudioSource.Play();
            }
        }
        else if (sceneName == "map_village3" || sceneName == "map_village3-1")
        {
            if (subAudioSource.isPlaying) subAudioSource.Stop();
        }
        // 메인 화면 / 스토리 씬
        else if (sceneName == "Startscenes" || sceneName == "Storyscenes")
        {
            if (subAudioSource.isPlaying) subAudioSource.Stop();
            if (TutorialAudioSource.isPlaying) TutorialAudioSource.Stop();

            if (!bgmAudioSource.isPlaying)
            {
                bgmAudioSource.UnPause();
            }
        }
        // 튜토리얼 씬
        else if (sceneName == "Chapter1-2" || sceneName == "Chapter1-2 2" || sceneName == "Chapter1-2 1")
        {
            if (bgmAudioSource.isPlaying) bgmAudioSource.Pause();
            if (subAudioSource.isPlaying) subAudioSource.Stop();

            if (!TutorialAudioSource.isPlaying)
            {
                TutorialAudioSource.Play();
            }
        }
        // 챕터1-3 씬
        else if (sceneName == "Chapter1-3" || sceneName == "Chapter1-3 1" || sceneName == "Chapter1-3 2" || sceneName == "Chapter1-3 3")
        {
            if (bgmAudioSource.isPlaying) bgmAudioSource.Pause();
            if (subAudioSource.isPlaying) subAudioSource.Stop();
            if (TutorialAudioSource.isPlaying) TutorialAudioSource.Stop();

            if (!fightAudioSource.isPlaying)
            {
                fightAudioSource.Play();
            }
            else if (!fireAudioSource.isPlaying)
            {
                fireAudioSource.Play();
            }
        }
    }
    public AudioSource GetCurrentBgm()
    {
        if (subAudioSource != null && subAudioSource.isPlaying)
            return subAudioSource;

        if (TutorialAudioSource != null && TutorialAudioSource.isPlaying)
            return TutorialAudioSource;

        if (bgmAudioSource != null && bgmAudioSource.isPlaying)
            return bgmAudioSource;

        if (fightAudioSource != null && fightAudioSource.isPlaying)
            return fightAudioSource;

        if (fireAudioSource != null && fireAudioSource.isPlaying)
            return fireAudioSource;

        return null;
    }
}
