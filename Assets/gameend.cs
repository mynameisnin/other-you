using UnityEngine;

public class gameend : MonoBehaviour
{
    public void Exit()
    {
        
        Invoke("QuitGame", 0.3f);
    }

    // 게임을 종료하는 함수
    private void QuitGame()
    {
        
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        
        Application.Quit();
#endif
    }
}
