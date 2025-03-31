using UnityEngine;
using TMPro;

public class RandomTextTMP : MonoBehaviour
{
    public TextMeshProUGUI textComponent; // TextMeshPro UI »ç¿ë
    public string[] messages = {};

    void Start()
    {
        InvokeRepeating("ShowRandomText", 0, 3.3f);
    }

    void ShowRandomText()
    {
        int randomIndex = Random.Range(0, messages.Length);
        textComponent.text = "<b>Tips</b>  \n\n" + messages[randomIndex];
    }
}
