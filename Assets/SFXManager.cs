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
    public AudioClip clip;
}

public class SFXManager : MonoBehaviour
{
    public static SFXManager Instance { get; private set; }

    [SerializeField] private List<SFXEntry> sfxEntries;
    [SerializeField] private int poolSize = 5; // AudioSource °³¼ö
    [SerializeField] private float volume = 1f;

    private Dictionary<SFXType, AudioClip> sfxDict;
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
        sfxDict = new Dictionary<SFXType, AudioClip>();
        foreach (var entry in sfxEntries)
        {
            if (!sfxDict.ContainsKey(entry.type))
                sfxDict[entry.type] = entry.clip;
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
        if (sfxDict.TryGetValue(type, out AudioClip clip))
        {
            AudioSource source = audioPool[currentIndex];
            source.PlayOneShot(clip);
            currentIndex = (currentIndex + 1) % poolSize;
        }
    }
}
