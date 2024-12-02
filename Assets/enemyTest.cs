using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class enemyTest : MonoBehaviour
{
    private Animator TestAnime;
    public GameObject bloodEffectPrefab; // ����Ʈ ������
    void Start()
    {
        TestAnime = GetComponent<Animator>();
    }

    public void ShowBloodEffect(Vector3 hitPosition)
    {
        // ����Ʈ ����
        GameObject bloodEffect = Instantiate(bloodEffectPrefab, hitPosition, Quaternion.identity);
        bloodEffect.transform.position += new Vector3(0f, 1f, -1); // ��ġ�̵�
        // 0.3�� �� �ڵ� ����
        Destroy(bloodEffect, 0.3f);
    }
    void OnTriggerEnter2D(Collider2D other)
    {

        if (other.CompareTag("PlayerAttack"))
        {
            Vector3 hitPosition = transform.position;

            // "hurt" �ִϸ��̼� Ʈ���� ����
            TestAnime.SetTrigger("hurt");
            ShowBloodEffect(hitPosition);
        }
        }
}