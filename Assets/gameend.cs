using UnityEngine;

public class gameend : MonoBehaviour
{
    public void Exit()
    {
        
        Invoke("QuitGame", 0.3f);
    }

    // ������ �����ϴ� �Լ�
    private void QuitGame()
    {
        
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        
        Application.Quit();
#endif
    }
}
