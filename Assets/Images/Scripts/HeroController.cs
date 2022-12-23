using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeroController : MonoBehaviour
{
    private float speed = 15f;
    private float jump_force = 10f;
    private float fallGravityMultiplier = 2f;
    private bool in_air;

    public ParticleSystem dust;

    Rigidbody2D rb;
    Animator anim;
    
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
    }

    
    void Update()
    {
        // jump
        float velocity = Input.GetAxis("Horizontal");
        if((Input.GetKey(KeyCode.UpArrow) && !in_air) || (Input.GetKey(KeyCode.W) && !in_air))
        {
            in_air = true;
            rb.velocity += Vector2.up * jump_force;
        }
        rb.velocity = new Vector2(velocity * speed, rb.velocity.y);
        SetAnimationProperties(velocity);

        // rotation 
        if(velocity != 0)
        {
            CreateDust();
            transform.localScale = new Vector3(((velocity < 0) ? -7:7), transform.localScale.y ) ;
        }
    }

    void FixedUpdate()
    {
        if(rb.velocity.y < 0)
        {
            rb.velocity += Vector2.up * Physics2D.gravity.y * fallGravityMultiplier * Time.fixedDeltaTime;
        }

    }

    void SetAnimationProperties(float velocity)
    {
        anim.SetBool("Run", velocity !=0);
        anim.SetBool("Jump", rb.velocity.y > 0);
        anim.SetBool("Fall", rb.velocity.y < 0);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.layer == LayerMask.NameToLayer("Ground"))
        {
            in_air = false;
        }
    }
    
    void CreateDust()
    {
        dust.Play();
    }

}
