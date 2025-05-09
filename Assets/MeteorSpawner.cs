using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeteorSpawner : MonoBehaviour
{
    public GameObject telegraphPrefab; // ������ ������
    public GameObject meteorPrefab; // ���༺ ������

    public float warningDuration = 1.0f; // ������ ǥ�� �ð�
    public float fallSpeed = 10f;

    public void SpawnMeteor(Vector2 spawnPos, Vector2 targetPos)
    {
        StartCoroutine(SpawnRoutine(spawnPos, targetPos));
    }

    private IEnumerator SpawnRoutine(Vector2 spawnPos, Vector2 targetPos)
    {
        // 1. ��� ǥ�� ����
        GameObject warning = Instantiate(telegraphPrefab, targetPos, Quaternion.identity);
        Vector2 direction = (targetPos - spawnPos).normalized;

        // �������� �缱���� ȸ��
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        warning.transform.rotation = Quaternion.Euler(0, 0, angle);

        yield return new WaitForSeconds(warningDuration);

        // 2. ������ ����
        Destroy(warning);

        // 3. ���༺ ���� �� ���� ����
        GameObject meteor = Instantiate(meteorPrefab, spawnPos, Quaternion.identity);
        Rigidbody2D rb = meteor.GetComponent<Rigidbody2D>();
        rb.velocity = direction * fallSpeed;
    }
}