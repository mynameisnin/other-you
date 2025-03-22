using System.Collections;
using UnityEngine;
using UnityEngine.Rendering.Universal; // Light2D ���

public class MagicAttack : MonoBehaviour
{
    public GameObject fireballPrefab;
    public Transform firePoint;
    public Light2D magicLight;
    public float fireballSpeed = 10f;
    public float fireballCooldown = 1.0f;
    public float magicEnergyCost = 10f; // ���� ��� �� �Ҹ�Ǵ� ������

    private bool canCast = true;
    public DevaEnergyBarUI DevaEnergyBarUI;

    void Start()
    {
        DevaEnergyBarUI = FindObjectOfType<DevaEnergyBarUI>();

        if (magicLight != null)
            magicLight.enabled = false;
    }

    public void SpawnMagic()
    {
        if (DevaEnergyBarUI == null)
        {
            Debug.LogWarning("EnergyBarUI�� ����Ǿ� ���� �ʽ��ϴ�.");
            return;
        }

        float currentEnergy = DevaEnergyBarUI.GetCurrentEnergy();

        if (currentEnergy < magicEnergyCost)
        {
            Debug.Log("���� ����: ������ ����");
            DevaEnergyBarUI.FlashBorder(); // �׵θ� ������
            return;
        }

        // ������ ����
        DevaEnergyBarUI.ReduceEnergy(magicEnergyCost);

        // ���� �߻�
        GameObject fireball = Instantiate(fireballPrefab, firePoint.position, Quaternion.identity);
        Rigidbody2D rb = fireball.GetComponent<Rigidbody2D>();

        SpriteRenderer playerSprite = GetComponent<SpriteRenderer>();
        float direction = playerSprite.flipX ? -1f : 1f;
        rb.velocity = new Vector2(fireballSpeed * direction, 0);
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
        if (magicLight != null)
        {
            magicLight.enabled = false;
        }
    }
}
