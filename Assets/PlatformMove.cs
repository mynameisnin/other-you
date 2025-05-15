using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlatformMove : MonoBehaviour
{
    public float horizontality; // x 방향
    public float verticality; // y방향
    public float speed;
    private Vector3 df;
    private Vector3 target;
    private bool turn = false;


    // Start is called before the first frame update
    void Start()
    {
        df = new Vector3(transform.position.x, transform.position.y, transform.position.z);
        target = new Vector3(df.x + horizontality, df.y + verticality, df.z);
    }

    // Update is called once per frame
    void Update()
    {
        if (!turn)
        {
            transform.position = Vector3.MoveTowards(
                transform.position,
                target,
                speed * Time.deltaTime);

            // 목표에 거의 도달했으면 반대 방향으로 전환
            if (Vector3.Distance(transform.position, target) < 0.01f)
            {
                turn = true;
            }
        }
        else
        {
            transform.position = Vector3.MoveTowards(
                transform.position,
                df,
                speed * Time.deltaTime);

            // 원래 위치에 거의 도달했으면 정지
            if (Vector3.Distance(transform.position, df) < 0.01f)
            {
                turn = false;
            }
        }
    }
}
