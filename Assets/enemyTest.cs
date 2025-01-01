using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class enemyTest : MonoBehaviour
{
    private Animator TestAnime;
    public GameObject[] bloodEffectPrefabs; // ���� ����Ʈ�� ���� �迭
    public GameObject parringEffects;

    void Start()
    {
        TestAnime = GetComponent<Animator>();
    }

    public void ShowBloodEffect(Vector3 hitPosition)
    {
        // �������� ����Ʈ�� ����
        int randomIndex = Random.Range(0, bloodEffectPrefabs.Length);
        GameObject selectedEffect = bloodEffectPrefabs[randomIndex];

        // ����Ʈ ����
        GameObject bloodEffect = Instantiate(selectedEffect, hitPosition, Quaternion.identity);

        // ��ġ �̵� (�ʿ信 ���� ����)
        bloodEffect.transform.position += new Vector3(0f, 1f, -1);

        // ���� �ð� �� �ڵ� ����
        Destroy(bloodEffect, 0.3f);
    }



    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("PlayerAttack"))
        {
            Vector3 hitPosition = transform.position;

            // "hurt" �ִϸ��̼� Ʈ���� ����
            TestAnime.SetTrigger("hurt");

            // �� ����Ʈ ����
            ShowBloodEffect(hitPosition);
        }
    }


}
