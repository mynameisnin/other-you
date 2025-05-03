using UnityEngine;

public class TimelineEventReceiver : MonoBehaviour
{
    public AudioSource earthquakeSfx;

    public void PlayEarthquakeSound()
    {
        if (earthquakeSfx != null && !earthquakeSfx.isPlaying)
        {
            earthquakeSfx.Play();
            Debug.Log("Ÿ�Ӷ��� Signal���� ���� ȿ���� ���!");
        }
    }
}
