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
        "7명의 신들이 고요한 회의실에 모여 있었다.\n\n그들은 세상을 다스릴 방법에 대해 고심하며\n\n신성한 논의를 이어가고 있었다.",
        "그러던 중, 분노의 여신이 자리에서 일어나\n\n무겁게 입을 열었다.\n\n“나는 더 이상 관찰자로 머물지 않겠다.\n\n신의 권한으로 인간을 직접 통치하겠다.”",
        "그녀의 목소리는 단호하고 강렬했으며,\n\n방 안의 공기를 얼어붙게 만들었다.",
        "정의의 여신은 즉각적으로 반응했다.\n\n“그것은 신성한 균형을 깨뜨리는 행위다.\n\n우리는 지켜보는 자로서 세상의 조화를 유지\n\n해야 한다. 인간은 자유로워야 한다.”",
        "분노의 여신은 짧은 침묵 속에서 정의의\n\n여신의 말을 곱씹었지만,\n\n이내 그녀의 얼굴엔 억눌리지 않은 분노가\n\n떠올랐다.",
        "“네가 말하는 조화는 허울뿐이다.\n\n세상은 혼돈 속에 있고, 나는 그 혼돈을\n\n바로잡을 것이다.\n\n너희의 반대에도 나는 내 길을 갈 것이다.”",
        "그녀는 차가운 시선을 던지며 회의실을 떠났다.",
        "문이 닫히는 소리가 울려 퍼지자,\n\n남은 신들 사이에 무거운 정적이 흘렀다.",
        "정의의 여신은 고개를 숙이며 숨을 고르고 있었고,\n\n다른 신들은 깊은 고민에 빠졌다.\n\n분노의 여신이 남긴 말은 단순한 선언이 아니었다.\n\n그것은 다가올 갈등과 신들의 운명을 뒤흔들 전조였다."
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
        SceneManager.LoadScene("Map_village"); // 원하는 다음 씬 이름으로 변경
    }
}
