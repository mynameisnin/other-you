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
        if (!canCast) return;

        float currentEnergy = energyBarUI != null ? energyBarUI.GetCurrentEnergy() : 0f;

        if (currentEnergy < magicEnergyCost && energyBarUI != null)
        {
            Debug.Log("? 마법 사용 불가: ENERGY 부족!");
            energyBarUI.FlashBorder();
            return;
        }

        StartCoroutine(CastMagicCoroutine());
    }

    private IEnumerator CastMagicCoroutine()
    {
        canCast = false;

        if (energyBarUI != null)
        {
            energyBarUI.ReduceEnergy(magicEnergyCost);
        }

        GetComponent<Animator>().SetTrigger("Cast");

        yield return new WaitForSeconds(0.3f); // 마법 시전 딜레이

        // ?? Fireball 생성
        GameObject fireball = Instantiate(fireballPrefab, firePoint.position, Quaternion.identity);
        Rigidbody2D rb = fireball.GetComponent<Rigidbody2D>();

        float direction = transform.localScale.x > 0 ? 1f : -1f;
        rb.velocity = new Vector2(fireballSpeed * direction, 0);

        yield return new WaitForSeconds(fireballCooldown);
        canCast = true;
    }
}