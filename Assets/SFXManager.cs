using UnityEngine;
using System.Collections.Generic;

public enum SFXType
{
    SwordSwing,
    SwordSwing2,
    SwordHit,
    Jump,
    Dash,
    FootStep,
    Explosion,
    Teleport,
    NomalAttackSFX,
    DevaskillAttacSFX,
    SkillBigerLaser,
    BuffSkill,
    BladeSkill,
    AdamJump,
}

[System.Serializable]
public class SFXEntry
{
    public SFXType type;
    public AudioSource Source; // AudioClip¿Ã æ∆¥— AudioSource
}

public class SFXManager : MonoBehaviour
{
    public static SFXManager Instance { get; private set; }

    [SerializeField] private List<SFXEntry> sfxEntries;
    [SerializeField] private int poolSize = 5;
    [SerializeField] private float volume = 1f;

    private Dictionary<SFXType, AudioSource> sfxDict;
    private AudioSource[] audioPool;
    private int currentIndex = 0;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            InitSFXDict();
            InitAudioPool();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void InitSFXDict()
    {
        sfxDict = new Dictionary<SFXType, AudioSource>();
        foreach (var entry in sfxEntries)
        {
            if (!sfxDict.ContainsKey(entry.type))
                sfxDict[entry.type] = entry.Source;
        }
    }

    private void InitAudioPool()
    {
        audioPool = new AudioSource[poolSize];
        for (int i = 0; i < poolSize; i++)
        {
            GameObject sourceObj = new GameObject("SFXAudio_" + i);
            sourceObj.transform.SetParent(this.transform);
            AudioSource source = sourceObj.AddComponent<AudioSource>();
            source.playOnAwake = false;
            source.volume = volume;
            audioPool[i] = source;
        }
    }

    public void Play(SFXType type)
    {
        if (sfxDict.TryGetValue(type, out AudioSource sourceClip))
        {
            if (sourceClip.clip == null)
            {
                Debug.LogWarning($"SFXType {type} has no assigned AudioClip.");
                return;
            }

            AudioSource pooledSource = audioPool[currentIndex];
            pooledSource.clip = sourceClip.clip;
            pooledSource.volume = sourceClip.volume;
            pooledSource.pitch = sourceClip.pitch;
            pooledSource.Play();

            currentIndex = (currentIndex + 1) % poolSize;
        }
        else
        {
            Debug.LogWarning($"SFXType {type} not found in SFXManager.");
        }
    }
}
