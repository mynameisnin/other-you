using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class enemyTest : MonoBehaviour
{
    private Animator TestAnime;
    public GameObject bloodEffectPrefab; // 이펙트 프리팹
    void Start()
    {
        TestAnime = GetComponent<Animator>();
    }

    public void ShowBloodEffect(Vector3 hitPosition)
    {
        // 이펙트 생성
        GameObject bloodEffect = Instantiate(bloodEffectPrefab, hitPosition, Quaternion.identity);
        bloodEffect.transform.position += new Vector3(0f, 1f, -1); // 위치이동
        // 0.3초 뒤 자동 삭제
        Destroy(bloodEffect, 0.3f);
    }
    void OnTriggerEnter2D(Collider2D other)
    {

        if (other.CompareTag("PlayerAttack"))
        {
            Vector3 hitPosition = transform.position;

            // "hurt" 애니메이션 트리거 실행
            TestAnime.SetTrigger("hurt");
            ShowBloodEffect(hitPosition);
        }
        }
}