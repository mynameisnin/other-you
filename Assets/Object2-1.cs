using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Object2_1 : MonoBehaviour
{
    public bool check1 = false;
    private Object2_2 oj2_2;
    public bool opne = false;
    public GameObject otherObject;
    public GameObject oj2; // 길을 막는 오브젝트

    void Start()
    {
        oj2_2 = otherObject.GetComponent<Object2_2>();
    }

    void Update()
    {
        if (check1 && oj2_2.check2)
        {
            oj2.SetActive(false); // 길 열기 - 오브젝트 비활성화
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
