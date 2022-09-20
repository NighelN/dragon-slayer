using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AxeMovement : MonoBehaviour
{

    public int attackDamage = 1;
    float speed = 4, rotateTimer;
    Rigidbody2D rb;
    Quaternion originalRotation;
    public GameObject spawnedObject;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void Start()
    {
        //Makes the axe move to the right or left
        rb.velocity = transform.right * speed;
        //Grabs the original rotation
        originalRotation = transform.rotation;
    }

    private void LateUpdate()
    {
        //Handles a rotation timer for the axe
        if (Time.time > rotateTimer)
        {
            //Rotates the axe
            transform.Rotate(Vector3.forward * -90);
            //Sets the timer for rotating
            rotateTimer = Time.time + 0.15f;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        //Checks if the axe hit the foreground
        if(collision.collider.name.Equals("Foreground"))
        {
            DestroyAxe();
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        //Checks if the axe hit an enemy
        if (collision.gameObject.tag.Equals("Enemy"))
        {
            DestroyAxe();
            //Grabs the enemy you hit with the axe
            EnemyController controller = collision.GetComponent<EnemyController>();
            //Decrease the health of the enemy
            controller.DecreaseHealth(attackDamage, collision.gameObject);
        }
        //Checks if the axe hit an candle
        else if(collision.gameObject.tag.Equals("Candle"))
        {
            //Spawns the broken candles
            GameManager.instance.SpawnBrokenObject(gameObject.transform, collision.gameObject.tag);
            //Spawns a random pickup
            GameManager.instance.SpawnRandomPickup(collision.gameObject.transform);
            //Destroy the candle
            Destroy(collision.gameObject);
        }
    }

    void DestroyAxe()
    {
        //Destroy the axe
        Destroy(gameObject);
        //Spawns the pickable version of the axe
        Instantiate(spawnedObject, transform.position, originalRotation);
    }

}
