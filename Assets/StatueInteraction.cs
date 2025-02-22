using UnityEngine;
using UnityEngine.Rendering.Universal; // Light2D 사용을 위해 필요
using DG.Tweening; // DOTween 사용

public class StatueInteraction : MonoBehaviour
{
    private Light2D statueLight;
    private bool isActivated = false; // 한 번 붉은색으로 바뀌었는지 확인하는 변수
    private bool isPlayerNearby = false; // 플레이어가 근처에 있는지 확인

    public Color blueColor = Color.blue;
    public Color redColor = Color.red;
    public ParticleSystem featherEffect; // 깃털 파티클 시스템

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

            // 충격파 오브젝트 처음에는 안 보이게 설정
            shockwaveRenderer.gameObject.SetActive(false);
        }
    }

    void Update()
    {
        // 플레이어가 근처에 있고 ↑ 키를 누르면 활성화
        if (isPlayerNearby && Input.GetKeyDown(KeyCode.UpArrow) && !isActivated)
        {
            statueLight.color = redColor; // 붉은빛으로 변경
            isActivated = true; // 한 번 활성화되면 유지

            if (featherEffect != null)
            {
                featherEffect.Play(); // 깃털 효과 실행
            }

            StartShockwaveEffect(); // 충격파 효과 실행
            StartLightEffect(); // Light2D Inner 값 증가
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("PlayerAttack")) // 플레이어가 동상 근처에 왔을 때만 체크
        {
            isPlayerNearby = true; // 플레이어가 근처에 있음을 표시
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("PlayerAttack")) // 플레이어가 멀어지면
        {
            isPlayerNearby = false; // 범위에서 벗어남
        }
    }

    private void StartShockwaveEffect()
    {
        if (shockwaveRenderer == null || shockwaveMaterial == null)
            return;

        // 충격파 오브젝트 활성화
        shockwaveRenderer.gameObject.SetActive(true);

        // 머터리얼의 WaveDistanceFromCenter 값을 DOTween을 이용해 서서히 증가
        shockwaveRenderer.GetPropertyBlock(propertyBlock);
        propertyBlock.SetFloat("_WaveDistanceFromCenter", -0.1f); // 초기 값 설정
        shockwaveRenderer.SetPropertyBlock(propertyBlock);

        float newShockwaveDuration = 15.0f; // 충격파 지속 시간 (기존 1.5f → 3.0f로 증가)
        float newMaxWaveDistance = 4.5f; // 최대 확산 거리 (기존 2.0f → 3.5f로 증가)

        DOTween.To(() => -0.1f, x =>
        {
            shockwaveRenderer.GetPropertyBlock(propertyBlock);
            propertyBlock.SetFloat("_WaveDistanceFromCenter", x);
            shockwaveRenderer.SetPropertyBlock(propertyBlock);
        }, newMaxWaveDistance, newShockwaveDuration) // 지속 시간을 늘리고, 확산 거리를 증가
        .SetEase(Ease.OutSine) // 확산이 점점 부드럽게 퍼지도록 설정
        .OnComplete(() =>
        {
            // 효과 종료 후 충격파 오브젝트 비활성화
            shockwaveRenderer.gameObject.SetActive(false);
        });
    }


    private void StartLightEffect()
    {
        // DOTween으로 Light2D의 Inner 값을 서서히 증가
        DOTween.To(() => statueLight.pointLightInnerRadius, x => statueLight.pointLightInnerRadius = x, lightInnerMax, lightInnerDuration)
            .SetEase(Ease.OutQuad);
    }
}
