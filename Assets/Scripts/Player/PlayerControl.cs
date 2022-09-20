using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerControl : MonoBehaviour
{
    Transform groundCheck;
    float speed = 2, jumpForce = 235, groundCheckRadius = 0.105f, dashSpeed = 100;
    public bool isAttacking = false;
    public int attackDamage = 1;

    Rigidbody2D rb;
    public SpriteRenderer spriteRenderer;
    public Animator animator;
    public Vector3 lastGroundPosition;
    InputManager inputManager;

    float attackTimer, attackReset, throwTimer, dashTimer, maxDash = 0.5f, dashCooldownTimer = 2f, rotationTime, groundCheckTimer;
    bool isGrounded, isThrowing, isClimbing, isDashing, canAttack;
    PlatformEffector2D onPlatform;
    bool dashCooldown;

    private void Awake()
    {
        //Initialize the ground check component(Transform)
        groundCheck = transform.GetChild(0).GetComponent<Transform>();
        //Initialize the rigidbody component
        rb = GetComponent<Rigidbody2D>();
        //Initialize the sprite renderer component
        spriteRenderer = GetComponent<SpriteRenderer>();
        //Initialize the animator component
        animator = GetComponent<Animator>();
        //Initialize the input manager component
        inputManager = GetComponent<InputManager>();
    }

    private void Update()
    {
        //If the player is dying disable input
        if (GameManager.instance.isDying())
        {
            rb.velocity = Vector2.zero;
            return;
        }

        HandleMovement();

        HandleJumping();

        HandleAttacking();

        HandleThrowing();

        HandleClimbing();

        HandleDash();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        //Checks if the player made a collision with the health pickup
        if(collision.tag.Equals("Health") && GameManager.instance.currentHealth < 3)
        {
            //Increase the health of the player
            GameManager.instance.IncreaseHealth();
            //Removes the pickable object from the map
            Destroy(collision.gameObject);
        }
        //Checks if the player made a collision with the throwable axe 'Was a sword first'
        if(collision.tag.Equals("Throwing sword"))
        {
            //Picksup the throwable axe
            GameManager.instance.PickupThrowableAxe();
            //Removes the pickable object from the map
            Destroy(collision.gameObject);
        }
    }

    
    void HandleMovement()
    {
        //Handles the regular movement of the player (left and right)
        rb.velocity = new Vector2(inputManager.horizontalMovement * speed, rb.velocity.y);
        //Checks if the player is grounded so it can update the animator for the walking animation
        if (isGrounded)
        {
            //Sets the walking animation in the animator
            animator.SetFloat("speed", Mathf.Abs(inputManager.horizontalMovement));
        }
        //Checks if the player is moving right and the player is looking left or checks if the player is moving right and is looking right 
        if (inputManager.horizontalMovement > 0.01f && spriteRenderer.flipX || inputManager.horizontalMovement < -0.01f && !spriteRenderer.flipX)
        {
            //Flips the players sprite
            spriteRenderer.flipX = !spriteRenderer.flipX;
            //Updates the throwing position
            FlipThrowPosition();
            //Updates the sword collider
            UpdateSwordCollider();
        }
    }

    void HandleJumping()
    {
        //Checks if an invisible box overlaps the solid layer
        isGrounded = Physics2D.OverlapBox(new Vector3(groundCheck.position.x + (spriteRenderer.flipX ? 0.12f : 0f), groundCheck.position.y), new Vector2(0.29f, 0.105f), groundCheckRadius, 1 << LayerMask.NameToLayer("Solid")); //0.36f
        //Checks if the player is grounded, not dashing, not climbing, not moving and the current time is higher then the last ground position
        if (isGrounded && !isDashing && !isClimbing && inputManager.horizontalMovement != 0 && Time.time > groundCheckTimer)
        {
            //Sets the last ground position
            lastGroundPosition = groundCheck.position;
            //Sets the last ground check timer
            groundCheckTimer = Time.time + .75f;
        }
        //Checks if the jump button is pressed and is grounded
        if (inputManager.aButtonPressed && isGrounded)
        {
            rb.AddForce(Vector2.up * jumpForce);
        }
        //Updates the animator for jumping
        animator.SetBool("jumping", !isGrounded);
    }

    void HandleAttacking()
    {
        //Checks if the attack button is pressed and the player can attack, also makes sure the player isnt climbing or dashing
        if (inputManager.bButtonPressed && canAttack && !isClimbing && !isDashing)
        {
            //Set attacking to true
            isAttacking = true;
            //Updates the animator
            animator.SetBool("attacking", isAttacking);
            //Makes it so the player cannot attack anymore
            canAttack = false;
            //Attack animation reset
            attackReset = Time.time + 0.325f;
            //Players attack timer
            attackTimer = Time.time + 0.65f;
        }
        //Checks if the current time is higher then the attack reset
        if(Time.time > attackReset)
        {
            //Sets attacking to false
            isAttacking = false;
            //Updates the animator for attacking
            animator.SetBool("attacking", isAttacking);
        }
        //Checks if the current time is higher then attack timer
        if(Time.time > attackTimer)
        {
            //Makes it so the player can attack again
            canAttack = true;
        }
    }

    void HandleThrowing()
    {
        //Checks if the player has a throwable
        bool hasThrowable = GameManager.instance.throwableObject != null;
        //Checks if the throw button is pessed and the player has a throwable and isnt throwing already and isnt climbing
        if (inputManager.xButtonPressed && hasThrowable && !isThrowing && !isClimbing)
        {
            //Spawn the axe
            Instantiate(GameManager.instance.throwableObject, transform.GetChild(1).transform.position, transform.GetChild(1).transform.rotation);
            //Makes it so the player does not have a throwable object anymore
            GameManager.instance.throwableObject = null;
            //Sets throwing to true
            isThrowing = true;
            //Sets the throwing reset timer
            throwTimer = Time.time + 0.3f;
            //Updates the animator to true
            animator.SetBool("throwing", isThrowing);
        }
        //Checks if the current time is higher then the throwing timer
        if (Time.time > throwTimer)
        {
            //Set throwing to false
            isThrowing = false;
            //Updates the animator to false
            animator.SetBool("throwing", isThrowing);
        }
    }

    void HandleClimbing()
    {
        //Checks if the user is climbing based on the animator
        isClimbing = animator.GetBool("climbing") || animator.GetBool("climbing_idle");
        //Checks if the player is hitting the ladder or the ladder top
        RaycastHit2D hitLadder = Physics2D.Raycast(groundCheck.position, Vector2.up, groundCheckRadius, 1 << LayerMask.NameToLayer("Ladders")), ladderTop = Physics2D.Raycast(groundCheck.position, Vector2.down, groundCheckRadius, 1 << LayerMask.NameToLayer("Solid"));

        //Checks if the player is colliding with the top of the ladder
        if(ladderTop.collider != null && ladderTop.collider.gameObject.tag.Equals("LadderTop"))
        {
            //Grabs the platform effector the player is standing on
            PlatformEffector2D platform = ladderTop.collider.GetComponent<PlatformEffector2D>();
            //If the player is actually standing on a platform
            if(platform != null)
            {
                //Set the platform the player is standing on for later use
                onPlatform = platform;
                //Checks if the player is going down
                if (inputManager.verticalMovement < 0)
                {
                    platform.rotationalOffset = 180;
                    rotationTime = Time.time + 0.5f;
                }
            }
        }
        //Checks if the platform the player was on isnt null and the the rotation time is lower then time or the player is moving up
        else if(onPlatform != null && (Time.time > rotationTime || inputManager.verticalMovement > 0))
        {
            //Reset the offset of the platformer
            onPlatform.rotationalOffset = 0;
            //Sets the platform the player is on to null
            onPlatform = null;
        }

        //Handles if the player is colliding with the regular ladders
        if (hitLadder.collider != null)
        {
            //Checks if the joystick of a joycon is held down in a specific way to get rid of a bug
            if (Mathf.Abs(inputManager.verticalMovement) >= 0.7071f && Mathf.Abs(inputManager.horizontalMovement) >= 0.7071f && isGrounded)
            {
                return;
            }
            //Checks if the player is moving down and is grounded
            if (inputManager.verticalMovement < 0 && isGrounded)
            {
                //Sets climbing & climbing idle to false
                animator.SetBool("climbing", false);
                animator.SetBool("climbing_idle", false);
            }
            //Checks if the player is going down or up
            else if (inputManager.verticalMovement != 0)
            {
                //Sets climbing to true & idle to false
                animator.SetBool("climbing", true);
                animator.SetBool("climbing_idle", false);
                //Sets the velocity for the movement
                rb.velocity = inputManager.verticalMovement > 0 ? Vector2.up : Vector2.down;
                //Sets gravity scale to 0
                rb.gravityScale = 0;
            }
            //Checks if the player isnt grounded and isnt moving and the gravityscale is 0
            else if (!isGrounded && inputManager.horizontalMovement == 0 && rb.gravityScale == 0)
            {
                //Sets velocity to zero
                rb.velocity = Vector2.zero;
                //Sets climbing to idle
                animator.SetBool("climbing_idle", true);
            }
        }
        //If the player is not hitting a ladder
        else
        {
            //Sets climbing to true & idle to false
            animator.SetBool("climbing", false);
            animator.SetBool("climbing_idle", false);
            //Reset the gravity scale
            rb.gravityScale = 1;
        }
    }

    void HandleDash()
    {
        //Checks if the dash button is held, isnt dashing, has no dash cooldown and isnt climbing
        if (inputManager.rbButtonHeld && !isDashing && !dashCooldown && !isClimbing)
        {
            //Checks if the player is attacking
            if(isAttacking)
            {
                //Sets attacking to false
                isAttacking = false;
                //Sets the attack animation to false
                animator.SetBool("attacking", isAttacking);
            }
            //Sets dashing to true
            isDashing = true;
            //Sets the dashing animation to true
            animator.SetBool("dashing", isDashing);
        }
        //If the player is dashing and isnt holding the dash button
        if(isDashing && !inputManager.rbButtonHeld && dashTimer >= 0.2f)
        {
            SetDashingCooldown();
        }
        //Checks if the player is dashing
        if(isDashing)
        {
            //Applies velocity to the player
            rb.velocity = new Vector2((spriteRenderer.flipX ? -1 : 1) * 5, 0);
            //Adds the time to the dash timer
            dashTimer += Time.deltaTime;
            //Checks if the current dash time is higher
            if(dashTimer >= maxDash)
            {
                SetDashingCooldown();
            }
        }
        //Checks if the player has a dash cooldown
        if(dashCooldown)
        {
            //Remove time from the dash cooldown
            dashTimer -= Time.deltaTime;
            //If the cool down is done set the cooldown to false
            if(dashTimer <= 0)
            {
                dashTimer = 0;
                dashCooldown = false;
            }
        }
    }

    void SetDashingCooldown()
    {
        //Sets the cooldown timer for the dash
        dashTimer = dashCooldownTimer;
        //Sets the dashing to false
        isDashing = false;
        //Sets the dash on cooldown
        dashCooldown = true;
        //Sets the dashing animation to false
        animator.SetBool("dashing", isDashing);
    }

    void UpdateSwordCollider()
    {
        //Updates the sword collider position
        transform.GetChild(2).localPosition = new Vector3(!spriteRenderer.flipX ? 0.1f : -0.097f, -0.127f, 0f);
    }

    void FlipThrowPosition()
    {
        //Changes the throwing positions of the axe
        transform.GetChild(1).localPosition = new Vector3(!spriteRenderer.flipX ? 0.193f : -0.193f, -0.074f, 0f);
        //Rotates the throwing rotation
        transform.GetChild(1).transform.Rotate(0f, 180f, 0f);
    }
}
