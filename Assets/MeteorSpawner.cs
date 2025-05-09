using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeteorSpawner : MonoBehaviour
{
    public GameObject telegraphPrefab; // 레이저 프리팹
    public GameObject meteorPrefab; // 소행성 프리팹

    public float warningDuration = 1.0f; // 레이저 표시 시간
    public float fallSpeed = 10f;

    public void SpawnMeteor(Vector2 spawnPos, Vector2 targetPos)
    {
        StartCoroutine(SpawnRoutine(spawnPos, targetPos));
    }

    private IEnumerator SpawnRoutine(Vector2 spawnPos, Vector2 targetPos)
    {
        // 1. 경고 표시 생성
        GameObject warning = Instantiate(telegraphPrefab, targetPos, Quaternion.identity);
        Vector2 direction = (targetPos - spawnPos).normalized;

        // 레이저를 사선으로 회전
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        warning.transform.rotation = Quaternion.Euler(0, 0, angle);

        yield return new WaitForSeconds(warningDuration);

        // 2. 레이저 제거
        Destroy(warning);

        // 3. 소행성 생성 및 방향 설정
        GameObject meteor = Instantiate(meteorPrefab, spawnPos, Quaternion.identity);
        Rigidbody2D rb = meteor.GetComponent<Rigidbody2D>();
        rb.velocity = direction * fallSpeed;
    }
}