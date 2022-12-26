using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private float speed = 10f;
    private float jump_force = 10f;
    private float rollForce = 6.0f;
    private float fallGravityMultiplier = 2f;
    private bool in_air;
    private bool rolling = false;
    private float rollDuration = 8.0f / 14.0f;
    private float rollCurrentTime;
    //public ParticleSystem dust;

    Rigidbody2D rb;
    Animator anim;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
    }


    void Update() 
    { 
    
        // Jump
        float velocity = Input.GetAxis("Horizontal");
        if (Input.GetKey(KeyCode.Space) && !in_air && !rolling)
        {
            in_air = true;
            rb.velocity += Vector2.up * jump_force;
        }
        // Move 
        rb.velocity = new Vector2(velocity * speed, rb.velocity.y);
        // SetAnimationProperties(velocity);

        // Rotation 
        if (velocity != 0)
        {
            //CreateDust();
            transform.localScale = new Vector3(((velocity < 0) ? -2 : 2), transform.localScale.y);
        }
        //Roll
        if (Input.GetKeyDown("left shift") && !rolling && velocity != 0)
        {
            rolling = true;
            rb.velocity = new Vector2(velocity * rollForce, rb.velocity.y);
        }

        if (rolling)
        {
            rollCurrentTime += Time.deltaTime;
        }

        if (rollCurrentTime > rollDuration)
        {
            rolling = false;
            rollCurrentTime = 0;
        }
        SetAnimationProperties(velocity);
    }

    void FixedUpdate()
    {
        if (rb.velocity.y < 0)
        {
            rb.velocity += Vector2.up * Physics2D.gravity.y * fallGravityMultiplier * Time.fixedDeltaTime;
        }

    }

    void SetAnimationProperties(float velocity)
    {
        anim.SetBool("Run", velocity != 0);
        anim.SetBool("Jump", rb.velocity.y > 0);
        anim.SetBool("Fall", rb.velocity.y < 0);
        anim.SetBool("Roll", rolling);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Ground"))
        {
            in_air = false;
        }
    }

    //void CreateDust()
    //{
    //    dust.Play();
    //}

}
