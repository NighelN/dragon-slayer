using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArrowDispenser : MonoBehaviour
{
    public GameObject arrow;
    public bool shootRight = true;
    GameObject newArrow;
    float dispenseTimer;
    Transform startPos;

    private void Awake()
    {
        //Gets the transform from the first child
        startPos = transform.GetChild(0).transform;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        HandleShooting(collision);
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        HandleShooting(collision);
    }

    void HandleShooting(Collider2D collision)
    {
        //Checks if the player entered the invisible collider for shooting an arrow
        if (collision.tag.Equals("Player") && Time.time > dispenseTimer)
        {
            //Sets a new arrow
            newArrow = Instantiate(arrow, startPos.position, startPos.rotation);
            //Sets the parent of the arrow
            newArrow.transform.parent = this.transform;
            //Sets the delay for shooting
            dispenseTimer = Time.time + 2.5f;
        }
    }
}
