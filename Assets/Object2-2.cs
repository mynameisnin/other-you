using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Object2_2 : MonoBehaviour
{
    public bool check2 = false;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("PlayerAttack"))
        {
            check2 = true;
            Debug.Log("크리스탈 2 타격");
            StartCoroutine(Check1Delay());
        }
    }

    private IEnumerator Check1Delay()
    {
        yield return new WaitForSeconds(0.5f); // 0.5초 대기
        check2 = false;
    }
}
