using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class StoryManager : MonoBehaviour
{
    public TextMeshProUGUI storyText;
    public Button nextButton;

    private int currentPageIndex = 0;

    private string[] storyPages = new string[]
    {
        "7���� �ŵ��� ����� ȸ�ǽǿ� �� �־���.\n�׵��� ������ �ٽ��� ����� ���� ����ϸ� �ż��� ���Ǹ� �̾�� �־���.",
        "�׷��� ��, �г��� ������ �ڸ����� �Ͼ ���̰� ���� ������.\n������ �� �̻� �����ڷ� �ӹ��� �ʰڴ�. ���� �������� �ΰ��� ���� ��ġ�ϰڴ�.��",
        "�׳��� ��Ҹ��� ��ȣ�ϰ� ����������,\n�� ���� ���⸦ ���ٰ� �������.",
        "������ ������ �ﰢ������ �����ߴ�.\n���װ��� �ż��� ������ ���߸��� ������.\n�츮�� ���Ѻ��� �ڷμ� ������ ��ȭ�� �����ؾ� �Ѵ�. �ΰ��� �����ο��� �Ѵ�.��",
        "�г��� ������ ª�� ħ�� �ӿ��� ������ ������ ���� ���þ�����,\n�̳� �׳��� �󱼿� �ﴭ���� ���� �г밡 ���ö���.",
        "���װ� ���ϴ� ��ȭ�� �����̴�.\n������ ȥ�� �ӿ� �ְ�, ���� �� ȥ���� �ٷ����� ���̴�.\n������ �ݴ뿡�� ���� �� ���� �� ���̴�.��",
        "�׳�� ������ �ü��� ������ ȸ�ǽ��� ������.",
        "���� ������ �Ҹ��� ��� ������,\n���� �ŵ� ���̿� ���ſ� ������ �귶��.",
        "������ ������ ���� ���̸� ���� ���� �־���,\n�ٸ� �ŵ��� ���� ��ο� ������.\n�г��� ������ ���� ���� �ܼ��� ������ �ƴϾ���.\n�װ��� �ٰ��� ����� �ŵ��� ����� ����� ��������."
    };

    void Start()
    {
        nextButton.onClick.AddListener(NextPage);
        ShowCurrentPage();
    }

    void ShowCurrentPage()
    {
        if (currentPageIndex < storyPages.Length)
        {
            storyText.text = storyPages[currentPageIndex];
        }
        else
        {
            LoadNextScene();
        }
    }

    void NextPage()
    {
        currentPageIndex++;
        ShowCurrentPage();
    }

    void LoadNextScene()
    {
        SceneManager.LoadScene("Map_village"); // ���ϴ� ���� �� �̸����� ����
    }
}
