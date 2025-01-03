using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundMove : MonoBehaviour
{
    [SerializeField]
    private Transform Camera;
    [SerializeField]
    private Transform CameraDefault;
    [SerializeField]
    private Transform BackDay;
    [SerializeField]
    private Transform BackNight1;
    [SerializeField]
    private Transform BackNight2;
    [SerializeField]
    private Transform BackNight3;
    [SerializeField]
    private Transform BackNight4;
    [SerializeField]
    private Transform BackNight5;
    [SerializeField]
    private Transform Eclipse;

    int backChange = 0;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        MoveDay(BackDay);
        MoveNight(BackNight1);
        MoveNight(BackNight2);
        MoveNight(BackNight3);
        MoveNight(BackNight4);
        MoveNight(BackNight5);
        MoveEclipse(Eclipse);

        if (Input.GetKeyDown(KeyCode.Space))
        {
            DayChange();
        }
    }

    void MoveDay(Transform back)
    {
        float d = CameraDefault.transform.position.x - Camera.transform.position.x;
        back.position = new Vector2(Camera.position.x + d / 10, Camera.position.y + backChange);
    }

    void MoveNight(Transform back)
    {
        float d = CameraDefault.transform.position.x - Camera.transform.position.x;
        back.position = new Vector2(Camera.position.x + d / 10, Camera.position.y + 20 + backChange);
    }

    void MoveEclipse(Transform back)
    {
        back.position = new Vector2(Camera.position.x + 3, Camera.position.y + 3 + 20 + backChange);
    }

    void DayChange()
    {
        if (backChange == 0){
            backChange = -20;
        }else{
            backChange = 0;
        }
    }
}
