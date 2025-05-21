using UnityEngine;

public class TimelineEventReceiver : MonoBehaviour
{
    public AudioSource earthquakeSfx;

    public void PlayEarthquakeSound()
    {
        if (earthquakeSfx != null && !earthquakeSfx.isPlaying)
        {
            earthquakeSfx.time = 28f;
            earthquakeSfx.Play();
            Debug.Log("Ÿ�Ӷ��� Signal���� ���� ȿ���� ���!");
        }
    }
}
