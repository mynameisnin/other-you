using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public class TimelineStarter : MonoBehaviour
{
    public PlayableDirector timeline; // 타임라인 연결
    public GameObject startButton; // UI 버튼 연결

    public void PlayTimeline()
    {
        if (timeline != null)
        {
            timeline.Play();
        
        }
    }
}