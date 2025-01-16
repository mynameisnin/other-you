using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundMove : MonoBehaviour
{
    [SerializeField]
    private Transform camera;
    [SerializeField]
    private Transform cameraDefault;
    [SerializeField]
    private Transform backDay1;
    [SerializeField]
    private Transform backDay2;
    [SerializeField]
    private Transform backDay3;
    [SerializeField]
    private Transform backDay4;
    [SerializeField]
    private Transform backDay5;
    [SerializeField]
    private Transform backNight1;
    [SerializeField]
    private Transform backNight2;
    [SerializeField]
    private Transform backNight3;
    [SerializeField]
    private Transform backNight4;
    [SerializeField]
    private Transform backNight5;
    [SerializeField]
    private Transform eclipse;
    [SerializeField]
    private Transform sun;

    int backChange = 0;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //¹è°æ ¿òÁ÷ÀÓ
        float d = cameraDefault.transform.position.x - camera.transform.position.x;

        MoveDay(backDay1, 0);
        MoveDay(backDay2, d / 40);
        MoveDay(backDay3, d / 30);
        MoveDay(backDay4, d / 25);
        MoveDay(backDay5, d / 20);
        MoveSun(sun);
        MoveNight(backNight1, 0);
        MoveNight(backNight2, d / 40);
        MoveNight(backNight3, d / 30);
        MoveNight(backNight4, d / 25);
        MoveNight(backNight5, d / 20);
        MoveEclipse(eclipse);

        //¹ã ³· ¹Ù²Ù±â
        if (Input.GetKeyDown(KeyCode.Space))
        {
            DayChange();
        }
    }

    void MoveDay(Transform back, float move)
    {
        back.position = new Vector2(camera.position.x + move, camera.position.y + backChange);
    }

    void MoveNight(Transform back, float move)
    {
        back.position = new Vector2(camera.position.x + move, camera.position.y + 20 + backChange);
    }

    void MoveSun(Transform back)
    {
        back.position = new Vector2(camera.position.x + 3, camera.position.y + 3 + backChange);
    }

    void MoveEclipse(Transform back)
    {
        back.position = new Vector2(camera.position.x + 3, camera.position.y + 3 + 20 + backChange);
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
