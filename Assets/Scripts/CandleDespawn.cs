using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CandleDespawn : MonoBehaviour
{

    
    private void Update()
    {
        //Rotates the broken candle.
        transform.Rotate(Vector3.forward * 5);
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        //If the candle hits the floor remove it from the game
        if (collision.name.Equals("Foreground"))
        {
            Destroy(gameObject);
        }
    }
}
