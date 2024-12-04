using System.Collections;
using UnityEngine;
using TMPro;

public class TextFade : MonoBehaviour
{
    public TextMeshProUGUI targetText; // ���̵� ȿ���� �� TextMeshPro ������Ʈ
    public float fadeDuration = 1.0f; // ���̵� ��/�ƿ� ���� �ð�
    public float delayBetweenFades = 0.5f; // ���̵� ������ ��� �ð�
    public float minAlpha = 0.2f; // �ؽ�Ʈ�� ������� �ʰ� ������ �ּ� ���� �� (0~1)

    private void Start()
    {
        StartCoroutine(FadeLoop());
    }

    private IEnumerator FadeLoop()
    {
        while (true)
        {
            // ���̵��� ����
            yield return StartCoroutine(FadeIn());
            // ���
            yield return new WaitForSeconds(delayBetweenFades);
            // ���̵�ƿ� ����
            yield return StartCoroutine(FadeOut());
            // ���
            yield return new WaitForSeconds(delayBetweenFades);
        }
    }

    private IEnumerator FadeIn()
    {
        Color color = targetText.color;
        color.a = minAlpha; // �ּ� ���� ������ ����
        targetText.color = color;

        float elapsedTime = 0;
        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            color.a = Mathf.Lerp(minAlpha, 1f, elapsedTime / fadeDuration); // �ּҰ����� 1����
            targetText.color = color;
            yield return null;
        }
    }

    private IEnumerator FadeOut()
    {
        Color color = targetText.color;
        color.a = 1f; // ������ ���¿��� ����
        targetText.color = color;

        float elapsedTime = 0;
        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            color.a = Mathf.Lerp(1f, minAlpha, elapsedTime / fadeDuration); // 1���� �ּҰ�����
            targetText.color = color;
            yield return null;
        }
    }
}
