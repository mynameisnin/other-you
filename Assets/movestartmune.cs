using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class movestartmune : MonoBehaviour
{
    public void LoadNextScene()
    {
        SceneManager.LoadScene("Startmunescenes");
    }
}
