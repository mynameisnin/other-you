using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AdamMovement : MonoBehaviour
{
    Rigidbody2D AdamRigidebody;
    Animator AdamAnime;
    SpriteRenderer AdamSpeite;

    public float AdamMoveSpeed = 3f;

    // Start is called before the first frame update
    void Start()
    {
        AdamRigidebody = GetComponent<Rigidbody2D>();
        AdamAnime = GetComponent<Animator>();
        AdamSpeite = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        AdamMove();
        AdamAnimation();
    }

    void AdamMove()
    {
        float hor = Input.GetAxis("Horizontal");
        AdamRigidebody.velocity = new Vector2 (hor * AdamMoveSpeed, AdamRigidebody.velocity.y);
    }
    void AdamAnimation()
    {
        float hor = Input.GetAxis("Horizontal");
        if (Mathf.Abs(hor)> 0.00f)
        {
            AdamAnime.SetBool("run", true);
        }
        else
        {
            AdamAnime.SetBool ("run", false);
        }
    }
}
