using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Playables;

public class TimeLineSecenManager : MonoBehaviour
{
    public string nextSceneName; // ������ �� �̸�
    public Vector2 spawnPosition; // �̵��� ���� ���� ��ġ

    [SerializeField] private PlayableDirector timeline; // Ÿ�Ӷ��� ����� ����

    private bool hasSceneChanged = false; // �ߺ� ������

    private void Update()
    {
        if (!hasSceneChanged && Input.GetKeyDown(KeyCode.Return))
        {
            SkipTimeline();
        }
    }

    private void SkipTimeline()
    {
        if (timeline != null)
        {
            timeline.time = timeline.duration; // Ÿ�Ӷ��� ������ ����
            timeline.Evaluate();               // �� �������� ���� �ݿ�
        }

        ChangeScene();
    }

    public void ChangeScene()
    {
        if (hasSceneChanged) return;
        hasSceneChanged = true;

        SceneManager.LoadScene(nextSceneName);
    }
}
