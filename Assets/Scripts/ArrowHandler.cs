using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArrowHandler : MonoBehaviour
{
    Rigidbody2D rb;
    SpriteRenderer spriteRenderer;
    float speed = 5f;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void Start()
    {
        //Gets the arrow dispenser script
        ArrowDispenser dispenser = transform.parent.GetComponent<ArrowDispenser>();
        //Sets the flipY based on if the dispenser is shooting right or not
        spriteRenderer.flipY = !dispenser.shootRight;
    }

    void Update()
    {
        //Makes the arrow move left or right
        rb.velocity = new Vector2((spriteRenderer.flipY ? -1 : 1) * speed, rb.velocity.y);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        int attackDmg = GameManager.instance.GetAttackDamage(collision);
        //If the player isnt attacking, meaning remove the arrow
        if(attackDmg == 0)
        {
            RemoveArrow();
        }
        //Meaning the player is attacking, increase score by 100 and remove the arrow
        else if (attackDmg > 0)
        {
            GameManager.instance.IncreaseScore(100);
            RemoveArrow();
        }
    }

    //When the arrow leaves the camera, remove the arrow
    private void OnBecameInvisible()
    {
        RemoveArrow();
    }
    
    void RemoveArrow()
    {
        Destroy(gameObject);
    }
}
