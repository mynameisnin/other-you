using UnityEngine;
using UnityEngine.Rendering.Universal; // Light2D ����� ���� �ʿ�
using DG.Tweening; // DOTween ���

public class StatueInteraction : MonoBehaviour
{
    private Light2D statueLight;
    private bool isActivated = false; // �� �� ���������� �ٲ������ Ȯ���ϴ� ����
    private bool isPlayerNearby = false; // �÷��̾ ��ó�� �ִ��� Ȯ��

    public Color blueColor = Color.blue;
    public Color redColor = Color.red;
    public ParticleSystem featherEffect; // ���� ��ƼŬ �ý���

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

            // ����� ������Ʈ ó������ �� ���̰� ����
            shockwaveRenderer.gameObject.SetActive(false);
        }
    }

    void Update()
    {
        // �÷��̾ ��ó�� �ְ� �� Ű�� ������ Ȱ��ȭ
        if (isPlayerNearby && Input.GetKeyDown(KeyCode.UpArrow) && !isActivated)
        {
            statueLight.color = redColor; // ���������� ����
            isActivated = true; // �� �� Ȱ��ȭ�Ǹ� ����

            if (featherEffect != null)
            {
                featherEffect.Play(); // ���� ȿ�� ����
            }

            StartShockwaveEffect(); // ����� ȿ�� ����
            StartLightEffect(); // Light2D Inner �� ����
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("PlayerAttack")) // �÷��̾ ���� ��ó�� ���� ���� üũ
        {
            isPlayerNearby = true; // �÷��̾ ��ó�� ������ ǥ��
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("PlayerAttack")) // �÷��̾ �־�����
        {
            isPlayerNearby = false; // �������� ���
        }
    }

    private void StartShockwaveEffect()
    {
        if (shockwaveRenderer == null || shockwaveMaterial == null)
            return;

        // ����� ������Ʈ Ȱ��ȭ
        shockwaveRenderer.gameObject.SetActive(true);

        // ���͸����� WaveDistanceFromCenter ���� DOTween�� �̿��� ������ ����
        shockwaveRenderer.GetPropertyBlock(propertyBlock);
        propertyBlock.SetFloat("_WaveDistanceFromCenter", -0.1f); // �ʱ� �� ����
        shockwaveRenderer.SetPropertyBlock(propertyBlock);

        float newShockwaveDuration = 15.0f; // ����� ���� �ð� (���� 1.5f �� 3.0f�� ����)
        float newMaxWaveDistance = 4.5f; // �ִ� Ȯ�� �Ÿ� (���� 2.0f �� 3.5f�� ����)

        DOTween.To(() => -0.1f, x =>
        {
            shockwaveRenderer.GetPropertyBlock(propertyBlock);
            propertyBlock.SetFloat("_WaveDistanceFromCenter", x);
            shockwaveRenderer.SetPropertyBlock(propertyBlock);
        }, newMaxWaveDistance, newShockwaveDuration) // ���� �ð��� �ø���, Ȯ�� �Ÿ��� ����
        .SetEase(Ease.OutSine) // Ȯ���� ���� �ε巴�� �������� ����
        .OnComplete(() =>
        {
            // ȿ�� ���� �� ����� ������Ʈ ��Ȱ��ȭ
            shockwaveRenderer.gameObject.SetActive(false);
        });
    }


    private void StartLightEffect()
    {
        // DOTween���� Light2D�� Inner ���� ������ ����
        DOTween.To(() => statueLight.pointLightInnerRadius, x => statueLight.pointLightInnerRadius = x, lightInnerMax, lightInnerDuration)
            .SetEase(Ease.OutQuad);
    }
}
