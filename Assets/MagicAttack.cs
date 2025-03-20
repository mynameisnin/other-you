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
        //  Fireball ���� (�ִϸ��̼� �̺�Ʈ���� ȣ���)
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

        // ?? ���� �ִϸ��̼� ���� (������ �ִϸ��̼� �̺�Ʈ���� �߻�)
        GetComponent<Animator>().SetTrigger("Attack");

        // ���� �ִϸ��̼��� ���� ������ ���
        yield return new WaitForSeconds(0.5f); // (�ִϸ��̼� ���̿� ���� ����)

        yield return new WaitForSeconds(fireballCooldown);
        canCast = true;
    }
    public void SpawnMagic()
    {
        // ?? Fireball ���� (�ִϸ��̼� �̺�Ʈ���� ȣ���)
        GameObject fireball = Instantiate(fireballPrefab, firePoint.position, Quaternion.identity);
        Rigidbody2D rb = fireball.GetComponent<Rigidbody2D>();

        // �÷��̾��� ������ Ȯ���Ͽ� ���� �߻� ���� ����
        SpriteRenderer playerSprite = GetComponent<SpriteRenderer>();
        float direction = playerSprite.flipX ? -1f : 1f;

        // �߻� ���� ����
        rb.velocity = new Vector2(fireballSpeed * direction, 0);

        // ������ ĳ���� �������� ȸ���ǵ��� ���� (�ʿ� ��)
        fireball.transform.localScale = new Vector3(direction, 1f, 1f);
    }
}