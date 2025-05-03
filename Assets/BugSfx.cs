using UnityEngine;

public class BugSfx : MonoBehaviour
{
    public AudioSource bugSfx;

    public void PlayBugSound()
    {
        if (bugSfx != null && !bugSfx.isPlaying)
        {
            bugSfx.Play();
            Debug.Log("Ÿ�Ӷ��� Signal���� �ͶѶ�� ȿ���� ���!");
        }
    }
}
