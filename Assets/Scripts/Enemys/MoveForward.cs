using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveForward : MonoBehaviour
{
    public float speed;
    float turnDelay;
    Rigidbody2D rb;
    SpriteRenderer spriteRenderer;
    EnemyController enemyController;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        enemyController = GetComponent<EnemyController>();
    }

    // Update is called once per frame
    void Update()
    {
        rb.velocity = new Vector2((spriteRenderer.flipX ? -1 : 1) * speed, rb.velocity.y);
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        int attackDmg = GameManager.instance.GetAttackDamage(collision);
        //If attack damage is higher then 0 increase score and decrease enemyys health
        if (attackDmg > 0 && Time.time > enemyController.invincibleTime)
        {
            GameManager.instance.IncreaseScore(1);
            enemyController.DecreaseHealth(attackDmg, gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        //Checks if the enemy recently turned around
        if (turnDelay > Time.time)
        {
            return;
        }
        //If enemy hits the foreground or invisible turn around collision
        if (collision.gameObject.name.Equals("Foreground") || collision.gameObject.tag.Equals("Turnaround")) //Turnaround
        {
            spriteRenderer.flipX = !spriteRenderer.flipX;
            turnDelay = Time.time + 1;
        }
    }
}