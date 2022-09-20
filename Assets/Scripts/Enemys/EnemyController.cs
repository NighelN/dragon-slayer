using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyController : MonoBehaviour
{

    public GameObject healthBarPrefab;
    GameObject healthBar;
    public int maxHealth;
    int currentHealth;

    public float invincibleTime
    {
        get;
        set;
    }

    float barVisible;

    private void Awake()
    {
        currentHealth = maxHealth;
        healthBar = Instantiate(healthBarPrefab, new Vector3(transform.position.x, transform.position.y + 0.55f), transform.rotation);
        healthBar.transform.parent = gameObject.transform;
        healthBar.SetActive(false);
    }

    private void Update()
    {
        //Calculates the width
        int width = (currentHealth * 29) / maxHealth;
        Transform currentHealthBar = healthBar.transform.GetChild(2);
        //Scales the green health bar based on  current health
        currentHealthBar.localScale = new Vector3(width, 3.5f);
        //Places the green bar on the correct position based on width
        currentHealthBar.transform.localPosition = new Vector3(0 + ((29 - width) * -0.008f), 0, 0);

        if (Time.time > invincibleTime && invincibleTime > 0)
        {
            invincibleTime = 0;
        }

        if (Time.time > barVisible && barVisible > 0)
        {
            barVisible = 0;
            healthBar.SetActive(false);
        }
    }

    public void DecreaseHealth(int damage, GameObject _gameObject)
    {
        //Decrease the health of the enemy (if the damage is higher then the current health decrease by current health) else damage
        currentHealth -= damage > currentHealth ? currentHealth : damage;
        //Sets the invincible time of the enemy
        invincibleTime = Time.time + 0.5f;
        //Timer for disabling the health bar
        barVisible = Time.time + 5f;
        //Shows the health bar
        healthBar.SetActive(true);
        //If health is lower then 1 delete it
        if (currentHealth <= 0)
        {
            Destroy(_gameObject);
        }
    }

}
