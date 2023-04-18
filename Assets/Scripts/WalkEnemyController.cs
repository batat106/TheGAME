using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Diagnostics;
using TMPro;

public class WalkEnemyController : MonoBehaviour
{
    public GameObject player;
    public TextMeshProUGUI livesText;
    public PlayerController playerController;
    public int enemyLive;
    public float distanceToPlayer;
    public float attackDistance;

    public float leftPoint;
    public float rightPoint;
    public float waitTimeLeftPoint;
    public float waitTimeRightPoint;

    private Dictionary<int, (string type, float value)> moveScript = new Dictionary<int, (string, float)>();
    private Stopwatch waitStartTime = new Stopwatch();
    private int currentStep;
    private int currentDirection;
    private (string type, float value) currentType;

    private float speed = 3f;
    private float attackSpeed = 5f;
    private float xScale;

    private Rigidbody2D rb;
    private Animator anim;

    //Attack
    public float cooldown = 2f;
    private float lastAttackedAt = -9999f;

    void Start()
    {
        FillMovingScriptList();
        FillStartDirection();

        rb = GetComponent<Rigidbody2D>();
        anim = transform.GetChild(0).GetComponent<Animator>();
        xScale = transform.localScale.x;
        enemyLive = (enemyLive > 0) ? enemyLive : 3;
        livesText.text = "HP: " + enemyLive;   
    }

    
    void Update()
    {
        if (!CanUpdate())
        {
            return;
        }
        if (anim.GetBool("Attack"))
        {
            CheckAttackTimeOut();
            return;
        }
        CheckPlayerDestination();
        if (currentType.type == "Attack" && playerController.live > 0)
        {
            PlayAttackScript();
        }
        else
        {
            PlayMoveScript();
        }
    }
    
    bool CanUpdate()
    {
        if (anim.GetBool("Damage"))
            return false;
        if (enemyLive == 0)
        {
           Destroy(transform.gameObject);
        }
        return true;
    }

    // bel to start
    void FillMovingScriptList()
    {
        moveScript[0] = ("Move", rightPoint = transform.position.x + ((rightPoint != 0) ? rightPoint : 10f));
        moveScript[1] = ("Wait", waitTimeRightPoint = (waitTimeRightPoint != 0) ? waitTimeRightPoint : 2f);
        moveScript[2] = ("Move", leftPoint = transform.position.x + ((leftPoint != 0) ? leftPoint : -10f));
        moveScript[3] = ("Wait", waitTimeLeftPoint = (waitTimeLeftPoint != 0) ? waitTimeLeftPoint : 2f);

        distanceToPlayer = (distanceToPlayer > 0) ? distanceToPlayer : 12f;
        attackDistance = (attackDistance > 0) ? attackDistance : 5f;
    }
    
    void FillStartDirection()
    {
        currentStep = 0;
        currentType.type = "Walk";
        currentDirection = 1;
    }
    //
    
    void PlayMoveScript()
    {
        if (moveScript[currentStep].type == "Move")
        {
            MoveEnemyToScriptPoint();
        } else if (moveScript[currentStep].type == "Wait")
        {
            EnemyWaitOnPoint();
        }
    }
    
    void MoveEnemyToScriptPoint()
    {
        if ((currentDirection > 0) ? (transform.position.x >= moveScript[currentStep].value) : (transform.position.x <= moveScript[currentStep].value))
        {
            anim.SetBool("Run", false);
            ChangeWalkState();
            return;
        }
        Move();
    }

    void Move()
    {
        float current_speed = (currentType.type == "Attack") ? attackSpeed : speed;
        transform.localScale = new Vector3(-xScale * currentDirection, transform.localScale.y);
        rb.velocity = new Vector2(currentDirection * current_speed, rb.velocity.y);
        //livesText.transform.localScale = new Vector3(currentDirection, livesText.transform.localScale.y);
        SetAnimationProperties();
    }

    void EnemyWaitOnPoint()
    {
        if (waitStartTime.Elapsed.TotalSeconds >= moveScript[currentStep].value)
        {
            waitStartTime.Stop();
            ChangeWalkState();
        }
    }

    void ChangeWalkState()
    {
        currentStep = (currentStep < moveScript.Count - 1) ? currentStep + 1 : 0;

        if (moveScript[currentStep].type == "Wait")
            waitStartTime.Restart();
        if (moveScript[currentStep].type == "Move")
            currentDirection *= -1;
    }
    
    private void SetAnimationProperties()
    {
        anim.SetBool("Run", rb.velocity.x != 0);
        anim.SetBool("Damage", false);
        anim.SetBool("Attack", false);
    }

    void CheckPlayerDestination()
    {
        float distance = player.transform.position.x - transform.position.x;
        if (Mathf.Abs(distance) <= distanceToPlayer)
        {
            currentType = (type: "Attack", value: player.transform.position.x);
        } else if (currentType.type != "Walk")
        {
            FillStartDirection();
        }
    }

    void PlayAttackScript() 
    { 
    
        float distation = currentType.value - transform.position.x;
        float point = (distation > 0) ? ((currentType.value <= moveScript[0].value) ? currentType.value - attackDistance : moveScript[0].value) : ((currentType.value >= moveScript[2].value) ? currentType.value + attackDistance : moveScript[2].value);

        currentDirection = (distation > 0) ? 1 : -1;
        if ((currentDirection > 0) ? (transform.position.x >= point) : (transform.position.x <= point))
        {
            transform.localScale = new Vector3(-xScale * currentDirection, transform.localScale.y);
            anim.SetBool("Run", false);
            if (Mathf.Abs(distation) <= attackDistance)
            {
                if (Time.time > lastAttackedAt + cooldown)
                {
                    anim.SetBool("Attack", true);
                    lastAttackedAt = Time.time;
                }
                
            }
            else
            {
                anim.SetBool("Attack", false);
            }
            return;
        }
        Move();
    }

    //IEnumerator AttackReload()
    //{
    //    anim.SetBool("Run", false);
    //    yield return new WaitForSeconds(5);
    //}
    
    void CheckAttackTimeOut()
    {
        if (anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1)
            anim.SetBool("Attack", false);
    }
    
    void OnTriggerEnter2D(Collider2D collider)
    {
        if (!collider.IsTouching(transform.GetComponent<CapsuleCollider2D>()))
            return;
        if (collider.gameObject.name == "DeathPit")
            enemyLive = 0;
        if (collider.gameObject.name == "DamageZone" && !anim.GetBool("Damage"))
        {
            enemyLive--;
            anim.SetBool("Damage", true);
            int damageDirection = ((transform.position.x - collider.transform.position.x) > 0) ? -1 : 1;
            rb.velocity = new Vector2(7f * damageDirection, 3f);
            livesText.text = "HP: " + enemyLive;
        }
    }
    
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Ground") && anim.GetBool("Damage")) 
            anim.SetBool("Damage", false);
    }
}
