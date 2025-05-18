using UnityEngine;
using UnityEngine.Rendering.Universal; // Light2D 사용
using DG.Tweening; // DOTween 사용

public class StatueInteraction : MonoBehaviour
{
    private Light2D statueLight;
    private bool isActivated = false; // 한 번 붉은색으로 바뀌었는지 확인하는 변수
    private bool isPlayerNearby = false; // 플레이어가 근처에 있는지 확인
    private bool isPanelOpen = false; // 패널이 열려있는지 확인

    public Color blueColor = Color.blue;
    public Color redColor = Color.red;
    public ParticleSystem featherEffect; // 깃털 파티클 시스템

    [Header("UI Settings")]
    public CanvasGroup statPanel;  // 스탯 패널의 CanvasGroup
    public float fadeDuration = 0.5f;  // 패널 페이드 시간

    [Header("Shockwave Settings")]
    public SpriteRenderer shockwaveRenderer; // 충격파 머터리얼을 가지고 있는 오브젝트
    private MaterialPropertyBlock propertyBlock;
    private Material shockwaveMaterial;
    public float shockwaveDuration = 1.5f; // 충격파 지속 시간
    public float maxWaveDistance = 2.0f; // 최대 확산 거리

    [Header("Light Settings")]
    public float lightInnerMax = 7f; // Inner 최대 값
    public float lightInnerDuration = 1.2f; // Inner 값이 변화하는 시간

    void Start()
    {
        statueLight = GetComponent<Light2D>();
        statueLight.color = blueColor; // 초기 색상 설정
        statueLight.pointLightInnerRadius = 0f; // 처음에는 Inner 값이 0

        if (featherEffect != null)
            featherEffect.Stop();

        if (shockwaveRenderer != null)
        {
            propertyBlock = new MaterialPropertyBlock();
            shockwaveMaterial = shockwaveRenderer.material;
            shockwaveRenderer.gameObject.SetActive(false);
        }

        if (statPanel != null)
        {
            statPanel.alpha = 0; // 스탯 패널 기본적으로 안 보이게 설정
            statPanel.gameObject.SetActive(false);
        }
    }

    void Update()
    {
        if (isPlayerNearby && Input.GetKeyDown(KeyCode.UpArrow))
        {
            if (!isActivated)
            {
                ActivateStatue();
            }
            else
            {
                if (isPanelOpen)
                {
                    HideStatPanel(); // 패널이 열려있다면 닫기
                }
                else
                {
                    ShowStatPanel(); // 패널이 닫혀있다면 열기
                }
            }
        }
    }

    private void ActivateStatue()
    {
        statueLight.color = redColor; // 붉은빛으로 변경
        isActivated = true; // 한 번 활성화되면 유지

        if (featherEffect != null)
            featherEffect.Play(); // 깃털 효과 실행

        StartShockwaveEffect(); // 충격파 효과 실행
        StartLightEffect(); // Light2D Inner 값 증가
        ShowStatPanel(); // 스탯 패널 표시

        //  리스폰 위치 갱신
        if (SpawnManager.Instance != null)
            SpawnManager.Instance.spawnPosition = transform.position;

        if (PlayerStats.Instance != null)
        {
            PlayerStats.Instance.Heal(PlayerStats.Instance.maxHealth);
            PlayerStats.Instance.SetCurrentEnergy(PlayerStats.Instance.maxEnergy);
            PlayerStats.Instance.SetCurrentMana(PlayerStats.Instance.maxMana);
        }

        //  데바 전체 회복
        if (DevaStats.Instance != null)
        {
            DevaStats.Instance.Heal(DevaStats.Instance.maxHealth);
            DevaStats.Instance.SetCurrentEnergy(DevaStats.Instance.maxEnergy);
            DevaStats.Instance.SetCurrentMana(DevaStats.Instance.maxMana);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player")||other.CompareTag("DevaPlayer")) // 플레이어가 근처에 왔을 때만 체크
        {
            isPlayerNearby = true;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player")) // 플레이어가 멀어지면
        {
            isPlayerNearby = false;
            HideStatPanel(); // 플레이어가 멀어지면 패널 숨기기
        }
    }

    private void StartShockwaveEffect()
    {
        if (shockwaveRenderer == null || shockwaveMaterial == null)
            return;

        shockwaveRenderer.gameObject.SetActive(true);
        shockwaveRenderer.GetPropertyBlock(propertyBlock);
        propertyBlock.SetFloat("_WaveDistanceFromCenter", -0.1f);
        shockwaveRenderer.SetPropertyBlock(propertyBlock);

        float newShockwaveDuration = 15.0f;
        float newMaxWaveDistance = 4.5f;

        DOTween.To(() => -0.1f, x =>
        {
            shockwaveRenderer.GetPropertyBlock(propertyBlock);
            propertyBlock.SetFloat("_WaveDistanceFromCenter", x);
            shockwaveRenderer.SetPropertyBlock(propertyBlock);
        }, newMaxWaveDistance, newShockwaveDuration)
        .SetEase(Ease.OutSine)
        .OnComplete(() =>
        {
            shockwaveRenderer.gameObject.SetActive(false);
        });
    }

    private void StartLightEffect()
    {
        DOTween.To(() => statueLight.pointLightInnerRadius, x => statueLight.pointLightInnerRadius = x, lightInnerMax, lightInnerDuration)
            .SetEase(Ease.OutQuad);
    }

    private bool isFirstActivation = true; // 첫 활성화 여부 확인

    private void ShowStatPanel()
    {
        if (statPanel == null) return;

        statPanel.gameObject.SetActive(true); // 패널 활성화
        isPanelOpen = true; // 패널이 열려있음을 표시

        if (isFirstActivation)
        {
            isFirstActivation = false; // 첫 실행 이후에는 딜레이 없이 실행되도록 설정
            DOVirtual.DelayedCall(0.9f, () =>
            {
                if (statPanel != null) // 패널이 아직 존재하는지 확인
                {
                    statPanel.DOFade(1, fadeDuration)
                        .SetUpdate(true) // UI 애니메이션이 정지되지 않도록
                        .SetEase(Ease.OutQuad)
                        .OnComplete(() => Time.timeScale = 0f); // 패널이 완전히 뜬 후 게임 멈춤
                }
            });
        }
        else
        {
            statPanel.DOFade(1, fadeDuration)
                .SetUpdate(true)
                .SetEase(Ease.OutQuad)
                .OnComplete(() => Time.timeScale = 0f); // 패널이 완전히 뜬 후 게임 멈춤
        }
    }

    private void HideStatPanel()
    {
        if (statPanel == null) return;

        statPanel.DOFade(0, fadeDuration)
            .SetUpdate(true) // UI 애니메이션이 정지되지 않도록
            .SetEase(Ease.OutQuad)
            .OnComplete(() =>
            {
                statPanel.gameObject.SetActive(false); // 완전히 사라지면 비활성화
                isPanelOpen = false; // 패널이 닫힘을 표시
                Time.timeScale = 1f; // 패널이 닫힌 후 게임 정상 진행
            });
    }
}