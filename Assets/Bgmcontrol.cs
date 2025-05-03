using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Bgmcontrol : MonoBehaviour
{
    public static Bgmcontrol Instance; // �̱��� �ν��Ͻ�
    public AudioSource bgmAudioSource; // ���� BGM AudioSource
    public AudioSource subAudioSource; // ���� BGM�� ���� AudioSource
    public AudioClip townBGM;          // ���� ������ ����� BGM

    private const string BGMVolumeKey = "BGMVolume"; // ���� ���� Ű

    private void Awake()
    {
        // �̱��� ����
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

        // ���� �ҷ�����
        float savedVolume = PlayerPrefs.GetFloat(BGMVolumeKey, 0.5f);
        if (bgmAudioSource != null) bgmAudioSource.volume = savedVolume;
        if (subAudioSource != null) subAudioSource.volume = savedVolume;

        SceneManager.sceneLoaded += OnSceneLoaded;

        // ���� BGM ����
        if (!bgmAudioSource.isPlaying) bgmAudioSource.Play();
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        string sceneName = scene.name;

        if (sceneName == "map_village")
        {
            if (bgmAudioSource.isPlaying)
            {
                bgmAudioSource.Pause(); // ���� ���� �Ͻ�����
            }

            if (subAudioSource.clip != townBGM)
            {
                subAudioSource.clip = townBGM;
            }

            if (!subAudioSource.isPlaying)
            {
                subAudioSource.Play(); // ���� ���� ���
            }
        }
        else if (sceneName == "Startscenes" || sceneName == "Storyscenes")
        {
            if (subAudioSource.isPlaying)
            {
                subAudioSource.Stop(); // ���� ���� ����
            }

            if (!bgmAudioSource.isPlaying)
            {
                bgmAudioSource.UnPause(); // ���� ���� �̾ ���
            }
        }
    }
}
