using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal; // Light2D ���
public class MagicAttack : MonoBehaviour
{
    public GameObject fireballPrefab; // ?? �⺻ ���� (���̾)
    public Transform firePoint; // ������ �����Ǵ� ��ġ
    public Light2D magicLight; //  ���� ����Ʈ�� Light2D �߰�
    public float fireballSpeed = 10f;
    public float fireballCooldown = 1.0f;
    public float magicEnergyCost = 10f; // ���� ��� �� ������ �Ҹ�

    private bool canCast = true;
    private EnergyBarUI energyBarUI;

    void Start()
    {
        energyBarUI = FindObjectOfType<EnergyBarUI>(); // ������ UI ã��
        if (magicLight != null)
        {
            magicLight.enabled = false; // ������ �� ����Ʈ ��Ȱ��ȭ
        }
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

public void AttackLight()
{
        if (magicLight != null)
        {
            magicLight.enabled = true;
        }
}

public void EndAttacks()
    {
        //  ���� ���� �� ����Ʈ ����
        if (magicLight != null)
        {
            magicLight.enabled = false;
        }
    }
}