using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyDamageBumpAgainst : MonoBehaviour
{
    public int damageAmount = 10; // 플레이어에게 입힐 대미지
    void Start()
    {
        Collider2D playerCollider = GameObject.FindWithTag("Player").GetComponent<Collider2D>();
        Collider2D enemyCollider = GetComponent<Collider2D>();

        Physics2D.IgnoreCollision(enemyCollider, playerCollider, true);
    }
   
}