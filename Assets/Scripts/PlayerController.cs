using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public int live = 3;
    private (float x, float y) respawn_pos;
    
    private float speed = 10f;
    private float jump_force = 10f;
    private float rollForce = 6.0f;
    private float fallGravityMultiplier = 2f;

    private int currentAttack = 0;
    private float timeSinceAttack = 0.0f;
    
    private bool  in_air;
    //private bool attack = false;
    
    private bool  rolling = false;
    private float rollDuration = 8.0f / 14.0f;
    private float rollCurrentTime;
    //public ParticleSystem dust;

    private bool  damage;
    private bool  can_start_game;

    // Attack delay
    public float cooldown = 1f;
    private float lastAttackedAt = -9999f;

    //test
    public GameObject enemy;
    
    
    Rigidbody2D rb;
    Animator anim;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();

        respawn_pos.x = transform.position.x;
        respawn_pos.y = transform.position.y;

        damage = false;
        can_start_game = true;
    }

    void Update() 
    {
        if (live == 0)
        {
            anim.SetBool("Dead", true);
            GoToStartPoint();
            return;
        }
        if (!CanUpdate())
        {
            return;
        }
        
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
        // Attack (!in_air 
        timeSinceAttack += Time.deltaTime;
        
        if (Input.GetMouseButtonDown(0))
        {
            //GetComponent<Animator>().Play("Player_Attack");
            //attack = true;
            //anim.Play("Player_Attack1");
            currentAttack++;
            anim.SetBool("Attack", true);

            currentAttack = ((currentAttack > 3) ? 1 : currentAttack);

            if (timeSinceAttack > 1.0f)
                currentAttack = 1;
            if (Time.time > lastAttackedAt + cooldown)
            {
                anim.Play("Player_Attack" + currentAttack);
                lastAttackedAt += cooldown;
                timeSinceAttack = 0.0f;
            }
            
        }
        else
        {
            SetAnimationProperties(velocity);
        }

        if (live == 0)
        {
            GoToStartPoint();
            return;
        }
        
        

    }

    bool CanUpdate()
    {
        if (!can_start_game)
        {
            return false;
        } else if (damage)
        {
            return false;
        } else if (live == 0)
        {
            return false;
        }
        return true;
    }
   
    //void FixedUpdate()
   //{
   //    if (rb.velocity.y < 0)
   //    {
   //        rb.velocity += Vector2.up * Physics2D.gravity.y * fallGravityMultiplier * Time.fixedDeltaTime;
   //    }                 
   //
   //}

    void GoToStartPoint()
    {
        can_start_game = false;
        anim.SetBool("Hit", false);

        rb.velocity = new Vector2(0f, 0f);
        SetAnimationProperties(rb.velocity.x);

        

        StartCoroutine(StartGameAfterTimeOut());
        
    }
    IEnumerator StartGameAfterTimeOut()
    {
        yield return new WaitForSeconds(2);
        transform.position = new Vector3(respawn_pos.x, respawn_pos.y);
        anim.SetBool("Dead", false);
        live = 3;
        damage = false;
        can_start_game = true;
    }

    void SetAnimationProperties(float velocity)
    {

        if ((anim.GetCurrentAnimatorStateInfo(0).IsName("Player_Attack1") || anim.GetCurrentAnimatorStateInfo(0).IsName("Player_Attack2") || anim.GetCurrentAnimatorStateInfo(0).IsName("Player_Attack3")) && anim.GetCurrentAnimatorStateInfo(0).normalizedTime < 1f) // 
        {
            return;
        }
        anim.SetBool("Attack", false);
        anim.SetBool("Run", velocity != 0);
        anim.SetBool("Jump", rb.velocity.y > 0.1);
        anim.SetBool("Fall", rb.velocity.y < -0.1);
        anim.SetBool("Roll", rolling);
        anim.SetBool("Hit", false);
        
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Ground"))
        {
            in_air = false;
            if (damage)
            {
                damage = false;
                anim.SetBool("Hit", false);
            }
        }
    }
    
    void OnTriggerEnter2D(Collider2D collider)
    {
        if (!collider.IsTouching(transform.GetComponent<CapsuleCollider2D>()))
            return;
        if (collider.gameObject.name == "DeathPit")
        {
            live = 0;
        }
        if (collider.gameObject.name == "Trap" && !damage)
        {
            live--;
            damage = true;
            anim.SetBool("Hit", true);
            rb.velocity = new Vector2((transform.localScale.x > 0) ? -10f : 10f, 5f);
        }
        if (collider.gameObject.name == "AttackZone" && !damage)
        {
            float distance = transform.position.x - enemy.transform.position.x; 
            live--;
            damage = true;
            anim.SetBool("Hit", true);
            rb.velocity = new Vector2((distance > 0) ? 7f : -7f, 5f);
            //rb.velocity = new Vector2((transform.localScale.x > 0) ? -7f : 7f, 5f);
        }
    }
    //void CreateDust()
    //{
    //    dust.Play();
    //}

}
