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
        // 1. ��ư�� �������� Ŭ�� �Ұ����ϰ�!
        skipButton.interactable = false;

        // 2. Ŭ�� �����ϰ� ����� �Լ� ȣ�� ���� (5�� ��)
        Invoke(nameof(EnableSkipButton), 5f);

        // 3. ��ư Ŭ�� �̺�Ʈ ����
        skipButton.onClick.AddListener(SkipStory);

        // 4. ���丮 Ÿ���� ����
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
