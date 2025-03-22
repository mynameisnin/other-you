using System.Collections;
using UnityEngine;
using UnityEngine.Rendering.Universal; // Light2D 사용

public class MagicAttack : MonoBehaviour
{
    public GameObject fireballPrefab;
    public Transform firePoint;
    public Light2D magicLight;
    public float fireballSpeed = 10f;
    public float fireballCooldown = 1.0f;
    public float magicEnergyCost = 10f; // 마법 사용 시 소모되는 에너지

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
            Debug.LogWarning("EnergyBarUI가 연결되어 있지 않습니다.");
            return;
        }

        float currentEnergy = DevaEnergyBarUI.GetCurrentEnergy();

        if (currentEnergy < magicEnergyCost)
        {
            Debug.Log("마법 실패: 에너지 부족");
            DevaEnergyBarUI.FlashBorder(); // 테두리 깜빡임
            return;
        }

        // 에너지 차감
        DevaEnergyBarUI.ReduceEnergy(magicEnergyCost);

        // 마법 발사
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
