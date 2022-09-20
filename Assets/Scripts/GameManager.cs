using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager instance = null;
    public GameObject throwableAxe;
    public GameObject playerObject, brokenCandle;
    public Image[] healthSprites;
    public Text scoreText, timeText;
    public GameObject[] pickUps;

    PlayerControl player;
    Color defaultColor;

    public int currentHealth
    {
        get;
        set;
    }

    public float invincibleTime
    {
        get;
        set;
    }

    public int currentScore
    {
        get;
        set;
    }

    public float currentTime
    {
        get;
        set;
    }

    float hitTime
    {
        get;
        set;
    }

    public GameObject throwableObject
    {
        get;
        set;
    }

    private void Awake()
    {
        //If the instance is null set the instance to this object
        if (instance == null)
            instance = this;
        //If the istance isnt this object remove it from the game
        else if (instance != this)
            Destroy(gameObject);
        //Sets the player controller
        player = playerObject.GetComponent<PlayerControl>();
        //Sets the beginning health to 3
        currentHealth = 3;
        //Sets the score to 0
        currentScore = 0;
        //Sets the time to 0
        currentTime = 0;
        //Makes it so this object isnt removed on loading a new scene
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        //Sets the default color of the player for later use (resetting the invincible time)
        defaultColor = player.spriteRenderer.color;
    }

    private void Update()
    {
        if (SceneManager.GetActiveScene().name.Equals("Lose") || SceneManager.GetActiveScene().name.Equals("TItle Screen") || SceneManager.GetActiveScene().name.Equals("Reken-OPs") || SceneManager.GetActiveScene().name.Equals("Finished")) return;
        //Incase the instance gets nulled set it with this object
        if (instance == null)
            instance = this;
        //Updates the score text
        scoreText.text = "Score: " + currentScore;
        //Updates the time text with rounding down
        timeText.text = "Time: " + Mathf.Round(currentTime);
        //If the current time is higher then the invincibleTime reset the color of the player sprite
        if (Time.time > invincibleTime && invincibleTime > 0)
        {
            player.spriteRenderer.color = defaultColor;
            invincibleTime = 0;
        }
        //If the current time is higher then the hitTime reset the animator's gotHit
        if (Time.time > hitTime && hitTime > 0)
        {
            player.animator.SetBool("gothit", false);
            hitTime = 0;
        }
        //If the time of the current animation gets above 1 AND the player's dying animation is playing load scene 0 (title screen)
        if (player.animator.GetCurrentAnimatorStateInfo(0).normalizedTime > 1 && player.animator.GetBool("dying"))
        {
            SceneManager.LoadScene("Lose");
        }
    }

    public bool isDying()
    {
        return player.animator.GetBool("dying");
    }

    private void FixedUpdate()
    {
        //Increase the timer
        currentTime += Time.deltaTime;
    }

    public void PickupThrowableAxe()
    {
        //Fills the throwable object with the throwable axe
        throwableObject = throwableAxe;
    }

    public void IncreaseScore(int increase)
    {
        //Increase the score by how much is giving though the 'increase' parameter
        currentScore += increase;
    }

    public void IncreaseHealth()
    {
        //Enables the health sprite again
        healthSprites[currentHealth].enabled = true;
        //Increases the health by one
        currentHealth++;
    }

    public void DecreaseHealth()
    {
        //If the player is dying don't decrease health
        if (player.animator.GetBool("dying")) return;
        //Removes 1 health counter
        currentHealth--;
        //Disabled one of the health sprites
        healthSprites[currentHealth].enabled = false;
        //Adds to the timer for the player being invincible
        invincibleTime = Time.time + 1;
        //Sets the timer for reseting the animator
        hitTime = Time.time + 0.2f;
        //Sets got hit to true in the animator
        player.animator.SetBool("gothit", true);
        //Changes the opacity of the player's sprite to indicate hes invincible
        player.spriteRenderer.color = new Color(1f, 1f, 1f, .5f);
        //If the player has no health left call the death method
        if (currentHealth <= 0)
        {
            HandleDeath();
        }
    }

    public void HandleDeath()
    {
        //Sets the color of the player back to default
        player.spriteRenderer.color = defaultColor;
        //Sets the got hit to false
        player.animator.SetBool("gothit", false);
        //Sets the dying to true
        player.animator.SetBool("dying", true);

    }
    public void SpawnRandomPickup(Transform _transform) 
    {
        //Calls a random pickup from the array - Anthony
        Instantiate(pickUps[Random.Range(0, pickUps.Length)],_transform.position, _transform.rotation); 
    }
    public void SpawnBrokenObject(Transform _transform, string tag)
    {
        GameObject _gameObject = null;
        switch(tag)
        {
            //If the tag of the object is candle fill the _gameObject with the brokenCancle prefab
            case "Candle":
                _gameObject = brokenCandle;
                break;
        }

        if (_gameObject == null) return;

        //Keeps looping until the int i is no longer larger than 3 - Anthony
        for (int i = 0; i < 3; i++)
        {
            _transform.TransformPoint(0, -100, 0);
            GameObject clone = Instantiate(_gameObject, _transform.position, Quaternion.identity);

            Rigidbody2D rb = clone.GetComponent<Rigidbody2D>();
            //Adds movement force in the right and upward direction - Anthony
            rb.AddForce(Vector3.right * Random.Range(-100, 50));
            rb.AddForce(Vector3.up * Random.Range(50, 150));
        }
    }

    public void ResetPosition()
    {
        //Gets the transform of the player
        Transform _t = player.GetComponent<Transform>();
        //Sets the position of the player with the last known ground position
        _t.position = new Vector3(player.lastGroundPosition.x, player.lastGroundPosition.y + .5f);
        //Decrease the health of the player
        DecreaseHealth();
    }

    public int GetAttackDamage(Collider2D collision)
    {
        int attackDamage = -1;
        //Checks if the name of the collision is the sword collider and if the player is attacking
        if (collision.name.Equals("Sword collider") && player.isAttacking)
        {
            //Sets the player attacking to false
            player.isAttacking = false;
            //Sets the attack damage based on the player's attack damage
            attackDamage = player.attackDamage;
        }
        //Checks if the name of the collision is the player and if the player isnt invincible
        else if (collision.name.Equals("Player") && Time.time > invincibleTime)
        {
            //Sets attack damage to 0 (meaning the player isnt attacking)
            attackDamage = 0;
            //Decrease the health of the player
            DecreaseHealth();
        }
        return attackDamage;
    }
}


