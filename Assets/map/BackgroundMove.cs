using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundMove : MonoBehaviour
{
    [SerializeField]
    private Transform Camera;
    [SerializeField]
    private Transform CameraDefault;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        float d = CameraDefault.transform.position.x - Camera.transform.position.x;
        transform.position = new Vector2(Camera.position.x + d/10, Camera.position.y);
    }
}
