using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Bgmcontrol : MonoBehaviour
{
    public static Bgmcontrol Instance; // �̱��� �ν��Ͻ�
    public AudioSource bgmAudioSource; // ���� BGM AudioSource
    public AudioSource subAudioSource; // ���� BGM�� ���� AudioSource
    public AudioSource TutorialAudioSource; // Ʃ�丮�� BGM�� ���� AudioSource
    public AudioSource fightAudioSource; // é��1-3 BGM�� ���� AudioSource
    public AudioSource fireAudioSource; // é��1-3 ��Ÿ�¼Ҹ��� ���� AudioSource
    public AudioSource DungeonAudioSource; // ���� BGM�� ���� AudioSource
    public AudioSource BossAudioSource; // ������ ���� AudioSource
    public AudioClip townBGM;          // ���� ������ ����� BGM
    public AudioClip fightBGM;          // é��1-3 ������ ����� BGM
    public AudioClip DungeonBGM;          // ���� ������ ����� BGM
    public AudioClip BossBgm;          // ���� ������ ����� BGM


    private const string BGMVolumeKey = "BGMVolume"; // ���� ���� Ű

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
        if (DungeonAudioSource != null) DungeonAudioSource.volume = savedVolume;
        if (BossAudioSource != null) BossAudioSource.volume = savedVolume;

        SceneManager.sceneLoaded += OnSceneLoaded;

        if (!bgmAudioSource.isPlaying) bgmAudioSource.Play();
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        string sceneName = scene.name;

        // ���� ��
        if (sceneName == "map_village" || sceneName == "map_village2")
        {
            if (bgmAudioSource.isPlaying) bgmAudioSource.Pause();
            if (TutorialAudioSource.isPlaying) TutorialAudioSource.Stop();
            if (fightAudioSource.isPlaying) TutorialAudioSource.Stop();
            if (fireAudioSource.isPlaying) TutorialAudioSource.Stop();
            if (DungeonAudioSource.isPlaying) TutorialAudioSource.Stop();
            if (BossAudioSource.isPlaying) BossAudioSource.Stop();

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
        // ���� ȭ�� / ���丮 ��
        else if (sceneName == "startscenes" || sceneName == "Storyscenes")
        {
            if (subAudioSource.isPlaying) subAudioSource.Stop();
            if (TutorialAudioSource.isPlaying) TutorialAudioSource.Stop();
            if (fightAudioSource.isPlaying) TutorialAudioSource.Stop();
            if (fireAudioSource.isPlaying) TutorialAudioSource.Stop();
            if (DungeonAudioSource.isPlaying) TutorialAudioSource.Stop();
            if (BossAudioSource.isPlaying) BossAudioSource.Stop();

            if (!bgmAudioSource.isPlaying)
            {
                bgmAudioSource.UnPause();
            }
        }
        // Ʃ�丮�� ��
        else if (sceneName == "Chapter1-2" || sceneName == "Chapter1-2 2" || sceneName == "Chapter1-2 1")
        {
            if (bgmAudioSource.isPlaying) bgmAudioSource.Pause();
            if (subAudioSource.isPlaying) subAudioSource.Stop();
            if (fightAudioSource.isPlaying) TutorialAudioSource.Stop();
            if (fireAudioSource.isPlaying) TutorialAudioSource.Stop();
            if (DungeonAudioSource.isPlaying) TutorialAudioSource.Stop();
            if (BossAudioSource.isPlaying) BossAudioSource.Stop();

            if (!TutorialAudioSource.isPlaying)
            {
                TutorialAudioSource.Play();
            }
        }
        // é��1-3 ��
        else if (sceneName == "Chapter1-3" || sceneName == "Chapter1-3 1" || sceneName == "Chapter1-3 2" || sceneName == "Chapter1-3 3")
        {
            if (bgmAudioSource.isPlaying) bgmAudioSource.Pause();
            if (subAudioSource.isPlaying) subAudioSource.Stop();
            if (TutorialAudioSource.isPlaying) TutorialAudioSource.Stop();
            if (DungeonAudioSource.isPlaying) TutorialAudioSource.Stop();
            if (BossAudioSource.isPlaying) BossAudioSource.Stop();

            if (!fightAudioSource.isPlaying)
            {
                fightAudioSource.Play();
            }
            else if (!fireAudioSource.isPlaying)
            {
                fireAudioSource.Play();
            }
        }
        // ���� ��
        else if (sceneName == "Chapter2" || sceneName == "Chapter2-1" || sceneName == "Chapter 2 Time Line")
        {
            if (bgmAudioSource.isPlaying) bgmAudioSource.Pause();
            if (subAudioSource.isPlaying) subAudioSource.Stop();
            if (TutorialAudioSource.isPlaying) TutorialAudioSource.Stop();
            if (fightAudioSource.isPlaying) fightAudioSource.Stop();
            if (fireAudioSource.isPlaying) fireAudioSource.Stop();
            if (BossAudioSource.isPlaying) BossAudioSource.Stop();

            if (!DungeonAudioSource.isPlaying)
            {
                DungeonAudioSource.Play();
            }
        }
        // ���� ������
        else if (sceneName == "FinalChapter" || sceneName =="FinalChapterHard")
        {
            if (bgmAudioSource.isPlaying) bgmAudioSource.Pause();
            if (subAudioSource.isPlaying) subAudioSource.Stop();
            if (TutorialAudioSource.isPlaying) TutorialAudioSource.Stop();
            if (fightAudioSource.isPlaying) fightAudioSource.Stop();
            if (fireAudioSource.isPlaying) fireAudioSource.Stop();
            if (DungeonAudioSource.isPlaying) DungeonAudioSource.Stop();

            if (!BossAudioSource.isPlaying)
            {
                BossAudioSource.clip = BossBgm;
                BossAudioSource.Play();
            }
            //���������� ��� ����
            else if (sceneName == "BossAdamMeetTimeLine" || sceneName == "end" || sceneName == "BossPractice" || sceneName == "BossPracticeHard"|| sceneName == "BossAdamMeetTimeLineHard")
            {
                if (bgmAudioSource.isPlaying) bgmAudioSource.Pause();
                if (subAudioSource.isPlaying) subAudioSource.Stop();
                if (TutorialAudioSource.isPlaying) TutorialAudioSource.Stop();
                if (fightAudioSource.isPlaying) fightAudioSource.Stop();
                if (fireAudioSource.isPlaying) fireAudioSource.Stop();
                if (DungeonAudioSource.isPlaying) DungeonAudioSource.Stop();
                if (BossAudioSource.isPlaying) BossAudioSource.Stop();
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

        if (DungeonAudioSource != null && DungeonAudioSource.isPlaying)
            return DungeonAudioSource;

        if (BossAudioSource != null && BossAudioSource.isPlaying)
            return BossAudioSource;

        return null;
    }
}
