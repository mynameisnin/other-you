using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Object2_1 : MonoBehaviour
{
    public bool check1 = false;
    private Object2_2 oj2_2;
    public bool opne = false;
    public GameObject otherObject;
    public GameObject oj2;

    // Start is called before the first frame update
    void Start()
    {
        oj2_2 = otherObject.GetComponent<Object2_2>();
    }

    // Update is called once per frame
    void Update()
    {
        if (check1 && oj2_2.check2)
        {
            //여기에 길 열림 구현 ---------------------------------------
            oj2.SetActive(false);
            opne = true;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("PlayerAttack"))
        {
            check1 = true;
            Debug.Log("크리스탈 1 타격");
            StartCoroutine(Check1Delay());
        }
    }

    private IEnumerator Check1Delay()
    {
        yield return new WaitForSeconds(0.5f); // 0.5초 대기
        check1 = false;
    }
}
