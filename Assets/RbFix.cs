using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RbFix : MonoBehaviour
{
    public Rigidbody2D rb;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
    }
}
