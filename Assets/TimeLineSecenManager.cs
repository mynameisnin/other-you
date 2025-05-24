using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Playables;

public class TimeLineSecenManager : MonoBehaviour
{
    public string nextSceneName; // 변경할 씬 이름
    public Vector2 spawnPosition; // 이동할 씬의 스폰 위치

    [SerializeField] private PlayableDirector timeline; // 타임라인 재생기 참조

    private bool hasSceneChanged = false; // 중복 방지용

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
            timeline.time = timeline.duration; // 타임라인 끝까지 점프
            timeline.Evaluate();               // 그 시점으로 강제 반영
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
