using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using System.Collections;

public class StoryManager : MonoBehaviour
{
    public TextMeshProUGUI storyText;
    public string[] storyLines;
    public float typingSpeed = 0.05f;
    public Button skipButton;

    private int currentLineIndex = 0;
    private bool isTyping = false;

    void Start()
    {
        // 1. 버튼은 보이지만 클릭 불가능하게!
        skipButton.interactable = false;

        // 2. 클릭 가능하게 만드는 함수 호출 예약 (5초 후)
        Invoke(nameof(EnableSkipButton), 5f);

        // 3. 버튼 클릭 이벤트 연결
        skipButton.onClick.AddListener(SkipStory);

        // 4. 스토리 타이핑 시작
        StartCoroutine(TypeLine());
    }

    void EnableSkipButton()
    {
        skipButton.interactable = true;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (isTyping)
            {
                StopAllCoroutines();
                storyText.text = storyLines[currentLineIndex];
                isTyping = false;
            }
            else
            {
                currentLineIndex++;
                if (currentLineIndex < storyLines.Length)
                    StartCoroutine(TypeLine());
                else
                    LoadNextScene();
            }
        }
    }

    IEnumerator TypeLine()
    {
        isTyping = true;
        storyText.text = "";
        foreach (char c in storyLines[currentLineIndex])
        {
            storyText.text += c;
            yield return new WaitForSeconds(typingSpeed);
        }
        isTyping = false;
    }

    void SkipStory()
    {
        LoadNextScene();
    }

    void LoadNextScene()
    {
        SceneManager.LoadScene("Map_village");
    }
}
