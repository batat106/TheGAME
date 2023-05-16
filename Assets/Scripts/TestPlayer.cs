using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestPlayer : MonoBehaviour
{
    Rigidbody2D rb2d;

    [Header("Background Variables")]
    public float speed = 5.0f;
    public float maxSpeed = 10.0f;
    public float jumpStrength = 10.0f;
    public float friction = 2.5f;

    public bool isGrounded = false;
    public int walling = 0; // 0 = no wall collision; 1 = left side collision; 2 = right side collision

    Animator anim;

    void Start()
    {
        rb2d = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
    }

    void LateUpdate()
    {
        float velocity = Input.GetAxis("Horizontal");

        if (rb2d.velocity.x > maxSpeed)
        {
            rb2d.velocity = new Vector2(maxSpeed, rb2d.velocity.y);
        }
        else if (rb2d.velocity.x < -maxSpeed)
        {
            rb2d.velocity = new Vector2(-maxSpeed, rb2d.velocity.y);
        }

        if (Input.GetAxisRaw("Horizontal") != 0.0f)
        {
            rb2d.AddForce(speed * Vector2.right * Input.GetAxisRaw("Horizontal"), ForceMode2D.Impulse);
        }
        else
        {
            rb2d.velocity = Vector2.Lerp(rb2d.velocity, new Vector2(0.0f, rb2d.velocity.y), friction);
        }

        if (Input.GetButtonDown("Jump"))
        {
            rb2d.velocity = new Vector2(rb2d.velocity.x, jumpStrength);
        }
        SetAnimationProperties(velocity);
    }
    void SetAnimationProperties(float velocity)
    {
        anim.SetBool("Run", velocity != 0);
    }
}