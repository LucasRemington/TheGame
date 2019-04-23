using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimator : MonoBehaviour {

    [HideInInspector] public Animator playerAnim; //The animator attached to the player.
    [HideInInspector] public SpriteRenderer playerSprite; //The sprite renderer attached to the player.
    [HideInInspector] public Rigidbody2D playerRGBD; //The rigidbody2d attached to the player.
    [HideInInspector] public CapsuleCollider2D playerCollider; //The collider2d attached to the player.
    [HideInInspector] public Vector3 playerPosition; //The current position of the player object.
    [HideInInspector] public GameObject playerDamageLeft;
    [HideInInspector] public GameObject playerDamageRight;
    public float thrust; //Variable that multiplies force applied to the player.
    public float attackingThrustSlowFactor;
    [HideInInspector] public float attackingVelCap;
    public float xVelocityCap; //Variable that 'caps' velocity once it goes over a certain threshhold: currently only applies to x axis. 
    [HideInInspector] public ScriptManager scriptManager; //A script that holds all the other scripts, for easy reference.
    [HideInInspector] public bool playerFalling; //True when the player is falling
    [HideInInspector] public bool fallTimerCheck; //True when the coroutine checking for falling is running
    public float fallTime; //Set to determine how long it takes for the fall animation to begin playing
    [HideInInspector] public bool playerAttacking; //true when the attack animation is playing: unset through animation events
    [HideInInspector] public bool canMove = false;

    private void Start() //Grabs appropriate references.
    {
        playerAnim = this.GetComponent<Animator>();
        playerRGBD= this.GetComponent<Rigidbody2D>();
        playerCollider = this.GetComponent<CapsuleCollider2D>();
        playerSprite = this.GetComponent<SpriteRenderer>();
        scriptManager = GameObject.FindWithTag("ScriptManager").GetComponent<ScriptManager>();
        playerDamageLeft = GameObject.Find("PlayerDamageLeft");
        playerDamageRight = GameObject.Find("PlayerDamageRight");
        attackingVelCap = xVelocityCap / attackingThrustSlowFactor;
        stopDealingDamage();
    }

    void LateUpdate () //This moves the player, and sets the appropriate bools in the animator. 
    {

        if (canMove == true)
        {
            if (Input.GetButton("Vertical") && scriptManager.selectObjects.holdingSelf == false && scriptManager.selectObjects.isTalking == false && playerAttacking == false)
            {
                playerAttacking = true;
                xVelocityCap = attackingVelCap;
                playerAnim.SetTrigger("Attack");
            }
            if (Input.GetButton("Horizontal") && Input.GetAxisRaw("Horizontal") > 0 && scriptManager.selectObjects.holdingSelf == false && scriptManager.selectObjects.isTalking == false)
            {
                playerAnim.SetBool("Walking", true);
                playerSprite.flipX = false;
                playerRGBD.AddRelativeForce(Vector3.right * thrust);
            }
            else if (Input.GetButton("Horizontal") && Input.GetAxisRaw("Horizontal") < 0 && scriptManager.selectObjects.holdingSelf == false && scriptManager.selectObjects.isTalking == false)
            {
                playerAnim.SetBool("Walking", true);
                playerSprite.flipX = true;
                playerRGBD.AddRelativeForce(Vector3.left * thrust);
            }
            else
            {
                playerAnim.SetBool("Walking", false);
                playerRGBD.velocity = new Vector2(0, playerRGBD.velocity.y);
            }
        }
        if (playerRGBD.velocity.y < -1 && playerAttacking == false)
        {
            if (fallTimerCheck == false && playerFalling == false)
            {
                StartCoroutine(fallTimer());
            }
            if (playerFalling == true)
            {
                playerAnim.SetBool("Falling", true);
            }
        }
        else
        {
            playerFalling = false;
            fallTimerCheck = false;
            playerAnim.SetBool("Falling", false);
        }

        if (playerRGBD.velocity.x > xVelocityCap && scriptManager.selectObjects.holdingSelf == false) // This caps the velocity, unless the player is holding themselves.
        {
            playerRGBD.velocity = new Vector2 (xVelocityCap, playerRGBD.velocity.y);
        }

        if (playerRGBD.velocity.x < -xVelocityCap && scriptManager.selectObjects.holdingSelf == false) // This caps the velocity, unless the player is holding themselves.
        {
            playerRGBD.velocity = new Vector2(-xVelocityCap, playerRGBD.velocity.y);
        }

        if (Input.GetButton("Jump") && scriptManager.selectObjects.isTalking == false) //Pressing space causes the player to drop the object, unless they are talking.
        {
            scriptManager.holdObjectMovement.heldObjectMove = false;
            StopHolding();
        }

        playerPosition = this.transform.position; //updates the player position on every frame, which other scripts depend on.

    }

    public void StartHolding ()  // This only matters for setting the appropriate bool in the animator.
    {
        playerAnim.SetBool("Holding", true);
    }

    public void StopHolding () // This 'drops' the held object. See HoldObjectMovement.
    {
        scriptManager.selectObjects.holdingSelf = false;
        scriptManager.holdObjectMovement.DropHeldObject();
        playerAnim.SetBool("Holding", false);
    }

    public IEnumerator fallTimer () //checks if the player is still falling after a certain amount of time
    {
        Debug.Log("check if falling");
        fallTimerCheck = true;
        int fallCounter = 0;
        for (int i = 0; i < 10; i++)
        {
            yield return new WaitForSeconds(fallTime);
            if (playerRGBD.velocity.y < -1)
            {
                fallCounter++;
            }
        }
        if (fallCounter >= 9)
        {
            playerFalling = true;
        }
    }

    public void DealDamage () //Called by animation event
    {
        if (playerSprite.flipX == true)
        {
            playerDamageLeft.SetActive(true);
        } else
        {
            playerDamageRight.SetActive(true);
        }
    }

    public void stopDealingDamage () //Called by animation event
    {
        playerDamageLeft.SetActive(false);
        playerDamageRight.SetActive(false);
    }

    public void StopAttacking ()
    {
        playerAttacking = false;
        xVelocityCap = attackingVelCap * attackingThrustSlowFactor;
    }

    public void CanMove () //called by animation event
    {
        canMove = true;
    }

    public void CantMove () //called by animation event
    {
        canMove = false;
    }

}
