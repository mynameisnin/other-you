using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MagicAttack : MonoBehaviour
{
    public GameObject fireballPrefab; // ?? �⺻ ���� (���̾)
    public Transform firePoint; // ������ �����Ǵ� ��ġ

    public float fireballSpeed = 10f;
    public float fireballCooldown = 1.0f;
    public float magicEnergyCost = 10f; // ���� ��� �� ������ �Ҹ�

    private bool canCast = true;
    private EnergyBarUI energyBarUI;

    void Start()
    {
        energyBarUI = FindObjectOfType<EnergyBarUI>(); // ������ UI ã��
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
            Debug.Log("? ���� ��� �Ұ�: ENERGY ����!");
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

        yield return new WaitForSeconds(0.3f); // ���� ���� ������

        // ?? Fireball ����
        GameObject fireball = Instantiate(fireballPrefab, firePoint.position, Quaternion.identity);
        Rigidbody2D rb = fireball.GetComponent<Rigidbody2D>();

        float direction = transform.localScale.x > 0 ? 1f : -1f;
        rb.velocity = new Vector2(fireballSpeed * direction, 0);

        yield return new WaitForSeconds(fireballCooldown);
        canCast = true;
    }
}