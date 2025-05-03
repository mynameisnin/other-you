using UnityEngine;

public class TimelineEventReceiver : MonoBehaviour
{
    public AudioSource earthquakeSfx;

    public void PlayEarthquakeSound()
    {
        if (earthquakeSfx != null && !earthquakeSfx.isPlaying)
        {
            earthquakeSfx.Play();
            Debug.Log("타임라인 Signal에서 지진 효과음 재생!");
        }
    }
}
