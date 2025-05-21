using System.Collections;
using UnityEngine;

public class SceneLoader : MonoBehaviour
{
    public void RebootGame()
    {
#if UNITY_STANDALONE
        string[] executableEndings = new string[] { "exe", "x86", "x86_64", "app" };
        string executablePath = Application.dataPath + "/../";

        // ����� ���� ���� ã��
        foreach (string file in System.IO.Directory.GetFiles(executablePath))
        {
            foreach (string ending in executableEndings)
            {
                if (file.ToLower().EndsWith("." + ending))
                {
                    Debug.Log("[Reboot] ����� �õ�: " + file);
                    System.Diagnostics.Process.Start(file); // �� �ν��Ͻ� ����
                    Application.Quit(); // ���� ����
                    return;
                }
            }
        }

        Debug.LogError("[Reboot] ���� ������ ã�� �� �����ϴ�.");
#else
        Debug.LogWarning("RebootGame()�� Standalone ���忡���� �۵��մϴ�.");
#endif
    }
}
