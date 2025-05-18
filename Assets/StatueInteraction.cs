using UnityEngine;
using UnityEngine.Rendering.Universal; // Light2D ���
using DG.Tweening; // DOTween ���

public class StatueInteraction : MonoBehaviour
{
    private Light2D statueLight;
    private bool isActivated = false; // �� �� ���������� �ٲ������ Ȯ���ϴ� ����
    private bool isPlayerNearby = false; // �÷��̾ ��ó�� �ִ��� Ȯ��
    private bool isPanelOpen = false; // �г��� �����ִ��� Ȯ��

    public Color blueColor = Color.blue;
    public Color redColor = Color.red;
    public ParticleSystem featherEffect; // ���� ��ƼŬ �ý���

    [Header("UI Settings")]
    public CanvasGroup statPanel;  // ���� �г��� CanvasGroup
    public float fadeDuration = 0.5f;  // �г� ���̵� �ð�

    [Header("Shockwave Settings")]
    public SpriteRenderer shockwaveRenderer; // ����� ���͸����� ������ �ִ� ������Ʈ
    private MaterialPropertyBlock propertyBlock;
    private Material shockwaveMaterial;
    public float shockwaveDuration = 1.5f; // ����� ���� �ð�
    public float maxWaveDistance = 2.0f; // �ִ� Ȯ�� �Ÿ�

    [Header("Light Settings")]
    public float lightInnerMax = 7f; // Inner �ִ� ��
    public float lightInnerDuration = 1.2f; // Inner ���� ��ȭ�ϴ� �ð�

    void Start()
    {
        statueLight = GetComponent<Light2D>();
        statueLight.color = blueColor; // �ʱ� ���� ����
        statueLight.pointLightInnerRadius = 0f; // ó������ Inner ���� 0

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
            statPanel.alpha = 0; // ���� �г� �⺻������ �� ���̰� ����
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
                    HideStatPanel(); // �г��� �����ִٸ� �ݱ�
                }
                else
                {
                    ShowStatPanel(); // �г��� �����ִٸ� ����
                }
            }
        }
    }

    private void ActivateStatue()
    {
        statueLight.color = redColor; // ���������� ����
        isActivated = true; // �� �� Ȱ��ȭ�Ǹ� ����

        if (featherEffect != null)
            featherEffect.Play(); // ���� ȿ�� ����

        StartShockwaveEffect(); // ����� ȿ�� ����
        StartLightEffect(); // Light2D Inner �� ����
        ShowStatPanel(); // ���� �г� ǥ��

        //  ������ ��ġ ����
        if (SpawnManager.Instance != null)
            SpawnManager.Instance.spawnPosition = transform.position;

        if (PlayerStats.Instance != null)
        {
            PlayerStats.Instance.Heal(PlayerStats.Instance.maxHealth);
            PlayerStats.Instance.SetCurrentEnergy(PlayerStats.Instance.maxEnergy);
            PlayerStats.Instance.SetCurrentMana(PlayerStats.Instance.maxMana);
        }

        //  ���� ��ü ȸ��
        if (DevaStats.Instance != null)
        {
            DevaStats.Instance.Heal(DevaStats.Instance.maxHealth);
            DevaStats.Instance.SetCurrentEnergy(DevaStats.Instance.maxEnergy);
            DevaStats.Instance.SetCurrentMana(DevaStats.Instance.maxMana);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player")||other.CompareTag("DevaPlayer")) // �÷��̾ ��ó�� ���� ���� üũ
        {
            isPlayerNearby = true;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player")) // �÷��̾ �־�����
        {
            isPlayerNearby = false;
            HideStatPanel(); // �÷��̾ �־����� �г� �����
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

    private bool isFirstActivation = true; // ù Ȱ��ȭ ���� Ȯ��

    private void ShowStatPanel()
    {
        if (statPanel == null) return;

        statPanel.gameObject.SetActive(true); // �г� Ȱ��ȭ
        isPanelOpen = true; // �г��� ���������� ǥ��

        if (isFirstActivation)
        {
            isFirstActivation = false; // ù ���� ���Ŀ��� ������ ���� ����ǵ��� ����
            DOVirtual.DelayedCall(0.9f, () =>
            {
                if (statPanel != null) // �г��� ���� �����ϴ��� Ȯ��
                {
                    statPanel.DOFade(1, fadeDuration)
                        .SetUpdate(true) // UI �ִϸ��̼��� �������� �ʵ���
                        .SetEase(Ease.OutQuad)
                        .OnComplete(() => Time.timeScale = 0f); // �г��� ������ �� �� ���� ����
                }
            });
        }
        else
        {
            statPanel.DOFade(1, fadeDuration)
                .SetUpdate(true)
                .SetEase(Ease.OutQuad)
                .OnComplete(() => Time.timeScale = 0f); // �г��� ������ �� �� ���� ����
        }
    }

    private void HideStatPanel()
    {
        if (statPanel == null) return;

        statPanel.DOFade(0, fadeDuration)
            .SetUpdate(true) // UI �ִϸ��̼��� �������� �ʵ���
            .SetEase(Ease.OutQuad)
            .OnComplete(() =>
            {
                statPanel.gameObject.SetActive(false); // ������ ������� ��Ȱ��ȭ
                isPanelOpen = false; // �г��� ������ ǥ��
                Time.timeScale = 1f; // �г��� ���� �� ���� ���� ����
            });
    }
}