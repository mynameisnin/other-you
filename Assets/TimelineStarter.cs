using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public class TimelineStarter : MonoBehaviour
{
    public PlayableDirector timeline; // Ÿ�Ӷ��� ����
    public GameObject startButton; // UI ��ư ����

    public void PlayTimeline()
    {
        if (timeline != null)
        {
            timeline.Play();
        
        }
    }
}