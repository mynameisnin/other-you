using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum KeyAction { UP, DOWN, LEFT, RIGHT, KEYCOUNT}

public static class KeySetting { public static Dictionary<KeyAction, KeyCode> keys = new Dictionary<KeyAction, KeyCode>(); }

public class KeyManager : MonoBehaviour
{
    KeyCode[] defaultKeys = new KeyCode[] { KeyCode.W, KeyCode.S, KeyCode.A, KeyCode.D };
    void Start()
    {
        for(int i = 0;i<(int)KeyAction.KEYCOUNT; i++)
        {
            KeySetting.keys.Add((KeyAction)i, defaultKeys[i]);
        }
    }
}
