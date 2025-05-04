using UnityEngine;

public class BugSfx : MonoBehaviour
{
    public AudioSource bugSfx;

    public void PlayBugSound()
    {
        if (bugSfx != null && !bugSfx.isPlaying)
        {
            bugSfx.Play();
            Debug.Log("타임라인 Signal에서 귀뚜라미 효과음 재생!");
        }
    }
}
