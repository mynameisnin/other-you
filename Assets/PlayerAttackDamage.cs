using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttackDamage : MonoBehaviour
{
    // Start is called before the first frame update
    public int NomalAttackDamage = 20; // 기본 공격 데미지

    public int GetNomalAttackDamage()
    {
        return NomalAttackDamage;
    }
}
