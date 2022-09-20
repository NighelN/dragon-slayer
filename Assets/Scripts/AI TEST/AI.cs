using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AI : MonoBehaviour
{
    public float speed;
    public GameObject projectile;

    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;
    private GameObject player;
    private EnemyController enemyController;
    private bool inRange;
    private float turnDelay;
    private float shootingCooldown = 3f;
    private float currentShootingCooldown;
    private IEnumerator ChargeTime;
    private string state;
    private bool shouldMove = true;
    

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        enemyController = GetComponent<EnemyController>();
        player = GameObject.Find("Player");
        ChargeTime = ChargeShot(1f);
    }

    // Update is called once per frame
    void Update()
    {
        rb.velocity = new Vector2(0, 0);
        Debug.Log("LOS state of " + gameObject.name.ToString() + " is " + inRange.ToString());
        if (inRange)
        {
            state = "Chase";
            
        }
        else
        {
            state = "Patrol";
            
        }
        switch(state)
        {
            case "Chase":
                Chase();
                break;
            case "Patrol":
                Patrol();
                break;
            default:
                break;
                
        }
        if (Vector2.Distance(player.transform.position, transform.position) < 5)
        {
            HandleRaycast();
        }
        if (currentShootingCooldown > 0f)
        {
            currentShootingCooldown -= Time.deltaTime;
        }
    }

    private IEnumerator ChargeShot(float ChargeTime)
    {
        shouldMove = false;
        yield return new WaitForSeconds(ChargeTime);
        {
            Instantiate(projectile, this.transform.position, Quaternion.identity);
            shouldMove = true;
        }
    }

    private void HandleShooting()
    {
        if (currentShootingCooldown <= 0f)
        {
            currentShootingCooldown = shootingCooldown;
            StartCoroutine(ChargeShot(0.7f));
        }
    }

    private void Patrol()
    {
        rb.velocity = new Vector2(0, 0);
        Move();
    }

    private void Move()
    {
        if (shouldMove == true)
        {
            Debug.Log("Should move");
            rb.velocity = new Vector2((spriteRenderer.flipX ? -1 : 1) * speed, rb.velocity.y);
        }
        
    }

    private void Chase()
    {
        if (player.transform.position.x > transform.position.x)
        {
            spriteRenderer.flipX = false;
            Move();
        }
        else if (player.transform.position.x < transform.position.x)
        {
            spriteRenderer.flipX = true;
            Move();
        }
        HandleShooting();
        
    }


    private void HandleRaycast()
    {
        RaycastHit2D raycastHit = Physics2D.Raycast(transform.position, player.transform.position - transform.position);
        if (raycastHit.transform.gameObject.CompareTag("Player"))
        {
            inRange = true;
        }
        else inRange = false;
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.name.Equals("Sword collider"))
        {
            bool isMeleeAttacking = collision.gameObject.GetComponent<PlayerControl>().isAttacking;

            if (isMeleeAttacking)
            {
                PlayerControl control = collision.transform.parent.GetComponent<PlayerControl>();
                bool isAttacking = control.isAttacking;
                int attackDamage = control.attackDamage;
                if (isAttacking && Time.time > enemyController.invincibleTime)
                {
                    GameManager.instance.IncreaseScore(1);
                    enemyController.DecreaseHealth(attackDamage, gameObject);
                    control.isAttacking = false;
                }
            }
            return;
        }
        else if (collision.name.Equals("Player") && Time.time > GameManager.instance.invincibleTime)
        {
            GameManager.instance.DecreaseHealth();
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        //Checks if the enemy recently turned around
        if (turnDelay > Time.time)
        {
            return;
        }
        if (collision.gameObject.name.Equals("Foreground"))
        {
            spriteRenderer.flipX = !spriteRenderer.flipX;
            turnDelay = Time.time + 1;
        }
    }
}
