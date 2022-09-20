using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    private Transform target;
    public GameObject player;
    public GameObject explosion;
    private List<GameObject> items = new List<GameObject>();

    private void Start()
    {
        player = GameObject.Find("Player");
        target = player.transform;
        //face the target
        transform.up = target.position - transform.position;
    }

    private void Update()
    {
        Accelerate();
    }

    private void Accelerate()
    {
        this.gameObject.GetComponent<Rigidbody2D>().AddForce(transform.up * 1.2f);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!items.Contains(other.gameObject))
        {
            items.Add(other.gameObject);
        }
        for (int i = 0; i < items.Count; i++)
        {
            if (items[i].gameObject.CompareTag("Foreground"))
            {
                Explode();
            }
            if (items[i].gameObject.CompareTag("Player"))
            {
                GameManager.instance.DecreaseHealth();
                Explode();
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (items.Contains(other.gameObject))
        {
            items.Remove(other.gameObject);
        }
    }

    private void Explode()
    {
        Instantiate(explosion, this.transform.position, Quaternion.identity);
        Destroy(this.gameObject);
    }
}
