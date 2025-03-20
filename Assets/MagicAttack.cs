using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MagicAttack : MonoBehaviour
{
    public GameObject fireballPrefab; // ?? 기본 마법 (파이어볼)
    public Transform firePoint; // 마법이 생성되는 위치

    public float fireballSpeed = 10f;
    public float fireballCooldown = 1.0f;
    public float magicEnergyCost = 10f; // 마법 사용 시 에너지 소모량

    private bool canCast = true;
    private EnergyBarUI energyBarUI;

    void Start()
    {
        energyBarUI = FindObjectOfType<EnergyBarUI>(); // 에너지 UI 찾기
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.X) && canCast)
        {
            CastMagic();
        }
    }

    public void CastMagic()
    {
        //  Fireball 생성 (애니메이션 이벤트에서 호출됨)
        GameObject fireball = Instantiate(fireballPrefab, firePoint.position, Quaternion.identity);
        Rigidbody2D rb = fireball.GetComponent<Rigidbody2D>();

        float direction = GetComponent<SpriteRenderer>().flipX ? -1f : 1f;
        rb.velocity = new Vector2(fireballSpeed * direction, 0);
    }


    private IEnumerator CastMagicCoroutine()
    {
        canCast = false;

        if (energyBarUI != null)
        {
            energyBarUI.ReduceEnergy(magicEnergyCost);
        }

        // ?? 공격 애니메이션 실행 (마법은 애니메이션 이벤트에서 발사)
        GetComponent<Animator>().SetTrigger("Attack");

        // 공격 애니메이션이 끝날 때까지 대기
        yield return new WaitForSeconds(0.5f); // (애니메이션 길이에 맞춰 조절)

        yield return new WaitForSeconds(fireballCooldown);
        canCast = true;
    }
    public void SpawnMagic()
    {
        // ?? Fireball 생성 (애니메이션 이벤트에서 호출됨)
        GameObject fireball = Instantiate(fireballPrefab, firePoint.position, Quaternion.identity);
        Rigidbody2D rb = fireball.GetComponent<Rigidbody2D>();

        // 플레이어의 방향을 확인하여 마법 발사 방향 결정
        SpriteRenderer playerSprite = GetComponent<SpriteRenderer>();
        float direction = playerSprite.flipX ? -1f : 1f;

        // 발사 방향 적용
        rb.velocity = new Vector2(fireballSpeed * direction, 0);

        // 마법이 캐릭터 방향으로 회전되도록 설정 (필요 시)
        fireball.transform.localScale = new Vector3(direction, 1f, 1f);
    }
}