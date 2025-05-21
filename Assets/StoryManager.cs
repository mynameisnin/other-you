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
        "7���� �ŵ��� ����� ȸ�ǽǿ� �� �־���.\n\n�׵��� ������ �ٽ��� ����� ���� ����ϸ�\n\n�ż��� ���Ǹ� �̾�� �־���.",
        "�׷��� ��, �г��� ������ �ڸ����� �Ͼ\n\n���̰� ���� ������.\n\n������ �� �̻� �����ڷ� �ӹ��� �ʰڴ�.\n\n���� �������� �ΰ��� ���� ��ġ�ϰڴ�.��",
        "�׳��� ��Ҹ��� ��ȣ�ϰ� ����������,\n\n�� ���� ���⸦ ���ٰ� �������.",
        "������ ������ �ﰢ������ �����ߴ�.\n\n���װ��� �ż��� ������ ���߸��� ������.\n\n�츮�� ���Ѻ��� �ڷμ� ������ ��ȭ�� ����\n\n�ؾ� �Ѵ�. �ΰ��� �����ο��� �Ѵ�.��",
        "�г��� ������ ª�� ħ�� �ӿ��� ������\n\n������ ���� ���þ�����,\n\n�̳� �׳��� �󱼿� �ﴭ���� ���� �г밡\n\n���ö���.",
        "���װ� ���ϴ� ��ȭ�� �����̴�.\n\n������ ȥ�� �ӿ� �ְ�, ���� �� ȥ����\n\n�ٷ����� ���̴�.\n\n������ �ݴ뿡�� ���� �� ���� �� ���̴�.��",
        "�׳�� ������ �ü��� ������ ȸ�ǽ��� ������.",
        "���� ������ �Ҹ��� ��� ������,\n\n���� �ŵ� ���̿� ���ſ� ������ �귶��.",
        "������ ������ ���� ���̸� ���� ���� �־���,\n\n�ٸ� �ŵ��� ���� ��ο� ������.\n\n�г��� ������ ���� ���� �ܼ��� ������ �ƴϾ���.\n\n�װ��� �ٰ��� ����� �ŵ��� ����� ����� ��������."
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
