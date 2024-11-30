using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DevaMovement : MonoBehaviour
{
    Rigidbody2D DevaRigidbody;
    Animator DevaAnime;
    SpriteRenderer DevaSprite;

    public float DevaMoveSpeed = 3f;
    // Start is called before the first frame update
    void Start()
    {
        DevaRigidbody = GetComponent<Rigidbody2D>();
        DevaAnime = GetComponent<Animator>();
        DevaSprite = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        DevaMove();
        DevaAnimation();
        DevaSpriteRender();
    }
    void DevaMove()
    {
        float hor = Input.GetAxis("Horizontal");
        DevaRigidbody.velocity = new Vector2 (hor * DevaMoveSpeed, DevaRigidbody.velocity.y);
    }
    void DevaAnimation()
    {
        float hor = Input.GetAxis("Horizontal");
        if(Mathf.Abs(hor) > 0.00f)
        {
            DevaAnime.SetBool("run", true);
        }
        else
        {
            DevaAnime.SetBool("run", false);
        }

    }
    void DevaSpriteRender()
    {
        if (Input.GetButton("Horizontal"))
        {
            DevaSprite.flipX = Input.GetAxisRaw("Horizontal") == -1;
        }
    }
}
