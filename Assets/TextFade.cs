using System.Collections;
using UnityEngine;
using TMPro;

public class TextFade : MonoBehaviour
{
    public TextMeshProUGUI targetText; // 페이드 효과를 줄 TextMeshPro 컴포넌트
    public float fadeDuration = 1.0f; // 페이드 인/아웃 지속 시간
    public float delayBetweenFades = 0.5f; // 페이드 사이의 대기 시간
    public float minAlpha = 0.2f; // 텍스트가 사라지지 않고 유지할 최소 알파 값 (0~1)

    private void Start()
    {
        StartCoroutine(FadeLoop());
    }

    private IEnumerator FadeLoop()
    {
        while (true)
        {
            // 페이드인 실행
            yield return StartCoroutine(FadeIn());
            // 대기
            yield return new WaitForSeconds(delayBetweenFades);
            // 페이드아웃 실행
            yield return StartCoroutine(FadeOut());
            // 대기
            yield return new WaitForSeconds(delayBetweenFades);
        }
    }

    private IEnumerator FadeIn()
    {
        Color color = targetText.color;
        color.a = minAlpha; // 최소 알파 값에서 시작
        targetText.color = color;

        float elapsedTime = 0;
        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            color.a = Mathf.Lerp(minAlpha, 1f, elapsedTime / fadeDuration); // 최소값에서 1까지
            targetText.color = color;
            yield return null;
        }
    }

    private IEnumerator FadeOut()
    {
        Color color = targetText.color;
        color.a = 1f; // 불투명 상태에서 시작
        targetText.color = color;

        float elapsedTime = 0;
        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            color.a = Mathf.Lerp(1f, minAlpha, elapsedTime / fadeDuration); // 1에서 최소값까지
            targetText.color = color;
            yield return null;
        }
    }
}
