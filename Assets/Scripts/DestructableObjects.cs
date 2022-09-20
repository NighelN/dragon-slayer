using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestructableObjects : MonoBehaviour
{
    private void OnTriggerStay2D(Collider2D collision)
    {
        
        if (collision.name.Equals("Player"))
        {
            //Checks if the player is attacking - Anthony
            bool isAttacking = collision.gameObject.GetComponent<PlayerControl>().isAttacking;

            if (isAttacking)
            {
                int chance = Random.Range(0, 100);

                //If the player is attacking, the method SpawnBrokenObject and SpawnRandomPickup will be called from the GameManager script - Anthony
                GameManager.instance.SpawnBrokenObject(transform, gameObject.tag);
                //20% chance to spawn a random pickup
                if (chance <= 20)
                {
                    GameManager.instance.SpawnRandomPickup(gameObject.transform);
                }
                //Gets rid of the game object that's colliding with "Player" - Anthony
                Destroy(gameObject);
            }
        }
    }
}