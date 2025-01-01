using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class enemyTest : MonoBehaviour
{
    private Animator TestAnime;
    public GameObject[] bloodEffectPrefabs; // 여러 이펙트를 담을 배열
    public GameObject parringEffects;

    void Start()
    {
        TestAnime = GetComponent<Animator>();
    }

    public void ShowBloodEffect(Vector3 hitPosition)
    {
        // 랜덤으로 이펙트를 선택
        int randomIndex = Random.Range(0, bloodEffectPrefabs.Length);
        GameObject selectedEffect = bloodEffectPrefabs[randomIndex];

        // 이펙트 생성
        GameObject bloodEffect = Instantiate(selectedEffect, hitPosition, Quaternion.identity);

        // 위치 이동 (필요에 따라 조정)
        bloodEffect.transform.position += new Vector3(0f, 1f, -1);

        // 일정 시간 뒤 자동 삭제
        Destroy(bloodEffect, 0.3f);
    }



    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("PlayerAttack"))
        {
            Vector3 hitPosition = transform.position;

            // "hurt" 애니메이션 트리거 실행
            TestAnime.SetTrigger("hurt");

            // 피 이펙트 실행
            ShowBloodEffect(hitPosition);
        }
    }


}
