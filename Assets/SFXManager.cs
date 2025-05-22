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
    Parry,
    Swich,
    hit,
    hit1,
    SkeletonAttack,
    SkeletonAttack2,
    AngryGodAttack,
    AngryGodFrame,
    AngryGodmMteor,
    AngryGodMeteorFalling,
    AngryGodActive,
    AngryGodDash,
    AngryGodEvasion,
    AngryGodSpawnSkill,
    AngryGodAngryGodPase2,
    AngryGodAngryGodUltimateSkill,
    Death
}

[System.Serializable]
public class SFXEntry
{
    public SFXType type;
    public AudioSource Source; // AudioClip이 아닌 AudioSource
}

public class SFXManager : MonoBehaviour
{
    public static SFXManager Instance { get; private set; }

    [SerializeField] private List<SFXEntry> sfxEntries;
    [SerializeField] private int poolSize = 5;
    [SerializeField] private float volume = 1f;

    [SerializeField] private UnityEngine.Audio.AudioMixerGroup sfxMixerGroup; // ✅ 오디오 믹서 그룹

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

#if UNITY_EDITOR
    [ContextMenu("자동으로 SFX 항목 채우기")]
    private void AutoFillSFXEntries()
    {
        foreach (SFXType type in System.Enum.GetValues(typeof(SFXType)))
        {
            if (!sfxEntries.Exists(e => e.type == type))
            {
                sfxEntries.Add(new SFXEntry { type = type });
            }
        }
    }
#endif

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

            // ✅ 오디오 믹서 그룹 연결
            source.outputAudioMixerGroup = sfxMixerGroup;

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

            // Death 사운드가 재생될 경우 모든 BGM 정지
            if (type == SFXType.Death)
            {
                StopAllBGMs();
            }

            AudioSource pooledSource = audioPool[currentIndex];
            pooledSource.clip = sourceClip.clip;
            pooledSource.volume = sourceClip.volume;
            pooledSource.pitch = sourceClip.pitch;
            pooledSource.loop = false; // SFX는 기본적으로 반복하지 않음
            pooledSource.Play();

            currentIndex = (currentIndex + 1) % poolSize;
        }
        else
        {
            Debug.LogWarning($"SFXType {type} not found in SFXManager.");
        }
    }

    // Death SFX 시 모든 BGM 정지용 함수
    private void StopAllBGMs()
    {
        if (Bgmcontrol.Instance == null) return;

        AudioSource[] allBGMs = new AudioSource[]
        {
            Bgmcontrol.Instance.bgmAudioSource,
            Bgmcontrol.Instance.subAudioSource,
            Bgmcontrol.Instance.TutorialAudioSource,
            Bgmcontrol.Instance.fightAudioSource,
            Bgmcontrol.Instance.fireAudioSource,
            Bgmcontrol.Instance.DungeonAudioSource,
            Bgmcontrol.Instance.BossAudioSource
        };

        foreach (AudioSource bgm in allBGMs)
        {
            if (bgm != null && bgm.isPlaying)
            {
                bgm.Stop(); // 또는 .Pause()로 일시정지도 가능
            }
        }
    }
}
